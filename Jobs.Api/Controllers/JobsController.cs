﻿using Jobs.Data.Objects;
using JobScaper.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobs.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<JobsController> _logger;

        public JobsController(ILogger<JobsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            using (var context = new DBScraperContext())
            {
                return context.Jobs.Count().ToString() + "this is what i  have";
            }
        }

        [HttpPost]
        public async void Post([FromBody] List<Job> jobs)
        {
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
        }
    }
}