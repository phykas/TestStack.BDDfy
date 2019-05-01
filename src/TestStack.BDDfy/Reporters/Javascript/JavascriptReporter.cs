using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TestStack.BDDfy.Reporters.GherkinFeature;

namespace TestStack.BDDfy.Reporters.Javascript
{
    public class JavascriptReporter : IBatchProcessor
    {
        public void Process(IEnumerable<Story> stories)
        {
            var features = "Features";
            if (!Directory.Exists(features)) Directory.CreateDirectory(features);
            var stories2 = stories.GroupBy(e => e.Namespace).Select(e =>
            {
                var tag = e.First().Metadata.Type.GetCustomAttributes(typeof(TagAttribute), false).FirstOrDefault();
                return new
                {
                    Namespace = e.Key,
                    Story = e.First().Metadata,
                    Tag = (tag as TagAttribute)?.Title ?? string.Empty,
                    Scenarios = e.SelectMany(c => c.Scenarios)
                };
            });
            File.WriteAllText("features.js", $"var result = {JsonConvert.SerializeObject(stories2)}");

        }
    }
}