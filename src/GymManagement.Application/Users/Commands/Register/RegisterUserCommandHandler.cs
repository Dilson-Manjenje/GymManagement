using ErrorOr;
using MediatR;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Admins;


namespace GymManagement.Application.Users.Commands.Register;

public class  RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ErrorOr<Admin>>
{
    private readonly IAdminsRepository _adminsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public RegisterUserCommandHandler(IUnitOfWork unitOfWork,
                                    IAdminsRepository adminsRepository)
    {
        _unitOfWork = unitOfWork;
        _adminsRepository = adminsRepository;
    }

    public async Task<ErrorOr<Admin>> Handle(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        var validator = new RegisterUserCommandValidator(_adminsRepository);
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => Error.Validation(code: e.PropertyName, description: e.ErrorMessage))
                .ToList();
            return errors;
        }

        // TODO: Associate Admin with Gym
        // var gym = await _gymsRepository.GetByIdAsync(command.GymId, cancellationToken);
        // if (gym is null)
        //     return GymErrors.GymNotFound(command.GymId);
        
        var newAdmin = new Admin( userName: command.UserName);  

        await _adminsRepository.AddAsync(newAdmin, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return newAdmin;
    }
}