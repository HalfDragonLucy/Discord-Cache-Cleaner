using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using AutoUpdaterDotNET;

namespace ReworkedDCC
{
    public partial class Form1 : Form
    {
        public static string cacheDirectory;
        public static bool runAtStart;

        public static readonly string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string defaultDirectory = roaming + @"\discord\Cache";
        public static readonly string directoryConfig = roaming + @"\DiscordCacheCleaner";
        public static readonly string fileConfig = directoryConfig + @"\config.json";

        public Form1()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            if (File.Exists(fileConfig))
            {
                SaveSystem.LoadSave(fileConfig);
                CheckForKey();

                if (cacheDirectory == null)
                {
                    return;
                }

                RunAtStartUpCheck.Checked = runAtStart;
                DirectoryLabel.Text = cacheDirectory;
                StartCleaning();
            }
            else
            {
                Directory.CreateDirectory(directoryConfig);
                File.Create(fileConfig).Close();

                SaveSystem.InitSave(fileConfig);

                if (Directory.Exists(defaultDirectory))
                {
                    cacheDirectory = defaultDirectory;

                    SaveSystem.Save(fileConfig);
                }
                else
                {
                    MessageBox.Show("We couldn't find your Discord cache directory.\n Please set it manually");
                }

            }
        }

        private void CheckForKey()
        {
            string key_path = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
            string key_name = "Discord Auto Cleaner";

            RegistryKey key = Registry.CurrentUser.OpenSubKey(key_path, true);

            if (runAtStart == true)
            {
                if (key == null)
                {
                    RegisteryAutoStart();
                }
            }
            else
            {
                try
                {
                    if (key != null)
                    {
                        key.DeleteValue(key_name);
                    }
                }
                catch (Exception)
                {
                    return;
                }               
            }

        }

        private void RegisteryAutoStart()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            key.SetValue("Discord Auto Cleaner", AppDomain.CurrentDomain.BaseDirectory + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe".ToString());
        }

        private void StartCleaning()
        {
            Thread cacheCleaner = new Thread(() =>
            {
                CheckCacheAndDelete(cacheDirectory);
            })
            {
                IsBackground = true
            };
            cacheCleaner.Start();
        }

        public static void CheckCacheAndDelete(string path)
        {
            if (path.Length > 0)
            {
                if (Process.GetProcessesByName("Discord").Length == 0)
                {
                    DirectoryInfo di = new DirectoryInfo(path);

                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }

                    Thread.Sleep(1000);
                }
                else
                {
                    var discord = Process.GetProcessesByName("Discord");
                    string pathDiscord = discord[0].MainModule.FileName;

                    discord[0].Kill();
                    Thread.Sleep(1000);

                    DirectoryInfo di = new DirectoryInfo(path);

                    foreach (FileInfo file in di.GetFiles())
                    {

                        file.Delete();

                    }

                    Process.Start(pathDiscord);
                }
            }
        }

        private void GitHubPictureBox_Click(object sender, EventArgs e)
        {
            string url = "https://github.com/HalfDragonLucy";
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void SetDirectoryButton_Click(object sender, EventArgs e)
        {
            CacheBrowser.Description = "Select your Discord Cache folder";
            CacheBrowser.UseDescriptionForTitle = true;
            if (CacheBrowser.ShowDialog() == DialogResult.OK)
            {
                cacheDirectory = CacheBrowser.SelectedPath;
                DirectoryLabel.Text = CacheBrowser.SelectedPath;
            }
        }

        private void SaveExitButton_Click(object sender, EventArgs e)
        {
            SaveSystem.Save(fileConfig);
            Environment.Exit(0);
        }

        private void RunAtStartUpCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (RunAtStartUpCheck.CheckState == CheckState.Checked)
            {
                runAtStart = true;
            }
            else
            {
                runAtStart = false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSystem.Save(fileConfig);
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            StartCleaning();
        }
    }
}
