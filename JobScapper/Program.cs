using HtmlAgilityPack;
using JobScaper.Scrapers;
using JobScapper.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JobScapper
{
    class Program
    {
        private static List<Job> jobs;
        static async Task Main(string[] args)
        {
            jobs = new List<Job>();
            IndeedScraper indeedScraper = new IndeedScraper();
            var result = await indeedScraper.fetchDataIndeed();
            jobs.AddRange(result);

            if (jobs.Count() > 0)
                Console.WriteLine("indeed count -> " + jobs.Count);
            Console.ReadLine();


        }

        

        private static List<Job> fetchDataMonster()
        {
            return new List<Job>();
        }
    }
}
