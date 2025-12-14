using ErrorOr;
using MediatR;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Subscriptions;
using GymManagement.Domain.Members;
using GymManagement.Domain.Gyms;

namespace GymManagement.Application.Subscriptions.Commands.CreateSubscription;

public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, ErrorOr<Subscription>>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly IMembersRepository _membersRepository;
    private readonly IGymsRepository _gymsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateSubscriptionCommandHandler(IUnitOfWork unitOfWork,
                                            IGymsRepository gymsRepository,
                                            ISubscriptionsRepository subscriptionsRepository,
                                            IMembersRepository membersRepository)

    {
        _unitOfWork = unitOfWork;
        _subscriptionsRepository = subscriptionsRepository;
        _membersRepository = membersRepository;
        _gymsRepository = gymsRepository;
    }
    public async Task<ErrorOr<Subscription>> Handle(CreateSubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        var member = await _membersRepository.GetByIdAsync(command.MemberId, cancellationToken);
        if (member is null)
            return MemberErrors.UserNotFound(command.MemberId);

        if (member.HasActiveSubscription())
            return MemberErrors.UserAlreadyHaveActiveSubscription(memberId: member.Id); 

        if (member.GymId is null || member.GymId!.Value == Guid.Empty)
            return MemberErrors.UserDontHaveGym(member.UserName, member.Id);
        
        var gymId = member.GymId!.Value;
        var gym = await _gymsRepository.GetByIdAsync(gymId, cancellationToken);
        if (gym is null)
            return GymErrors.GymNotFound(gymId);


        var subscription = new Subscription(subscriptionType: command.SubscriptionType,
                                            memberId: command.MemberId);

        await _subscriptionsRepository.AddAsync(subscription, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        
        return subscription;
    }
}