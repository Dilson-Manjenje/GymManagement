using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Gyms;
using MediatR;

namespace GymManagement.Application.Gyms.Commands.UpdateGym;

public class UpdateGymCommandHandler : IRequestHandler<UpdateGymCommand, ErrorOr<Gym>>
{
    private readonly IGymsRepository _gymsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateGymCommandHandler(IGymsRepository gymsRepository, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _gymsRepository = gymsRepository;
    }

    public async Task<ErrorOr<Gym>> Handle(UpdateGymCommand request, CancellationToken cancellationToken)
    {
        var gym = await _gymsRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (gym is null)
            return GymErrors.GymNotFound(request.Id);

        gym.UpdateGym(request.Name, request.Address);
        await _gymsRepository.UpdateAsync(gym, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return gym;
    }
}