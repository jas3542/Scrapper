using JobScapper.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobScapper
{
    class ReedLink : Link
    {
        public ReedLink() { }
        public ReedLink(string searched, string location, int radius, string sort, string domainUrl) {
            Searched = searched;
            Location = location;
            Radius = radius;
            Sort = sort;
            DomainURL = domainUrl;
        }

        // exemple : https://www.reed.co.uk/jobs/software-developer-jobs-in-london?proximity=5&sortby=DisplayDate
        public override string createWebsiteLink()
        {
            var searchedWord = Searched.Replace(" ", "-");
            var link = this.DomainURL+"/jobs/"+searchedWord+"-jobs-in-"+Location+"?proximity="+Radius+"&"+"sortby=DisplayDate";

            return link;

            
        }
    }
}
