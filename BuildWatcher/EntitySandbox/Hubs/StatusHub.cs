using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using EntitySandbox.Repositories;
using EntitySandbox.ViewModels;
using Microsoft.AspNet.SignalR;

namespace EntitySandbox.Hubs
{
    public class StatusHub : Hub
    {
        private const int PeriodInSec = 5;
        private const int SecondsAfterFinishedBuildCheck = 900;
        private readonly TfsApiRepository _tfsApiRepository;
        private List<RunningBuildTime> _recentNotFinishedBuilds;
        private int _second;

        public StatusHub()
        {
            _tfsApiRepository = new TfsApiRepository(System.Configuration.ConfigurationManager.AppSettings["TfsAddress"]);
            _second = 1000;
            _tfsApiRepository.GetLastBuilds(_second);
            _recentNotFinishedBuilds = new List<RunningBuildTime>();

            const int periodInSec = PeriodInSec;
            var obs = Observable.Interval(TimeSpan.FromSeconds(periodInSec),
                                          Scheduler.Default);
            obs.Subscribe(x => Send());
        }
         
        public void Send()
        {
            var notFinishedBuilds= _tfsApiRepository.GetNotFinishedBuilds();

            if (notFinishedBuilds.Count != _recentNotFinishedBuilds.Count)
            {
                // if there is less unfinished builds then it was
                if (_recentNotFinishedBuilds.Count > notFinishedBuilds.Count)
                {
                    Clients.All.refreshfview(_recentNotFinishedBuilds);
                }

                _recentNotFinishedBuilds = notFinishedBuilds;
                
                return;
            }

            notFinishedBuilds.ForEach(n =>
                n.AvarageBuildTime = _tfsApiRepository.RunningBuildTimes.First(r => r.BuildName == n.BuildName).AvarageBuildTime
                );

            Clients.All.getnotfinishedbuilds(notFinishedBuilds);
            
            _recentNotFinishedBuilds = notFinishedBuilds;
        }

        public bool IfAnyBuildWasCompletedRecently(int seconds) {
            var periodFrom = new TimeSpan(0, 0, seconds);

            return _tfsApiRepository.TfsBuildResults.Any(n => new TimeSpan(DateTime.Now.Ticks - n.FinishTime.Ticks).TotalMinutes < periodFrom.TotalMinutes);
        }


    }
}