using FluentAssertions;
using GymManagement.Domain.Gyms;
using TestCommon.TestConstants;

namespace GymManagement.Domain.UnitTests.Gyms;

public class GymTests
{

    [Fact]
    public void Create_ReturnValidGym()
    {
        var gym = new Gym(name: "FightClub", address: "Prenda");

        gym.Id.Should().NotBeEmpty();
        gym.Id.Should<Guid>().NotBeNull();

    }
    
    [Fact]
    public void Update_ChangeGymName()
    {
        var gym = new Gym(name: "FightClub", address: "Cassenda");

        gym.UpdateGym(name: Constants.Gyms.Name, address: Constants.Gyms.Address);

        // Assert
        gym.Name.Should().Match(Constants.Gyms.Name);
        gym.Address.Should().Match(Constants.Gyms.Address);
        gym.Id.Should().NotBeEmpty();
        gym.Id.Should<Guid>().NotBeNull();        
    }

}

