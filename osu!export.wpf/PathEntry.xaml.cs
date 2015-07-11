using System.Windows;
using System.Windows.Controls;


namespace osu_export.wpf
{
    /// <summary>
    /// Logique d'interaction pour PathEntry.xaml
    /// </summary>
    public partial class PathEntry : UserControl
    {
        // Using a DependencyProperty as the backing store for Filters.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FiltersProperty =
            DependencyProperty.Register("Filters", typeof(string), typeof(PathEntry), new PropertyMetadata(Dialogs.AllFilter));

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(PathEntry), new PropertyMetadata(string.Empty));

        // Using a DependencyProperty as the backing store for Mode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("PathMode", typeof(PathMode), typeof(PathEntry), new PropertyMetadata(PathMode.FileSelection));

        // Using a DependencyProperty as the backing store for Path.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(string), typeof(PathEntry), new FrameworkPropertyMetadata(string.Empty)
            {
                BindsTwoWayByDefault = true
            });

        private readonly Command pathBrowse;

        public PathEntry()
        {
            this.pathBrowse = new Command(this.OnPathBrowse);
            InitializeComponent();
        }

        public string Filters
        {
            get { return (string)this.GetValue(FiltersProperty); }
            set { this.SetValue(FiltersProperty, value); }
        }

        public string Label
        {
            get
            {
                return (string)this.GetValue(PathEntry.LabelProperty);
            }
            set
            {
                this.SetValue(PathEntry.LabelProperty, value);
            }
        }

        public PathMode Mode
        {
            get { return (PathMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        public string Path
        {
            get { return (string)this.GetValue(PathProperty); }
            set { this.SetValue(PathProperty, value); }
        }

        public Command PathBrowse
        {
            get
            {
                return this.pathBrowse;
            }
        }

        private void OnPathBrowse(object obj)
        {
            string path;
            switch (this.Mode)
            {
                case PathMode.FileSave:
                    if (Dialogs.TryFileSavePath(this.Label, this.Filters, out path))
                    {
                        this.Path = path;
                    }
                    break;

                case PathMode.FileSelection:
                    if (Dialogs.TryFilePath(this.Label, this.Filters, out path))
                    {
                        this.Path = path;
                    }
                    break;

                case PathMode.FolderSelection:
                    if (Dialogs.TryFolderPath(this.Label, out path))
                    {
                        this.Path = path;
                    }
                    break;
            }
        }

        public enum PathMode
        {
            FolderSelection,
            FileSelection,
            FileSave
        }
    }
}