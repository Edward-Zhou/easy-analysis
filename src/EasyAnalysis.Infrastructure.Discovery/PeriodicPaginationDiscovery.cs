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
        private string _nextLastStartUrl;

        private string _lastStartUrl;

        private bool _isFirst;

        public PeriodicPaginationDiscovery(PaginationDiscoveryConfigration config) : base(config)
        {
            _nextLastStartUrl = String.Empty;

            _lastStartUrl = string.Empty;
        }

        void IURIDiscovery.Start()
        {
            Logger.Current.Info("Start periodic pagination discovery");

            InternalStart();
        }

        private void InternalStart()
        {
            _isFirst = true;

            _isCanceled = false;

            _lastStartUrl = _nextLastStartUrl;

            _currentRunningTask = Dsicover();

            _currentRunningTask.Wait();

            Thread.Sleep(60 * 1000);

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
                _nextLastStartUrl = url;

                _isFirst = false;
            }

            base.InternalOnDiscovered(url);
        }
    }
}
