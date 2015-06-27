using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntitySandbox.Models
{
    [Table("dbo.FactCodeChurn")]
    public class FactCodeChurn
    {
        [Key]
        public int CodeChurnSK { get; set; }
        public string CodeChurnBK { get; set; }
        public int LinesAdded { get; set; }
        public int LinesModified { get; set; }
        public int LinesDeleted { get; set; }
        public int NetLinesAdded { get; set; }
    }
}