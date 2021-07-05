using JobScraper.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JobScraper
{
    class LocationService
    {
        private string _locationApiBaseUrl = "https://api.os.uk/search/names/v1/";
        private string _locationApiKey = "U6319wpJWaGGa1HHqjFcUBrB76qMZw4x";

        public Location getlocationData(string searchedPlace)
        {
            Location location;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_locationApiBaseUrl);
                var result = client.GetAsync(buildLink(searchedPlace)).Result.Content.ReadAsStringAsync().Result;
                location = JsonConvert.DeserializeObject<Location>(result);
            }
            return location; ;

        }

        private string buildLink(string city)
        {
            // https://api.os.uk/search/names/v1/find?query=Glasgow&fq=LOCAL_TYPE:City LOCAL_TYPE:Bay&key=
            string url = _locationApiBaseUrl + "/find?query=" + city + "&fq=LOCAL_TYPE:City LOCAL_TYPE:Village LOCAL_TYPE:Town&key=" + _locationApiKey;
            return url;
        }

        /// <summary>
        /// Convert British National Grid (BNG) to Latitute & Longitude
        /// </summary>
        /// <param name="easting"></param>
        /// <param name="northing"></param>
        /// <returns></returns>
        /// E.G. http://webapps.bgs.ac.uk/data/webservices/CoordConvert_LL_BNG.cfc?method=BNGtoLatLng&easting=429157&northing=623009
        public Coordinate convertBNGToLat_Lon(string easting, string northing)
        {
            string baseUrl = "http://webapps.bgs.ac.uk/data/webservices/CoordConvert_LL_BNG.cfc";
            string url = baseUrl+ "?method=BNGtoLatLng&easting=" + easting + "&" + "northing=" + northing;

            Coordinate result;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                result = JsonConvert.DeserializeObject<Coordinate>(client.GetAsync(url).Result.Content.ReadAsStringAsync().Result);
            }

            return result;
        }
    }
}
