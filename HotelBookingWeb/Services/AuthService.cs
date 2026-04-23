using HotelBookingWeb.Data;
using HotelBookingWeb.DTOs;
using HotelBookingWeb.Models;
using HotelBookingWeb.Interfaces;
using Microsoft.EntityFrameworkCore;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwt;
    private readonly PasswordHasher _hasher;
    private readonly IEmailService _emailService;

    public AuthService(AppDbContext context, IJwtService jwt, PasswordHasher hasher, IEmailService emailService)
    {
        _context = context;
        _jwt = jwt;
        _hasher = hasher;
        _emailService = emailService;
    }

    public async Task<AuthResponseDto> Register(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
            throw new Exception("User already exists");

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = _hasher.Hash(dto.Password),
            Role = "User"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            Token = _jwt.GenerateToken(user),
            Email = user.Email,
            Role = user.Role
        };
    }

    public async Task<AuthResponseDto> Login(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (user == null || !_hasher.Verify(dto.Password, user.PasswordHash))
            return null;

        // 🔥 SEND EMAIL AFTER LOGIN
        var subject = "Login Successful";

        var body = $@"
            <h3>Welcome back 👋</h3>
            <p>You have successfully logged in.</p>
            <p><b>Email:</b> {user.Email}</p>
            <p><b>Time:</b> {DateTime.Now}</p>
        ";

        await _emailService.SendEmailAsync(user.Email, subject, body);

        return new AuthResponseDto
        {
            Token = _jwt.GenerateToken(user),
            Email = user.Email,
            Role = user.Role
        };
    }
}