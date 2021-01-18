using JobScapper.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobScapper
{
    class IndeedLink : Link
    {
        public IndeedLink() { }
        public IndeedLink(string searched, string location, int radius, string sort, string domainUrl)
        {
            Searched = searched;
            Location = location;
            Radius = radius;
            Sort = sort;
            DomainURL = domainUrl;
        }

        public override string createWebsiteLink()
        {
            var searchedWord = Searched.Replace(" ", "+");
            var link = this.DomainURL+"/jobs?q="+searchedWord+"&"+"l="+Location+"&"+"radius="+Radius+"&"+"sort="+Sort;

            return link;
        }
    }
}
