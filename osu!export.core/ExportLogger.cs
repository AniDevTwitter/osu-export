using System;

namespace osu_export.core
{
    public delegate void ErrorLoggedHandler(object sender, ErrorLoggedEventArgs e);

    public class ErrorLoggedEventArgs : EventArgs
    {
        public ErrorLoggedEventArgs(Exception ex, string whatHappenned)
        {
            this.Exception = ex;
            this.Description = whatHappenned;
        }

        public string Description { get; private set; }

        public Exception Exception { get; private set; }
    }

    public class ExportLogger
    {
        private static readonly object locker = new object();
        private static ExportLogger instance = null;

        private ExportLogger()
        {
        }

        public event ErrorLoggedHandler ErrorLogged;

        public static ExportLogger GetInstance()
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new ExportLogger();
                    }
                }
            }
            return instance;
        }

        public void LogError(Exception ex, string whatHapenned)
        {
            if (this.ErrorLogged != null)
            {
                this.ErrorLogged(this, new ErrorLoggedEventArgs(ex, whatHapenned));
            }
        }
    }
}