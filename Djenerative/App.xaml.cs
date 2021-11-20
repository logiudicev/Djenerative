using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace Djenerative
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            try
            {
                Update();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private void Update()
        {
            string version = new WebClient().DownloadString("https://onedrive.live.com/download?cid=B0EF6D8226FA4E78&resid=B0EF6D8226FA4E78%21221726&authkey=ACSiUEwsBAZph3I").Replace("\\r\\n", "").Replace("\\n", "").Trim();
            bool success = int.TryParse(version, out int _);
            if (!success)
            {
                Environment.Exit(0);
            }
            if (Djenerative.Properties.Preset.Default.Version == int.Parse(version)) return;

            string download = Path.Combine(Environment.CurrentDirectory, "update.tmp");
            new WebClient().DownloadFile("https://onedrive.live.com/download?cid=B0EF6D8226FA4E78&resid=B0EF6D8226FA4E78%21221716&authkey=AP5XrkWUi_uQCRo", download);

            var process = Process.GetCurrentProcess();
            string fullPath = process.MainModule!.FileName!;
            string exe = Path.GetFileName(fullPath);
            string tmp = Path.GetFileName(download);

            Djenerative.Properties.Preset.Default.Version = int.Parse(version);
            Djenerative.Properties.Preset.Default.Save();

            {
                ProcessStartInfo Info = new ProcessStartInfo();
                Info.Arguments = $"/C choice /C Y /N /D Y /T 1 & Del {exe}";
                Info.WindowStyle = ProcessWindowStyle.Hidden;
                Info.CreateNoWindow = true;
                Info.FileName = "cmd.exe";
                Process.Start(Info);
            }
            {
                ProcessStartInfo Info = new ProcessStartInfo();
                Info.Arguments = $"/C choice /C Y /N /D Y /T 2 & Ren {tmp} {exe}";
                Info.WindowStyle = ProcessWindowStyle.Hidden;
                Info.CreateNoWindow = true;
                Info.FileName = "cmd.exe";
                Process.Start(Info);
            }
            {
                ProcessStartInfo Info = new ProcessStartInfo();
                Info.Arguments = $"/C choice /C Y /N /D Y /T 3 & start {fullPath}";
                Info.WindowStyle = ProcessWindowStyle.Hidden;
                Info.CreateNoWindow = true;
                Info.FileName = "cmd.exe";
                Process.Start(Info);
            }

            Environment.Exit(0);
        }
    }
}
