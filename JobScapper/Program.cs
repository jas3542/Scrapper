using JobScaper.DBContext;
using JobScaper.Scrapers;
using JobScapper.Objects;
using JobScraper.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScapper
{
    class Program
    {
        private static List<Job> jobs;
        static async Task Main(string[] args)
        {
            jobs = new List<Job>();

            //CWScraper CwJobsScraper = new CWScraper();
            //var result = await CwJobsScraper.fetchDataCWJobs();



            IndeedScraper indeedScraper = new IndeedScraper();
            var result = await indeedScraper.fetchDataIndeed();
            jobs.AddRange(result);

            //if (jobs.Count() > 0)
            //{
            //    using (var context = new DBScraperContext())
            //    {
            //        // TODO: This line should be changed in the future: 
            //        // context.Jobs.RemoveRange(context.Jobs);

            //        await context.AddRangeAsync(jobs);
            //        var timeOfInserting = context.updatedTime.Select(t => t).FirstOrDefault();
            //        if (timeOfInserting != null)
            //            context.updatedTime.Remove(timeOfInserting);
            //        UpdatedTime todays_date = new UpdatedTime();
            //        todays_date.Time = DateTime.Now.ToShortDateString();
            //        context.updatedTime.Add(todays_date);

            //        context.SaveChanges();
            //        Console.WriteLine("jobs inserted -> " + context.Jobs.Count().ToString());
            //    }

            //}

            Console.ReadLine();


        }
    }
}


//Job j = new Job();
//j.Company = "test1";
//j.ScrappedCompanyName = "aaaa";
//j.JobDescriptionLink = "lol";
//j.JobDetailedDescription = "idk";
//j.Location = "still idk";
//j.Salary = "we dont do this here";
//j.Title = "testing obs";
//context.Add<Job>(j);