namespace GymManagement.Contracts.Members;

public record CreateMemberRequest(string UserName,
                                  string Password,
                                  Guid GymId);

public record UpdateMemberRequest(string UserName,
                                  string? Password);