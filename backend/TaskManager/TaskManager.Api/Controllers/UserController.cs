using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using TaskManager.Api.Data;
using TaskManager.Api.DTOs;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public UserController(AppDbContext context, IMapper mapper, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _mapper = mapper;
        _configuration = configuration;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpPost("registration")]
    public async Task<IActionResult> Registration([FromBody] CreateUserDto createUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if(await _context.Users.AnyAsync(u => u.Username.ToLower() == createUserDto.Username.ToLower()))
        {
            return Conflict(new { message = "Użytkownik o podanej nazwie już istnieje!" });
        }

        if(await _context.Users.AnyAsync(u => u.Email.ToLower() == createUserDto.Email.ToLower()))
        {
            return Conflict(new { message = "Podany adres email jest już używany!" });
        }

        var newUser = new User
        {
            Email = createUserDto.Email,
            Username = createUserDto.Username,
            HashedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
            VerificationToken = CreateRandomToken(),
            CreateDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow,
            VerificationDate = null
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        try
        {
            //await SendVerificationEmail(newUser, newUser.VerificationToken);
        }

        catch(Exception ex)
        {
            Console.WriteLine($"Nie udało się wysłać e-maila weryfikacyjnego do {newUser.Email}: {ex.Message}");
        }
        return StatusCode(201, new { message = "Rejestracja pomyślna. Sprawdź swoją skrzynkę e-mail, aby aktywować konto." });
    }


    private string CreateRandomToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
    }

}








