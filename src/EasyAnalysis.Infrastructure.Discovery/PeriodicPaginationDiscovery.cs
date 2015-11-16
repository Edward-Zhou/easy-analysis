using EasyAnalysis.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyAnalysis.Infrastructure.Discovery
{
    public class PeriodicPaginationDiscovery : PaginationDiscovery, IURIDiscovery
    {
        private string _lastStartUrl;

        private bool _isFirst;

        public PeriodicPaginationDiscovery(PaginationDiscoveryConfigration config) : base(config)
        {
            _lastStartUrl = String.Empty;
        }

        void IURIDiscovery.Start()
        {
            InternalStart();
        }

        private void InternalStart()
        {
            _isFirst = true;

            _isCanceled = false;

            _currentRunningTask = Dsicover();

            _currentRunningTask.Wait();

            Thread.Sleep(60 * 1000);

            Logger.Current.Info("start another round #");

            InternalStart();
        }

        protected override void InternalOnDiscovered(string url)
        {
            if(url == _lastStartUrl)
            {
                Cancel();

                return;
            }

            if (_isFirst)
            {
                _lastStartUrl = url;

                _isFirst = false;
            }

            base.InternalOnDiscovered(url);
        }

    }
}
