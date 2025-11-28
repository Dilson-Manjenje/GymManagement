using ErrorOr;
using MediatR;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Admins;
using GymManagement.Domain.Gyms;


namespace GymManagement.Application.Users.Commands.Register;

public class  RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ErrorOr<Admin>>
{
    private readonly IAdminsRepository _adminsRepository;
    private readonly IGymsRepository _gymsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public RegisterUserCommandHandler(IUnitOfWork unitOfWork,
                                    IAdminsRepository adminsRepository,
                                    IGymsRepository gymsRepository)
    {
        _unitOfWork = unitOfWork;
        _adminsRepository = adminsRepository;
        _gymsRepository = gymsRepository;
    }

    public async Task<ErrorOr<Admin>> Handle(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {       
        var gym = await _gymsRepository.GetByIdAsync(command.GymId, cancellationToken);
        if (gym is null)
             return GymErrors.GymNotFound(command.GymId);
        
        var newAdmin = new Admin( gymId: command.GymId, userName: command.UserName);  
        await _adminsRepository.AddAsync(newAdmin, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return newAdmin;
    }
}