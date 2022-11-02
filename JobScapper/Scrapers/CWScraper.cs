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
            CookieContainer cookieContainer = new CookieContainer();
            CookieCollection cookiesCollection = new CookieCollection();
            WebHeaderCollection headersCollection = new WebHeaderCollection();
            List<Job> jobsCWJobs = new List<Job>();
            var url = _CWJobsLinks[0].createWebsiteLink();

            _web_client.PreRequest = req =>
            {
                req.CookieContainer = cookieContainer;
                req.Headers = headersCollection;
                return true;
            };
            // PostResponse :
            _web_client.PostResponse = (req,res) =>
            {
                cookiesCollection = res.Cookies;
                headersCollection = res.Headers;
            };
            // Initial call to get the cookies and headers:
            _web_client.Load("https://www.cwjobs.co.uk"); 

            cookieContainer.Add(cookiesCollection);

            // PreRequest:
            _web_client.PreRequest = req =>
            {
                req.CookieContainer = cookieContainer;
                req.Headers = headersCollection;
                req.Headers.Add("Referer", "https://www.cwjobs.co.uk/");
                return true;
            };

            // PostResponse :
            _web_client.PostResponse = (req, res) =>
            {
                cookiesCollection = res.Cookies;
                headersCollection = res.Headers;
            };

            HtmlDocument doc = _web_client.Load(url);

            cookieContainer = new CookieContainer();
            // expires on 2023-12-07T15:54:32.177Z: 
            cookiesCollection.Add(new Cookie("AnonymousUser", "MemberId=d73eb77d-b155-43ab-bcba-4e8e35ed28b6&IsAnonymous=True", "/", "www.cwjobs.co.uk"));
            cookieContainer.Add(cookiesCollection);

            var job_nodes = doc.DocumentNode.SelectNodes("//article[contains(@id, 'job-')]").Where(item => item.Id != "" && item.Id != null).ToList();

            // Pagination if any:
            var next_btn_href = this.getNextPageUrl(doc);

            // PreRequest:
            _web_client.PreRequest = req =>
            {
                req.CookieContainer = cookieContainer;
                req.Headers = headersCollection;
                req.Headers.Remove("Referer");
                req.Headers.Add("Referer", url);
                return true;
            };

            // PostResponse :
            _web_client.PostResponse = (req, res) =>
            {
                cookiesCollection = res.Cookies;
                headersCollection = res.Headers;
            };


            while (next_btn_href != "" && next_btn_href != null)
            {
                
                var new_document = _web_client.Load(next_btn_href);
                var job_nodes_other_pages = new_document.DocumentNode.SelectNodes("//article[contains(@id, 'job-')]").Where(item => item.Id != "" && item.Id != null).ToList();
                job_nodes.AddRange(job_nodes_other_pages);

                // PreRequest:
                _web_client.PreRequest = req =>
                {
                    req.CookieContainer = cookieContainer;
                    req.Headers = headersCollection;
                    req.Headers.Remove("Referer");
                    req.Headers.Add("Referer", next_btn_href);
                    return true;
                };
                // PostResponse :
                _web_client.PostResponse = (req, res) =>
                {
                    cookiesCollection = res.Cookies;
                    headersCollection = res.Headers;
                };

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
            var nextPageNode = doc.DocumentNode.SelectNodes("//a[contains(@aria-label,'Next')]");
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
                string title = job.SelectSingleNode("//a[@data-at='job-item-title']/h2").InnerText;
                jobFound.Title = HtmlEntity.DeEntitize(title);
                var location = job.SelectSingleNode("//li[@data-at='job-item-location']").InnerText;
                jobFound.Location = location;
                var l = _service.getlocationData(HtmlEntity.DeEntitize(location.Split(",")[0]));
                
                if (l.Results != null)
                {
                    var locationData = l.Results.FirstOrDefault().LocationData;
                    var coordinates = _service.convertBNGToLat_Lon(locationData.Geometry_x, locationData.Geometry_y);

                    jobFound.Coordinate_X = coordinates.LONGITUDE;
                    jobFound.Coordinate_Y = coordinates.LATITUDE;
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
