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
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_locationApiBaseUrl);
            var result = client.GetAsync(buildLink(searchedPlace)).Result.Content.ReadAsStringAsync().Result;
            Location location = JsonConvert.DeserializeObject<Location>(result);

            return location; ;

        }

        private string buildLink(string city)
        {
            string url = _locationApiBaseUrl + "/find?query=" + city + "&key=" + _locationApiKey;
            return url;
        }
    }
}
