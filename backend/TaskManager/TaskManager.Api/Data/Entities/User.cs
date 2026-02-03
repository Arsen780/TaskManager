using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Api.Data.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string Username {get;set;}
        public string? VerificationToken { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModificateDate { get; set; }
        public DateTime? VerificationDate { get; set; }
    }
}
