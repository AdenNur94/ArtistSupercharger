using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtistSupercharger.Shared
{
    public class SuperChargerArtist
    {
        public string Name { get; set; }
        public List<string> Genres { get; set; }
        public int Popularity { get; set; }
        public string ArtistID { get; set; }
        public int Followers { get; set; }
        public string ImageUrl { get; set; }

        public List<SuperChargerAlbum> Albums { get; set; }
        public List<Track> AlbumTracks { get; set; }
    }
}
