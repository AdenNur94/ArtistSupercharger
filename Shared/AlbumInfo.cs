using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtistSupercharger.Shared
{
    public class AlbumInfo
    {
        public bool success { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string uri { get; set; }
        public string name { get; set; }
        public Cover cover { get; set; }
        public int year { get; set; }
        public int track_count { get; set; }
        public string label { get; set; }
        public List<Disc> discs { get; set; }
    }

    public class Cover
    {
        public string uri { get; set; }
    }

    public class Disc
    {
        public int number { get; set; }
        public string name { get; set; }
        public List<Track> tracks { get; set; }
    }

    public class Track
    {
        public string uri { get; set; }
        public int playcount { get; set; }
        public string name { get; set; }
        public int popularity { get; set; }
        public int number { get; set; }
        public int duration { get; set; }
        public bool explicitContent { get; set; }
        public bool playable { get; set; }
        public List<Artist> artists { get; set; }
    }

}
