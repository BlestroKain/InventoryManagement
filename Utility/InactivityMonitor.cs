using System;
using System.Windows.Forms;

namespace RapiMesa.Utility
{
    public static class InactivityMonitor
    {
        private static Timer _timer;
        private static ActivityFilter _filter;
        private static Action _onTimeout;

        private class ActivityFilter : IMessageFilter
        {
            public event EventHandler Activity;
            public bool PreFilterMessage(ref Message m)
            {
                const int WM_MOUSEMOVE = 0x0200;
                const int WM_KEYDOWN = 0x0100;
                const int WM_LBUTTONDOWN = 0x0201;
                const int WM_RBUTTONDOWN = 0x0204;
                const int WM_MBUTTONDOWN = 0x0207;

                switch (m.Msg)
                {
                    case WM_MOUSEMOVE:
                    case WM_KEYDOWN:
                    case WM_LBUTTONDOWN:
                    case WM_RBUTTONDOWN:
                    case WM_MBUTTONDOWN:
                        Activity?.Invoke(this, EventArgs.Empty);
                        break;
                }
                return false;
            }
        }

        public static void Start(int timeoutSeconds, Action onTimeout)
        {
            _onTimeout = onTimeout;
            _timer = new Timer { Interval = timeoutSeconds * 1000 };
            _timer.Tick += (s, e) =>
            {
                _timer.Stop();
                _onTimeout?.Invoke();
            };
            _timer.Start();

            _filter = new ActivityFilter();
            _filter.Activity += (s, e) => Reset();
            Application.AddMessageFilter(_filter);
        }

        public static void Reset()
        {
            if (_timer == null) return;
            _timer.Stop();
            _timer.Start();
        }

        public static void Stop()
        {
            if (_filter != null)
            {
                Application.RemoveMessageFilter(_filter);
                _filter = null;
            }
            _timer?.Stop();
            _timer = null;
        }
    }
}
