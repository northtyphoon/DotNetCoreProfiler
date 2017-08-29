using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace EventPipe
{
    [EventSource(Name = "TestEventSource", Guid = "f150d8fb-960c-5e38-a69d-49bae6f97289")]
    public class TestEventSource : EventSource
    {
        public class Keywords
        {
            public const EventKeywords Request = (EventKeywords)1;
        }

        public static TestEventSource Log = new TestEventSource();

        private TestEventSource()
        {
        }
        
        [Event(1, Keywords = Keywords.Request)]
        public void RequestStart(string url)
        {
            Console.WriteLine($"{nameof(RequestStart)},{url}");
            WriteEvent(1, url);
        }

        [Event(2, Keywords = Keywords.Request)]
        public void RequestStop()
        {
            Console.WriteLine($"{nameof(RequestStop)}");
            WriteEvent(2);
        }        
    }      
}
