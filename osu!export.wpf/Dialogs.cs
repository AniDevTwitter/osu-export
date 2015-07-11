using Ookii.Dialogs.Wpf;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace osu_export.wpf
{
    public static class Dialogs
    {
        public const string AllFilter = "All files (*.*)|*.*";

        public static MessageBoxResult ShowMessage(string title, string description, MessageBoxButton buttons = MessageBoxButton.OK)
        {
            return App.Current.Dispatcher.Invoke(() => Xceed.Wpf.Toolkit.MessageBox.Show(App.Current.MainWindow, description, title, buttons));
        }

        public static bool TryFilePath(string dialogTitle, string filter, out string fileName)
        {
            var fileSelector = new VistaOpenFileDialog()
            {
                Title = dialogTitle,
                Filter = filter,
                Multiselect = false,
                CheckFileExists = true,
                DefaultExt = filter.Split('|').ElementAt(1).Replace("*", "") // It SHOULD be an extension method, so todo
            };
            if (fileSelector.ShowDialog() == true)
            {
                fileName = fileSelector.FileName;
                return true;
            }
            else
            {
                fileName = string.Empty;
                return false;
            }
        }

        public static bool TryFileSavePath(string dialogTitle, string filter, out string fileName)
        {
            var fileSelector = new VistaSaveFileDialog()
            {
                Title = dialogTitle,
                Filter = filter,
                DefaultExt = filter.Split('|').ElementAt(1).Replace("*", "") // It SHOULD be an extension method, so todo
            };
            if (fileSelector.ShowDialog() == true)
            {
                fileName = fileSelector.FileName;
                return true;
            }
            else
            {
                fileName = string.Empty;
                return false;
            }
        }

        public static bool TryFilesPaths(string dialogTitle, string filter, out List<string> filesPaths)
        {
            var fileSelector = new VistaOpenFileDialog()
            {
                Title = dialogTitle,
                Filter = filter,
                Multiselect = true,
                CheckFileExists = true,
                DefaultExt = filter.Split('|').ElementAt(1).Replace("*", "") // It SHOULD be an extension method, so todo
            };

            if (fileSelector.ShowDialog() == true)
            {
                filesPaths = fileSelector.FileNames.ToList();
                return true;
            }
            else
            {
                filesPaths = new List<string>();
                return false;
            }
        }

        public static bool TryFolderPath(string description, out string folderPath)
        {
            var folderSelector = new VistaFolderBrowserDialog()
            {
                Description = description
            };
            if (folderSelector.ShowDialog() == true)
            {
                folderPath = folderSelector.SelectedPath;
                return true;
            }
            else
            {
                folderPath = string.Empty;
                return false;
            }
        }
    }
}