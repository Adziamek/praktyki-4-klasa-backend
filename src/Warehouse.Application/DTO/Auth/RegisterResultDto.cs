using Microsoft.AspNetCore.Http;
using Warehouse.Domain.Entities;

namespace Warehouse.Application.DTO.Auth;

public class RegisterResultDto
{
    public User? User { get; init; }
    public string? Error { get; init; }
    public int? StatusCode { get; init; }

    public static RegisterResultDto Ok(User user)
    {
        return new RegisterResultDto
        {
            User = user,
            StatusCode = StatusCodes.Status201Created
        };
    }

    public static RegisterResultDto Fail(string error, int statusCode)
    {
        return new RegisterResultDto
        {
            Error = error,
            StatusCode = statusCode
        };
    }
}