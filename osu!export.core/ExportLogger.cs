using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Exception Exception { get; private set; }
        public string Description { get; private set; }
    }


    public class ExportLogger
    {
        private static readonly object locker = new object();
        private static ExportLogger instance = null;
        public event ErrorLoggedHandler ErrorLogged;

        private ExportLogger()
        {

        }

        public void LogError(Exception ex, string whatHapenned)
        {
            if(this.ErrorLogged == null)
            {
                this.ErrorLogged(this, new ErrorLoggedEventArgs(ex, whatHapenned));
            }
        }

        public static ExportLogger GetInstance()
        {
            if(instance == null)
            {
                lock(locker)
                {
                    if(instance == null)
                    {
                        instance = new ExportLogger();
                    }
                }
            }
            return instance;
        }
    }
}
