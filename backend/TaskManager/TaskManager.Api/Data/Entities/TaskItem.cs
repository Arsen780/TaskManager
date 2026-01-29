using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Api.Data.Entities
{
    [Table("Tasks")]
    public class TaskItem
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public DateTime Deadline { get; set;}
        public TaskStatus Status { get; set; }

    }
}
