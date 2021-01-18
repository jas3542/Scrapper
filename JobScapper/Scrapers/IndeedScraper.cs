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
                var pagination_nodes = pagination[0].ChildNodes;
            }
            // end Pagination

            jobsIndeed = await buildIndeedJobsList(job_nodes, ""/*links.Indeed.DomainURL*/);
            
            return jobsIndeed;
        }

        private Task<List<Job>> buildIndeedJobsList(HtmlNodeCollection job_nodes, string domainUrl)
        {
            List<Job> jobs_found = new List<Job>();

            foreach (var job_node in job_nodes)
            {
                Job jobFound = new Job();
                HtmlWeb web_client = new HtmlWeb();

                //jobFound.ScrappedCompanyName = "Indeed";
                if (job_node.SelectSingleNode("h2[contains(@class, 'title')]") != null)
                    jobFound.Title = job_node.SelectSingleNode("h2[contains(@class, 'title')]").InnerText;
                if (job_node.SelectSingleNode(".//div[contains(@class, 'location')]") != null)
                jobFound.Location = job_node.SelectSingleNode(".//div[contains(@class, 'location')]").InnerText;
                if (job_node.SelectSingleNode(".//span[contains(@class, 'company')]") != null)
                    jobFound.Company = job_node.SelectSingleNode(".//span[contains(@class, 'company')]").InnerText;
                if (job_node.SelectSingleNode(".//span[contains(@class, 'salaryText')]") != null)
                    jobFound.Salary = job_node.SelectSingleNode(".//span[contains(@class, 'salaryText')]").InnerText;
                if (job_node.SelectSingleNode(".//a[contains(@class, 'turnstileLink')]") != null)
                    jobFound.JobDescriptionLink = job_node.SelectSingleNode(".//a[contains(@class, 'turnstileLink')]").GetAttributeValue("href", "");

                if (jobFound.JobDescriptionLink != "")
                {
                    HtmlWeb web_client2 = new HtmlWeb();
                    var docWithJobFullDescription = web_client.Load(domainUrl + jobFound.JobDescriptionLink);
                    if (docWithJobFullDescription.DocumentNode.SelectSingleNode("//div[contains(@class, 'jobsearch-jobDescriptionText')]") != null)
                        jobFound.JobDetailedDescription = docWithJobFullDescription.DocumentNode.SelectSingleNode("//div[contains(@class, 'jobsearch-jobDescriptionText')]").InnerText;

                }
                jobs_found.Add(jobFound);
            }
            return Task.FromResult(jobs_found);
        }

    }
}
