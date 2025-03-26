using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderService.Interfaces;
using OrderService.Models;
using SharedLibrary.Models;
using SharedLibrary.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>Creates a new order</summary>
        /// <remarks>
        /// Sample request body:
        /// 
        ///     {
        ///       "CustomerId":  15,                          // Required (Must be a valid customer Id)
        ///       "ProductId":   12,                          // Required (Must be a valid product Id)
        ///       "Quantity":    3                            // Required (Must be greater than 0)
        ///       "TotalAmount": 15.25                        // Required (Must be greater than 0)
        ///       "OrderDate":   "2025-03-24T10:30:30.333Z"   // Required
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">OK. Returns the created record.</response>
        /// <response code="400">BadRequest. Invalid model properties.</response>
        /// <response code="401">Unauthorized. Invalid service token.</response>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.GetErrorMessages();
                return BadRequest(new ApiResponse<Order?>(false, StatusCodes.Status400BadRequest, errorMessage, null));
            }
            try
            {
                await _orderService.AddOrderAsync(order);
                _logger.LogInformation($"Order created for customer {order.CustomerId}");
                return Ok(new ApiResponse<Order>(true, StatusCodes.Status200OK, "", order));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add order");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<Order?>(false, 
                    StatusCodes.Status500InternalServerError, "Failed to add order", null));
            }
        }

        /// <summary>Delete a specific Order</summary>
        /// <param name="orderId">Requested Order Id to be deleted</param>
        /// <example>10</example>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized. Invalid service token.</response>
        /// <response code="404">NotFound. Requested record not found.</response>
        [HttpDelete("{orderId}")]
        [Authorize]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            try
            {
                var orderExists = await _orderService.GetOrderByIdAsync(orderId);
                if (orderExists == null)
                {
                    _logger.LogWarning($"Order with Id {orderId} not found");
                    return NotFound(new ApiResponse<Order?>(false, StatusCodes.Status404NotFound, $"Order with Id {orderId} not found", null));
                }

                await _orderService.DeleteOrderAsync(orderId);
                _logger.LogInformation($"Order with Id {orderId} deleted successfully");
                return Ok(new ApiResponse<Order?>(true, StatusCodes.Status200OK, "", null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete order with Id {orderId}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<Order?>(false, 
                    StatusCodes.Status500InternalServerError, $"Failed to delete order with Id {orderId}", null));
            }
        }

        /// <summary>Get order by Id</summary>
        /// <param name="orderId">Requested Order Id</param>
        /// <example>10</example>
        /// <response code="200">OK. Returns the requested record.</response>
        /// <response code="401">Unauthorized. Invalid service token.</response>
        /// <response code="404">NotFound. Requested record not found.</response>
        [HttpGet("{orderId}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning($"Order with Id {orderId} not found");
                    return NotFound(new ApiResponse<Order?>(false, StatusCodes.Status404NotFound, $"Order with Id {orderId} not found", null));
                }
                return Ok(new ApiResponse<Order>(true, StatusCodes.Status200OK, "", order));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get order with Id {orderId}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<Order?>(false,
                    StatusCodes.Status500InternalServerError, $"Failed to get order with Id {orderId}", null));
            }
        }

        /// <summary>Get all orders</summary>
        /// <response code="200">OK. Returns the requested list of records.</response>
        /// <response code="401">Unauthorized. Invalid service token.</response>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(new ApiResponse<IEnumerable<Order>>(true, StatusCodes.Status200OK, "", orders));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all orders");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<Order?>(false,
                    StatusCodes.Status500InternalServerError, "Failed to get all orders", null));
            }
        }

        /// <summary>Updates a specific order</summary>
        /// <param name="orderId">Requested Order Id (Record to be updated)</param>
        /// <remarks>
        /// Sample request body:
        /// 
        ///     {
        ///       "CustomerId":  15,                          // Required (Must be a valid customer Id)
        ///       "ProductId":   12,                          // Required (Must be a valid product Id)
        ///       "Quantity":    3                            // Required (Must be greater than 0)
        ///       "TotalAmount": 15.25                        // Required (Must be greater than 0)
        ///       "OrderDate":   "2025-03-24T10:30:30.333Z"   // Required
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">OK. Returns the created record.</response>
        /// <response code="400">BadRequest. Invalid model properties.</response>
        /// <response code="401">Unauthorized. Invalid service token.</response>
        /// <response code="404">NotFound. Requested record not found.</response>
        [HttpPut("{orderId}")]
        [Authorize]
        public async Task<IActionResult> UpdateOrder(int orderId, Order order)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.GetErrorMessages();
                return BadRequest(new ApiResponse<Order?>(false, StatusCodes.Status400BadRequest, errorMessage, null));
            }
            try
            {
                var orderExists = await _orderService.GetOrderByIdAsync(orderId);
                if (orderExists == null)
                {
                    _logger.LogWarning($"Order with Id {orderId} not found");
                    return NotFound(new ApiResponse<Order?>(false, StatusCodes.Status404NotFound, $"Order with Id {orderId} not found", null));
                }

                order.Id = orderId; // Ensure correct Id is set
                await _orderService.UpdateOrderAsync(order);
                _logger.LogInformation($"Order with Id {orderId} updated successfully");
                return Ok(new ApiResponse<Order>(true, StatusCodes.Status200OK, "", order));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update order with Id {orderId}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<Order?>(false, StatusCodes.Status500InternalServerError, "Failed to add order", null));
            }
        }
    }
}
