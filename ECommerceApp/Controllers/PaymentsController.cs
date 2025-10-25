using ECommerceApp.DTOs;
using ECommerceApp.DTOs.PaymentDTOs;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ECommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // Processes a payment for an order.
        [HttpPost("ProcessPayment")]
        public async Task<ActionResult<ApiResponse<PaymentResponseDTO>>> ProcessPayment([FromBody] PaymentRequestDTO paymentRequestDTO)
        {
            var response = await _paymentService.ProcessPaymentAsync(paymentRequestDTO);
            
            if(response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves payment details by payment ID.
        [HttpGet("GetPaymentById/{paymentId}")]

        public async Task<ActionResult<ApiResponse<PaymentResponseDTO>>> GetPaymentById(int paymentId)
        {
            var response = await _paymentService.GetPaymentByIdAsync(paymentId);

            if(response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode,response);
            }
            return Ok(response);
        }

        // Retrieves payment details associated with a specific order.
        [HttpGet("GetPaymentByOrderId/{orderId}")]
        public async Task<ActionResult<ApiResponse<PaymentResponseDTO >>> GetPaymentByOrderId(int orderId)
        {
            var response = await _paymentService.GetPaymentByOrderIdAsync(orderId);

            if(response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode,response);
            }
            return Ok(response);
        }

        // Updates the status of an existing payment.
        [HttpPut("UpdatePaymentStatus")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> UpdatePaymentStatus([FromBody] PaymentStatusUpdateDTO statusUpdateDTO)
        {
            var response = await _paymentService.UpdatePaymentStatusAsync(statusUpdateDTO);

            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Complete a Cash on Delivery (COD) payemnt.
        [HttpPost("CompleteCODPayment")]

        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> CompleteCODPayment([FromBody] CODPaymentUpdateDTO codPaymentUpdateDTO)
        {
            var response = await _paymentService.CompleteCODPaymentAsync(codPaymentUpdateDTO);

            if(response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
