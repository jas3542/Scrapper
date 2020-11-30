using System.ComponentModel.DataAnnotations;

namespace JobScraper.Objects
{
    class UpdatedTime
    {
        [Key]
        public int ID { get; set; }
        public string Time { get; set; }
    }
}
