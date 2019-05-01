using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TestStack.BDDfy.Reporters.GherkinFeature
{
    public class TagAttribute : Attribute
    {
        public string Title { get; set; }
    }

    public class GherkinFeatureReporter : IBatchProcessor
    {
        public void Process(IEnumerable<Story> stories)
        {
            var features = "Features";
            if (Directory.Exists(features))
            {
               Directory.Delete(features, true);
            }


            foreach (var grouped in stories.GroupBy(e => e.Namespace))
            {
                var story = grouped.First();
                var split = story.Namespace.Split(".");
                var relativePath = string.Join(".", split.Take(split.Count() - 1))
                    .Replace(story.Metadata.Type.Assembly.ManifestModule.ScopeName.Replace(".dll", string.Empty),
                        string.Empty)
                    .Replace(".", "\\");
                var path = "Features" + relativePath;
                Directory.CreateDirectory(path);
                var builder = new StringBuilder();
                builder.AppendLine($"Feature: {story.Metadata.Title}");
                builder.AppendLine($"\t{story.Metadata.Narrative1}");
                builder.AppendLine($"\t{story.Metadata.Narrative2}");
                builder.AppendLine($"\t{story.Metadata.Narrative3}");

                foreach (var scenario in grouped.SelectMany(e => e.Scenarios))
                {
                    builder.AppendLine(string.Empty);
                    builder.AppendLine($"\tScenario: {scenario.Title}");

                    foreach (var step in scenario.Steps.Where(e => e.ShouldReport))
                        builder.AppendLine($"\t\t{step.Title}");
                }

                File.WriteAllText($"{path}\\{story.Metadata.Title}.feature", builder.ToString());
            }
        }
    }
}