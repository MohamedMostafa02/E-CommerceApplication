using ECommerceApp.Models;
using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.DTOs.CancellationDTOs
{
    // DTO for updating cancellation status
    public class CancellationStatusUpdateDTO
    {
        [Required(ErrorMessage ="Cancellaion Id is required.")]
        public int CancellationId { get; set; }
        [Required]
        public CancellationStatus Status { get; set; }
        // Optional: Admin ID who is processijng the cancellation
        public int? ProcessedBy { get; set; }
        // Any Cancellation charges apply (if any)
        [Range(0,double.MaxValue,ErrorMessage ="CancellationCharges must be non-negative")]
        public decimal? CancellationCharges { get; set; }
        [StringLength(500,ErrorMessage ="Remarks can't exceed 500 characters.")]
        public string Remarks   { get; set; }
    }
}
