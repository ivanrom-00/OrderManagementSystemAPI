using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitCommunication
{
    internal class OrderPublisher
    {
        private static readonly ConcurrentDictionary<string, List<string>> Responses = new();
        public async static Task Main(string[] args)
        {
            // Set up connection
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "admin", Password = "admin" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            // Set queues:
            // - order_customer_validation: Queue for customer validation (Check the customer exists)
            // - order_product_validation: Queue for product validation (Check the product exists and has a valid stock)
            // - order_response: Queue for responses (Responses from customer and product validation)
            await channel.QueueDeclareAsync(queue: "order_customer_validation", durable: false, exclusive: false, autoDelete: false, arguments: null);
            await channel.QueueDeclareAsync(queue: "order_product_validation", durable: false, exclusive: false, autoDelete: false, arguments: null);
            await channel.QueueDeclareAsync(queue: "order_response", durable: false, exclusive: false, autoDelete: false, arguments: null);

            // Set up response consumer
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var correlationId = ea.BasicProperties.CorrelationId;
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                if (!Responses.ContainsKey(correlationId))
                {
                    Responses[correlationId] = new List<string>();
                }

                Responses[correlationId].Add(message);
            };

            // Set consumer for response queue
            await channel.BasicConsumeAsync(queue: "order_response", autoAck: true, consumer: consumer);

            // Logic for placing an Order
            var order = new { OrderId = 1, CustomerId = 101, ProductId = 202, Amount = 5 }; // Order to be placed

            // Set message properties (body content, correlationId, replyTo for listen response)
            var message = JsonConvert.SerializeObject(order);
            var body = Encoding.UTF8.GetBytes(message);
            var correlationId = Guid.NewGuid().ToString();
            var properties = new BasicProperties { };
            properties.ReplyTo = "order_response";
            properties.CorrelationId = correlationId;

            // Send messages
            await channel.BasicPublishAsync(exchange: "", routingKey: "order_customer_validation", mandatory: true, basicProperties: properties, body: body);
            await channel.BasicPublishAsync(exchange: "", routingKey: "order_product_validation", mandatory: true, basicProperties: properties, body: body);

            // Listen for responses
            var startTime = DateTime.UtcNow;
            while (!Responses.ContainsKey(correlationId) || Responses[correlationId].Count < 2)
            {
                // Set a timeout for waiting responses
                if ((DateTime.UtcNow - startTime).TotalSeconds > 5)
                {
                    return;
                }
                await Task.Delay(1000);
            }

            // Logic for evaulating responses
            var responses = Responses[correlationId];
            foreach (var response in responses)
            {
                // Logic
            }
        }
    }
}
