namespace GymManagement.Contracts.Members;

public record RegisterUserRequest(string UserName, string Password, Guid GymId);