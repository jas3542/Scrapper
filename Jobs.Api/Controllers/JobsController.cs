using Jobs.Data.Objects;
using JobScaper.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;
using System.Web;

namespace Jobs.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly ILogger<JobsController> _logger;
        private List<Job> _cacheJobsList;
        private readonly IMemoryCache _cache; // used to cached list of jobs

        public JobsController(ILogger<JobsController> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cacheJobsList = new List<Job>();
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<List<Job>>> Get()
        {
            await Task.Run(() => GetJobs());

            return Ok(_cacheJobsList);
        }

        [HttpGet]
        [Route("getOrderedList")]
        public async Task<ActionResult<List<Job>>> GetOrderedList([FromQuery]string sortBy)
        {
            GetJobs();

            var orderedJobsList = new List<Job>();
            if (sortBy == "" || sortBy == null)
            {
                return Ok(_cacheJobsList);
            }
            if (sortBy == "salary")
            {
                await Task.Run(() =>
                {
                    orderedJobsList = _cacheJobsList.OrderBy(item => {
                        string regexPattern_min = "[0-9.,]+";
                        Match[] matches = Regex.Matches(item.Salary, regexPattern_min).ToArray();
                        if (matches.Length > 0) { 
                            return matches[0].ToString();
                        }else
                        {
                            return item.Salary;
                        }
                    }).ToList();
                });
            }
            
            return Ok(orderedJobsList);
            
        }

        [HttpGet]
        [Route("getFilteredList")]
        public async Task<ActionResult<List<Job>>> GetFilteredList([FromQuery] string filterBy)
        {
            GetJobs();

            if (string.IsNullOrEmpty(filterBy))
            {
                return Ok(_cacheJobsList);
            }
            var filteredJobsList = new List<Job>();
            List<string> include = new List<string>();
            List<string> exclude = new List<string>();

            var wordWithoutSign = "";
            var decodedWord = HttpUtility.UrlDecode(filterBy);
            string[] words = decodedWord.Split(',');
            for(var i=0; i<words.Length; i++)
            {
                if (words[i].Contains("+")){
                    wordWithoutSign = words[i].Split("+")[1];
                    include.Add(wordWithoutSign.Trim().ToLower());
                }else if (words[i].Contains("-"))
                {
                    wordWithoutSign = words[i].Split("-")[1];
                    exclude.Add(wordWithoutSign.Trim().ToLower());
                }
            }

            // Including Jobs
            await Task.Run(() =>
            {
                if (include.Count > 0) {
                    filteredJobsList = _cacheJobsList.Where(item => {
                        for(int i= 0; i<include.Count(); i++)
                        {
                            bool found = false;
                            if (item.JobDetailedDescription != null)
                            {
                                if (item.JobDetailedDescription.ToLower().Contains(include[i]))
                                    found = true;
                            }
                            if (item.Title != null)
                            {
                                if (item.Title.ToLower().Contains(include[i]))
                                    found = true;
                            }
                            if (item.Location != null)
                            {
                                if (item.Location.ToLower().Contains(include[i]))
                                    found = true;
                            }
                            return found;
                        }
                        return false;
                    }).ToList();
                }else
                {
                    filteredJobsList = _cacheJobsList;
                }
            });

            // Excluding Jobs
            await Task.Run(() =>
            {
                if (exclude.Count() > 0) {
                    filteredJobsList = filteredJobsList.Where(item => {
                        for (int i = 0; i < exclude.Count(); i++)
                        {
                            if (item.JobDetailedDescription.ToLower().Contains(exclude[i]) || item.Title.ToLower().Contains(exclude[i])
                                    || item.Location.ToLower().Contains(exclude[i]))
                                return false;
                        }
                        return true;
                    }).ToList();
                }
            });

            return Ok(filteredJobsList);
        }

        [HttpGet]
        [Route("getMapMarkersList")]
        public async Task<ActionResult<List<JobMarker>>> GetMapMarkersList()
        {
            GetJobs();

            var mapMarkersList = new List<JobMarker>();
            await Task.Run(() =>
            {
                mapMarkersList = _cacheJobsList.Select(job =>
                {
                    JobMarker JobMarker = new JobMarker(job.Coordinate_X, job.Coordinate_Y, job.Title);
                    return JobMarker;
                }).ToList<JobMarker>();
            });

            return Ok(mapMarkersList);

        }

        [HttpPost]
        public async void Post([FromBody] List<Job> jobs)
        {
            if (jobs.Count() > 0)
            {
                using (var context = new DBScraperContext())
                {
                    //TODO: This line should be changed in the future: 
                    context.Jobs.RemoveRange(context.Jobs);
                    await context.SaveChangesAsync();

                    await context.Jobs.AddRangeAsync(jobs);
                    var timeOfInserting = context.updatedTime.Select(t => t).FirstOrDefault();
                    if (timeOfInserting != null)
                        context.updatedTime.Remove(timeOfInserting);
                    UpdatedTime todays_date = new UpdatedTime();
                    todays_date.Time = DateTime.Now.ToShortDateString();
                    context.updatedTime.Add(todays_date);

                    await context.SaveChangesAsync();
                }
            }
        }

        private async void GetJobs()
        {
            var listFound = _cache.TryGetValue("jobs_list", out _cacheJobsList);
            if (!listFound || _cacheJobsList.Count == 0)
            {
                using (var context = new DBScraperContext())
                {
                    _cacheJobsList = await context.Jobs.ToListAsync();
                    _cache.CreateEntry("jobs_list");
                    _cache.Set("jobs_list", _cacheJobsList);
                }
                
            }
        }
    }

    public class JobMarker {
        
        public JobMarker(string x, string y, string title)
        {
            X = x;
            Y = y;
            Title = title;
        }

        public string X { get; set; }
        public string Y { get; set; }
        public string Title { get; set; }

    }

}
