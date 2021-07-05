using System;
using System.Collections.Generic;
using System.Text;

namespace JobScraper.Models
{
    class Coordinate
    {
        public DEGMINSECLNG DEGMINSECLNG { get; set; }
        public string EASTING { get; set; }
        public string LONGITUDE { get; set; }
        public string NORTHING { get; set; }
        public DEGMINSECLAT DEGMINSECLAT { get; set; }
        public string LATITUDE { get; set; }
    }

    public class DEGMINSECLNG
    {
        public int DEGREES { get; set; }
        public float SECONDS { get; set; }
        public int MINUTES { get; set; }
    }

    public class DEGMINSECLAT
    {
        public int DEGREES { get; set; }
        public float SECONDS { get; set; }
        public int MINUTES { get; set; }
    }
}

