using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using osu_export.core;
using System.Collections.ObjectModel;

namespace osu_export.wpf
{
    public class MainViewModel : AbstractViewModel<MainViewModel>
    {
        private readonly ObservableCollection<string> errors;
        private string installPath;
        private string outputFolder;
        private int progress;
        private Command export;

        public MainViewModel()
        {
            this.errors = new ObservableCollection<string>();
            this.progress = 0;
            this.outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), @"osu!export");
            if(!OsuFolder.FolderPathFromRegistry(out this.installPath))
            {
                this.installPath = string.Empty;
            }
            Directory.CreateDirectory(this.outputFolder);
            this.export = new Command(this.ExportAction, this.CanExport);
            ExportLogger.GetInstance().ErrorLogged += OnError;
        }

        public Command Export
        {
            get
            {
                return this.export;
            }
        }

        private void OnError(object sender, ErrorLoggedEventArgs e)
        {
            this.errors.Add("Error : " + e.Description + " [" + e.Exception.ToString() + "]");
        }

        public ObservableCollection<string> Errors
        {
            get
            {
                return this.errors;
            }
        }

        private bool CanExport(object obj)
        {
            return Directory.Exists(this.outputFolder) && Directory.Exists(this.installPath);
        }

        private void ExportAction(object obj)
        {
            this.export.IsEnabled = false;
            this.Progress = 0;
            var bgw = new OsuFolder(this.installPath).ExportSongs(this.outputFolder);
            bgw.ProgressChanged += this.ExportProgressChanged;
            bgw.RunWorkerCompleted += this.ExportCompleted;
            bgw.RunWorkerAsync();
        }

        private void ExportCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Dialogs.ShowMessage("Finished !", "Finished with the export, check the logs for potential problems encountered");
            this.export.IsEnabled = true;
        }

        private void ExportProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Progress = e.ProgressPercentage;
        }

        public int Progress
        {
            get
            {
                return this.progress;
            }
            set
            {
                this.progress = value;
                this.OnPropertyChanged(this.NameOf(x => x.Progress));
            }
        }

        public string OutputFolder
        {
            get
            {
                return this.outputFolder;
            }
            set
            {
                this.outputFolder = value;
                this.OnPropertyChanged(this.NameOf(x => x.OutputFolder));
                this.export.RaiseCanExecuteChanged();
            }
        }

        public string InstallPath
        {
            get
            {
                return this.installPath;
            }
            set
            {
                this.installPath = value;
                this.OnPropertyChanged(this.NameOf(x => x.InstallPath));
                this.export.RaiseCanExecuteChanged();
            }
        }
    }
}
