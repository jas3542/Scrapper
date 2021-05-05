using HtmlAgilityPack;
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
        public CWScraper(LocationService service = null)
        {
            _service = service;
            _web_client = new HtmlWeb();
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

            // Pagination if any:
            var next_btn_href = this.getNextPageUrl(doc);
            while (next_btn_href != "" && next_btn_href != null)
            {
                string nexyPage_url = _ReedLinks[0].DomainURL + "/" + next_btn_href;
                var new_document = await _web_client.LoadFromWebAsync(nexyPage_url);
                var job_nodes_other_pages = new_document.DocumentNode.SelectNodes("//article[contains(@class, 'job-result')]").Where(item => item.Id != "" && item.Id != null).ToList();
                job_nodes.AddRange(job_nodes_other_pages);

                next_btn_href = this.getNextPageUrl(new_document);
            }
            // end Pagination

            foreach (var job in job_nodes)
            {
                var jobFound = await buildCWJobsList(job, _CWJobsLinks[0].DomainURL);
                jobsCWJobs.Add(jobFound);
            }

            return jobsCWJobs;
        }

        /// <summary>
        /// Extracts the url of the next page
        /// </summary>
        /// <param name="doc"> The html document which will contain the pagination list/bar</param>
        /// <returns> The url of the next page </returns>
        private string getNextPageUrl(HtmlDocument doc)
        {
            var nextPageNode = doc.DocumentNode.SelectNodes("//a[contains(@class,'next')]");
            if (nextPageNode != null)
            {
                var nextUrl = nextPageNode[0].GetAttributeValue("href", ""); ; // Child nodes are the pagination pages (1,2,3).
                return nextUrl;
            }
            return "";
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
                    jobFound.Coordinate_X = locationData.Geometry_x;
                    jobFound.Coordinate_Y = locationData.Geometry_y;
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
