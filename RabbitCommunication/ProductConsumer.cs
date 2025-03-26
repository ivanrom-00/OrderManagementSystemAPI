using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RabbitCommunication
{
    internal class ProductConsumer
    {
        public async static Task Main()
        {
            // Set up connection
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "admin", Password = "admin" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            // Set queue:
            // - order_product_validation: Queue for product validation (Check the product exists and has a valid stock)
            await channel.QueueDeclareAsync(queue: "order_product_validation", durable: false, exclusive: false, autoDelete: false, arguments: null);

            // Set consumer for product validation queue
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                // Map event arguments to variables
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var order = JsonConvert.DeserializeObject<dynamic>(message);

                // Simulate products stock (this would be replaced with database queries using the repository)
                var productsStock = new Dictionary<int, int> { { 202, 10 }, { 203, 2 } };
                // Check if product exists and has a valid stock
                bool isValid = productsStock.ContainsKey((int)order.ProductId) && (int)order.Amount <= productsStock[(int)order.ProductId];
    
                // Build response
                var response = isValid ? "Valid" : "Invalid";
                var responseBody = Encoding.UTF8.GetBytes(response);

                var replyProps = ea.BasicProperties;
                var responseProps = new BasicProperties { };
                responseProps.CorrelationId = replyProps.CorrelationId;

                // Send response in the same channel
                using var responseChannel = await connection.CreateChannelAsync();
                await responseChannel.BasicPublishAsync(exchange: "", routingKey: replyProps.ReplyTo, mandatory: true, basicProperties: responseProps, body: responseBody);
            };

            // Consume messages from product validation queue
            await channel.BasicConsumeAsync(queue: "order_product_validation", autoAck: true, consumer: consumer);
        }
    }
}
