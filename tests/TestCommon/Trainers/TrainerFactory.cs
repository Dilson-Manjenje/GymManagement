using GymManagement.Domain.Trainers;

namespace TestCommon.Trainers;

public static class TrainerFactory
{
    public static Trainer CreateTrainer(Guid gymId, Guid memberId, string name, string phone, string? email, string specialization)
    {
        // return new Trainer(gymId: gymId, name: name, memberId: memberId, phone: phone, specialization: specialization, email: email);
        return new Trainer(name, phone, specialization, gymId, memberId, email);
    }
}
