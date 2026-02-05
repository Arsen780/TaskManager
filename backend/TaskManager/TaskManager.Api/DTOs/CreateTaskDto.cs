using System.ComponentModel.DataAnnotations;
using TaskManager.Api.Data.Entities;

namespace TaskManager.Api.DTOs
{
    public class CreateTaskDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public TaskItemStatus Status { get; set; }
        public DateTime Deadline { get; set; }
    }
}
