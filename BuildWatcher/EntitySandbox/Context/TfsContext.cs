using System.Data.Entity;
using EntitySandbox.Models;

namespace EntitySandbox.Context
{
    public class TfsContext : DbContext
    {
        public TfsContext() : base("TfsContext") {}

        public IDbSet<DimBuilds> DimBuilds { get; set; }
        public IDbSet<FactBuildDetails> FactBuildDetailses { get; set; }
        public IDbSet<FactCodeChurn> FactCodeChurns { get; set; }
        public IDbSet<FactRunCoverage> FactRunCoverages { get; set; }
    }
}