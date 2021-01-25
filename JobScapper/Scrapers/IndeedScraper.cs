using HtmlAgilityPack;
using JobScapper.Objects;
using JobScraper.Scrapers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JobScaper.Scrapers
{
    class IndeedScraper : Scraper
    {
        public IndeedScraper()
        {

        }

        public async Task<List<Job>> fetchDataIndeed()
        {
            List<Job> jobsIndeed = new List<Job>();

            var indeed_url = _IndeedLinks[0].createWebsiteLink(); //TODO delete this

            HttpClient httpClient = new HttpClient();
            HtmlWeb web_client = new HtmlWeb();
            var doc = await web_client.LoadFromWebAsync(indeed_url);
            var job_nodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'row')]");

            // Pagination if any:
            var pagination = doc.DocumentNode.SelectNodes("//ul[contains(@class,'pagination-list')]");
            if (pagination != null)
            {
                var pagination_nodes = pagination[0].ChildNodes; // Child nodes are the pagination pages (1,2,3).
            }
            // end Pagination

            foreach (var job in job_nodes)
            {
                var jobFound = await buildIndeedJobsList(job, _CWJobsLinks[0].DomainURL);
                jobsIndeed.Add(jobFound);
            }
            
            
            return jobsIndeed;
        }

        private async Task<Job> buildIndeedJobsList(HtmlNode job, string domainUrl)
        {
            Job jobFound = new Job();
            await Task.Run(() =>
            {
                HtmlWeb web_client = new HtmlWeb();

                //jobFound.ScrappedCompanyName = "Indeed";
                if (job.SelectSingleNode("h2[contains(@class, 'title')]") != null)
                    jobFound.Title = job.SelectSingleNode("h2[contains(@class, 'title')]").InnerText;
                if (job.SelectSingleNode(".//div[contains(@class, 'location')]") != null)
                    jobFound.Location = job.SelectSingleNode(".//div[contains(@class, 'location')]").InnerText;
                if (job.SelectSingleNode(".//span[contains(@class, 'company')]") != null)
                    jobFound.Company = job.SelectSingleNode(".//span[contains(@class, 'company')]").InnerText;
                if (job.SelectSingleNode(".//span[contains(@class, 'salaryText')]") != null)
                    jobFound.Salary = job.SelectSingleNode(".//span[contains(@class, 'salaryText')]").InnerText;
                if (job.SelectSingleNode(".//a[contains(@class, 'turnstileLink')]") != null)
                    jobFound.JobDescriptionLink = job.SelectSingleNode(".//a[contains(@class, 'turnstileLink')]").GetAttributeValue("href", "");

                if (jobFound.JobDescriptionLink != "")
                {
                    HtmlWeb web_client2 = new HtmlWeb();
                    var docWithJobFullDescription = web_client.Load(domainUrl + jobFound.JobDescriptionLink);
                    if (docWithJobFullDescription.DocumentNode.SelectSingleNode("//div[contains(@class, 'jobsearch-jobDescriptionText')]") != null)
                        jobFound.JobDetailedDescription = docWithJobFullDescription.DocumentNode.SelectSingleNode("//div[contains(@class, 'jobsearch-jobDescriptionText')]").InnerText;

                }
            });

            return jobFound;
        }

    }
}
