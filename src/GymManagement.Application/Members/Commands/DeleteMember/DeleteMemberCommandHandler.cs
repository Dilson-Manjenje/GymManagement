using ErrorOr;
using MediatR;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Members;
using GymManagement.Domain.Gyms;


namespace GymManagement.Application.Members.Commands.DeleteMember;

public class  DeleteMemberCommandHandler : IRequestHandler<DeleteMemberCommand, ErrorOr<Unit>>
{
    private readonly IMembersRepository _membersRepository;
    private readonly IUnitOfWork _unitOfWork;
    public DeleteMemberCommandHandler(IUnitOfWork unitOfWork,
                                    IMembersRepository membersRepository)
    {
        _unitOfWork = unitOfWork;
        _membersRepository = membersRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteMemberCommand command, CancellationToken cancellationToken = default)
    {       
        var member = await _membersRepository.GetByIdAsync(command.MemberId, cancellationToken);
        if (member is null)
             return MemberErrors.MemberNotFound(command.MemberId);
        
        await _membersRepository.RemoveAsync(member, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}