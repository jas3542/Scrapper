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
using JobScraper;

namespace JobScaper.Scrapers
{
    // TODO: Pagination is not added yet
    class CWScraper : Scraper
    {
        private HtmlWeb _web_client;
        private LocationService _service;

        private WebHeaderCollection _headersCollection= null;
        public CWScraper(LocationService service = null)
        {
            _service = service;

            _web_client = new HtmlWeb();

            _headersCollection = new WebHeaderCollection();
            //_headersCollection.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.90 Safari/537.36");
            //_headersCollection.Add(HttpRequestHeader.Referer, "https://www.cwjobs.co.uk/");
            //_headersCollection.Add(HttpRequestHeader.Host, "www.cwjobs.co.uk");
            //_headersCollection.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
            //_headersCollection.Add(HttpRequestHeader.AcceptLanguage, "es-ES,es;q=0.9");
            //_headersCollection.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");







            //_web_client.PreRequest = req =>
            //{
            //    req.Headers.Add( _headersCollection);
            //    return true;
            //};
        }

        public async Task<List<Job>> fetchDataCWJobs()
        {
            CookieContainer cookieContainer = null;
            CookieCollection cookiesCollection = null;
            WebHeaderCollection headersCollection = null;
            List<Job> jobsCWJobs = new List<Job>();
            
            // PostResponse :
            _web_client.PostResponse = (req,res) =>
            {
                cookiesCollection = res.Cookies;
                headersCollection = res.Headers;
            };
            // Initial call to get the cookies and headers:
            _web_client.Load(_CWJobsLinks[0].DomainURL); 
            cookieContainer = new CookieContainer();
            cookieContainer.Add(cookiesCollection);
            // PreRequest:
            _web_client.PreRequest = req =>
            {
                req.CookieContainer = cookieContainer;
                req.Headers = headersCollection;
                return true;
            };
            var url = _CWJobsLinks[0].createWebsiteLink();
            HtmlDocument doc = _web_client.Load(url);
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
                var location = job.SelectSingleNode(".//li[contains(@class, 'location')]/span").InnerText;
                jobFound.Location = location;
                var l = _service.getlocationData(HtmlEntity.DeEntitize(location.Split(",")[0]));
                
                if (l.Results != null)
                {
                    var locationData = l.Results.FirstOrDefault().LocationData;
                    jobFound.Borough = locationData.District_borough;
                    if (locationData.Local_Type.ToLower() == "city")
                    {
                        jobFound.City = locationData.Name;
                    }else
                    {
                        jobFound.City = locationData.Populated_place;
                    }

                }

                jobFound.Company = job.SelectSingleNode(".//li[contains(@class, 'company')]/h3/a").InnerText;
                string salary = job.SelectSingleNode(".//li[contains(@class, 'salary')]").InnerText;
                jobFound.Salary = HtmlEntity.DeEntitize(salary);
                jobFound.JobDescriptionLink = job.SelectSingleNode(".//div[contains(@class,'job-title')]/a").GetAttributeValue("href", "");
                
                if (jobFound.JobDescriptionLink != "")
                {
                    _web_client.PreRequest = req =>
                    {
                        req.Headers.Add(HttpRequestHeader.Referer, "https://www.cwjobs.co.uk/jobs/software-developer/in-wolverhampton?radius=0&Sort=2");
                        
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
