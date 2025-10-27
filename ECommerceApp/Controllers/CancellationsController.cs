using ECommerceApp.DTOs;
using ECommerceApp.DTOs.CancellationDTOs;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancellationsController : ControllerBase
    {
        private readonly CancellationService _cancellationService;
        // Inject the CancellationService via constructor
        public CancellationsController(CancellationService cancellationService)
        {
            _cancellationService = cancellationService;
        }

        // Endpoint for customers to request an order cancellation.
        [HttpPost("RequestCancellation")]
        public async Task<ActionResult<ApiResponse<CancellationResponseDTO>>> RequestCancellation([FromBody] CancellationRequestDTO requestDTO)
        {
            var response = await _cancellationService.RequestCancellationAsync(requestDTO);

            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }

            return Ok(response);
        }

        // Endpoint for retrieve all cancellation requests
        [HttpGet("GetAllCancellation")]
        public async Task<ActionResult<ApiResponse<List<CancellationResponseDTO>>>> GetAllCancellations()
        {
            var response = await _cancellationService.GetAllCancellationsAsync();

            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }

            return Ok(response);
        }

        // Endpoint to retrieve cancellation details by cancellation ID.
        [HttpGet("GetCancellationById /{id}")]
        public async Task<ActionResult<ApiResponse<CancellationResponseDTO>>> GetCancellationById(int id)
        {
            var response = await _cancellationService.GetCancellationByIdAsync(id);

            if(response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }

            return Ok(response);
        }

        // Endpoint for administrators to update the status of a cancellation request
        [HttpPut("UpdateCancellationStatus")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> UpdateCancellationStatus([FromBody] CancellationStatusUpdateDTO cancellationStatusUpdateDTO)
        {
            var response = await _cancellationService.UpdateCancellationStatusAsync(cancellationStatusUpdateDTO);

            if( response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }

            return Ok(response);
        }
    }
}