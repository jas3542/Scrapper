using HtmlAgilityPack;
using JobScaper.DBContext;
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
            //jobs = new List<Job>();
            //IndeedScraper indeedScraper = new IndeedScraper();
            //var result = await indeedScraper.fetchDataIndeed();
            //jobs.AddRange(result);

            //if (jobs.Count() > 0) { 
                using (var context = new DBScraperContext())
                {
                //await context.AddRangeAsync(jobs);
                Job j = new Job();
                j.Company = "test1";
                j.ScrappedCompanyName = "aaaa";
                j.JobDescriptionLink = "lol";
                j.JobDetailedDescription = "idk";
                j.Location = "still idk";
                j.Salary = "we dont do this here";
                j.Title = "testing obs";
                context.Add<Job>(j);
                context.SaveChanges();
                    Console.WriteLine("indeed count -> " + context.Jobs.Count().ToString());
                }
                Console.WriteLine("Indeed jobs inserted");
           // }

            Console.ReadLine();


        }
    }
}
