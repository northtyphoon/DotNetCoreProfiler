using EventPipe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace mvc
{
    static class PropertyExtensions
    {
        public static object GetProperty(this object _this, string propertyName)
        {
            return _this.GetType().GetTypeInfo().GetDeclaredProperty(propertyName)?.GetValue(_this);
        }
    }

    public class Program
    {
        static IDisposable aspnetSubscription = null;
        static IDisposable listenerSubscription = null;

        public static void Main(string[] args)
        {
            System.Console.WriteLine(nameof(Main));

            listenerSubscription = DiagnosticListener.AllListeners.Subscribe(listener =>
            {
                System.Console.WriteLine(listener.Name);
                if (listener.Name == "Microsoft.AspNetCore")
                {
                    aspnetSubscription = listener.Subscribe(e => {
                        switch (e.Key)
                        {
                            case "Microsoft.AspNetCore.Hosting.BeginRequest":
                                {
                                    string path = e.Value.GetProperty("httpContext").GetProperty("Request").GetProperty("Path").ToString();
                                    TestEventSource.Log.RequestStart(path);
                                    break;
                                }
                            case "Microsoft.AspNetCore.Hosting.EndRequest":
                                {
                                    TestEventSource.Log.RequestStop();
                                    break;
                                }
                            default: break;
                        }
                    });
                }
            });

            DiagnosticSource mySource = new DiagnosticListener("MyDiagnosticSource");

            mySource.Write("MyEvent", "MyPayload");

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            System.Console.WriteLine("BuildWebHost");
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
        }
    }
}
