using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.Interfaces;
using ProductService.Models;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        /// <summary>Generates an access token for using the service</summary>
        /// <remarks>
        /// Sample request body:
        /// 
        ///     {
        ///       "Username":  "username",     // Required
        ///       "Password":  "password",     // Required
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">OK. Returns the access token.</response>
        /// <response code="401">Unauthorized. Invalid service credentials.</response>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request.Username == "admin" && request.Password == "admin")
            {
                var token = _tokenService.GenerateToken("1", request.Username);
                return Ok(new LoginResponse { Token = token });
            }

            return Unauthorized();
        }
    }
}
