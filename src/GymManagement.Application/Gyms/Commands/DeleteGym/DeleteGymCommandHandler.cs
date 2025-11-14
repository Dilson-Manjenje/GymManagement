using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Gyms;
using MediatR;

namespace GymManagement.Application.Gyms.Commands.DeleteGym;

public class DeleteGymCommandHandler : IRequestHandler<DeleteGymCommand, ErrorOr<Deleted>>
{
    private readonly IGymsRepository _gymsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public DeleteGymCommandHandler(IGymsRepository gymsRepository,
                                IUnitOfWork unitOfWork)
    {
        _gymsRepository = gymsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Deleted>> Handle(DeleteGymCommand command, CancellationToken cancellationToken = default)
    {
        var gym = await _gymsRepository.GetByIdAsync(command.Id, cancellationToken);
        
        if (gym is null)
            return GymErrors.GymNotFound(command.Id);
        
        // TODO: remove all rooms and sessions from gym 
        await _gymsRepository.RemoveAsync(gym, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
    