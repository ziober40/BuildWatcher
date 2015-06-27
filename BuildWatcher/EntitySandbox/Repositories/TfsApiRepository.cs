using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EntitySandbox.ViewModels;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement.Client;

namespace EntitySandbox.Repositories
{
    public class TfsApiRepository
    {
        private readonly TfsTeamProjectCollection _tfs;
        private readonly IBuildServer _buildServer;
        private IBuildDetailSpec _buildDetailSpec;
        private List<RunningBuildTime> _notFinishedBuilds;

        public List<RunningBuildTime> RunningBuildTimes { get; private set; }
        public List<TfsBuildResults> TfsBuildResults { get; private set; }

        public TfsApiRepository(string uri)
        {
            

            var tfsUri = new Uri(uri);
            //var credential = new NetworkCredential("bartek.ziobrowski@live.com", "kubekstolik40");
            //_tfs = new TfsTeamProjectCollection(tfsUri,credential);
            _tfs = new TfsTeamProjectCollection(tfsUri);

            try
            {
                _tfs.EnsureAuthenticated();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Autentykacja sie nie powiodla.({0})", ex.Message));
            }


            _buildServer = (IBuildServer)_tfs.GetService(typeof(IBuildServer));


        }

        public List<TfsBuildResults> GetLastBuilds(int hoursFromNow) {

            _buildDetailSpec = _buildServer.CreateBuildDetailSpec("*");
            _buildDetailSpec.MinFinishTime = DateTime.Now.AddHours(-hoursFromNow);
            _buildDetailSpec.InformationTypes = null;  // for speed improvement
            _buildDetailSpec.QueryOrder = BuildQueryOrder.FinishTimeDescending;
            _buildDetailSpec.QueryOptions = QueryOptions.All;
            TfsBuildResults = new List<TfsBuildResults>();
            RunningBuildTimes = new List<RunningBuildTime>();

            var builds = _buildServer.QueryBuilds(_buildDetailSpec).Builds;

            builds.ToList().ForEach(build => TfsBuildResults.Add(new TfsBuildResults { 
                BuildNumber = build.BuildNumber,
                CompilationStatus = build.CompilationStatus == BuildPhaseStatus.Succeeded ? "Succeeded" : "Failed",
                DurationInMinutes = new TimeSpan(build.FinishTime.Ticks - build.StartTime.Ticks).TotalMinutes,
                FinishTime = build.FinishTime,
                LabelName = build.BuildNumber,
                StartTime = build.StartTime,
                Uri = build.Uri.ToString(),
                TeamProject = build.TeamProject,
                BuildDefinitionName = build.BuildDefinition.Name,
                IsCompleted = build.BuildFinished
            }));

            CalculateAvaregeBuildRuns();
          
            return TfsBuildResults;
        }

        public void CalculateAvaregeBuildRuns()
        {
            RunningBuildTimes = (from d in TfsBuildResults
                where d.IsCompleted
                group d by d.BuildDefinitionName
                into grp
                select new RunningBuildTime
                {
                    BuildName = grp.Key,
                    AvarageBuildTime = grp.Average(ed => ed.DurationInMinutes),
                }).ToList();
        }



        // I Don't have permission to do that
        //public void SubscribeToTfsEventService() {
        //    var es = _tfs.GetService(typeof(IEventService)) as IEventService;
        //    var eventName = string.Format("<PT N=\"Display name of event \"/>");
        //    var filter = string.Format("\"PortfolioProject\" = '{0}'", "Blah");
        //    var del = new DeliveryPreference
        //    {
        //        Address = "Webservice address",
        //        Schedule = DeliverySchedule.Immediate,
        //        Type = DeliveryType.Soap
        //    };
        //    if (es != null) {
        //        es.SubscribeEvent("WorkItemChangedEvent", filter, del, eventName);
        //    }
        //}

        public List<RunningBuildTime> GetNotFinishedBuilds()
        {
            _notFinishedBuilds = new List<RunningBuildTime>();
            _buildDetailSpec = _buildServer.CreateBuildDetailSpec("*");
            _buildDetailSpec.MinFinishTime = DateTime.Now.AddHours(-100);
            _buildDetailSpec.InformationTypes = null;  // for speed improvement
            _buildDetailSpec.QueryOrder = BuildQueryOrder.FinishTimeDescending;
            _buildDetailSpec.QueryOptions = QueryOptions.All;

            var builds = _buildServer.QueryBuilds(_buildDetailSpec).Builds;

            builds.Where(b => b.BuildFinished == false).ToList().ForEach(build => _notFinishedBuilds.Add(new RunningBuildTime
            {
                BuildDuration = new TimeSpan(DateTime.Now.Ticks - build.StartTime.Ticks).TotalMinutes,
                BuildName = build.BuildDefinition.Name,
                BuildStatus = build.CompilationStatus == BuildPhaseStatus.Succeeded ? "Succeeded" : "Failed"
            }));

            return _notFinishedBuilds;
        }


        public List<TfsBuildResults> GetLastBuild(int hoursFromNow, string team)
        {

            _buildDetailSpec = _buildServer.CreateBuildDetailSpec("*");
            _buildDetailSpec.MinFinishTime = DateTime.Now.AddHours(-hoursFromNow);
            _buildDetailSpec.InformationTypes = null;  // for speed improvement
            _buildDetailSpec.QueryOrder = BuildQueryOrder.FinishTimeDescending;
            _buildDetailSpec.QueryOptions = QueryOptions.All;
            var stopper = new Stopwatch();
            stopper.Start();

            var tfsBuildResults = new List<TfsBuildResults>();
            var builds = _buildServer.QueryBuilds(_buildDetailSpec).Builds;

            builds.Where(k => k.TeamProject == team).ToList().ForEach(build => tfsBuildResults.Add(new TfsBuildResults
            {
                BuildNumber = build.BuildNumber,
                CompilationStatus = build.CompilationStatus == BuildPhaseStatus.Succeeded ? "Succeeded" : "Failed",
                DurationInMinutes = new TimeSpan(build.FinishTime.Ticks - build.StartTime.Ticks).Minutes,
                FinishTime = build.FinishTime,
                LabelName = build.LabelName,
                StartTime = build.StartTime,
                Uri = build.Uri.ToString(),
                TeamProject = build.BuildDefinition.Name,
            }));

            return tfsBuildResults;
        }

        public Tuple<int,int> GetTestResult(Uri buildUri, string teamProject)
        {
            var tms = _tfs.GetService<ITestManagementService>();
            var testRuns = tms.GetTeamProject(teamProject).TestRuns.ByBuild(buildUri);
            var enumerable = testRuns as IList<ITestRun> ?? testRuns.ToList();
            if (!enumerable.Any()) return Tuple.Create(0, 0);

            var last = enumerable.Last();

            return Tuple.Create(last.PassedTests, last.TotalTests);
        }
    }
}