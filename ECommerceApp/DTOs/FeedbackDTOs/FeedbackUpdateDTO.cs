using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.DTOs.FeedbackDTOs
{
    // DTO for updating feedback
    public class FeedbackUpdateDTO
    {
        [Required(ErrorMessage ="FeedbackId is required.")]
        public int FeedbackId { get; set; }
        [Required(ErrorMessage ="CustomerId is required.")]
        public int CustomerId { get; set; }
        [Required(ErrorMessage ="Rating is required.")]
        [Range(1,5,ErrorMessage ="Rating must be between 1 and 5.")]
        public int Rating { get; set; }
        [StringLength(1000,ErrorMessage ="Comment can't exceed 1000 characters.")]
        public string Comment { get; set; }
    }
}
