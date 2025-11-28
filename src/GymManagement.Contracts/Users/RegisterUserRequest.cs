using System.ComponentModel.DataAnnotations;

namespace GymManagement.Contracts.Users;

public record RegisterUserRequest(string UserName, string Password, Guid GymId);