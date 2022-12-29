using ArtistSupercharger.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtistSupercharger.Shared
{
    public class Playlist : SpotifyBase
    {
        public bool Collaborative { get; set; }
        public string Description { get; set; }
        public ExternalUrls ExternalUrls { get; set; }
        public Followers Followers { get; set; }
        public string Href { get; set; }
        public string Id { get; set; }
        public string ImageUrl { get; set; }
        public List<Image> Images { get; set; }
        public List<Album> Albums { get; set; }
        public string Name { get; set; }
        public Owner Owner { get; set; }
        public Tracks PlaylistTracks { get; set; }
        public object PrimaryColor { get; set; }
        public bool Public { get; set; }
        public string SnapshotId { get; set; }
        public Tracks Tracks { get; set; }
        public string Type { get; set; }
        public string Uri { get; set; }
    }
}
