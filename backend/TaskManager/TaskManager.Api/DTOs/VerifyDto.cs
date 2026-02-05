using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.DTOs
{
    public class VerifyDto
    {
        [Required]
        public string Token { get; set; }
    }
}
