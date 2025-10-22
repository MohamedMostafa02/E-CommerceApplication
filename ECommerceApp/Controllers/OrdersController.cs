using ECommerceApp.DTOs;
using ECommerceApp.DTOs.OrderDTOs;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        // Inject the OrderService.
        public  OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        // Creates a new order.
        // Post: api/Orders/CreateOrder

        [HttpPost("CreateOrder")]
        public async Task<ActionResult<ApiResponse<OrderResponseDTO>>> CreateOrder([FromBody] OrderCreateDTO orderCreateDTO)
        {
            var response = await _orderService.CreateOrderAsync(orderCreateDTO);

            if(response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves an order by it's ID.
        // GET: api/Orders/GetOrderById/{id}.
        [HttpGet("GetOrderById/{id}")]
        public async Task<ActionResult<ApiResponse<OrderResponseDTO>>> GetOrderById(int id)
        {
            var response = await _orderService.GetOrderByIdAsync(id);

            if( response.StatusCode != 200 )
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Updates the status of an existing order.
        // PUT: api/Orders/UpdateOrderStatus.
        [HttpPut("UpdateOrderStatus")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> UpdateOrderStatus([FromBody] OrderStatusUpdateDTO orderStatusUpdateDTO)
        {
            var response = await _orderService.UpdateOrderStatusAsync(orderStatusUpdateDTO);

            if(Response.StatusCode != 200 )
            {
                  return StatusCode(response.StatusCode, response);
            }

            return Ok(response);
        }

        // Retrieve all orders
        // GET: api/Orders/GetAllOrders
        [HttpGet("GetAllOrders")]
        public async Task<ActionResult<ApiResponse<List<OrderResponseDTO>>>> GetAllOrders()
        {
            var response = await _orderService.GetAllOrdersAsync();

            if(response.StatusCode != 200 )
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieve all orders for a specific customer.
        // GET: api/Orders/GetOrderByCustomer/{customerId}

        [HttpGet("GetOrderByCustomer/{customerId}")]
        public async Task<ActionResult<List<OrderResponseDTO>>> GetOrdersByCustomer(int customerId)
        {
            var response = await _orderService.GetOrdersByCustomerAsync(customerId);

            if(response.StatusCode != 200 )
            {
                return StatusCode(response.StatusCode,response);
            }
            return Ok(response);
        }
    }
}
