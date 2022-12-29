using ArtistSupercharger.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using ArtistSupercharger.Server.SpofiyRepositories;
using ArtistSupercharger.Shared.Responses;

namespace ArtistSupercharger.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArtistController : ControllerBase
    {

        private readonly ILogger<ArtistController> _logger;
        private readonly ISpotifyRepository _spotifyRepo;
        public ArtistController(ILogger<ArtistController> logger, ISpotifyRepository spotifyRepo)
        {
            _logger = logger;
            _spotifyRepo = spotifyRepo;
        }

        [HttpGet]
        public async Task<ActionResult<SuperChargerArtist>> GetArtist(string artistName)
        {
            var accesstoken = _spotifyRepo.GetAccessToken();

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);
            HttpResponseMessage response = await client.GetAsync("https://api.spotify.com/v1/search?q=" + artistName + "&type=artist&market=SE&limit=1");

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();


                SpotifyArtistResponse spotifyArtistResponse = JsonConvert.DeserializeObject<SpotifyArtistResponse>(responseBody);

                SuperChargerArtist artist = new SuperChargerArtist
                {
                    Name = spotifyArtistResponse.artists.items[0].name,
                    ArtistID = spotifyArtistResponse.artists.items[0].id,
                    Followers = spotifyArtistResponse.artists.items[0].followers.total,
                    Popularity = spotifyArtistResponse.artists.items[0].popularity,
                    Genres = new List<string>(),
                    ImageUrl = spotifyArtistResponse.artists.items[0].images[0].url
                };

                foreach (var genre in spotifyArtistResponse.artists.items[0].genres)
                {
                    artist.Genres.Add(genre);
                }


                response = await client.GetAsync("https://api.spotify.com/v1/artists/" + artist.ArtistID + "/albums");

                responseBody = await response.Content.ReadAsStringAsync();
                var albums = JsonConvert.DeserializeObject<SpotifyAlbumResponse>(responseBody);
                artist.Albums = new List<SuperChargerAlbum>();
                var albumItems = albums.Items.ToList();
                foreach (var album in albumItems)
                {
                    if (album.album_type == "album")
                    {
                        artist.Albums.Add(new SuperChargerAlbum
                        {
                            Type = album.album_type,
                            Id = album.id,
                            ExternalUrls = album.external_urls,
                            Href = album.href,
                            TotalTracks = album.total_tracks,
                            Uri = album.uri,
                            Images = album.images,
                            Name = album.name,
                            ReleaseDate = album.release_date,
                            ReleaseDatePrecision = album.release_date_precision
                        });
                    }
                }

                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.DefaultRequestHeaders.Authorization = null;
                client.DefaultRequestHeaders.Add("User-Agent", "Other");

                if (artist.Albums.Count > 1)
                {
                    var albumId = artist.Albums.First().Id;

                    var albumInfoUrl = $"https://api.t4ils.dev/albumPlayCount?albumid={albumId}";

                    response = await client.GetAsync(albumInfoUrl);

                    responseBody = await response.Content.ReadAsStringAsync();
                    var albumInfo = JsonConvert.DeserializeObject<AlbumInfo>(responseBody);
                    var albumTracks = albumInfo.data;
                    artist.AlbumTracks = new List<Track>();
                    var albumLength = artist.Albums.First().TotalTracks;
                    for (int i = 0; i < albumLength; i++)
                    {
                        foreach (var track in albumTracks.discs)
                        {
                            // track.tracks[i].duration = (track.tracks[i].duration / 60000);
                            artist.AlbumTracks.Add(track.tracks[i]);
                        }
                    }


                    if (!string.IsNullOrEmpty(albumInfo.data.label))
                    {
                        artist.Albums.First().Label = albumInfo.data.label;
                    }

                    return artist;
                }
                return artist;
            }
            else
            {
                return null;
            }
        }
    }
}