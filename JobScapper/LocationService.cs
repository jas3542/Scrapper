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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cityname"></param>
        /// <returns></returns>

        private string _locationApiBaseUrl = "https://api.os.uk/search/names/v1/";
        private string _locationApiKey = "U6319wpJWaGGa1HHqjFcUBrB76qMZw4x";
        public Location getPostcodeByCityname(string cityName)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_locationApiBaseUrl);
            var result = client.GetAsync(buildLinkForCityname(cityName)).Result.Content.ReadAsStringAsync().Result;
            Location location = JsonConvert.DeserializeObject<Location>(result);

            return location;
        }

        public Location getCitynameByPostcode(string postCode)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_locationApiBaseUrl);
            var result = client.GetAsync(buildLinkForPostcode(postCode)).Result.Content.ReadAsStringAsync().Result;
            Location location = JsonConvert.DeserializeObject<Location>(result);

            return location; ;

        }

        // E.g https://api.os.uk/search/names/v1/find?query=london&key=U6319wpJWaGGa1HHqjFcUBrB76qMZw4x&fq=LOCAL_TYPE:city
        private string buildLinkForCityname(string city)
        {
            string url = _locationApiBaseUrl + "/find?query=" + city + "&key=" + _locationApiKey + "&fq=LOCAL_TYPE:city";
            return url;
        }

        // E.g https://api.os.uk/search/names/v1/find?query=london&key=U6319wpJWaGGa1HHqjFcUBrB76qMZw4x&fq=LOCAL_TYPE:postcode
        private string buildLinkForPostcode(string city)
        {
            string url = _locationApiBaseUrl+"/find?query="+ city + "&key=" + _locationApiKey + "&fq=LOCAL_TYPE:postcode";
            return url;
        }
    }
}
