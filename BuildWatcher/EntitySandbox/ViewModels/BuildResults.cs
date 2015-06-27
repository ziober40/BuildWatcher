using System;

namespace EntitySandbox.ViewModels
{
    public class BuildResults
    {

       

        public string BuildSK { get; set; }
        public string BuildBK { get; set; }
        public string BuildID { get; set; }
        public int BuildStatusSK { get; set; }
        public string BuildName { get; set; }
        public string BuildDefinitionName { get; set; }
        public DateTime BuildStartTime { get; set; }

        public int BuildDetailsSK { get; set; }
        public string BuildDetailsBK { get; set; }
        public Int64 BuildDuration { get; set; }

        public int RunCoverageSK { get; set; }
        public string RunCoverageBK { get; set; }
        public int LinesCovered { get; set; }
        public int LinesNotCovered { get; set; }
        public int BlocksCovered { get; set; }
        public int BlocksNotCovered { get; set; }
    }
}