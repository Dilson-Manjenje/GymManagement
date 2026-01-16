using ErrorOr;
using MediatR;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Members;
using GymManagement.Domain.Gyms;


namespace GymManagement.Application.Members.Commands.CreateMember;

public class  CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, ErrorOr<Guid>>
{
    private readonly IMembersRepository _membersRepository;
    private readonly IGymsRepository _gymsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateMemberCommandHandler(IUnitOfWork unitOfWork,
                                    IMembersRepository membersRepository,
                                    IGymsRepository gymsRepository)
    {
        _unitOfWork = unitOfWork;
        _membersRepository = membersRepository;
        _gymsRepository = gymsRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateMemberCommand command, CancellationToken cancellationToken = default)
    {       
        var gym = await _gymsRepository.GetByIdAsync(command.GymId, cancellationToken);
        if (gym is null)
             return GymErrors.GymNotFound(command.GymId);
        
        var newMember = new Member( gymId: command.GymId, userName: command.UserName);  
        await _membersRepository.AddAsync(newMember, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return newMember.Id;
    }
}