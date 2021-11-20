using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;

namespace Djenerative
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string VersionLink = "https://onedrive.live.com/download?cid=B0EF6D8226FA4E78&resid=B0EF6D8226FA4E78%21221726&authkey=ACSiUEwsBAZph3I";
        private const string DownloadLink = "https://onedrive.live.com/download?cid=B0EF6D8226FA4E78&resid=B0EF6D8226FA4E78%21221716&authkey=AP5XrkWUi_uQCRo";

        public App()
        {
        }

        private void Update()
        {
            try
            {
                string version = new WebClient().DownloadString(VersionLink).Replace("\\r\\n", "").Replace("\\n", "").Trim();
                bool success = int.TryParse(version, out int _);
                if (!success)
                {
                    Environment.FailFast("No Internet");
                }
                if (Djenerative.Properties.Preset.Default.Version == int.Parse(version)) return;

                string download = Path.Combine(Environment.CurrentDirectory, "update.tmp");
                new WebClient().DownloadFile(DownloadLink, download);

                var process = Process.GetCurrentProcess();
                string fullPath = process.MainModule!.FileName!;
                string exe = Path.GetFileName(fullPath);
                string tmp = Path.GetFileName(download);

                Djenerative.Properties.Preset.Default.Version = int.Parse(version);
                Djenerative.Properties.Preset.Default.Save();

                ProcessStartInfo Info = new ProcessStartInfo();
                Info.Arguments = $"/C choice /C Y /N /D Y /T 3 & Del \"\"\"{exe}\"\"\" & TIMEOUT /T 1 & Ren \"\"\"{tmp}\"\"\" \"\"\"{exe}\"\"\" & TIMEOUT /T 1 & \"\"\"{fullPath}\"\"\"";
                Info.WindowStyle = ProcessWindowStyle.Hidden;
                Info.CreateNoWindow = true;
                Info.FileName = "cmd.exe";
                Process.Start(Info);

                Environment.FailFast("Updating");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Environment.FailFast("Update Failed");
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
#if DEBUG
            return;
#endif
            Process proc = Process.GetCurrentProcess();
            int count = Process.GetProcesses().Count(p => p.ProcessName == proc.ProcessName);

            if (count > 1)
            {
                Environment.FailFast("Already Running");
            }
            Update();
        }
    }
}
