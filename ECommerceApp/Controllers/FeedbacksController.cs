using ECommerceApp.DTOs;
using ECommerceApp.DTOs.FeedbackDTOs;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private readonly FeedbackService _feedbackService;

        public FeedbacksController(FeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        // Sumbits feedback for a product.
        [HttpPost("SubmitFeedback")]
        public async Task<ActionResult<ApiResponse<FeedbackResponseDTO>>> SubmitFeedback([FromBody] FeedbackCreateDTO feedbackCreateDTO)
        {
            var response = await _feedbackService.SubmitFeedbackAsync(feedbackCreateDTO);

            if(response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }

            return Ok(response);
        }

        // Retrieves all feedbacks for a specific product.
        [HttpGet("GetFeedbackForProduct/{productId}")]
        public async Task<ActionResult<ApiResponse<ProductFeedbackResponseDTO>>>GetFeedbackForProduct(int productId)
        {
            var response = await _feedbackService.GetFeedbackForProductAsync(productId);

            if(response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode,response);
            }
            return Ok(response);
        }

        // Retrieves all feedback (admin use).
        [HttpGet("GetAllFeedbacks")]
        public async Task<ActionResult<ApiResponse<List<FeedbackResponseDTO >>>>GetAllFeedbacks()
        {
            var response= await _feedbackService.GetAllFeedbacksAsync();

            if(response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }

            return Ok(response);
        }

        // Updates a specific feedback entry.
        [HttpPut("UpdateFeedback")]
        public async Task<ActionResult<ApiResponse<FeedbackResponseDTO>>> UpdateFeedback([FromBody] FeedbackUpdateDTO feedbackUpdateDTO)
        {
            var response = await _feedbackService.UpdateFeedbackAsync(feedbackUpdateDTO);

            if( response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }

            return Ok(response);
        }

        // Deletes a specific feedback entry.
        [HttpDelete("DeleteFeedback")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> DeleteFeedback([FromBody] FeedbackDeleteDTO feedbackDeleteDTO)
        {
            var response = await _feedbackService.DeleteFeedbackAsync (feedbackDeleteDTO);

            if(response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }

            return Ok(response);
        }
    }
}
