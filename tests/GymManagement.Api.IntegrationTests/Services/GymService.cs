using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GymManagement.Api.Common;
using GymManagement.Api.IntegrationTests.Common;
using GymManagement.Contracts.Gyms;
using Newtonsoft.Json;

namespace GymManagement.Api.IntegrationTests.Services;

public class GymService : BaseHttpService
{
    public GymService(HttpClient client) : base(client) { }

    public async Task<GymResponse?> CreateGym(string name, string address)
    {
        var request = new GymRequest(name, address);
        var response = await PostAsync("Gyms", request);

        if (!response.IsSuccessStatusCode)
            return null;

        var resourceId = await response.Content.ReadFromJsonAsync<ResourceIdentifier>();

        var gym = await GetAsync<GymResponse>($"Gyms/{resourceId?.Id}");

        return gym!;
    }

    public async Task<GymResponse?> GetGymAsync(Guid id, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await _client.GetAsync($"{ApiEndPoints.Gyms.Base}/{id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
            return null;
            
        string content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<GymResponse>(content);
    }
}