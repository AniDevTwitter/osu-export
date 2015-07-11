using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;

namespace osu_export.core
{
    public class OsuFolder
    {
        private readonly string path;

        public OsuFolder(string path)
        {
            this.path = path;
        }

        public IEnumerable<OsuBeatmapFolder> BeatmapsFolders
        {
            get
            {
                foreach (var songDir in Directory.GetDirectories(Path.Combine(this.path, OsuBeatmapFolder.Subpath)))
                {
                    yield return new OsuBeatmapFolder(songDir);
                }
            }
        }

        public int CountBeatmapsFolders
        {
            get
            {
                return Directory.GetDirectories(Path.Combine(this.path, OsuBeatmapFolder.Subpath)).Length;
            }
        }

        public static bool FolderPathFromRegistry(out string path)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (var subkey_name in key.GetSubKeyNames())
            {
                RegistryKey subkey = key.OpenSubKey(subkey_name);
                if ((string)subkey.GetValue("DisplayName") == "osu!")
                {
                    var uninstallstring = subkey.GetValue("UninstallString").ToString();
                    path = Path.GetDirectoryName(uninstallstring.Remove(uninstallstring.Length - 11));
                    return true;
                }
            }
            path = default(string);
            return false;
        }

        public BackgroundWorker ExportSongs(string folder)
        {
            var retVal = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = false
            };
            var i = 0;
            var count = this.CountBeatmapsFolders;
            Directory.CreateDirectory(folder);
            retVal.DoWork += (sender, e) => this.BeatmapsFolders.AsParallel().ForAll(x =>
            {
                retVal.ReportProgress((int)Math.Round((((double)Interlocked.Increment(ref i) / (double)count)) * 100));
                x.ExportSong(folder);
            });
            return retVal;
        }
    }
}