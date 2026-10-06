using GymManagement.Application.Members.Commands.CreateMember;
using GymManagement.Application.Trainers.Commands.CreateTrainer;

namespace TestCommon.Trainers;

public static class TrainerCommandFactory
{
    public static CreateTrainerCommand GetCreateTrainerCommand(Guid memberId, string name, string phone, string? email, string specialization)
    {
        return new CreateTrainerCommand(Name: name,
                                        Phone: phone,
                                        Email: email,
                                        Specialization: specialization,
                                        MemberId: memberId);
    }
}