using Jobs.Data.Objects;
using JobScaper.Data;
using JobScaper.Scrapers;
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
            //var result_cwjobs = await CwJobsScraper.fetchDataCWJobs();
            //jobs.AddRange(result_cwjobs);

            IndeedScraper indeedScraper = new IndeedScraper();
            var result_indeed = await indeedScraper.fetchDataIndeed();
            jobs.AddRange(result_indeed);

            if (jobs.Count() > 0)
            {
                using (var context = new DBScraperContext())
                {
                    // TODO: This line should be changed in the future: 
                    // context.Jobs.RemoveRange(context.Jobs);

                    await context.AddRangeAsync(jobs);
                    var timeOfInserting = context.updatedTime.Select(t => t).FirstOrDefault();
                    if (timeOfInserting != null)
                        context.updatedTime.Remove(timeOfInserting);
                    UpdatedTime todays_date = new UpdatedTime();
                    todays_date.Time = DateTime.Now.ToShortDateString();
                    context.updatedTime.Add(todays_date);

                    context.SaveChanges();
                    Console.WriteLine("total jobs inserted -> " + context.Jobs.Count().ToString());
                }

            }

            // Console.ReadLine();


        }
    }
}
