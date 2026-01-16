using ErrorOr;
using MediatR;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Members;
using GymManagement.Domain.Gyms;


namespace GymManagement.Application.Members.Commands.UpdateMember;

public class  UpdateMemberCommandHandler : IRequestHandler<UpdateMemberCommand, ErrorOr<Guid>>
{
    private readonly IMembersRepository _membersRepository;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateMemberCommandHandler(IUnitOfWork unitOfWork,
                                    IMembersRepository membersRepository)
    {
        _unitOfWork = unitOfWork;
        _membersRepository = membersRepository;        
    }

    public async Task<ErrorOr<Guid>> Handle(UpdateMemberCommand command, CancellationToken cancellationToken = default)
    {       
        var member = await _membersRepository.GetByIdAsync(command.Id, cancellationToken);
        if (member is null)
            return MemberErrors.MemberNotFound(command.Id);

        var result = member.Update(command.UserName,
                                   command.Password);

        if (result.IsError)
            return result.Errors;
        
        await _membersRepository.UpdateAsync(member, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return member.Id;
    }
}