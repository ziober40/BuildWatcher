using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntitySandbox.Models
{
    [Table("dbo.FactRunCoverage")]
    public class FactRunCoverage
    {
        [Key]
        public int RunCoverageSK { get; set; }
        public string RunCoverageBK { get; set; }
        public int LinesCovered { get; set; }
        public int LinesNotCovered { get; set; }
        public int BlocksCovered { get; set; }
        public int BlocksNotCovered { get; set; }
        public int BuildSK { get; set; }
    }
}