using AutoMapper;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TaskManager.Api.Data;
using TaskManager.Api.Data.Entities;
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

    [HttpPost("Registration")]
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
            CreatedDate = DateTime.UtcNow,
            ModificateDate = DateTime.UtcNow,
            VerificationDate = null
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        try
        {
            await SendVerificationEmail(newUser, newUser.VerificationToken);
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Nie udało się wysłać e-maila weryfikacyjnego do {newUser.Email}: {ex.Message}");
        }
        return StatusCode(201, new { message = "Rejestracja pomyślna. Sprawdź swoją skrzynkę e-mail, aby aktywować konto." });
    }
    private string CreateRandomToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
    }

    private async Task SendVerificationEmail(User user, string token)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");

        var smtpServer = emailSettings.GetValue<string>("SmtpServer");
        var port = emailSettings.GetValue<int>("Port");
        var username = emailSettings.GetValue<string>("Username");
        var password = emailSettings.GetValue<string>("Password");
        var senderName = emailSettings.GetValue<string>("SenderName");
        var senderEmail = emailSettings.GetValue<string>("SenderEmail");

        var verificationLink = $"http://localhost:5173/verify-email/{token}";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(senderName, senderEmail));
        message.To.Add(new MailboxAddress(user.Username, user.Email));
        message.Subject = "Potwierdź swój adres e-mail";

        message.Body = new TextPart("html")
        {
            Text = $@"
            <p>Witaj {user.Username},</p>
            <p>Aby aktywować konto, kliknij link:</p>
            <a href='{verificationLink}'>Aktywuj konto</a>"
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(username, password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
    ////// Weryfikacja maila //////

    [HttpPost("VerifyEmail")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyDto verifydto)
    {
        if (string.IsNullOrEmpty(verifydto.Token))
        {
            return BadRequest(new { message = "Token jest wymagany!" });
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == verifydto.Token);

        if (user == null)
        {
            return BadRequest(new { message = "Nieprawidłowy token weryfikacyjny!" });
        }

        if (user.VerificationDate != null)
        {
            return BadRequest(new { message = "To konto zostało już zweryfikowane!" });
        }

        user.VerificationDate = DateTime.UtcNow;
        user.VerificationToken = null;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Konto zostało pomyślnie zweryfikowane. Możesz się teraz zalogować." });
    }


    ////////////  LOGIN  ///////////////////

        [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == loginDto.Username.ToLower());
        if(user != null && user.VerificationDate == null)
        {
            return Unauthorized(new { message = "Konto nie zostało aktywowane za pomoca adresu email!" });
        }

        if(user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.HashedPassword))
        {
            return Unauthorized(new { message = "Błędne hasło, lub login!" });
        }

        var userDto = _mapper.Map<UserDto>(user);
        var token = GenerateJwtToken(user);
        return Ok(new { token = token, user = userDto });
    }
    /// JWT ///
    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddDays(1);
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
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
            new {id = task.Id},
        task
        );
    }

    [HttpGet("task/{id}")]
    public async Task<IActionResult> GetTaskById (Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
            return NotFound();

        return Ok(task);
    }


}
