using TaskManager.Api.Data.Entities;

namespace TaskManager.Api.DTOs
{
    public class GetTasksQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; } = "asc";
        public TaskItemStatus? Status { get; set; }
    }
}
