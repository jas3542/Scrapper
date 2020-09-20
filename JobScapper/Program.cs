using HtmlAgilityPack;
using JobScapper.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace JobScapper
{
    class Program
    {
        private static List<Job> jobs;
        static void Main(string[] args)
        {
            jobs = new List<Job>();
            jobs.AddRange(fetchDataIndeed());
            if (jobs.Count() > 0)
                Console.WriteLine("indeed count -> "+ jobs.Count);
            Console.ReadLine();


        }

        private static List<Job> fetchDataIndeed()
        {
            List<Job> jobsIndeed = new List<Job>();

            Link links = JsonConvert.DeserializeObject<Link>(File.ReadAllText("webLinks.json"));
            var indeed_url = links.Indeed.createWebsiteLink();

            HttpClient httpClient = new HttpClient();
            HtmlWeb web_client = new HtmlWeb();
            var doc = web_client.Load(indeed_url);
            var job_nodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'result')]");
            foreach (var job in job_nodes)
            {
                Job jobFound = new Job();
                //jobFound.ScrappedCompanyName = "Indeed";
                jobFound.Title = job.SelectSingleNode("//h2[contains(@class, 'title')]").InnerText;
                jobFound.Location = job.SelectSingleNode("//div[contains(@class, 'location')]").InnerText;
                jobFound.Company = job.SelectSingleNode("//span[contains(@class, 'company')]").InnerText;
                jobFound.Salary = job.SelectSingleNode("//span[contains(@class, 'salaryText')]").InnerText;
                jobFound.JobDescriptionLink = job.SelectSingleNode("//a[contains(@class, 'turnstileLink')]").GetAttributeValue("href", "");

                if (jobFound.JobDescriptionLink != "")
                {
                    HtmlWeb web_client2 = new HtmlWeb();
                    var docWithJobFullDescription = web_client.Load(links.Indeed.DomainURL + jobFound.JobDescriptionLink);
                    jobFound.JobDetailedDescription = docWithJobFullDescription.DocumentNode.SelectSingleNode("//div[contains(@class, 'jobsearch-jobDescriptionText')]").InnerText;

                }
                jobsIndeed.Add(jobFound);
                
            }
            return jobsIndeed;
        }
    }
}
