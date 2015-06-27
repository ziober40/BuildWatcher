using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntitySandbox.Models
{
    [Table("dbo.FactBuildDetails")]
    public class FactBuildDetails
    {
        [Key]
        public int BuildDetailsSK { get; set; }
        public string BuildDetailsBK { get; set; }
        public Int64 BuildDuration { get; set; }
        public int BuildStatusSK { get; set; }
    }
}