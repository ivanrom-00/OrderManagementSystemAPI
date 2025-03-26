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
    internal class CustomerConsumer
    {
        public async static Task Main()
        {
            // Set up connection
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "admin", Password = "admin" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            // Set queue:
            // - order_customer_validation: Queue for customer validation (Check the customer exists)
            await channel.QueueDeclareAsync(queue: "order_customer_validation", durable: false, exclusive: false, autoDelete: false, arguments: null);

            // Set consumer for customer validation queue
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                // Map event arguments to variables
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var order = JsonConvert.DeserializeObject<dynamic>(message);

                // Simulate valid customer ids (this would be replaced with database queries using the repository)
                var validCustomers = new[] { 101, 102, 103 };
                // Check if customer exists
                bool isValid = Array.Exists(validCustomers, id => id == (int)order.CustomerId);

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

            // Consume messages from customer validation queue
            await channel.BasicConsumeAsync(queue: "order_customer_validation", autoAck: true, consumer: consumer);
        }
    }
}
