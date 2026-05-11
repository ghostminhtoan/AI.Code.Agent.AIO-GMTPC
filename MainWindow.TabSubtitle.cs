using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Management;
using Microsoft.Win32;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Net.Http;
using System.Windows.Controls;
using System.Windows.Data;

namespace AICodeAgentAIOGMTPC
{
    public partial class MainWindow
    {
        /*
         * AI Summary:
         * Date: 2026-04-24 (2)
         * - Updated InstallSampleVideoAsync to download sample video into %USERPROFILE%\Videos and open its containing folder
         * Date: 2026-04-24
         * - Added ChkDownloadSampleVideo and InstallSampleVideoAsync to download sample video directly to C:\ and open its containing folder
         * Date: 2026-04-13
         * - Added ChkSubtitleDraftGMTPC and InstallSubtitleDraftGMTPCAsync with download to C:\, desktop shortcut, and open file
         * Date: 2026-03-29 (3)
         * - Added 3 new checkboxes: ChkBoilsoftVideoSplitter, ChkVibe, ChkMKVToolNix
         * - Using InstallWithPromptAsync mechanism (Yes/No dialog)
         * Date: 2026-03-29 (2)
         * - Added Desktop shortcut creation for VidCoder after download
         * Date: 2026-03-29
         * - Created MainWindow.TabSubtitle.cs for Subtitle tab
         * - Added ChkVidCoder_Click, InstallVidCoderAsync with GitHub latest version probe
         * Note: ChkSubtitleEdit_Click and InstallSubtitleEditAsync remain in MainWindow.TabOffice.cs
         */

