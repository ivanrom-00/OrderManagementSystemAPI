using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using OrderService.Interfaces;
using OrderService.Models;
using RabbitMQ.Client;
using SharedLibrary.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OrderService.Services
{
    public class OrderManagementService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderManagementService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task AddOrderAsync(Order order)
        {
            // TO DO: Communicate with microservices to validate Customer, Product and Product Stock
            // Set up necessary configurations for RabbitMQ:
            // - Factory
            // - Connection
            // - Channel
            // - Queue
            // - Exchange
            // - Publish message
            // - Wait and consume message response
            // - Do validations
            // - If validations pass, add order
            await _orderRepository.AddOrderAsync(order);
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            await _orderRepository.DeleteOrderAsync(orderId);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return orders;
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            return order;
        }

        public async Task UpdateOrderAsync(Order order)
        {
            await _orderRepository.UpdateOrderAsync(order);
        }
    }
}
