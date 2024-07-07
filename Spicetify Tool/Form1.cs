using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spicetify_Tool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void Install_Click(object sender, EventArgs e)
        {
            try
            {
                status.Text = "Status: Checking Spotify version";

                bool isMicrosoftStoreVersion = IsMicrosoftStoreVersionInstalled();
                if (isMicrosoftStoreVersion)
                {
                    status.Text = "Status: Uninstalling Microsoft Store version of Spotify";
                    await UninstallMicrosoftStoreVersion();
                }

                status.Text = "Status: Installing standalone version of Spotify";
                await InstallStandaloneVersion();

                status.Text = "Status: Installing Spicetify";
                await ExecutePowerShellCommandAsync("iwr -useb https://raw.githubusercontent.com/spicetify/cli/main/install.ps1 | iex");

                status.Text = "Status: Installing Spicetify Marketplace";
                await ExecutePowerShellCommandAsync("iwr -useb https://raw.githubusercontent.com/spicetify/marketplace/main/resources/install.ps1 | iex");

                status.Text = "Status: Installation Complete";
                MessageBox.Show("Spicetify successfully installed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                status.Text = "Status: Error during installation";
                MessageBox.Show($"Error during installation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsMicrosoftStoreVersionInstalled()
        {
            string msStorePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Packages\SpotifyAB.SpotifyMusic_zpdnekdrzrea0";
            return System.IO.Directory.Exists(msStorePath);
        }

        private Task UninstallMicrosoftStoreVersion()
        {
            return Task.Run(() =>
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-NoProfile -ExecutionPolicy Bypass -Command \"Get-AppxPackage *SpotifyAB.SpotifyMusic* | Remove-AppxPackage\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
                    process.BeginOutputReadLine();

                    process.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                }
            });
        }

        private Task InstallStandaloneVersion()
        {
            return Task.Run(() =>
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-NoProfile -ExecutionPolicy Bypass -Command \"Invoke-WebRequest -Uri https://download.scdn.co/SpotifySetup.exe -OutFile $env:USERPROFILE\\Downloads\\SpotifySetup.exe; Start-Process -FilePath $env:USERPROFILE\\Downloads\\SpotifySetup.exe -Wait\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
                    process.BeginOutputReadLine();

                    process.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                }
            });
        }

        private Task ExecutePowerShellCommandAsync(string command)
        {
            return Task.Run(() =>
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
                    process.BeginOutputReadLine();

                    process.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                }
            });
        }

        private void Spotify_version_check_Click(object sender, EventArgs e)
        {
            string version = CheckSpotifyVersion();
            status.Text = version;
        }
        private string CheckSpotifyVersion()
        {
            if (IsMicrosoftStoreVersionInstalled2())
            {
                return "Status: Microsoft Store version of Spotify is installed.";
            }
            else if (IsStandaloneVersionInstalled())
            {
                return "Status: Standalone version of Spotify is installed.";
            }
            else
            {
                return "Status: No version of Spotify is installed.";
            }
        }

        private bool IsMicrosoftStoreVersionInstalled2()
        {
            string msStorePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Packages\SpotifyAB.SpotifyMusic_zpdnekdrzrea0";
            return System.IO.Directory.Exists(msStorePath);
        }

        private bool IsStandaloneVersionInstalled()
        {
            string standalonePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Spotify";
            return System.IO.Directory.Exists(standalonePath);
        }

        private async void Remove_Spicetify_Click(object sender, EventArgs e)
        {
            try
            {
                status.Text = "Status: Removing Spicetify";
                await ExecutePowerShellCommandAsync("spicetify restore");
                await ExecutePowerShellCommandAsync("Remove-Item -Recurse -Force $env:APPDATA\\spicetify");
                await ExecutePowerShellCommandAsync("Remove-Item -Recurse -Force $env:LOCALAPPDATA\\spicetify");

                status.Text = "Status: Spicetify removed successfully";
                MessageBox.Show("Spicetify removed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                status.Text = "Status: Error during removal";
                MessageBox.Show($"Error during removal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void status_Click(object sender, EventArgs e)
        {

        }

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