        // ===================================================================
        // TabSubtitle â€” VidCoder
        // ===================================================================
        private void ChkVidCoder_Click(object sender, RoutedEventArgs e)
        {
            if (ChkVidCoder.IsChecked == true)
            {
                UpdateStatus("ÄÃ£ chá»n: VidCoder", "Green");
            }
            else
            {
                UpdateStatus("ÄÃ£ há»§y chá»n: VidCoder", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private async Task InstallVidCoderAsync()
        {
            try
            {
                // BÆ°á»›c 1: Táº¡o folder C:\Vidcoder náº¿u chÆ°a tá»“n táº¡i
                string vidCoderFolder = @"C:\Vidcoder";
                if (!Directory.Exists(vidCoderFolder))
                {
                    Directory.CreateDirectory(vidCoderFolder);
                    UpdateStatus($"ÄÃ£ táº¡o folder {vidCoderFolder}", "Cyan");
                }

                // BÆ°á»›c 2: Táº£i VidCoder.exe tá»« link cá»‘ Ä‘á»‹nh cá»§a MMT
                string vidCoderExeUrl = VIDCODER_DOWNLOAD_URL;
                string vidCoderExePath = Path.Combine(vidCoderFolder, "VidCoder.exe");
                
                UpdateStatus("Äang táº£i VidCoder...", "Cyan");
                await DownloadWithProgressAsync(vidCoderExeUrl, vidCoderExePath, "VidCoder");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                // BÆ°á»›c 3: Táº£i file VidCoder.sqlite tá»« MMT repo
                string vidCoderSqliteUrl = "https://github.com/ghostminhtoan/MMT/releases/download/v1.0/VidCoder.sqlite";
                string vidCoderSqlitePath = Path.Combine(vidCoderFolder, "VidCoder.sqlite");

                UpdateStatus("Äang táº£i VidCoder.sqlite...", "Cyan");
                using (WebClient client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(vidCoderSqliteUrl, vidCoderSqlitePath);
                }

                UpdateStatus("ÄÃ£ táº£i xong VidCoder.sqlite", "Green");

                // BÆ°á»›c 4: Táº¡o shortcut trÃªn Desktop
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string shortcutPath = Path.Combine(desktopPath, "VidCoder.lnk");
                
                // XÃ³a shortcut cÅ© náº¿u tá»“n táº¡i
                if (File.Exists(shortcutPath))
                {
                    File.Delete(shortcutPath);
                }
                
                // Táº¡o shortcut má»›i sá»­ dá»¥ng WshShell
                try
                {
                    Type shellType = Type.GetTypeFromProgID("WScript.Shell");
                    if (shellType != null)
                    {
                        object shell = Activator.CreateInstance(shellType);
                        object shortcut = shellType.InvokeMember("CreateShortcut", System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { shortcutPath });
                        
                        // Set cÃ¡c thuá»™c tÃ­nh shortcut
                        shellType.InvokeMember("TargetPath", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { vidCoderExePath });
                        shellType.InvokeMember("WorkingDirectory", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { vidCoderFolder });
                        shellType.InvokeMember("Description", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { "VidCoder - Video transcoder" });
                        shellType.InvokeMember("Save", System.Reflection.BindingFlags.InvokeMethod, null, shortcut, null);
                        
                        UpdateStatus("ÄÃ£ táº¡o shortcut VidCoder trÃªn Desktop", "Green");
                    }
                }
                catch (Exception ex)
                {
                    UpdateStatus($"KhÃ´ng thá»ƒ táº¡o shortcut: {ex.Message}", "Orange");
                }

                // BÆ°á»›c 5: Chá»‰ cháº¡y file .exe sau khi táº£i xong SQLite
                UpdateStatus("Äang má»Ÿ VidCoder...", "Cyan");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = vidCoderExePath,
                    UseShellExecute = true,
                    WorkingDirectory = vidCoderFolder
                };
                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    UpdateStatus("VidCoder Ä‘Ã£ Ä‘Æ°á»£c má»Ÿ!", "Green");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lá»—i khi cÃ i Ä‘áº·t VidCoder: {ex.Message}", "Red");
            }
        }

        // Legacy GitHub-version probe kept in case we ever need a fallback again.
        private async Task<string> GetLatestVidCoderVersionAsync()
        {
            try
            {
                // Sá»­ dá»¥ng GitHub API Ä‘á»ƒ láº¥y danh sÃ¡ch releases
                string apiUrl = "https://api.github.com/repos/RandomEngy/VidCoder/releases";
                
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
                request.UserAgent = "AI-Code-Agent-AIO-GMTPC";
                request.Accept = "application/json";

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string json = await reader.ReadToEndAsync();
                    
                    // Parse JSON Ä‘Æ¡n giáº£n Ä‘á»ƒ tÃ¬m táº¥t cáº£ versions
                    var versions = new List<(string Version, int BuildNumber)>();
                    
                    // TÃ¬m táº¥t cáº£ cÃ¡c tag_name cÃ³ dáº¡ng v*
                    int startIndex = 0;
                    while ((startIndex = json.IndexOf("\"tag_name\":", startIndex)) != -1)
                    {
                        startIndex += "\"tag_name\":".Length;
                        int quoteStart = json.IndexOf('"', startIndex);
                        if (quoteStart == -1) break;
                        
                        quoteStart++;
                        int quoteEnd = json.IndexOf('"', quoteStart);
                        if (quoteEnd == -1) break;
                        
                        string tagName = json.Substring(quoteStart, quoteEnd - quoteStart);
                        
                        // Chá»‰ láº¥y cÃ¡c tag cÃ³ dáº¡ng vX.Y.Z
                        if (tagName.StartsWith("v") && tagName.Length > 1)
                        {
                            // Parse version number Ä‘á»ƒ so sÃ¡nh
                            string versionNum = tagName.TrimStart('v');
                            int buildNumber = ParseVersionToNumber(versionNum);
                            versions.Add((tagName, buildNumber));
                        }
                        
                        startIndex = quoteEnd + 1;
                    }

                    // TÃ¬m version cÃ³ sá»‘ build lá»›n nháº¥t
                    if (versions.Count > 0)
                    {
                        var latest = versions.OrderByDescending(v => v.BuildNumber).First();
                        return latest.Version;
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lá»—i khi tÃ¬m phiÃªn báº£n VidCoder: {ex.Message}", "Orange");
            }

            return null;
        }

        /// <summary>
        /// Chuyá»ƒn version string (X.Y.Z) thÃ nh sá»‘ Ä‘á»ƒ so sÃ¡nh
        /// </summary>
        private int ParseVersionToNumber(string version)
        {
            try
            {
                var parts = version.Split('.');
                if (parts.Length >= 3)
                {
                    int major = int.TryParse(parts[0], out var m) ? m : 0;
                    int minor = int.TryParse(parts[1], out var n) ? n : 0;
                    int build = int.TryParse(parts[2], out var b) ? b : 0;
                    
                    // CÃ´ng thá»©c: major * 1000000 + minor * 1000 + build
                    return major * 1000000 + minor * 1000 + build;
                }
            }
            catch { }

            return 0;
        }

        // ===================================================================
        // TabSubtitle â€” Boilsoft Video Splitter
        // ===================================================================
        private void ChkBoilsoftVideoSplitter_Click(object sender, RoutedEventArgs e)
        {
            if (ChkBoilsoftVideoSplitter.IsChecked == true)
            {
                UpdateStatus("ÄÃ£ chá»n: Boilsoft Video Splitter", "Green");
            }
            else
            {
                UpdateStatus("ÄÃ£ há»§y chá»n: Boilsoft Video Splitter", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private async Task InstallBoilsoftVideoSplitterAsync()
        {
            try
            {
                UpdateStatus("Äang táº£i Boilsoft Video Splitter...", "Cyan");
                string boilsoftPath = Path.Combine(GetGMTPCFolder(), "Boilsoft.VideoSplitter.exe");
                await DownloadWithProgressAsync(BOILSOFT_VIDEO_SPLITTER_DOWNLOAD_URL, boilsoftPath, "Boilsoft Video Splitter");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                // Hiá»ƒn thá»‹ popup Ä‘á»ƒ há»i ngÆ°á»i dÃ¹ng chá»n cÃ i Ä‘áº·t
                MessageBoxResult result = MessageBox.Show("Yes = CÃ i Ä‘áº·t tá»± Ä‘á»™ng (silent)\nNo = CÃ i Ä‘áº·t thá»§ cÃ´ng (GUI)", "CÃ i Ä‘áº·t Boilsoft Video Splitter", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Cancel)
                {
                    UpdateStatus("ÄÃ£ há»§y cÃ i Ä‘áº·t Boilsoft Video Splitter", "Yellow");
                    if (File.Exists(boilsoftPath))
                    {
                        File.Delete(boilsoftPath);
                    }
                    return;
                }

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = boilsoftPath,
                    UseShellExecute = true
                };

                if (result == MessageBoxResult.Yes)
                {
                    // CÃ i Ä‘áº·t tá»± Ä‘á»™ng
                    startInfo.Arguments = BOILSOFT_VIDEO_SPLITTER_INSTALL_ARGUMENTS;
                    UpdateStatus("Äang cÃ i Ä‘áº·t Boilsoft Video Splitter (silent)...", "Yellow");
                }
                else
                {
                    // CÃ i Ä‘áº·t thá»§ cÃ´ng
                    UpdateStatus("Äang má»Ÿ Boilsoft Video Splitter installer (thá»§ cÃ´ng)...", "Yellow");
                }

                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("CÃ i Ä‘áº·t Boilsoft Video Splitter hoÃ n táº¥t!", "Green");
                }

                if (File.Exists(boilsoftPath))
                {
                    File.Delete(boilsoftPath);
                    UpdateStatus("ÄÃ£ xÃ³a file Boilsoft.VideoSplitter.exe", "Cyan");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lá»—i khi cÃ i Ä‘áº·t Boilsoft Video Splitter: {ex.Message}", "Red");
            }
        }

        // ===================================================================
        // TabSubtitle â€” Vibe
        // ===================================================================
        private void ChkVibe_Click(object sender, RoutedEventArgs e)
        {
            if (ChkVibe.IsChecked == true)
            {
                UpdateStatus("ÄÃ£ chá»n: Vibe", "Green");
            }
            else
            {
                UpdateStatus("ÄÃ£ há»§y chá»n: Vibe", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private async Task InstallVibeAsync()
        {
            try
            {
                UpdateStatus("Äang táº£i Vibe...", "Cyan");
                string vibePath = Path.Combine(GetGMTPCFolder(), "Vibe.exe");
                await DownloadWithProgressAsync(VIBE_DOWNLOAD_URL, vibePath, "Vibe");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                // Hiá»ƒn thá»‹ popup Ä‘á»ƒ há»i ngÆ°á»i dÃ¹ng chá»n cÃ i Ä‘áº·t
                MessageBoxResult result = MessageBox.Show("Yes = CÃ i Ä‘áº·t tá»± Ä‘á»™ng (silent)\nNo = CÃ i Ä‘áº·t thá»§ cÃ´ng (GUI)", "CÃ i Ä‘áº·t Vibe", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Cancel)
                {
                    UpdateStatus("ÄÃ£ há»§y cÃ i Ä‘áº·t Vibe", "Yellow");
                    if (File.Exists(vibePath))
                    {
                        File.Delete(vibePath);
                    }
                    return;
                }

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = vibePath,
                    UseShellExecute = true
                };

                if (result == MessageBoxResult.Yes)
                {
                    // CÃ i Ä‘áº·t tá»± Ä‘á»™ng
                    startInfo.Arguments = VIBE_INSTALL_ARGUMENTS;
                    UpdateStatus("Äang cÃ i Ä‘áº·t Vibe (silent)...", "Yellow");
                }
                else
                {
                    // CÃ i Ä‘áº·t thá»§ cÃ´ng
                    UpdateStatus("Äang má»Ÿ Vibe installer (thá»§ cÃ´ng)...", "Yellow");
                }

                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("CÃ i Ä‘áº·t Vibe hoÃ n táº¥t!", "Green");
                }

                if (File.Exists(vibePath))
                {
                    File.Delete(vibePath);
                    UpdateStatus("ÄÃ£ xÃ³a file Vibe.exe", "Cyan");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lá»—i khi cÃ i Ä‘áº·t Vibe: {ex.Message}", "Red");
            }
        }

        // ===================================================================
        // TabSubtitle â€” MKVToolNix MKVCleaver
        // ===================================================================
        private void ChkMKVToolNix_Click(object sender, RoutedEventArgs e)
        {
            if (ChkMKVToolNix.IsChecked == true)
            {
                UpdateStatus("ÄÃ£ chá»n: MKVToolNix MKVCleaver", "Green");
            }
            else
            {
                UpdateStatus("ÄÃ£ há»§y chá»n: MKVToolNix MKVCleaver", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private async Task InstallMKVToolNixAsync()
        {
            try
            {
                UpdateStatus("Äang táº£i MKVToolNix MKVCleaver...", "Cyan");
                string mkvtoolnixPath = Path.Combine(GetGMTPCFolder(), "MKVToolNix.MKVCleaver.exe");
                await DownloadWithProgressAsync(MKVTOOLNIX_DOWNLOAD_URL, mkvtoolnixPath, "MKVToolNix MKVCleaver");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                // Hiá»ƒn thá»‹ popup Ä‘á»ƒ há»i ngÆ°á»i dÃ¹ng chá»n cÃ i Ä‘áº·t
                MessageBoxResult result = MessageBox.Show("Yes = CÃ i Ä‘áº·t tá»± Ä‘á»™ng (silent)\nNo = CÃ i Ä‘áº·t thá»§ cÃ´ng (GUI)", "CÃ i Ä‘áº·t MKVToolNix MKVCleaver", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Cancel)
                {
                    UpdateStatus("ÄÃ£ há»§y cÃ i Ä‘áº·t MKVToolNix MKVCleaver", "Yellow");
                    if (File.Exists(mkvtoolnixPath))
                    {
                        File.Delete(mkvtoolnixPath);
                    }
                    return;
                }

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = mkvtoolnixPath,
                    UseShellExecute = true
                };

                if (result == MessageBoxResult.Yes)
                {
                    // CÃ i Ä‘áº·t tá»± Ä‘á»™ng
                    startInfo.Arguments = MKVTOOLNIX_INSTALL_ARGUMENTS;
                    UpdateStatus("Äang cÃ i Ä‘áº·t MKVToolNix MKVCleaver (silent)...", "Yellow");
                }
                else
                {
                    // CÃ i Ä‘áº·t thá»§ cÃ´ng
                    UpdateStatus("Äang má»Ÿ MKVToolNix MKVCleaver installer (thá»§ cÃ´ng)...", "Yellow");
                }

                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("CÃ i Ä‘áº·t MKVToolNix MKVCleaver hoÃ n táº¥t!", "Green");
                }

                if (File.Exists(mkvtoolnixPath))
                {
                    File.Delete(mkvtoolnixPath);
                    UpdateStatus("ÄÃ£ xÃ³a file MKVToolNix.MKVCleaver.exe", "Cyan");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lá»—i khi cÃ i Ä‘áº·t MKVToolNix MKVCleaver: {ex.Message}", "Red");
            }
        }

        // ===================================================================
        // TabSubtitle â€” Subtitle Draft GMTPC
        // ===================================================================
        private void ChkSubtitleDraftGMTPC_Click(object sender, RoutedEventArgs e)
        {
            if (ChkSubtitleDraftGMTPC.IsChecked == true)
            {
                UpdateStatus("ÄÃ£ chá»n: Subtitle Draft GMTPC", "Green");
            }
            else
            {
                UpdateStatus("ÄÃ£ há»§y chá»n: Subtitle Draft GMTPC", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private async Task InstallSubtitleDraftGMTPCAsync()
        {
            try
            {
                // BÆ°á»›c 1: Táº£i file vá» á»• C:\
                string subtitleDraftFolder = @"C:\";
                string subtitleDraftExe = Path.Combine(subtitleDraftFolder, "Subtitle draft GMTPC.exe");

                UpdateStatus("Äang táº£i Subtitle Draft GMTPC...", "Cyan");
                await DownloadWithProgressAsync(SUBTITLE_DRAFT_GMTPC_DOWNLOAD_URL, subtitleDraftExe, "Subtitle Draft GMTPC");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus("ÄÃ£ táº£i xong Subtitle Draft GMTPC", "Green");

                // BÆ°á»›c 2: Táº¡o shortcut trÃªn Desktop
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string shortcutPath = Path.Combine(desktopPath, "Subtitle Draft GMTPC.lnk");

                // XÃ³a shortcut cÅ© náº¿u tá»“n táº¡i
                if (File.Exists(shortcutPath))
                {
                    File.Delete(shortcutPath);
                }

                // Táº¡o shortcut má»›i sá»­ dá»¥ng WshShell
                try
                {
                    Type shellType = Type.GetTypeFromProgID("WScript.Shell");
                    if (shellType != null)
                    {
                        object shell = Activator.CreateInstance(shellType);
                        object shortcut = shellType.InvokeMember("CreateShortcut", System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { shortcutPath });

                        // Set cÃ¡c thuá»™c tÃ­nh shortcut
                        shellType.InvokeMember("TargetPath", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { subtitleDraftExe });
                        shellType.InvokeMember("WorkingDirectory", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { subtitleDraftFolder });
                        shellType.InvokeMember("Description", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { "Subtitle Draft GMTPC" });
                        shellType.InvokeMember("Save", System.Reflection.BindingFlags.InvokeMethod, null, shortcut, null);

                        UpdateStatus("ÄÃ£ táº¡o shortcut Subtitle Draft GMTPC trÃªn Desktop", "Green");
                    }
                }
                catch (Exception ex)
                {
                    UpdateStatus($"KhÃ´ng thá»ƒ táº¡o shortcut: {ex.Message}", "Orange");
                }

                // BÆ°á»›c 3: Má»Ÿ file
                UpdateStatus("Äang má»Ÿ Subtitle Draft GMTPC...", "Cyan");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = subtitleDraftExe,
                    UseShellExecute = true,
                    WorkingDirectory = subtitleDraftFolder
                };
                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    UpdateStatus("Subtitle Draft GMTPC Ä‘Ã£ Ä‘Æ°á»£c má»Ÿ!", "Green");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lá»—i khi cÃ i Ä‘áº·t Subtitle Draft GMTPC: {ex.Message}", "Red");
            }
        }

        // ===================================================================
        // TabSubtitle â€” Download sample video
        // ===================================================================
        private void ChkDownloadSampleVideo_Click(object sender, RoutedEventArgs e)
        {
            if (ChkDownloadSampleVideo.IsChecked == true)
            {
                UpdateStatus("ÄÃ£ chá»n: Download sample video", "Green");
            }
            else
            {
                UpdateStatus("ÄÃ£ há»§y chá»n: Download sample video", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private async Task InstallSampleVideoAsync()
        {
            try
            {
                string targetFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                string fileName = Path.GetFileName(new Uri(SAMPLE_VIDEO_DOWNLOAD_URL).LocalPath);
                string sampleVideoPath = Path.Combine(targetFolder, fileName);

                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                UpdateStatus($"Äang táº£i sample video vá» {targetFolder}...", "Cyan");
                await DownloadWithProgressAsync(SAMPLE_VIDEO_DOWNLOAD_URL, sampleVideoPath, "Sample video");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus($"ÄÃ£ táº£i xong sample video vÃ o {targetFolder}", "Green");

                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"/select,\"{sampleVideoPath}\"",
                    UseShellExecute = true
                });

                UpdateStatus("ÄÃ£ má»Ÿ thÆ° má»¥c chá»©a sample video", "Green");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lá»—i khi táº£i sample video: {ex.Message}", "Red");
            }
        }
    }
}

