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
            if (!Directory.Exists(features)) Directory.CreateDirectory(features);

            foreach (IGrouping<string, Story> grouped in stories.GroupBy(e => e.Namespace))
            {
                Story story = grouped.First();
                StringBuilder builder = new StringBuilder();
                builder.AppendLine($"Feature: {story.Metadata.Title}");
                builder.AppendLine($"Feature: {story.Metadata.Title}");
                builder.AppendLine($"\t{story.Metadata.Narrative1}");
                builder.AppendLine($"\t{story.Metadata.Narrative2}");
                builder.AppendLine($"\t{story.Metadata.Narrative3}");

                foreach (var scenario in grouped.SelectMany(e => e.Scenarios))
                {
                    builder.AppendLine(string.Empty);
                    builder.AppendLine($"\tScenario: {scenario.Title}");

                    foreach (var step in scenario.Steps.Where(e => e.ShouldReport))
                    {
                        builder.AppendLine($"\t\t{step.Title}");
                    }

                }

                File.WriteAllText($"Features\\{story.Metadata.Title}.feature", builder.ToString());
            }
        }
    }
}