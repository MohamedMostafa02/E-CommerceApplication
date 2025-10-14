using System.ComponentModel.DataAnnotations;
namespace ECommerceApp.DTOs.AddressesDTOs
{
    // DTO for create customer address
    public class AddressCreateDTO
    {
        [Required(ErrorMessage ="CustomerId is required.")]
        public int CustomerId { get; set; }
        [Required(ErrorMessage ="Address Line 1 is required.")]
        [StringLength(100,ErrorMessage ="Address Line 1 can't exceed 100 characters.")]
        public string AddressLine1 { get; set; }
        [Required(ErrorMessage ="Address Line 2 is required.")]
        [StringLength(100,ErrorMessage ="Address Line 2 can't exceed 100 characters.")]
        public string AddressLine2 { get; set; }
        [Required(ErrorMessage ="City is required.")]
        [StringLength(50,ErrorMessage ="City can't exceed 50 characters.")]
        public string City { get; set; }
        [Required(ErrorMessage ="State is requierd.")]
        [StringLength(50,ErrorMessage ="State can't exceed 50 characters.")]
        public string State {  get; set; }
        [Required(ErrorMessage ="Postal Code is required.")]
        [RegularExpression(@"^\d{4,6}$",ErrorMessage ="Invalid Postal Code.")]
        public string PostalCode { get; set; }
        [Required(ErrorMessage ="Country is required.")]
        [StringLength(50,ErrorMessage ="Country can't exceed 50 characters.")]
        public string Country { get; set; }
    }
}
