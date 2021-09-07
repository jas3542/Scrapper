using Jobs.Data.Objects;
using JobScaper.Api;
using JobScaper.Scrapers;
using JobScraper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JobScapper
{
    class Program
    {
        private static List<Job> jobs;
        static async Task Main(string[] args)
        {
            jobs = new List<Job>();
            LocationService service = new LocationService();
            
            CWScraper CwJobsScraper = new CWScraper(service);
            var result_cwjobs = await CwJobsScraper.fetchDataCWJobs();
            jobs.AddRange(result_cwjobs);

            //IndeedScraper indeedScraper = new IndeedScraper(service);
            //var result_indeed = await indeedScraper.fetchDataIndeed();
            //jobs.AddRange(result_indeed);

            //ReedScraper ReedScraper = new ReedScraper(service);
            //var result_Reedjobs = await ReedScraper.fetchDataCWJobs();
            //jobs.AddRange(result_Reedjobs);

            if (jobs.Count() > 0)
            {
                HttpClient client = new HttpClient();
                var jsonString = JsonConvert.SerializeObject(jobs);
                client.BaseAddress = new Uri("https://localhost:44300/");
                var response = client.PostAsync("Jobs", 
                    new StringContent(jsonString, Encoding.UTF8, "application/json")).Result;
            }

            // Console.ReadLine();

        }
    }
}
