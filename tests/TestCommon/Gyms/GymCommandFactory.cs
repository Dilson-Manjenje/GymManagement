using GymManagement.Application.Gyms.Commands.CreateGym;
using TestCommon.TestConstants;

namespace TestCommon.Gyms;

public static class GymCommandFactory
{
    public static CreateGymCommand CreateGymCommand(string? name = null, string? address = null)
    {
        return new CreateGymCommand(Name: name!,
                                    Address: address!);
    }
}