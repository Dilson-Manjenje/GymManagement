using GymManagement.Api.IntegrationTests.Common;
using GymManagement.Api.Common;
using System.Net.Http.Json;
using GymManagement.Contracts.Rooms;
using Newtonsoft.Json;

namespace GymManagement.Api.IntegrationTests.Services;

public class RoomService : BaseHttpService
{
    public RoomService(HttpClient client) : base(client) { }

    public async Task<RoomResponse?> GetRoomAsync(Guid id, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await _client.GetAsync($"{ApiEndPoints.Rooms.Base}/{id}", cancellationToken);
        
        if (!response.IsSuccessStatusCode)
            return null;

        //var resourceId = await response.Content.ReadFromJsonAsync<ResourceIdentifier>();
        string content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<RoomResponse>(content);
    }

    public async Task<RoomResponse?> CreateRoom(CreateRoomRequest request, CancellationToken cancellationToken = default)
    {   
        HttpResponseMessage response = await _client.PostAsJsonAsync($"{ApiEndPoints.Rooms.Base}", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return null;

        var resourceIdentifier = await response.Content.ReadFromJsonAsync<ResourceIdentifier>();

        if (resourceIdentifier is null)
            return null;

        var room = await GetRoomAsync(resourceIdentifier.Id, cancellationToken);

        return room;
    }
   
}