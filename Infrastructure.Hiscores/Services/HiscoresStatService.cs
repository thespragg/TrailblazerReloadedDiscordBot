using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using Domain.Contracts;
using Domain.Models;

namespace Infrastructure.Hiscores.Services;

public class HiscoresStatService : IStatService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://secure.runescape.com/";
    private const string StatUrl = "m=hiscore_oldschool_seasonal/index_lite.ws?player=";
    
    public HiscoresStatService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    public async Task<PlayerStats> GetStats(string username)
    {
        var res = await _httpClient.GetAsync(StatUrl + username);
        var content = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PlayerStats>(content)!;
    }
}