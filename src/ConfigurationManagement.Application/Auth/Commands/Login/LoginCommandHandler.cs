using ConfigurationManagement.Application.Auth.Dto;
using ConfigurationManagement.Domain.Abstractions;
using ConfigurationManagement.Infrastructure.Persistence.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConfigurationManagement.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthDto>
{
    private readonly ConfigurationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        ConfigurationDbContext context,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILogger<LoginCommandHandler> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<AuthDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken) 
                ?? throw new UnauthorizedAccessException("Invalid email or password.");
            
            if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            await _context.SaveChangesAsync(cancellationToken);

            var token = _tokenService.GenerateToken(user.Id, user.Email, user.FullName);
            var expiresAt = DateTime.UtcNow.AddMinutes(60);

            _logger.LogInformation("User {UserId} logged in successfully", user.Id);

            return new AuthDto
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Token = token,
                ExpiresAt = expiresAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed login attempt for email {Email}", request.Email);
            throw;
        }
    }
}