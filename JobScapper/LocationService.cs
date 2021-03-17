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
        public string getCityPostcode(string cityName)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(_locationApiBaseUrl);

            var result = client.GetAsync(buildLink(cityName)).Result.Content.ReadAsStringAsync().Result;
            var parsedObject = JObject.Parse(result);
            
            Location l = JsonConvert.DeserializeObject<Location>(result);
            return "";

        }
        
        // E.g https://api.os.uk/search/names/v1/find?query=london&key=U6319wpJWaGGa1HHqjFcUBrB76qMZw4x
        private string buildLink(string city)
        {
            string url = _locationApiBaseUrl+"/find?query="+ city + "&key=" + _locationApiKey + "&fq=LOCAL_TYPE:city";
            return url;
        }
    }
}
