using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManager.Api.Data;
using TaskManager.Api.Data.Entities;
using TaskManager.Api.DTOs;
using AutoMapper;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public TaskController(AppDbContext context, IMapper mapper, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _mapper = mapper;
        _configuration = configuration;
        _webHostEnvironment = webHostEnvironment;
    }

    ////// CREATE TASK //////
    [Authorize]
    [HttpPost("CreateTask")]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDTO createTaskDto)
    {
        if (!ModelState.IsValid)
        {
            return (BadRequest(ModelState));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim.Value);

        var task = new TaskItem
        {
            Name = createTaskDto.Name,
            Description = createTaskDto.Description,
            CreationDate = DateTime.UtcNow,
            Deadline = createTaskDto.Deadline,
            Status = createTaskDto.Status,
            UserId = userId
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetTaskById),
            new { id = task.Id },
        task
        );
    }

    [HttpGet("task/{id}")]
    public async Task<IActionResult> GetTaskById(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
            return NotFound();

        return Ok(task);
    }

    ////// USUWANIE TASKA //////
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = Guid.Parse(userIdClaim.Value);

        var task = await _context.Tasks.FirstOrDefaultAsync(
            t => t.Id == id && t.UserId == userId);

        if (task == null)
            return NotFound();

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    ////// Updaate taska //////
    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateTask (Guid id, [FromBody] UpdateTaskDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = Guid.Parse(userIdClaim.Value);

        var task = await _context.Tasks.FirstOrDefaultAsync(
            t => t.Id == id && t.UserId == userId);

        if(task == null)
        {
            return NotFound();
        }

        task.Name = dto.Name;
        task.Description = dto.Description;
        task.Status = dto.Status;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    ////// Zmiana statusu //////
    [HttpPatch("{id:guid}/status")]
    [Authorize]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTaskStatusDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = Guid.Parse(userIdClaim.Value);

        var task = await _context.Tasks.FirstOrDefaultAsync(
            t => t.Id == id && t.UserId == userId);

        if(task == null)
        {
            return NotFound();
        }

        task.Status = dto.Status;

        await _context.SaveChangesAsync();

        return NoContent();
    }



}
