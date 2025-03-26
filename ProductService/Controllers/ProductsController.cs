using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.Interfaces;
using ProductService.Models;
using System.ComponentModel.DataAnnotations;
using SharedLibrary.Models;
using SharedLibrary.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>Creates a new product</summary>
        /// <remarks>
        /// Sample request body:
        /// 
        ///     {
        ///       "Name":    "Product",      // Required
        ///       "Price":   10.5,           // Required (Must be greater than 0)
        ///       "Stock":   10              // Required (Must be greater than 0)
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">OK. Returns the created record.</response>
        /// <response code="400">BadRequest. Invalid model properties.</response>
        /// <response code="401">Unauthorized. Invalid service token.</response>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.GetErrorMessages();
                return BadRequest(new ApiResponse<Product?>(false, StatusCodes.Status400BadRequest, errorMessage, null));
            }
            try
            {
                await _productService.AddProductAsync(product);
                _logger.LogInformation($"Product created successfully: {product.Name}");
                return Ok(new ApiResponse<Product>(true, StatusCodes.Status200OK, "", product));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add product");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<Product?>(false,
                    StatusCodes.Status500InternalServerError, "Failed to add product", null));
            }
        }

        /// <summary>Get all products</summary>
        /// <response code="200">OK. Returns the requested list of records.</response>
        /// <response code="401">Unauthorized. Invalid service token.</response>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(new ApiResponse<IEnumerable<Product>>(true, StatusCodes.Status200OK, "", products));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all products");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<Product?>(false,
                    StatusCodes.Status500InternalServerError, "Failed to get all products", null));
            }
        }

        /// <summary>Get product by Id</summary>
        /// <param name="productId">Requested Product Id</param>
        /// <example>10</example>
        /// <response code="200">OK. Returns the requested record.</response>
        /// <response code="401">Unauthorized. Invalid service token.</response>
        /// <response code="404">NotFound. Requested record not found.</response>
        [HttpGet("{productId}")]
        [Authorize]
        public async Task<IActionResult> GetProductById(int productId)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning($"Product with id {productId} not found");
                    return NotFound(new ApiResponse<Product?>(false, StatusCodes.Status404NotFound, $"Product with Id {productId} not found", null));
                }
                return Ok(new ApiResponse<Product>(true, StatusCodes.Status200OK, "", product));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get product with Id {productId}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<Product?>(false,
                    StatusCodes.Status500InternalServerError, $"Failed to get product with Id {productId}", null));
            }
        }

        /// <summary>Updates a specific product</summary>
        /// <param name="productId">Requested Product Id (Record to be updated)</param>
        /// <remarks>
        /// Sample request body:
        /// 
        ///     {
        ///       "Name":    "NewProduct",   // Required
        ///       "Price":   10.5,           // Required (Must be greater than 0)
        ///       "Stock":   10              // Required (Must be greater than 0)
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">OK. Returns the created record.</response>
        /// <response code="400">BadRequest. Invalid model properties.</response>
        /// <response code="401">Unauthorized. Invalid service token.</response>
        /// <response code="404">NotFound. Requested record not found.</response>
        [HttpPut("{productId}")]
        [Authorize]
        public async Task<IActionResult> UpdateProduct(int productId, Product product)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.GetErrorMessages();
                return BadRequest(new ApiResponse<Product?>(false, StatusCodes.Status400BadRequest, errorMessage, null));
            }
            try
            {
                var productExists = await _productService.GetProductByIdAsync(productId);
                if (productExists == null)
                {
                    _logger.LogWarning($"Product with id {productId} not found");
                    return NotFound(new ApiResponse<Product?>(false, StatusCodes.Status404NotFound, $"Product with Id {productId} not found", null));
                }

                product.Id = productId; // Ensure correct Id is set
                await _productService.UpdateProductAsync(product);
                _logger.LogInformation($"Product with Id {productId} updated successfully");
                return Ok(new ApiResponse<Product>(true, StatusCodes.Status200OK, "", product));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update product with Id {productId}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<Product?>(false,
                    StatusCodes.Status500InternalServerError, $"Failed to update product with Id {productId}", null));
            }
        }
    }
}
