using HtmlAgilityPack;
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

            var result = await fetchDataIndeed();
            jobs.AddRange(result);

            if (jobs.Count() > 0)
                Console.WriteLine("indeed count -> " + jobs.Count);
            Console.ReadLine();


        }

        private async static Task<List<Job>> fetchDataIndeed()
        {
            List<Job> jobsIndeed = new List<Job>();

            Link links = JsonConvert.DeserializeObject<Link>(File.ReadAllText("webLinks.json"));
            var indeed_url = links.Indeed.createWebsiteLink();

            HttpClient httpClient = new HttpClient();
            HtmlWeb web_client = new HtmlWeb();
            var doc = web_client.Load(indeed_url);
            var job_nodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'result')]");

            // Pagination if any:
            var pagination = doc.DocumentNode.SelectNodes("//ul[contains(@class,'pagination-list')]");
            if (pagination != null) { 
                var pagination_nodes = pagination[0].ChildNodes;
            }
            // end Pagination

            foreach (var job in job_nodes)
            {
                var jobFound = await buildJobsList(job, links.Indeed.DomainURL);
                jobsIndeed.Add(jobFound);
               
            }
            return jobsIndeed;
        }

        private async static Task<Job> buildJobsList(HtmlNode job, string domainUrl)
        {
            Job jobFound = new Job();
            await Task.Run(() =>
            {
                HtmlWeb web_client = new HtmlWeb();


                //jobFound.ScrappedCompanyName = "Indeed";
                jobFound.Title = job.SelectSingleNode("//h2[contains(@class, 'title')]").InnerText;
                jobFound.Location = job.SelectSingleNode("//div[contains(@class, 'location')]").InnerText;
                jobFound.Company = job.SelectSingleNode("//span[contains(@class, 'company')]").InnerText;
                jobFound.Salary = job.SelectSingleNode("//span[contains(@class, 'salaryText')]").InnerText;
                jobFound.JobDescriptionLink = job.SelectSingleNode("//a[contains(@class, 'turnstileLink')]").GetAttributeValue("href", "");

                if (jobFound.JobDescriptionLink != "")
                {
                    HtmlWeb web_client2 = new HtmlWeb();
                    var docWithJobFullDescription = web_client.Load(domainUrl + jobFound.JobDescriptionLink);
                    jobFound.JobDetailedDescription = docWithJobFullDescription.DocumentNode.SelectSingleNode("//div[contains(@class, 'jobsearch-jobDescriptionText')]").InnerText;

                }
            });

            return jobFound;
        }

        private static List<Job> fetchDataMonster()
        {
            return new List<Job>();
        }
    }
}
