using Microsoft.EntityFrameworkCore;
using ECommerceApp.Models;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.AddressesDTOs;
using ECommerceApp.Data;
namespace ECommerceApp.Services
{
    public class AddressService
    {
        private readonly ApplicationDbContext _context;

        public AddressService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<AddressResponseDTO>> CreateAddressAsync(AddressCreateDTO addressCreateDTO)
        {
            try
            {
                // Check if customer exists
                var customer = await _context.Customers.FindAsync(addressCreateDTO.CustomerId);
                if (customer == null)
                {
                    return new ApiResponse<AddressResponseDTO>(404, "Customer not found.");
                }

                // Manual mapping from DTO to Model
                var address = new Address
                {
                    CustomerID = addressCreateDTO.CustomerId,
                    AddressLine1 = addressCreateDTO.AddressLine1,
                    AddressLine2 = addressCreateDTO.AddressLine2,
                    City = addressCreateDTO.City,
                    State = addressCreateDTO.State,
                    PostalCode = addressCreateDTO.PostalCode,
                    Country = addressCreateDTO.Country
                };

                // Add address to the database

                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();

                // Map to AddressResponseDTO
                var addressResponse = new AddressResponseDTO
                {
                    Id = address.Id,
                    CustomerId = customer.Id,
                    AddressLine1 = address.AddressLine1,
                    AddressLine2 = address.AddressLine2,
                    City = address.City,
                    State = address.State,
                    PostalCode = address.PostalCode,
                    Country = address.Country
                };

                return new ApiResponse<AddressResponseDTO>(200, addressResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AddressResponseDTO>(500, $"An unexpected error occured while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<AddressResponseDTO>> GetAddressByIdAsync(int id)
        {
            try
            {
              var address = await _context.Addresses.AsNoTracking().FirstOrDefaultAsync(ad => ad.Id == id);

                if(address == null)
                {
                    return new ApiResponse<AddressResponseDTO>(404, "Address not found.");
                }

                // Map to AddressResponseDTO
                var addressResponse = new AddressResponseDTO
                {
                    Id= address.Id,
                    CustomerId = address.CustomerID,
                    AddressLine1 = address.AddressLine1,
                    AddressLine2 = address.AddressLine2,
                    City = address.City,
                    State = address.State,
                    PostalCode = address.PostalCode,
                    Country = address.Country
                };

                return new ApiResponse<AddressResponseDTO>(200,addressResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AddressResponseDTO>(500,$"Anexpected error occurred while processing your request, Error: {ex.Message}" );
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateAddressAsync(AddressUpdateDTO addressUpdateDTO)
        {
            try
            {
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(ad => ad.Id == addressUpdateDTO.AddressId && ad.CustomerID == addressUpdateDTO.CustomerId);

                if (address == null)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Address not found.");
                }

                // Update address properties

                address.AddressLine1 = addressUpdateDTO.AddressLine1;
                address.AddressLine2 = addressUpdateDTO.AddressLine2;
                address.City = addressUpdateDTO.City;
                address.State = addressUpdateDTO.State;
                address.PostalCode = addressUpdateDTO.PostalCode;
                address.Country = addressUpdateDTO.Country;

                await _context.SaveChangesAsync();

                // Perpare Confirmation Message

                var confirmationMessage = new ConfirmationResponseDTO
                {
                    Message = $"Address with Id {addressUpdateDTO.AddressId} updated successfully."
                };

                return new ApiResponse<ConfirmationResponseDTO>(200,confirmationMessage);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, $"Anexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }
        
        public async Task<ApiResponse<ConfirmationResponseDTO>> DeleteAddressAsync(AddressDeleteDTO addressDeleteDTO)
        {
            try
            {
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(ad => ad.Id == addressDeleteDTO.AddressId && ad.CustomerID == addressDeleteDTO.CustomerId);

                if (address == null)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Address not found.");
                }

                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();

                // Prepare Confirmation message

                var confirmationMessage = new ConfirmationResponseDTO
                {
                    Message = $"Address with Id {addressDeleteDTO.AddressId} deleted successfully."
                };
                return new ApiResponse<ConfirmationResponseDTO>(200, confirmationMessage);
            }
            catch (Exception ex)
            {
                
                return new ApiResponse<ConfirmationResponseDTO>(500, $"Anexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<AddressResponseDTO>>> GetAddressesByCustomerAsync(int customerId)
        {
            try
            {
                var customer = await _context.Customers
                    .AsNoTracking()
                    .Include(c => c.Addresses)
                    .FirstOrDefaultAsync(c => c.Id == customerId);

                if (customer == null)
                {
                    return new ApiResponse<List<AddressResponseDTO>>(404, "Customer not found.");
                }

                var addresses = customer.Addresses.Select(ad => new AddressResponseDTO
                {
                    Id = ad.Id,
                    CustomerId = ad.CustomerID,
                    AddressLine1 = ad.AddressLine1,
                    AddressLine2 = ad.AddressLine2,
                    City = ad.City,
                    State = ad.State,
                    PostalCode = ad.PostalCode,
                    Country = ad.Country
                }).ToList();

                return new ApiResponse<List<AddressResponseDTO>>(200, addresses);
                    
            }
            catch (Exception ex)
            {

                return new ApiResponse<List<AddressResponseDTO>>(500, $"Anexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        
    }
}
