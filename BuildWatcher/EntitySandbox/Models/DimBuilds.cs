using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntitySandbox.Models
{
    [Table("dbo.DimBuild")]
    public class DimBuilds
    {
        [Key]
        public int BuildSK { get; set; }
        public string BuildBK { get; set; }
        public string BuildID { get; set; }
        public string BuildName { get; set; }
        public string BuildDefinitionName { get; set; }
        public DateTime BuildStartTime { get; set; }
    }
}