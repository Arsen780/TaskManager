using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.DTOs
{
    public class CreateUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression(
            @"^(?=.*[A-Z])(?=.*\d).{8,}$",
            ErrorMessage = "Hasło musi mieć co najmniej 8 znaków, w tym co najmniej jedną dużą literę i jedną cyfrę.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana!")]
        [MinLength(6, ErrorMessage = "Nazwa użytkownika musi mieć conajmniej 6 znaków!")]
        [MaxLength(25, ErrorMessage = "Maksymalna długość dla nazwy użytkownika wynosi 25 znaków!")]
        public string Username { get; set; }
    }
}
