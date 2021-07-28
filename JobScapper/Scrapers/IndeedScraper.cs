using HtmlAgilityPack;
using Jobs.Data.Objects;
using JobScapper;
using JobScapper.Objects;
using JobScraper;
using JobScraper.Scrapers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JobScaper.Scrapers
{
    class IndeedScraper : Scraper
    {
        private LocationService _service;
        public IndeedScraper(LocationService service = null)
        {
            _service = service;
        }

        public async Task<List<Job>> fetchDataIndeed()
        {
            List<Job> jobsIndeed = new List<Job>();

            var indeed_url = _IndeedLinks[0].createWebsiteLink(); 

            HtmlWeb web_client = new HtmlWeb();
            var doc = await web_client.LoadFromWebAsync(indeed_url);
            var job_nodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'row')]")?.ToList();

            // Pagination if any:
            var next_btn_href = this.getNextPageUrl(doc);
            while (next_btn_href != "" && next_btn_href != null)
            {
                string nexyPage_url = _IndeedLinks[0].DomainURL + "/" + next_btn_href;
                var new_document = await web_client.LoadFromWebAsync(nexyPage_url);
                var job_nodes_other_pages = new_document.DocumentNode.SelectNodes("//div[contains(@class, 'row')]");
                job_nodes.AddRange(job_nodes_other_pages);

                next_btn_href = this.getNextPageUrl(new_document);
            }
            // end Pagination

            foreach (var job in job_nodes)
            {
                var jobFound = await buildIndeedJobsList(job, _IndeedLinks[0].DomainURL);
                jobsIndeed.Add(jobFound);
            }
            
            
            return jobsIndeed;
        }

        /// <summary>
        /// Extracts the url of the next page
        /// </summary>
        /// <param name="doc"> The html document which will contain the pagination list/bar</param>
        /// <returns> The url of the next page </returns>
        private string getNextPageUrl(HtmlDocument doc)
        {
            var pagination = doc.DocumentNode.SelectNodes("//ul[contains(@class,'pagination-list')]");
            if (pagination != null)
            {
                var pagination_nodes = pagination[0].ChildNodes; // Child nodes are the pagination pages (1,2,3).
                if (pagination_nodes.Count > 0)
                {
                    var child_nodes = pagination_nodes[pagination_nodes.Count - 1].ChildNodes;
                    var next_btn_href = child_nodes[0].GetAttributeValue("href", "");
                    
                    return next_btn_href;
                }
            }
            return "";
        }

        private async Task<Job> buildIndeedJobsList(HtmlNode job, string domainUrl)
        {
            Job jobFound = new Job();
            await Task.Run(() =>
            {
                HtmlWeb web_client = new HtmlWeb();

                jobFound.ScrappedCompanyName = "Indeed";
                if (job.SelectSingleNode("h2[contains(@class, 'title')]") != null) {
                    string title = job.SelectSingleNode("h2[contains(@class, 'title')]").InnerText;
                    jobFound.Title = HtmlEntity.DeEntitize(title);
                }
                if (job.SelectSingleNode(".//div[contains(@class, 'location')]") != null)
                {
                    var location = job.SelectSingleNode(".//div[contains(@class, 'location')]").InnerText;
                    
                    jobFound.Location = location;
                    var l = _service.getlocationData(HtmlEntity.DeEntitize(location));

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
                        }
                        else
                        {
                            jobFound.City = locationData.Populated_place;
                        }

                    }
                }
                if (job.SelectSingleNode(".//span[contains(@class, 'company')]") != null)
                    jobFound.Company = job.SelectSingleNode(".//span[contains(@class, 'company')]").InnerText;
                if (job.SelectSingleNode(".//span[contains(@class, 'salaryText')]") != null) { 
                    string salary = job.SelectSingleNode(".//span[contains(@class, 'salaryText')]").InnerText;
                    jobFound.Salary = HtmlEntity.DeEntitize(salary);
                }
                if (job.SelectSingleNode(".//a[contains(@class, 'turnstileLink')]") != null) { 
                    var descriptionLink = job.SelectSingleNode(".//a[contains(@class, 'turnstileLink')]").GetAttributeValue("href", "");
                    jobFound.JobDescriptionLink = domainUrl + jobFound.JobDescriptionLink;
                }

                if (jobFound.JobDescriptionLink != "")
                {
                    HtmlWeb web_client2 = new HtmlWeb();
                    var docWithJobFullDescription = web_client.Load(jobFound.JobDescriptionLink);
                    //if (docWithJobFullDescription.DocumentNode.SelectSingleNode("//div[contains(@class, 'jobsearch-jobDescriptionText')]") != null)
                    
                    string description = docWithJobFullDescription.DocumentNode.SelectSingleNode("//div[contains(@class, 'jobsearch-jobDescriptionText')]")?.InnerHtml;
                    jobFound.JobDetailedDescription = HtmlEntity.DeEntitize(description);
                }
            });

            return jobFound;
        }

    }
}
