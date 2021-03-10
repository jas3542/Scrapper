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
using System.Text.RegularExpressions;

namespace JobScaper.Scrapers
{
    // TODO: Pagination is not added yet
    class CWScraper : Scraper
    {
        private HtmlWeb _web_client;
        private string _web_client_userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36";
        
        private CookieContainer _cookieContainer;
        private Cookie _cookie;

        public CWScraper()
        {
            this.CreateCookieContainerWithCookie();

            _web_client = new HtmlWeb();
            _web_client.UserAgent = _web_client_userAgent;
            _web_client.UseCookies = true;
        }

        /// <summary>
        ///  this Cookie assignment should be dynamic. 
        /// </summary>
        private void CreateCookieContainerWithCookie()
        {
            _cookieContainer = new CookieContainer();

            // Expires in year 2025:
            _cookie = new Cookie("AnonymousUser", "MemberId=c7a38036-6622-4569-b390-43a0f82220e6&IsAnonymous=True", "/", "www.cwjobs.co.uk");
            _cookieContainer.Add(_cookie);

        }
        public async Task<List<Job>> fetchDataCWJobs()
        {
            List<Job> jobsCWJobs = new List<Job>();

            var url = _CWJobsLinks[0].createWebsiteLink();
            var doc = _web_client.Load(url);

            var job_nodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'job')]").Where(item => item.Id != "" && item.Id != null).ToList();
            
            foreach (var job in job_nodes)
            {
                
                var jobFound = await buildCWJobsList(job, _CWJobsLinks[0].DomainURL);
                jobsCWJobs.Add(jobFound);

            }

            return jobsCWJobs;
        }

        private async Task<Job> buildCWJobsList(HtmlNode job, string domainUrl)
        {
            Job jobFound = new Job();
            await Task.Run(() =>
            {
                
                jobFound.ScrappedCompanyName = "CwJobs";
                string title = job.SelectSingleNode(".//div[contains(@class,'job-title')]/a").InnerText;
                jobFound.Title = HtmlEntity.DeEntitize(title);
                jobFound.Location = job.SelectSingleNode(".//li[contains(@class, 'location')]").InnerText;
                jobFound.Company = job.SelectSingleNode(".//li[contains(@class, 'company')]/h3/a").InnerText;
                string salary = job.SelectSingleNode(".//li[contains(@class, 'salary')]").InnerText;
                jobFound.Salary = HtmlEntity.DeEntitize(salary);
                jobFound.JobDescriptionLink = job.SelectSingleNode(".//div[contains(@class,'job-title')]/a").GetAttributeValue("href", "");
                
                if (jobFound.JobDescriptionLink != "")
                {
                    _web_client.PreRequest = req =>
                    {
                        //req.Headers.Add(HttpRequestHeader.UserAgent, _web_client_userAgent);
                        req.Headers.Add(HttpRequestHeader.Referer, "https://www.cwjobs.co.uk/jobs/software-developer/in-wolverhampton?radius=0&Sort=2");
                        //req.Timeout = 300000;
                        //req.ReadWriteTimeout = 300000;

                        //req.Headers.Add(HttpRequestHeader.AcceptLanguage, "es-ES,es;q=0.9");
                        //req.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                        //req.Headers.Add(HttpRequestHeader.CacheControl, "max-age=0");
                        //req.Headers.Add("upgrade-insecure-requests", "1");

                        req.CookieContainer = _cookieContainer;
                        return true;
                    };
                    var docWithJobFullDescription = _web_client.Load(jobFound.JobDescriptionLink);

                    string description = docWithJobFullDescription.DocumentNode.SelectSingleNode(".//div[(@class= 'job-description')]").InnerHtml;
                    jobFound.JobDetailedDescription = HtmlEntity.DeEntitize(description);
                }
            });

            return jobFound;
        }

    }
}
