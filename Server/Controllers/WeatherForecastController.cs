using ArtistSupercharger.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace ArtistSupercharger.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<Artist> GetArtist(string artistName)
        {

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://api.spotify.com/v1/search?q="+artistName+ "&type=artist&market=SE&limit=1");

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();


                Artist artist = JsonConvert.DeserializeObject<Artist>(responseBody);
                return artist;

            }
            else
            {
                return null;
            }
        }
    }
}