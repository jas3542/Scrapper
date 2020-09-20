using System;
using System.Collections.Generic;
using System.Text;

namespace JobScapper
{
    class Indeed
    {
        public string Searched { get; set; }
        public string Location { get; set; }
        public int Radius { get; set; }
        public string Sort { get; set; }
        public string DomainURL { get; set; }
        public string createWebsiteLink()
        {
            var searchedWord = Searched.Replace(" ", "+");
            var link = this.DomainURL+"/jobs?q="+searchedWord+"&"+"l="+Location+"&"+"radius="+Radius+"&"+"sort="+Sort;

            return link;
        }
    }
}
