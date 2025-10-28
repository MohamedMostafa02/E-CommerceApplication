using ECommerceApp.Models;
using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.DTOs.RefundDTOs
{
    // DTO for requesting a refund.
    public class RefundRequestDTO
    {
        [Required(ErrorMessage ="Cancellation ID is required.")]
        public int CancellationId { get;set; }
        [Required(ErrorMessage ="Refund Method is required.")]
        public RefundMethod RefundMethod { get;set; }
        [StringLength(500,ErrorMessage ="Refund Reason can't exceed 500 characters.")]
        public string? RefundReason { get;set; }
        [Required(ErrorMessage ="ProcessedBy is required.")]
        public int ProcessedBy  { get;set; }
    }
}
