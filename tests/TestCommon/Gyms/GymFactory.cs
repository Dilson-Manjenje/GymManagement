using GymManagement.Domain.Gyms;
using TestCommon.TestConstants;

namespace TestCommon.Gyms;

public static class GymFactory
{
    public static Gym CreateGym(string name = Constants.Gyms.Name,
                                string address = Constants.Gyms.Address,
                                Guid? id = null)
    {
        return new Gym(
            name: name,
            address: address,
            id: id ?? Constants.Gyms.Id
        );
    }
}