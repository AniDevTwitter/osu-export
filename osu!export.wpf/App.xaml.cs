using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace osu_export.wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly DebugTraceListener tracer = new DebugTraceListener();

        private class DebugTraceListener : TextWriterTraceListener
        {
            public static readonly string LogPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\osu!export\osu!export.log";

            public DebugTraceListener()
                : base(LogPath, "Tracer")
            {
                this.NeedIndent = true;
                this.TraceOutputOptions = TraceOptions.Timestamp | TraceOptions.Callstack | TraceOptions.ProcessId | TraceOptions.ThreadId | TraceOptions.DateTime;
            }

            public void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
            {
                Trace.TraceError("Exception occured, sender is : " + sender.ToString() + " detail : " + e.Exception.ToString());
                Dialogs.ShowMessage(@"An unhandled exception occured", "More info at : " + LogPath);
                e.Handled = true;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DebugTraceListener.LogPath));
            this.DispatcherUnhandledException += tracer.OnDispatcherUnhandledException;
            //PresentationTraceSources.Refresh();
            Trace.Listeners.Add(tracer);
            Debug.Listeners.Add(tracer);
#if DEBUG
            var level = SourceLevels.Critical | SourceLevels.Error | SourceLevels.Warning;
            var bindingLevel = SourceLevels.Critical | SourceLevels.Error;
#else
            var level = SourceLevels.Critical | SourceLevels.Error | SourceLevels.Warning;
            var bindingLevel = SourceLevels.Critical | SourceLevels.Error;

#endif
            PresentationTraceSources.AnimationSource.Switch.Level = level;
            PresentationTraceSources.AnimationSource.Listeners.Add(tracer);

            PresentationTraceSources.DependencyPropertySource.Switch.Level = level;
            PresentationTraceSources.DependencyPropertySource.Listeners.Add(tracer);

            PresentationTraceSources.DocumentsSource.Switch.Level = level;
            PresentationTraceSources.DocumentsSource.Listeners.Add(tracer);

            PresentationTraceSources.FreezableSource.Switch.Level = level;
            PresentationTraceSources.FreezableSource.Listeners.Add(tracer);

            PresentationTraceSources.HwndHostSource.Switch.Level = level;
            PresentationTraceSources.HwndHostSource.Listeners.Add(tracer);

            PresentationTraceSources.MarkupSource.Switch.Level = level;
            PresentationTraceSources.MarkupSource.Listeners.Add(tracer);

            PresentationTraceSources.NameScopeSource.Switch.Level = level;
            PresentationTraceSources.NameScopeSource.Listeners.Add(tracer);

            PresentationTraceSources.ResourceDictionarySource.Switch.Level = level;
            PresentationTraceSources.ResourceDictionarySource.Listeners.Add(tracer);

            PresentationTraceSources.RoutedEventSource.Switch.Level = level;
            PresentationTraceSources.RoutedEventSource.Listeners.Add(tracer);

            PresentationTraceSources.ShellSource.Switch.Level = level;
            PresentationTraceSources.ShellSource.Listeners.Add(tracer);

            PresentationTraceSources.MarkupSource.Switch.Level = level;
            PresentationTraceSources.MarkupSource.Listeners.Add(tracer);

            PresentationTraceSources.DataBindingSource.Switch.Level = bindingLevel;
            PresentationTraceSources.DataBindingSource.Listeners.Add(tracer);
            base.OnStartup(e);
        }
    }
}