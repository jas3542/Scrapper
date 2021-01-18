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
using System.Linq;
using JobScapper;

namespace JobScaper.Scrapers
{
    class CWScraper : Scraper
    {
        private HtmlWeb _web_client;
        private string _web_client_userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36";
        public CWScraper()
        {
           Webclient
            _web_client = new HtmlWeb();
            _web_client.UserAgent = _web_client_userAgent;
            _web_client.UseCookies = true;
        }

        public async Task<List<Job>> fetchDataCWJobs()
        {
            List<Job> jobsCWJobs = new List<Job>();

            var url = _CWJobsLinks[0].createWebsiteLink();
            var initial_doc = _web_client.Load(@"http://www.cwjobs.co.uk/");
            
            var doc = _web_client.Load(@"http://www.cwjobs.co.uk/jobs/software-developer/in-wolverhampton?radius=0&Sort=2");
            
            
            var job_nodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'job')]");

            foreach (var job in job_nodes)
            {
                var jobFound = await buildIndeedJobsList(job, _CWJobsLinks[0].DomainURL);
                jobsCWJobs.Add(jobFound);

            }
            return jobsCWJobs;
        }

        private async Task<Job> buildIndeedJobsList(HtmlNode job, string domainUrl)
        {
            Job jobFound = new Job();
            await Task.Run(() =>
            {
                //jobFound.ScrappedCompanyName = "CwJobs";
                jobFound.Title = job.SelectSingleNode("//div[contains(@class,'job-title')]/a").InnerText;
                jobFound.Location = job.SelectSingleNode("//div[contains(@class, 'location')]/span").InnerText;
                jobFound.Company = job.SelectSingleNode("//li[contains(@class, 'company')]/h3/a").InnerText;
                jobFound.Salary = job.SelectSingleNode("//li[contains(@class, 'salary')]").InnerText;
                jobFound.JobDescriptionLink = job.SelectSingleNode("//div[contains(@class,'job-title')]/a").GetAttributeValue("href", "");

                if (jobFound.JobDescriptionLink != "")
                {
                    var docWithJobFullDescription = _web_client.Load(jobFound.JobDescriptionLink);

                    _web_client.UserAgent = _web_client_userAgent;
                    Console.WriteLine("hello/n");
                    jobFound.JobDetailedDescription = docWithJobFullDescription.DocumentNode.SelectSingleNode("//div[contains(@class, 'job-description')]").InnerText;

                }
            });

            return jobFound;
        }

    }
}
