using CustomerService.Interfaces;
using CustomerService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models;
using SharedLibrary.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace CustomerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        /// <summary>Creates a new customer</summary>
        /// <remarks>
        /// Sample request body:
        /// 
        ///     {
        ///       "FirstName": "CustomerName",          // Required
        ///       "LastName": "CustomerLastName"        // Required
        ///       "Email": "email@email.com"            // Required
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">OK. Returns the created record.</response>
        /// <response code="400">BadRequest. Invalid model properties.</response>
        /// <response code="401">Unauthorized. Invalid service token.</response>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.GetErrorMessages();
                return BadRequest(new ApiResponse<Customer?>(false, StatusCodes.Status400BadRequest, errorMessage, null));
            }
            try
            {
                await _customerService.AddCustomerAsync(customer);
                _logger.LogInformation($"Customer created successfully: {customer.Email}");
                return Ok(new ApiResponse<Customer>(true, StatusCodes.Status200OK, "", customer));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add customer");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<Customer?>(false,
                    StatusCodes.Status500InternalServerError, "Failed to add order", null));
            }
        }

        /// <summary>Get all customers</summary>
        /// <response code="200">OK. Returns the requested list of records.</response>
        /// <response code="401">Unauthorized. Invalid service token.</response>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                return Ok(new ApiResponse<IEnumerable<Customer>>(true, StatusCodes.Status200OK, "", customers));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all customers");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<Customer?>(false,
                    StatusCodes.Status500InternalServerError, "Failed to get all customers", null));
            }
        }

        /// <summary>Get customer by Id</summary>
        /// <param name="customerId">Requested Customer Id</param>
        /// <example>10</example>
        /// <response code="200">OK. Returns the requested record.</response>
        /// <response code="401">Unauthorized. Invalid service token.</response>
        /// <response code="404">NotFound. Requested record not found.</response>
        [HttpGet("{customerId}")]
        [Authorize]
        public async Task<IActionResult> GetCustomerById(int customerId)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(customerId);
                if (customer == null)
                {
                    _logger.LogWarning($"Customer with Id {customerId} not found");
                    return NotFound(new ApiResponse<Customer?>(false, StatusCodes.Status404NotFound, $"Customer with Id {customerId} not found", null));
                }
                return Ok(new ApiResponse<Customer>(true, StatusCodes.Status200OK, "", customer));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get customer with Id {customerId}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<Customer?>(false,
                    StatusCodes.Status500InternalServerError, $"Failed to get customer with Id {customerId}", null));
            }
        }

        /// <summary>Update customer</summary>
        /// <remarks>
        /// Sample request body:
        /// 
        ///     {
        ///       "FirstName": "NewCustomerName",          // Required
        ///       "LastName": "NewCustomerLastName"        // Required
        ///       "Email": "new_email@email.com"           // Required
        ///     }
        /// 
        /// </remarks>
        /// <param name="customerId">Requested Customer Id (Record to be updated)</param>
        /// <example>10</example>
        /// <response code="200">OK. Returns the updated record.</response>
        /// <response code="400">BadRequest. Invalid model properties..</response>
        /// <response code="401">Unauthorized. Invalid service token.</response>
        /// <response code="404">NotFound. Requested record not found.</response>
        [HttpPut("{customerId}")]
        [Authorize]
        public async Task<IActionResult> UpdateCustomer(int customerId, Customer customer)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.GetErrorMessages();
                return BadRequest(new ApiResponse<Customer?>(false, StatusCodes.Status400BadRequest, errorMessage, null));
            }
            try
            {
                var customerExists = await _customerService.GetCustomerByIdAsync(customerId);
                if (customerExists == null)
                {
                    _logger.LogWarning($"Customer with Id {customerId} not found");
                    return NotFound(new ApiResponse<Customer?>(false, StatusCodes.Status404NotFound, $"Customer with Id {customerId} not found", null));
                }

                customer.Id = customerId; // Ensure correct Id is set
                await _customerService.UpdateCustomerAsync(customer);
                _logger.LogInformation($"Customer with Id {customerId} updated successfully");
                return Ok(new ApiResponse<Customer>(true, StatusCodes.Status200OK, "", customer));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update customer with Id {customerId}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<Customer?>(false,
                    StatusCodes.Status500InternalServerError, $"Failed to update customer with Id {customerId}", null));
            }
        }
    }
}
