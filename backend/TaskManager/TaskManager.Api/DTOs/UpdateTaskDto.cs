using System;
using System.Threading.Tasks;
using TaskManager.Api.Data.Entities;

namespace TaskManager.Api.DTOs
{
    public class UpdateTaskDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskItemStatus Status { get; set; }
    }
}
