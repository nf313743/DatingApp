using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public sealed record RegisterDto(
    string UserName, 
    [StringLength(8, MinimumLength = 4)]string Password);