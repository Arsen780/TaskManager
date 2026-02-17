using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.DTOs
{
    public class ChangePasswordDto
    {
        [EmailAddress]
        public string CurrentPassword { get; set; }

        [Required]
        [RegularExpression(
        @"^(?=.*[A-Z])(?=.*\d).{8,}$",
        ErrorMessage = "Hasło musi mieć co najmniej 8 znaków, w tym co najmniej jedną dużą literę i jedną cyfrę.")]
        public string NewPassword { get; set; }
    }
}
