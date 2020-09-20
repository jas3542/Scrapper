using System;
using System.Collections.Generic;
using System.Text;

namespace JobScapper.Objects
{
    class Job
    {
        public Job()
        {

        }
        public string ScrappedCompanyName { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Company { get; set; }
        public string Salary { get; set; }
        public string JobDescriptionLink { get; set; }
        public string JobDetailedDescription { get; set; }
    }
}
