using FluentAssertions;
using GymManagement.Domain.Gyms;
using GymManagement.Domain.Rooms;
using TestCommon.Gyms;
using TestCommon.Rooms;
using TestCommon.TestConstants;

namespace GymManagement.Domain.UnitTests.Rooms;

public class RoomTests
{

    [Fact]
    public void Create_ReturnValidRoom()
    {
        var gym = GymFactory.GetFightGym();

        var room = new Room("Spinning", 2, gym.Id);

        room.Id.Should().NotBeEmpty();
        room.Id.Should<Guid>().NotBeNull();

    }


    [Fact]
    public void Update_ChangeRoomName()
    {
        var gym = new Gym(name: "FightClub", address: "Cassenda");

        gym.UpdateGym(name: Constants.Gyms.Name, address: Constants.Gyms.Address);

        // Assert
        gym.Name.Should().Match(Constants.Gyms.Name);
        gym.Address.Should().Match(Constants.Gyms.Address);
        gym.Id.Should().NotBeEmpty();
        gym.Id.Should<Guid>().NotBeNull();
    }

    [Fact]
    public void Disable_DisableRoom()
    {
        var room = RoomFactory.GetKickBoxigRoomOfFightGym();

        var result = room.DisableRoom();
        // Assert
        result.IsError.Should().BeFalse();
        room.IsAvailable.Should().BeFalse();
    }
}

