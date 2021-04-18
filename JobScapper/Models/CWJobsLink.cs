using HtmlAgilityPack;
using JobScapper.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobScapper
{
    class CWJobsLink : Link
    {
        public CWJobsLink() { }
        public CWJobsLink(string searched, string location, int radius, string sort, string domainUrl) {
            Searched = searched;
            Location = location;
            Radius = radius;
            Sort = sort;
            DomainURL = domainUrl;
        }

        // exemple : https://www.cwjobs.co.uk/jobs/software-developer/in-wolverhampton?radius=0&Sort=2
        // exemple2 : https://www.cwjobs.co.uk/jobs/software-developer--and-c%23-and-not-noir-and-not-senior-and-not-lead/in-birmingham?q=Software+Developer++And+c%23+AND+NOT+Noir+AND+NOT+Senior+AND+NOT+Lead&radius=0&Sort=2
        public override string createWebsiteLink()
        {
            var searchedWord = Searched.Replace(" ", "-");
            var link = this.DomainURL+"/jobs/"+searchedWord+"/"+"in-"+Location+"?radius="+Radius+"&"+"Sort="+Sort;

            return link;

            
        }
    }
}
