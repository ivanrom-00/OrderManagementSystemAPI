using OrderService.Models;

namespace OrderService.Interfaces
{
    public interface IOrderService
    {
        Task AddOrderAsync(Order order);
        Task DeleteOrderAsync(int orderId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task UpdateOrderAsync(Order order);
    }
}
