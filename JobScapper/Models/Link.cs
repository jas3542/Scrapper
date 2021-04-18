using System;
using System.Collections.Generic;
using System.Text;

namespace JobScapper.Objects
{
    class Link
    {
        public string Platform { get; set; }
        public string Searched { get; set; }
        public string Location { get; set; }
        public int Radius { get; set; }
        public string Sort { get; set; }
        public string DomainURL { get; set; }
        public string FromAge { get; set; } // Used to get the result of "x" days only
        public virtual string createWebsiteLink()
        {
            return "";
        }
        
    }
}
