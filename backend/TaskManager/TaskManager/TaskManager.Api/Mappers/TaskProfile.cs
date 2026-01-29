namespace TaskManager.Api.Mappers
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<Task, TaskDto>();
            CreateMap<CreateTaskDto, Task>();
        }
    }
}
