using ArtistSupercharger.Client.Pages;
using ArtistSupercharger.Server.SpofiyRepositories;
using ArtistSupercharger.Shared;
using ArtistSupercharger.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ArtistSupercharger.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlaylistsController : Controller
    {
        private readonly ILogger<PlaylistsController> _logger;
        private readonly ISpotifyRepository _spotifyRepo;

        public PlaylistsController(ILogger<PlaylistsController> logger, ISpotifyRepository spotifyRepo)
        {
            _logger = logger;
            _spotifyRepo = spotifyRepo;
        }

        [HttpGet]
        public async Task<ActionResult<Playlist>> GetPlaylist(string playlistID)
        {
            var accesstoken = _spotifyRepo.GetAccessToken();

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);
            HttpResponseMessage response = await client.GetAsync("https://api.spotify.com/v1/playlists/" + playlistID);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
                SpotifyPlaylistResponse spotifyPlaylistResponse = JsonConvert.DeserializeObject<SpotifyPlaylistResponse>(responseBody);


                Playlist playlists = new Playlist
                {
                    Name = spotifyPlaylistResponse.name,
                    Description = spotifyPlaylistResponse.description,
                    ImageUrl = spotifyPlaylistResponse.images[0].url,
                    PlaylistTracks = spotifyPlaylistResponse.tracks
                    
                };
     


                return playlists;
            }

            return null;
        }
    }
}
