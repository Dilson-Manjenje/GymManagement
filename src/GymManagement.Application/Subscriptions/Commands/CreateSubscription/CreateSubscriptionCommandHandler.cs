using ErrorOr;
using MediatR;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Subscriptions;
using GymManagement.Domain.Members;
using GymManagement.Domain.Gyms;

namespace GymManagement.Application.Subscriptions.Commands.CreateSubscription;

public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, ErrorOr<Guid>>
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
    public async Task<ErrorOr<Guid>> Handle(CreateSubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        var member = await _membersRepository.GetByIdAsync(command.MemberId, cancellationToken);
        if (member is null)
            return MemberErrors.MemberNotFound(command.MemberId);

        if (member.GymId is null || member.GymId.Value == Guid.Empty)
            return MemberErrors.MemberDontHaveGym(userName: member.UserName, memberId: member.Id);

        var gymId = member.GymId.Value;        
        var gym = await _gymsRepository.GetByIdAsync(gymId, cancellationToken);

        if (gym is null)
            return GymErrors.GymNotFound(gymId);

        bool hasActiveSubscription = await _subscriptionsRepository.HasActiveSubscription(memberId: command.MemberId);

        if (hasActiveSubscription)
            return MemberErrors.MemberAlreadyHaveActiveSubscription(memberId: member.Id); 
        
        var subscription = new Subscription(subscriptionType: command.SubscriptionType,
                                            memberId: command.MemberId);

        await _subscriptionsRepository.AddAsync(subscription, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        
        return subscription.Id;
    }
}