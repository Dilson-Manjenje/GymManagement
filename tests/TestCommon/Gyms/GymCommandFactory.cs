using GymManagement.Application.Gyms.Commands.CreateGym;
using TestCommon.TestConstants;

namespace TestCommon.Gyms;

public static class GymCommandFactory
{
    public static CreateGymCommand CreateGymCommand()
    {
        return new CreateGymCommand(Name: Constants.Gyms.Name, Address: Constants.Gyms.Address);
    }
}