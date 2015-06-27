using System.Linq;
using EntitySandbox.Context;
using EntitySandbox.ViewModels;

namespace EntitySandbox.Repositories
{
    public class TfsRepository
    {
        private readonly TfsContext _context;

        public TfsRepository(TfsContext context)
        {
            _context = context;
        }

        public IQueryable<BuildResults> GetLast(int amount) {
            return _context.DimBuilds
                    .Join(_context.FactBuildDetailses, dimbuild => dimbuild.BuildBK, factbuild => factbuild.BuildDetailsBK, (dimbuild, factbuild) => new { dimbuild, factbuild })
                    .Join(_context.FactRunCoverages, @t => @t.dimbuild.BuildSK, factcov => factcov.BuildSK, (@t, factcov) => new
                    {
                        @t.dimbuild.BuildBK,
                        @t.dimbuild.BuildDefinitionName,
                        @t.factbuild.BuildDetailsSK,
                        @t.factbuild.BuildDuration,
                        @t.dimbuild.BuildID,
                        @t.dimbuild.BuildName,
                        @t.dimbuild.BuildSK,
                        @t.dimbuild.BuildStartTime,
                        @t.factbuild.BuildStatusSK,
                        factcov.BlocksCovered,
                        factcov.BlocksNotCovered,
                        factcov.LinesCovered,
                        factcov.LinesNotCovered
                    })
                    .GroupBy(p => new
                    {
                        p.BuildBK,
                        p.BuildDefinitionName,
                        p.BuildDetailsSK,
                        p.BuildDuration,
                        p.BuildID,
                        p.BuildName,
                        p.BuildSK,
                        p.BuildStartTime,
                        p.BuildStatusSK
                    })
                    .Select(k => new BuildResults
                    {
                        BuildSK = k.Key.BuildBK,
                        BuildDefinitionName = k.Key.BuildDefinitionName,
                        BuildDetailsSK = k.Key.BuildDetailsSK,
                        BuildDuration = k.Key.BuildDuration,
                        BuildID = k.Key.BuildID,
                        BuildName = k.Key.BuildName,
                        BuildStartTime = k.Key.BuildStartTime,
                        BuildStatusSK = k.Key.BuildStatusSK,
                        BlocksCovered = k.Sum(n => n.BlocksCovered),
                        BlocksNotCovered = k.Sum(n => n.BlocksNotCovered),
                        LinesCovered = k.Sum(n => n.LinesCovered),
                        LinesNotCovered = k.Sum(n => n.LinesNotCovered),
                    })
                    .OrderByDescending(n => n.BuildStartTime)
                    .Take(amount);
        } 
    }
}