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
using JobScraper;

namespace JobScaper.Scrapers
{
    class ReedScraper : Scraper
    {
        private HtmlWeb _web_client;
        private string _web_client_userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36";
        private string _web_client_userAgent_for_pagination = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.90 Safari/537.36";
        private LocationService _service;

        public ReedScraper(LocationService service = null)
        {
            _service = service;
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

            // Pagination if any:
            var next_btn_href = this.getNextPageUrl(doc);
            while (next_btn_href != "" && next_btn_href != null)
            {
                _web_client.UserAgent = _web_client_userAgent_for_pagination;
                string nexyPage_url = _ReedLinks[0].DomainURL + "/" + next_btn_href;
                var new_document = await _web_client.LoadFromWebAsync(nexyPage_url);
                var job_nodes_other_pages = new_document.DocumentNode.SelectNodes("//article[contains(@class, 'job-result')]").Where(item => item.Id != "" && item.Id != null).ToList();
                job_nodes.AddRange(job_nodes_other_pages);

                next_btn_href = this.getNextPageUrl(new_document);
            }
            // end Pagination

            foreach (var job in job_nodes)
            {
                var jobFound = await buildReedList(job, _ReedLinks[0].DomainURL);
                jobsReed.Add(jobFound);

            }

            return jobsReed;
        }

        /// <summary>
        /// Extracts the url of the next page
        /// </summary>
        /// <param name="doc"> The html document which will contain the pagination list/bar</param>
        /// <returns> The url of the next page </returns>
        private string getNextPageUrl(HtmlDocument doc)
        {
            var nextPageNode = doc.DocumentNode.SelectNodes("//a[contains(@id,'nextPage')]");
            if (nextPageNode != null)
            {
                var nextUrl = nextPageNode[0].GetAttributeValue("href", ""); ; // Child nodes are the pagination pages (1,2,3).
                return nextUrl;
            }
            return "";
        }

        private async Task<Job> buildReedList(HtmlNode job, string domainUrl)
        {
            Job jobFound = new Job();
            await Task.Run(() =>
            {
                jobFound.ScrappedCompanyName = "Reed";
                string title = job.SelectSingleNode(".//h3[contains(@class,'title')]")?.InnerText;
                jobFound.Title = HtmlEntity.DeEntitize(title);
                //jobFound.Location = job.SelectSingleNode(".//li[contains(@class, 'location')]")?.InnerText;

                var location = job.SelectSingleNode(".//li[contains(@class, 'location')]").InnerText;
                jobFound.Location = location;
                var l = _service.getlocationData(HtmlEntity.DeEntitize(location));

                if (l.Results != null)
                {
                    var locationData = l.Results.FirstOrDefault().LocationData;
                    jobFound.Borough = locationData.District_borough;
                    if (locationData.Local_Type.ToLower() == "city")
                    {
                        jobFound.City = locationData.Name;
                    }
                    else
                    {
                        jobFound.City = locationData.Populated_place;
                    }

                }



                jobFound.Company = job.SelectSingleNode(".//div[contains(@class, 'posted-by')]/a")?.InnerText;
                string salary = job.SelectSingleNode(".//li[contains(@class, 'salary')]")?.InnerText;
                jobFound.Salary = HtmlEntity.DeEntitize(salary);
                jobFound.JobDescriptionLink = job.SelectSingleNode(".//a[contains(@class,'job-block-link')]")?.GetAttributeValue("href", "");

                if (jobFound.JobDescriptionLink != "")
                {
                    var docWithJobFullDescription = _web_client.Load(domainUrl +"/" +jobFound.JobDescriptionLink);

                    string description = docWithJobFullDescription.DocumentNode.SelectSingleNode(".//div[(@class= 'description')]")?.InnerHtml;
                    jobFound.JobDetailedDescription = HtmlEntity.DeEntitize(description);
                }
            });

            return jobFound;
        }

    }
}
