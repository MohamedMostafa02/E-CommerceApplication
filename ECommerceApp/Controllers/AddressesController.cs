using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECommerceApp.Services;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.AddressesDTOs;

namespace ECommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly AddressService _addressService;

        // Injection the services
        public AddressesController(AddressService addressService)
        {
            _addressService = addressService;
        }

        // Create a new address for customer
        [HttpPost("CreateAddress")]

        public async Task<ActionResult<ApiResponse<AddressResponseDTO>>> CreateAddress([FromBody] AddressCreateDTO addressCreateDTO)
        {
            var response = await _addressService.CreateAddressAsync(addressCreateDTO);
            if(response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves an address by ID
        [HttpGet("GetAddressById/{id}")]
        public async Task<ActionResult<ApiResponse<AddressResponseDTO>>> GetAddressById(int id)
        {
            var response  = await _addressService.GetAddressByIdAsync(id);

            if(response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode,response);
            }
            return Ok(response);
        }

        // Update an existing address

        [HttpPut("UpdateAddress")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> UpdateAddress([FromBody] AddressUpdateDTO addressUpdateDTO)
        {
            var response = await _addressService.UpdateAddressAsync(addressUpdateDTO);

            if( response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Delete an address by ID
        [HttpDelete("DeleteAddress")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> DeleteAddress([FromBody] AddressDeleteDTO addressDeleteDTO)
        {
            var response = await _addressService.DeleteAddressAsync(addressDeleteDTO);

            if(response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves all addresses for a specific customer.
        [HttpGet("GetAddressesByCustomer/{customerId}")]
        public async Task<ActionResult<ApiResponse<List<AddressResponseDTO>>>> GetAddressesByCustomer(int customerId)
        {
            var response = await _addressService.GetAddressesByCustomerAsync(customerId);

            if(response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode,response);
            }
            return Ok(response);
        }


    }
}
