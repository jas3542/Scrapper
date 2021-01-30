using System.ComponentModel.DataAnnotations;

namespace Jobs.Data.Objects
{
    public class UpdatedTime
    {
        [Key]
        public int ID { get; set; }
        public string Time { get; set; }
    }
}
