using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using AdonisUI.Controls;
using MessageBox = AdonisUI.Controls.MessageBox;
using MessageBoxImage = AdonisUI.Controls.MessageBoxImage;
using MessageBoxResult = AdonisUI.Controls.MessageBoxResult;

namespace Djenerative
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string UpdateForce = "update.txt";
        private const string VersionLink = "https://onedrive.live.com/download?cid=B0EF6D8226FA4E78&resid=B0EF6D8226FA4E78%21221726&authkey=ACSiUEwsBAZph3I";
        private const string DownloadLink = "https://onedrive.live.com/download?cid=B0EF6D8226FA4E78&resid=B0EF6D8226FA4E78%21221716&authkey=AP5XrkWUi_uQCRo";
        private readonly HttpClient _client = new();

        public App()
        {
        }

        private void UpdateCheck()
        {
            try
            {
                int version = 0;

                try
                {
                    using HttpResponseMessage response = _client.GetAsync(VersionLink).Result;
                    using HttpContent content = response.Content;
                    string result = content.ReadAsStringAsync().Result;
                    string versionStr = result.Replace("\\r\\n", "").Replace("\\n", "").Trim();
                    version = int.Parse(versionStr);
                }
                catch
                {
                    MessageBox.Show("Please make sure the internet is connected", "No Internet");
                    Environment.FailFast("No Internet");
                }
                
                if (version == 0)
                {
                    MessageBox.Show("This version has expired", "Expired");
                    Environment.FailFast("Disabled");
                }

                if (File.Exists(UpdateForce))
                {
                    File.Delete(UpdateForce);
                    Update(version);
                    return;
                }

                if (Djenerative.Properties.Preset.Default.Version == version) return;

                Update(version);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Environment.FailFast("Update Failed");
            }
        }

        private void Update(int version)
        {
            var messageBox = new MessageBoxModel
            {
                Text = "We are going to update Djenerative.",
                Caption = "Update Available",
                Icon = MessageBoxImage.Information,
                Buttons = MessageBoxButtons.OkCancel(),
            };

            MessageBox.Show(messageBox);
            if (messageBox.Result == MessageBoxResult.Cancel) Environment.FailFast("Neglected Update");

            string download = Path.Combine(Environment.CurrentDirectory, "update.tmp");

            var data = _client.GetByteArrayAsync(DownloadLink).Result;
            File.WriteAllBytesAsync(download, data);

            var process = Process.GetCurrentProcess();
            string fullPath = process.MainModule!.FileName!;
            string exe = Path.GetFileName(fullPath);
            string tmp = Path.GetFileName(download);

            Djenerative.Properties.Preset.Default.Version = version;
            Djenerative.Properties.Preset.Default.Save();

            ProcessStartInfo info = new()
            {
                Arguments = $"/C choice /C Y /N /D Y /T 3 & Del \"\"\"{exe}\"\"\" & Ren \"\"\"{tmp}\"\"\" \"\"\"{exe}\"\"\" & \"\"\"{fullPath}\"\"\" & pause",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            };
            Process.Start(info);

            Environment.Exit(0);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {

#if DEBUG
            return;
#endif
#pragma warning disable CS0162 // Unreachable code detected

            Process proc = Process.GetCurrentProcess();
            int count = Process.GetProcesses().Count(p => p.ProcessName == proc.ProcessName);

            if (count > 1)
            {
                Environment.FailFast("Already Running");
            }
            UpdateCheck();
        }
    }
}
