using GymManagement.Domain.Common;

namespace GymManagement.Domain.Sessions.Events;

public record SessionCanceledEvent(Guid SessionId): IDomainEvent;
public record SessionFinalizedEvent(Guid SessionId): IDomainEvent;