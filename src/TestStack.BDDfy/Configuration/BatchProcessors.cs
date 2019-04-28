using System.Collections.Generic;
using System.Linq;
using TestStack.BDDfy.Processors;
using TestStack.BDDfy.Reporters.Diagnostics;
using TestStack.BDDfy.Reporters.Html;
using TestStack.BDDfy.Reporters.MarkDown;

namespace TestStack.BDDfy.Configuration
{
    public class BatchProcessors
    {
        IEnumerable<IBatchProcessor> _GetProcessors()
        {
            var htmlReporter = HtmlReport.ConstructFor(StoryCache.Stories);
            if (htmlReporter != null)
                yield return htmlReporter;

            var htmlMetroReporter = HtmlMetroReport.ConstructFor(StoryCache.Stories);
            if (htmlMetroReporter != null)
                yield return htmlMetroReporter;

            var markDown = MarkDownReport.ConstructFor(StoryCache.Stories);
            if (markDown != null)
                yield return markDown;

            var diagnostics = DiagnosticsReport.ConstructFor(StoryCache.Stories);
            if (diagnostics != null)
                yield return diagnostics;

            foreach (var addedProcessor in _addedProcessors)
            {
                yield return addedProcessor;
            }
        }

        public BatchProcessorFactory HtmlReport { get; } = new BatchProcessorFactory(() => new HtmlReporter(new DefaultHtmlReportConfiguration()));

        public BatchProcessorFactory HtmlMetroReport { get; } = new BatchProcessorFactory(() => new HtmlReporter(new DefaultHtmlReportConfiguration(), new MetroReportBuilder()), false);

        public BatchProcessorFactory MarkDownReport { get; } = new BatchProcessorFactory(() => new MarkDownReporter(), false);

        public BatchProcessorFactory DiagnosticsReport { get; } = new BatchProcessorFactory(() => new DiagnosticsReporter(), false);

        readonly List<IBatchProcessor> _addedProcessors = new List<IBatchProcessor>();

        public BatchProcessors Add(IBatchProcessor processor)
        {
            _addedProcessors.Add(processor);
            return this;
        }

        public IEnumerable<IBatchProcessor> GetProcessors()
        {
            return _GetProcessors().ToList();
        }
    }
}