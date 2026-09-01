using GymManagement.Application.Sessions.Commands.CreateSession;

namespace TestCommon.Sessions;

public static class SessionCommandFactory
{
    public static CreateSessionCommand GetCreateSessionCommand(Guid roomId,
                                   Guid trainerId,
                                   string title,
                                   DateTime? startDate = null,
                                   DateTime? endDate = null)
    {
        return new CreateSessionCommand(roomId, trainerId, title, startDate, endDate);
    }
}