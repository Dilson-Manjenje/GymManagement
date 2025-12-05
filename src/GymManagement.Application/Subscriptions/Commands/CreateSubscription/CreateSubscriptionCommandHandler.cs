using ErrorOr;
using MediatR;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Subscriptions;
using GymManagement.Domain.Admins;

namespace GymManagement.Application.Subscriptions.Commands.CreateSubscription;

public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, ErrorOr<Subscription>>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly IAdminsRepository _adminsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateSubscriptionCommandHandler(IUnitOfWork unitOfWork,
                                            ISubscriptionsRepository subscriptionsRepository,
                                            IAdminsRepository adminsRepository)

    {
        _unitOfWork = unitOfWork;
        _subscriptionsRepository = subscriptionsRepository;
        _adminsRepository = adminsRepository;
    }
    public async Task<ErrorOr<Subscription>> Handle(CreateSubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        var admin = await _adminsRepository.GetByIdAsync(command.AdminId, cancellationToken);
        if (admin is null)
            return AdminErrors.UserNotFound(command.AdminId);

        if (admin.HasActiveSubscription())
            return AdminErrors.UserAlreadyHaveActiveSubscription(adminId: admin.Id); 

        var subscription = new Subscription(subscriptionType: command.SubscriptionType,
                                            adminId: command.AdminId);

        await _subscriptionsRepository.AddSubscriptionAsync(subscription, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        
        return subscription;
    }
}