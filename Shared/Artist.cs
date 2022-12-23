using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtistSupercharger.Shared
{
    public class Artist
    {
        public string Name { get; set; }
        public string Genre { get; set; }
        public int Popularity { get; set; }
        public int ArtistID { get; set; }
        public int Followers { get; set; }
        public string ImageUrl { get; set; }
    }
}
