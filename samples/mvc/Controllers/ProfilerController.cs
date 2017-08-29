using EventPipe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mvc.Models;

namespace mvc.Controllers
{
    [Route("api/[controller]")]
    public class ProfilerController : Controller
    {
        [HttpPost("Start")]
        public JsonResult Start()
        {
            var process = Process.GetCurrentProcess();
            var outputFile = Path.Combine(Path.GetTempPath(), $"{process.ProcessName}-{process.Id}-{DateTime.UtcNow:ddMMHHmmss}.netperf");
            Console.WriteLine($"outputFile: {outputFile}");

            TraceConfiguration config = CreateConfiguration(outputFile);
            TraceControl.Enable(config);

            return Json(outputFile);
        }

        [HttpPost("Stop")]
        public JsonResult Stop()
        {
            TraceControl.Disable();
            
            return Json("Stopped");
        }

        private TraceConfiguration CreateConfiguration(string outputFile)
        {
            // Setup the configuration values.
            uint circularBufferMB = 1024; // 1 GB
            uint level = 5; // Verbose

            // Create a new instance of EventPipeConfiguration.
            TraceConfiguration config = new TraceConfiguration(outputFile, circularBufferMB);
            // Setup the provider values.
            // Public provider.
            string providerName = "e13c0d23-ccbc-4e12-931b-d9cc2eee27e4";
            UInt64 keywords = 0x4c14fccbd;

            // Enable the provider.
            config.EnableProvider(providerName, keywords, level);

            // Private provider.
            providerName = "763fd754-7086-4dfe-95eb-c01a46faf4ca";
            keywords = 0x4002000b;

            // Enable the provider.
            config.EnableProvider(providerName, keywords, level);

            // Sample profiler.
            providerName = "3c530d44-97ae-513a-1e6d-783e8f8e03a9";
            keywords = 0x0;

            // Enable the provider.
            config.EnableProvider(providerName, keywords, level);

            // TPL.
            providerName = "2e5dba47-a3d2-4d16-8ee0-6671ffdcd7b5";
            keywords = 0x1 | 0x2 | 0x4 | 0x40 | 0x80;

            // Enable the provider.
            config.EnableProvider(providerName, keywords, level);            

            // TestProvider.
            providerName = "f150d8fb-960c-5e38-a69d-49bae6f97289";
            keywords = 0xffffffff;

            // Enable the provider.
            config.EnableProvider(providerName, keywords, level);            

            return config;
        }        
    }
}
