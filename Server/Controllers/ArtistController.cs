using ArtistSupercharger.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace ArtistSupercharger.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArtistController : ControllerBase
    {

        private readonly ILogger<ArtistController> _logger;

        public ArtistController(ILogger<ArtistController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<Artist>> GetArtist(string artistName)
        {
            var accesstoken = GetAccessToken();

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);
            HttpResponseMessage response = await client.GetAsync("https://api.spotify.com/v1/search?q=" + artistName + "&type=artist&market=SE&limit=1");

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();


                SpotifyArtistResponse spotifyArtistResponse = JsonConvert.DeserializeObject<SpotifyArtistResponse>(responseBody);

                Artist artist = new Artist
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
                artist.Albums = new List<Album>();
                var albumItems = albums.Items.ToList();
                foreach (var album in albumItems)
                {
                    if (album.album_type == "album")
                    {
                        artist.Albums.Add(new Album
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
                            ReleaseDatePrecision = album.release_date_precision,
                        });
                    }
                }

                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.DefaultRequestHeaders.Authorization = null;
                client.DefaultRequestHeaders.Add("User-Agent", "Other");
                var albumId = artist.Albums.FirstOrDefault().Id;
                var url = "https://api.t4ils.dev/albumPlayCount?albumid=" + albumId;
                response = await client.GetAsync(url);

                responseBody = await response.Content.ReadAsStringAsync();
                var albumInfo = JsonConvert.DeserializeObject<AlbumInfo>(responseBody);
                var albumTracks = albumInfo.data;
                artist.AlbumTracks = new List<Track>();
                var albumLength = artist.Albums.FirstOrDefault().TotalTracks;
                for (int i = 0; i < albumLength; i++)
                {
                    foreach (var track in albumTracks.discs)
                    {
                        artist.AlbumTracks.Add(track.tracks[i]);
                    }
                }



                return artist;
            }
            else
            {
                return null;
            }
        }


        public string GetAccessToken()
        {
            SpotifyToken token = new SpotifyToken();
            string url5 = "https://accounts.spotify.com/api/token";
            var clientid = "bfcd4b2e6b1c4d2a98003cd51257b1b4";
            var clientsecret = "84d633a35f7c4eb285235aad12b4765c";

            //request to get the access token
            var encode_clientid_clientsecret = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", clientid, clientsecret)));

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url5);

            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Accept = "application/json";
            webRequest.Headers.Add("Authorization: Basic " + encode_clientid_clientsecret);

            var request = ("grant_type=client_credentials");
            byte[] req_bytes = Encoding.ASCII.GetBytes(request);
            webRequest.ContentLength = req_bytes.Length;

            Stream strm = webRequest.GetRequestStream();
            strm.Write(req_bytes, 0, req_bytes.Length);
            strm.Close();

            HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
            String json = "";
            using (Stream respStr = resp.GetResponseStream())
            {
                using (StreamReader rdr = new StreamReader(respStr, Encoding.UTF8))
                {
                    string responseFromServer = rdr.ReadToEnd();
                    token = JsonConvert.DeserializeObject<SpotifyToken>(responseFromServer);
                }
            }
            return token.access_token;
        }
    }
}