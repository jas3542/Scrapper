using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jobs.Data.Objects
{
    public class Job
    {
        public Job()
        {

        }
        [Key]
        public int ID { get; set; }
        public string ScrappedCompanyName { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Coordinate_X { get; set; }
        public string Coordinate_Y { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string Borough { get; set; }
        public string Company { get; set; }
        public string Salary { get; set; }
        public string JobDescriptionLink { get; set; }
        public string JobDetailedDescription { get; set; }
    }
}
