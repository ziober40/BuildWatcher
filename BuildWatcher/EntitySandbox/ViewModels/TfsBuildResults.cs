using System;

namespace EntitySandbox.ViewModels
{
    public class TfsBuildResults
    {
        public string Uri { get; set; }
        public string BuildNumber { get; set; }
        public string CompilationStatus { get; set; }
        public double DurationInMinutes { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public string LabelName { get; set; }
        public string TeamProject { get; set; }
        public int PassedTests { get; set; }
        public int TotalTests { get; set; }
        public string BuildDefinitionName { get; set; }
        public bool IsCompleted { get; set; }
    }
}