using AutoMapper;
using TaskManager.Api.Data.Entities;
using TaskManager.Api.DTOs;

namespace TaskManager.Api.Mappers;

public class TaskProfile : Profile
{
    public TaskProfile()
    {
        CreateMap<TaskItem, TaskDto>();
        CreateMap<CreateTaskDTO, TaskItem>();
    }
}
