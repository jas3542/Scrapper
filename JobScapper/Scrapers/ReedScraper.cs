using HtmlAgilityPack;
using JobScapper.Objects;
using JobScraper.Scrapers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using JobScapper;
using System.Net;
using System.Net.Http;
using Jobs.Data.Objects;

namespace JobScaper.Scrapers
{
    // TODO: Pagination is not added yet
    class ReedScraper : Scraper
    {
        private HtmlWeb _web_client;
        private string _web_client_userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36";
        
        public ReedScraper()
        {
            _web_client = new HtmlWeb();
            _web_client.UserAgent = _web_client_userAgent;
            _web_client.UseCookies = true;
        }

        public async Task<List<Job>> fetchDataCWJobs()
        {
            List<Job> jobsReed = new List<Job>();

            var url = _ReedLinks[0].createWebsiteLink();
            var doc = _web_client.Load(url);

            var job_nodes = doc.DocumentNode.SelectNodes("//article[contains(@class, 'job-result')]").Where(item => item.Id != "" && item.Id != null).ToList();
            
            foreach (var job in job_nodes)
            {
                var jobFound = await buildReedList(job, _ReedLinks[0].DomainURL);
                jobsReed.Add(jobFound);

            }

            return jobsReed;
        }

        private async Task<Job> buildReedList(HtmlNode job, string domainUrl)
        {
            Job jobFound = new Job();
            await Task.Run(() =>
            {
                //jobFound.ScrappedCompanyName = "CwJobs";
                jobFound.Title = job.SelectSingleNode(".//h3[contains(@class,'title')]").InnerText;
                jobFound.Location = job.SelectSingleNode(".//li[contains(@class, 'location')]").InnerText;
                jobFound.Company = job.SelectSingleNode(".//div[contains(@class, 'posted-by')]").InnerText;
                jobFound.Salary = job.SelectSingleNode(".//li[contains(@class, 'salary')]").InnerText;
                jobFound.JobDescriptionLink = job.SelectSingleNode(".//a[contains(@class,'job-block-link')]").GetAttributeValue("href", "");

                if (jobFound.JobDescriptionLink != "")
                {
                    var docWithJobFullDescription = _web_client.Load(domainUrl +"/" +jobFound.JobDescriptionLink);

                    jobFound.JobDetailedDescription = docWithJobFullDescription.DocumentNode.SelectSingleNode(".//div[contains(@class, 'description')]").InnerText;

                }
            });

            return jobFound;
        }

    }
}
