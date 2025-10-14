using System.ComponentModel.DataAnnotations;
namespace ECommerceApp.DTOs.AddressesDTOs
{
    // DTO for deleting Address
    public class AddressDeleteDTO
    {
        [Required(ErrorMessage ="CustomerId is required.")]
        public int CustomerId { get; set; }
        [Required(ErrorMessage ="AddressId is required.")]
        public int AddressId { get; set; }
    }
}
