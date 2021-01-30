using JobScapper;
using JobScapper.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace JobScraper.Scrapers
{
    class Scraper
    {
        protected List<Link> _links { get; set; }
        protected List<CWJobsLink> _CWJobsLinks { get; set; }
        protected List<IndeedLink> _IndeedLinks { get; set; }
        public Scraper()
        {
            _links = JsonConvert.DeserializeObject<List<Link>>(File.ReadAllText("webLinks.json"));

            _CWJobsLinks = _links.Where(item => item.Platform.Equals("CWJobs"))
                .Select(i => new CWJobsLink(i.Searched, i.Location, i.Radius, i.Sort, i.DomainURL)).ToList();

            _IndeedLinks = _links.Where(item => item.Platform.Equals("Indeed"))
                .Select(i => new IndeedLink(i.Searched, i.Location, i.Radius, i.Sort, i.DomainURL, i.FromAge)).ToList();

        }

    }
}
