using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ECommerceApp.Models
{
    // Represent a Customer's address
    public class Address
    {
        public int Id { get; set; }
        public int CustomerID { get; set; }
        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; }

        [Required(ErrorMessage ="Address Line 1 is required")]
        [StringLength(100,ErrorMessage ="Address Line 1 can't exceed more than 100 characters.")]
        public string AddressLine1 {  get; set; }
        [Required(ErrorMessage ="Address Line 2 is required")]
        [StringLength(100,ErrorMessage ="Address Line 2 can't exceed more than 100 characters.")]
        public string AddressLine2 { get; set; }
        [Required(ErrorMessage ="City is required")]
        [StringLength(50,ErrorMessage ="City Can't exceed more than 50 characters.")]
        public string City { get; set; }
        [Required(ErrorMessage ="State is required")]
        [StringLength(50,ErrorMessage ="State can't exceed more than 50 characters")]
        public string State {  get; set; }
        [Required(ErrorMessage ="Postal Code is required")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage ="Counter is required")]
        [StringLength(50,ErrorMessage ="Country can't exceed more than 50 characters")]
        public string Country { get; set; } 
    }
}
