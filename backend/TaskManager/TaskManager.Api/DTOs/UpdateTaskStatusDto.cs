using TaskManager.Api.Data.Entities;

namespace TaskManager.Api.DTOs
{
    public class UpdateTaskStatusDto
    {
        public TaskItemStatus Status { get; set; }
    }
}
