// =======================================================================
// MainWindow.RestoredMethods.cs
// Chá»©c nÄƒng: HÃ m bá»• trá»£, phÆ°Æ¡ng thá»©c dÃ¹ng chung cho toÃ n á»©ng dá»¥ng
//            (GetBrush, GetColor, GetGMTPCFolder, Defender exclusion...)
// Cáº­p nháº­t gáº§n Ä‘Ã¢y:
//   - 2026-03-07: Cáº­p nháº­t cÃ¡c Install methods sá»­ dá»¥ng constants tá»«
//                 SystemArguments.cs theo AI_WORKFLOW.md
//   - 2026-03-07: Fix Java installer exit code -1 handling, use JAVA_DOWNLOAD_URL
//   - 2026-03-07: Removed Zalo installation support
//   - 2026-03-10: Fix IDM download - use DownloadWithProgressAsync (multi-segment)
//                 instead of DownloadSingleConnectionAsync
//   - 2026-03-17: Updated GetGMTPCFolder() to use _selectedTempDrivePath
// =======================================================================
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AICodeAgentAIOGMTPC
{
    public partial class MainWindow
    {
        // ===================== Color / Brush Helpers =====================
        private SolidColorBrush GetBrush(string colorName)
        {
            Color color = GetColor(colorName);
            return new SolidColorBrush(color);
        }

        private Color GetColor(string colorName)
        {
            switch (colorName.ToLower())
            {
                case "red":     return Colors.Red;
                case "green":   return Colors.LimeGreen;
                case "yellow":  return Colors.Yellow;
                case "cyan":    return Colors.Cyan;
                case "orange":  return Colors.Orange;
                case "gray":    return Colors.Gray;
                default:        return Colors.Yellow;
            }
        }

        // ===================== GMTPC Folder Helper =====================
        /// <summary>
        /// Get the GMTPC folder path - returns the selected temp folder path
        /// This ensures all downloads use the user-selected temp folder
        /// </summary>
        private string GetGMTPCFolder()
        {
            // Use the selected temp folder path if available
            if (!string.IsNullOrEmpty(_selectedTempDrivePath))
            {
                if (!Directory.Exists(_selectedTempDrivePath))
                    Directory.CreateDirectory(_selectedTempDrivePath);

                return _selectedTempDrivePath;
            }

            // Default to LocalAppData if nothing selected
            string tempPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "GMTPC", "AI Code Agent AIO-GMTPC");

            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            return tempPath;
        }

        // ===================== Windows Defender Exclusion =====================
        private void AddDefenderExclusion()
        {
            try
            {
                string exclusionPath = GetGMTPCFolder();
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"Add-MpPreference -ExclusionPath '{exclusionPath}' -Force\"",
                    UseShellExecute = true,
                    Verb = "runas",
                    CreateNoWindow = true
                };
                Process process = Process.Start(startInfo);
                if (process != null) process.WaitForExit();
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lá»—i khi thÃªm exclusion cho Windows Defender: {ex.Message}", "Red");
            }
        }

        private void RemoveDefenderExclusion()
        {
            try
            {
                string exclusionPath = GetGMTPCFolder();
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"Remove-MpPreference -ExclusionPath '{exclusionPath}' -Force\"",
                    UseShellExecute = true,
                    Verb = "runas",
                    CreateNoWindow = true
                };
                Process process = Process.Start(startInfo);
                if (process != null) process.WaitForExit();

                if (Directory.Exists(exclusionPath))
                    Directory.Delete(exclusionPath, true);
            }
            catch { }
        }

        // ===================== Automated Process =====================
        private async Task StartAutomatedProcessAsync()
        {
            await Task.Delay(500, _cancellationTokenSource?.Token ?? CancellationToken.None);
            await RunAutomatedProcessAsync();
        }

        private void ScrollToBottom() { /* KhÃ´ng dÃ¹ng */ }
        private void ActivateWindows()
        {
            UpdateStatus("Äang kÃ­ch hoáº¡t Windows...", "Cyan");
            string activateWindowsCmdPath = Path.Combine(GetGMTPCFolder(), "ACTIVATE.WINDOWS.cmd");
            try
            {
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                     client.DownloadFile("https://github.com/ghostminhtoan/MMT/releases/download/activate/ACTIVATE.WINDOWS.cmd", activateWindowsCmdPath);
                }
                ProcessStartInfo startInfo = new ProcessStartInfo { FileName = activateWindowsCmdPath, UseShellExecute = true, Verb = "runas" };
                Process process = Process.Start(startInfo);
                if (process != null)
                {
                    process.WaitForExit();
                }
                UpdateStatus("ÄÃ£ má»Ÿ cá»­a sá»• kÃ­ch hoáº¡t Windows", "Green");
                
                // Wait 2 seconds before showing message
                System.Threading.Thread.Sleep(2000);
                
                // Show message after activation completes
                UpdateStatus("Press \"0\" to continue", "Yellow");
                MessageBox.Show("Press \"0\" to continue", "ThÃ´ng bÃ¡o", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) { UpdateStatus($"Lá»—i: {ex.Message}", "Red"); }
        }

        private void PauseWindowsUpdate()
        {
            UpdateStatus("Äang truy cáº­p tÃ­nh nÄƒng Pause Windows Update...", "Cyan");
            string pauseUpdateScriptPath = Path.Combine(GetGMTPCFolder(), "pause.update.win.11.ps1");
            try
            {
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                     client.DownloadFile("https://github.com/ghostminhtoan/MMT/releases/download/test/pause.update.win.11.ps1", pauseUpdateScriptPath);
                }
                ProcessStartInfo startInfo = new ProcessStartInfo 
                { 
                    FileName = "powershell.exe",
                    Arguments = $"-ExecutionPolicy Bypass -File \"{pauseUpdateScriptPath}\"",
                    UseShellExecute = true, 
                    Verb = "runas" 
                };
                Process process = Process.Start(startInfo);
                if (process != null)
                {
                    process.WaitForExit();
                }
                UpdateStatus("ÄÃ£ má»Ÿ cÃ´ng cá»¥ Pause Windows Update", "Green");
            }
            catch (Exception ex) { UpdateStatus($"Lá»—i: {ex.Message}", "Red"); }
        }

        private async Task InstallAndActivateWinRARAsync()
        {
            await InstallWinRARAsync();
        }

        private async Task InstallAndActivateBIDAsync()
        {
            await InstallBIDAsync();
        }

        private async Task InstallWinRARAsync()
        {
            string winrarPath = Path.Combine(GetGMTPCFolder(), "WinRAR.exe");
            try
            {
                // ===== Step 1: Download and Install WinRAR =====
                UpdateStatus("Äang táº£i WinRAR...", "Cyan");
                await DownloadWithRetryAsync(WINRAR_DOWNLOAD_URL, winrarPath, "WinRAR");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Äang cháº¡y WinRAR installer ( " + WINRAR_INSTALL_ARGUMENTS + " )...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo { FileName = winrarPath, Arguments = WINRAR_INSTALL_ARGUMENTS, UseShellExecute = true };
                Process process = Process.Start(startInfo);
                if (process != null) { await Task.Run(() => process.WaitForExit()); UpdateStatus("CÃ i Ä‘áº·t WinRAR hoÃ n táº¥t.", "Green"); }
                if (File.Exists(winrarPath)) File.Delete(winrarPath);
            }
            catch (Exception ex) { UpdateStatus($"Lá»—i: {ex.Message}", "Red"); }
        }

        private async Task InstallBIDAsync()
        {
            string bidPath = Path.Combine(GetGMTPCFolder(), "bid_setup_x64.exe");
            string bidActivatePath = Path.Combine(GetGMTPCFolder(), "Bulk.image.downloader.patch.exe");
            string bidExePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Bulk Image Downloader", "BID.exe");
            try
            {
                // ===== Step 1: Download and Install BID =====
                UpdateStatus("Äang táº£i Bulk Image Downloader...", "Cyan");
                string downloadUrl = await GetBIDDownloadLinkAsync();

                await DownloadWithProgressAsync(downloadUrl, bidPath, "Bulk Image Downloader");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Äang cháº¡y BID installer...", "Yellow");
                ProcessStartInfo installStartInfo = new ProcessStartInfo { FileName = bidPath, Arguments = BID_INSTALL_ARGUMENTS, UseShellExecute = true };
                Process installProcess = Process.Start(installStartInfo);
                if (installProcess != null)
                {
                    await Task.Run(() => installProcess.WaitForExit());
                    UpdateStatus("CÃ i Ä‘áº·t BID hoÃ n táº¥t.", "Green");
                }
                if (File.Exists(bidPath)) File.Delete(bidPath);

                // ===== Step 2: Show message BEFORE activation =====
                UpdateStatus("Click vÃ o nÃºt Install rá»“i táº¯t", "Yellow");
                MessageBox.Show("Click vÃ o nÃºt Install rá»“i táº¯t", "ThÃ´ng bÃ¡o", MessageBoxButton.OK, MessageBoxImage.Information);

                // ===== Step 3: Download and Run Activation =====
                await Task.Delay(1000);
                UpdateStatus("Äang táº£i BID patch...", "Cyan");
                await DownloadWithProgressAsync("https://github.com/ghostminhtoan/MMT/releases/download/activate/Bulk.image.downloader.patch.exe", bidActivatePath, "BID Patch");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Äang cháº¡y BID patch...", "Yellow");
                ProcessStartInfo activateStartInfo = new ProcessStartInfo { FileName = bidActivatePath, UseShellExecute = true };
                Process activateProcess = Process.Start(activateStartInfo);
                if (activateProcess != null)
                {
                    await Task.Run(() => activateProcess.WaitForExit());
                }
                if (File.Exists(bidActivatePath)) File.Delete(bidActivatePath);

                // ===== Step 4: Wait for BID to close =====
                UpdateStatus("Äang chá» BID táº¯t...", "Cyan");
                while (true)
                {
                    Process[] bidProcesses = Process.GetProcessesByName("BID");
                    if (bidProcesses.Length == 0) break;
                    await Task.Delay(500);
                }
                await Task.Delay(1000);

                // ===== Step 5: Open BID.exe =====
                if (File.Exists(bidExePath))
                {
                    UpdateStatus("Äang má»Ÿ BID...", "Green");
                    Process.Start(bidExePath);
                }
                else
                {
                    UpdateStatus("KhÃ´ng tÃ¬m tháº¥y BID.exe", "Yellow");
                }
            }
            catch (Exception ex) { UpdateStatus($"Lá»—i: {ex.Message}", "Red"); }
        }

        /// <summary>
        /// Láº¥y link táº£i BID má»›i nháº¥t tá»« bulkimagedownloader.com
        /// TÃ¬m link cÃ³ dáº¡ng: bid_*_*_setup_x64.exe
        /// </summary>
        private async Task<string> GetBIDDownloadLinkAsync()
        {
            try
            {
                UpdateStatus("Äang láº¥y link táº£i BID má»›i nháº¥t...", "Cyan");
                
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                    
                    // Táº£i trang download
                    string html = await client.GetStringAsync("https://bulkimagedownloader.com/download");
                    
                    // TÃ¬m link cÃ³ dáº¡ng bid_*_*_setup_x64.exe
                    string pattern = @"/files/bid_\d+_\d+_setup_x64\.exe";
                    System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(html, pattern);
                    
                    if (match.Success)
                    {
                        string downloadPath = match.Value;
                        string fullUrl = "https://bulkimagedownloader.com" + downloadPath;
                        UpdateStatus($"ÄÃ£ tÃ¬m tháº¥y link BID má»›i: {fullUrl}", "Green");
                        return fullUrl;
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"KhÃ´ng thá»ƒ láº¥y link tá»± Ä‘á»™ng, dÃ¹ng link dá»± phÃ²ng: {ex.Message}", "Yellow");
            }
            
            // Fallback link cÅ© náº¿u khÃ´ng tÃ¬m tháº¥y
            return "https://bulkimagedownloader.com/files/bid_6_62_setup_x64.exe";
        }

        private async Task InstallIDMAsync()
        {
            string idmPath = Path.Combine(GetGMTPCFolder(), "idman625build3.exe");
            string activatePath = Path.Combine(GetGMTPCFolder(), "IDM_6.4x_rabbit.exe");
            string idmExePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Internet Download Manager", "IDMan.exe");
            string idmBackupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Internet Download Manager", "IDMan.exe.bak");
            string tempCheckPath = Path.Combine(Path.GetTempPath(), "IDM_Setup_Temp", "IDM0.tmp");
            try
            {
                // ===== Step 1: Kill IDM =====
                UpdateStatus("Äang Ä‘Ã³ng IDM (náº¿u Ä‘ang cháº¡y)...", "Cyan");
                KillProcessByName("idman");
                await Task.Delay(500);

                // ===== Step 2: Delete backup file if exists =====
                if (File.Exists(idmBackupPath))
                {
                    UpdateStatus("Äang dá»n dáº¹p tá»‡p sao lÆ°u cÅ©...", "Cyan");
                    File.Delete(idmBackupPath);
                }

                // ===== Step 3: Download and install IDM =====
                UpdateStatus("Äang táº£i Internet Download Manager...", "Cyan");
                await DownloadWithProgressAsync(IDM_DOWNLOAD_URL, idmPath, "Internet Download Manager");

                // Reset progress bar
                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                // Run installer with arguments
                UpdateStatus("Äang cháº¡y IDM installer ( " + IDM_INSTALL_ARGUMENTS + " )...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = idmPath,
                    Arguments = IDM_INSTALL_ARGUMENTS,
                    UseShellExecute = true
                };
                Process process = Process.Start(startInfo);
                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                }

                // ===== Step 4: Download and run activate tool =====
                UpdateStatus("Äang táº£i cÃ´ng cá»¥ kÃ­ch hoáº¡t...", "Cyan");
                await DownloadWithProgressAsync(IDM_ACTIVATE_URL, activatePath, "IDM Activate");

                // Reset progress bar
                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                // ===== Step 4.1: Wait for IDM0.tmp to disappear before running crack =====
                UpdateStatus("Äang chá» IDM hoÃ n táº¥t cÃ i Ä‘áº·t (IDM0.tmp biáº¿n máº¥t)...", "Yellow");
                while (File.Exists(tempCheckPath))
                {
                    await Task.Delay(500);
                }
                await Task.Delay(1000); // ThÃªm delay nhá» Ä‘á»ƒ Ä‘áº£m báº£o á»•n Ä‘á»‹nh

                // Run activate tool
                UpdateStatus("Click IDM to activate, cÃ³ thá»ƒ bá» qua update.", "Yellow");
                ProcessStartInfo activateStartInfo = new ProcessStartInfo
                {
                    FileName = activatePath,
                    UseShellExecute = true
                };
                Process activateProcess = Process.Start(activateStartInfo);
                if (activateProcess != null)
                {
                    await Task.Run(() => activateProcess.WaitForExit());

                    if (File.Exists(activatePath))
                    {
                        File.Delete(activatePath);
                        UpdateStatus("ÄÃ£ xÃ³a cÃ´ng cá»¥ kÃ­ch hoáº¡t", "Cyan");
                    }
                }

                // ===== Step 5: Open browser tabs for IDM integration =====
                UpdateStatus("Äang má»Ÿ trang tÃ­ch há»£p IDM cho trÃ¬nh duyá»‡t...", "Cyan");
                Process.Start("https://microsoftedge.microsoft.com/addons/detail/idm-integration-module/llbjbkhnmlidjebalopleeepgdfgcpec");
                Process.Start("https://chromewebstore.google.com/detail/idm-integration-module/ngpampappnmepgilojfohadhhmbhlaek");
                await Task.Delay(1000);

                // ===== Step 6: Check temp file and delete installer =====
                if (!File.Exists(tempCheckPath))
                {
                    if (File.Exists(idmPath))
                    {
                        File.Delete(idmPath);
                        UpdateStatus("ÄÃ£ xÃ³a file cÃ i Ä‘áº·t IDM", "Cyan");
                    }
                }

                // ===== Step 7: Post-Install Loop (5 times: Open IDM -> Kill IDM) =====
                for (int i = 0; i < 5; i++)
                {
                    if (File.Exists(idmExePath))
                    {
                        UpdateStatus($"Láº§n {i + 1}/5: Äang má»Ÿ IDM...", "Cyan");
                        Process.Start(idmExePath);
                    }

                    await Task.Delay(1500);

                    UpdateStatus($"Láº§n {i + 1}/5: Äang Ä‘Ã³ng IDM...", "Cyan");
                    KillProcessByName("idman");

                    if (i < 4)
                    {
                        await Task.Delay(1500);
                    }
                }

                UpdateStatus("ÄÃ£ cÃ i xong IDM!", "Green");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lá»—i khi cÃ i IDM: {ex.Message}", "Red");
            }
        }

        /// <summary>
        /// Kills all processes with the specified name
        /// </summary>
        private void KillProcessByName(string processName)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(processName);
                foreach (Process process in processes)
                {
                    try
                    {
                        if (!process.HasExited)
                        {
                            process.Kill();
                            process.WaitForExit();
                        }
                    }
                    catch
                    {
                        // Ignore if process cannot be killed
                    }
                    finally
                    {
                        process.Dispose();
                    }
                }
            }
            catch
            {
                // Ignore if cannot get processes by name
            }
        }

        private async Task InstallVcredistAsync()
        {
            UpdateStatus("Äang táº£i Vcredist...", "Cyan");
            string vcredistPath = Path.Combine(GetGMTPCFolder(), "vcredist.all.in.one.by.MMT.Windows.Tech.exe");
            try
            {
                await DownloadWithProgressAsync(VCREDIST_DOWNLOAD_URL, vcredistPath, "Vcredist");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });
                UpdateStatus("Äang cÃ i Ä‘áº·t Vcredist ( " + VCREDIST_INSTALL_ARGUMENTS + " )...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo { FileName = vcredistPath, Arguments = VCREDIST_INSTALL_ARGUMENTS, UseShellExecute = true };
                Process process = Process.Start(startInfo);
                if (process != null) { await Task.Run(() => process.WaitForExit()); UpdateStatus("CÃ i Ä‘áº·t Vcredist hoÃ n táº¥t.", "Green"); }
                if (File.Exists(vcredistPath)) File.Delete(vcredistPath);
            } catch (Exception ex) { UpdateStatus($"Lá»—i: {ex.Message}", "Red"); }
        }

        private async Task InstallDirectXAsync()
        {
            UpdateStatus("Äang táº£i DirectX...", "Cyan");
            string directxPath = Path.Combine(GetGMTPCFolder(), "DirectX.exe");
            try
            {
                await DownloadWithProgressAsync(DIRECTX_DOWNLOAD_URL, directxPath, "DirectX");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });
                UpdateStatus("Äang cÃ i Ä‘áº·t DirectX ( " + DIRECTX_INSTALL_ARGUMENTS + " )...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo { FileName = directxPath, Arguments = DIRECTX_INSTALL_ARGUMENTS, UseShellExecute = true };
                Process process = Process.Start(startInfo);
                if (process != null) { await Task.Run(() => process.WaitForExit()); UpdateStatus("CÃ i Ä‘áº·t DirectX hoÃ n táº¥t.", "Green"); }
                if (File.Exists(directxPath)) File.Delete(directxPath);
            } catch (Exception ex) { UpdateStatus($"Lá»—i: {ex.Message}", "Red"); }
        }

        private Task InstallJavaAsync()
        {
            BtnJava_Click(null, null);
            return Task.CompletedTask;
        }

        private Task InstallOpenALAsync()
        {
            BtnOpenAL_Click(null, null);
            return Task.CompletedTask;
        }

        private async Task WaitForIDM2TmpFileToDisappear()
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), "IDM");
            for (int i = 0; i < 50; i++) // 5 seconds timeout
            {
                try
                {
                    if (Directory.Exists(tempFolder) && Directory.GetFiles(tempFolder, "IDM*.tmp").Length > 0)
                    {
                        await Task.Delay(100);
                    }
                    else break;
                }
                catch { break; }
            }
        }

        private void BtnVcredist_Click(object sender, RoutedEventArgs e)
        {
            ChkVcredist.IsChecked = true;
            _ = InstallVcredistAsync();
        }

        private void BtnInstallIDM_Click(object sender, RoutedEventArgs e)
        {
            ChkInstallIDM.IsChecked = true;
            _ = InstallIDMAsync();
        }

        private async Task RunAutomatedProcessAsync()
        {
            await InstallIDMAsync();
        }

        private void BtnActivateWindows_Click(object sender, RoutedEventArgs e)
        {
            ChkActivateWindows.IsChecked = true;
            _ = Task.Run(() => ActivateWindows());
        }

        private void BtnPauseWindowsUpdate_Click(object sender, RoutedEventArgs e)
        {
            ChkPauseWindowsUpdate.IsChecked = true;
            _ = Task.Run(() => PauseWindowsUpdate());
        }

        private void BtnInstallWinRAR_Click(object sender, RoutedEventArgs e)
        {
            ChkInstallWinRAR.IsChecked = true;
            _ = InstallWinRARAsync();
        }

        private void BtnInstallBID_Click(object sender, RoutedEventArgs e)
        {
            ChkInstallBID.IsChecked = true;
            _ = InstallBIDAsync();
        }

        private void BtnFixIDMExtension_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Äang má»Ÿ cÃ¡c liÃªn káº¿t extension...", "Cyan");
            Process.Start("https://microsoftedge.microsoft.com/addons/detail/idm-integration-module/llbjbkhnmlidjebalopleeepgdfgcpec");
            Process.Start("https://chromewebstore.google.com/detail/idm-integration-module/ngpampappnmepgilojfohadhhmbhlaek");
        }

        private async void BtnCrackIDM_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Äang táº£i vÃ  cháº¡y IDM crack...", "Cyan");
            string idmCrackPath = Path.Combine(GetGMTPCFolder(), "IDM_6.4x_Crack.exe");
            try
            {
                await DownloadWithProgressAsync("https://github.com/ghostminhtoan/MMT/releases/download/activate/IDM_6.4x_Crack.exe", idmCrackPath, "IDM Crack");
                Process.Start(idmCrackPath);
            }
            catch (Exception ex) { UpdateStatus($"Lá»—i: {ex.Message}", "Red"); }
        }

        private async void BtnRunBIDActivation_Click(object sender, RoutedEventArgs e)
        {
            string bidActivatePath = Path.Combine(GetGMTPCFolder(), "Bulk.image.downloader.patch.exe");
            string bidExePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Bulk Image Downloader", "BID.exe");
            try
            {
                // ===== Step 1: Show message =====
                UpdateStatus("Click vÃ o nÃºt Install rá»“i táº¯t", "Yellow");
                MessageBox.Show("Click vÃ o nÃºt Install rá»“i táº¯t", "ThÃ´ng bÃ¡o", MessageBoxButton.OK, MessageBoxImage.Information);

                // ===== Step 2: Download and Run Activation =====
                await Task.Delay(1000);
                UpdateStatus("Äang táº£i BID patch...", "Cyan");
                await DownloadWithProgressAsync("https://github.com/ghostminhtoan/MMT/releases/download/activate/Bulk.image.downloader.patch.exe", bidActivatePath, "BID Patch");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Äang cháº¡y BID patch...", "Yellow");
                ProcessStartInfo activateStartInfo = new ProcessStartInfo { FileName = bidActivatePath, UseShellExecute = true };
                Process activateProcess = Process.Start(activateStartInfo);
                if (activateProcess != null)
                {
                    await Task.Run(() => activateProcess.WaitForExit());
                }
                if (File.Exists(bidActivatePath)) File.Delete(bidActivatePath);

                // ===== Step 3: Wait for BID to close =====
                UpdateStatus("Äang chá» BID táº¯t...", "Cyan");
                while (true)
                {
                    Process[] bidProcesses = Process.GetProcessesByName("BID");
                    if (bidProcesses.Length == 0) break;
                    await Task.Delay(500);
                }
                await Task.Delay(1000);

                // ===== Step 4: Open BID.exe =====
                if (File.Exists(bidExePath))
                {
                    UpdateStatus("Äang má»Ÿ BID...", "Green");
                    Process.Start(bidExePath);
                }
                else
                {
                    UpdateStatus("KhÃ´ng tÃ¬m tháº¥y BID.exe", "Yellow");
                }
            }
            catch (Exception ex) { UpdateStatus($"Lá»—i: {ex.Message}", "Red"); }
        }

        // ===================== TabPopular â€” Button Click Handlers =====================
        private async void BtnDirectX_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Äang táº£i DirectX...", "Cyan");
            string directXPath = Path.Combine(GetGMTPCFolder(), "directx_installer.exe");
            try
            {
                await DownloadWithProgressAsync("https://download.microsoft.com/download/1/7/1/1718CCC4-6315-4D8E-9543-8E28A4E99C9C/dxwebsetup.exe", directXPath, "DirectX Installer");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });
                UpdateStatus("Äang cháº¡y DirectX installer vá»›i lá»‡nh /q...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo { FileName = directXPath, Arguments = "/q", UseShellExecute = true };
                Process process = Process.Start(startInfo);
                if (process != null) { await Task.Run(() => process.WaitForExit()); UpdateStatus(process.ExitCode == 0 ? "CÃ i Ä‘áº·t DirectX thÃ nh cÃ´ng!" : $"MÃ£ lá»—i: {process.ExitCode}", process.ExitCode == 0 ? "Green" : "Red"); }
                if (File.Exists(directXPath)) File.Delete(directXPath);
            }
            catch (Exception ex) { UpdateStatus($"Lá»—i: {ex.Message}", "Red"); }
        }

        private async void BtnJava_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Äang táº£i Java...", "Cyan");
            string javaInstallerPath = Path.Combine(GetGMTPCFolder(), "java_installer.exe");
            try
            {
                await DownloadWithProgressAsync(JAVA_DOWNLOAD_URL, javaInstallerPath, "Java Installer");
                UpdateStatus("Äang cháº¡y Java installer...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo { FileName = javaInstallerPath, Arguments = JAVA_INSTALL_ARGUMENTS, UseShellExecute = true };
                Process process = Process.Start(startInfo);
                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    // Java web installer thÆ°á»ng tráº£ -1 hoáº·c 16389 khi thÃ nh cÃ´ng
                    bool isSuccess = process.ExitCode == 0 || process.ExitCode == -1 || process.ExitCode == 16389;
                    UpdateStatus(isSuccess ? "CÃ i Ä‘áº·t Java thÃ nh cÃ´ng!" : $"MÃ£ lá»—i: {process.ExitCode}", isSuccess ? "Green" : "Red");
                }
                if (File.Exists(javaInstallerPath)) File.Delete(javaInstallerPath);
            }
            catch (Exception ex) { UpdateStatus($"Lá»—i: {ex.Message}", "Red"); }
        }

        private async void BtnOpenAL_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Äang táº£i OpenAL...", "Cyan");
            string openALInstallerPath = Path.Combine(GetGMTPCFolder(), "OpenAL.exe");
            try
            {
                await DownloadWithProgressAsync("https://github.com/ghostminhtoan/MMT/releases/download/v1.0/OpenAL.exe", openALInstallerPath, "OpenAL Installer");
                UpdateStatus("Äang cháº¡y OpenAL installer vá»›i lá»‡nh /s...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo { FileName = openALInstallerPath, Arguments = "/s", UseShellExecute = true };
                Process process = Process.Start(startInfo);
                if (process != null) { await Task.Run(() => process.WaitForExit()); UpdateStatus(process.ExitCode == 0 ? "CÃ i Ä‘áº·t OpenAL thÃ nh cÃ´ng!" : $"MÃ£ lá»—i: {process.ExitCode}", process.ExitCode == 0 ? "Green" : "Red"); }
                if (File.Exists(openALInstallerPath)) File.Delete(openALInstallerPath);
            }
            catch (Exception ex) { UpdateStatus($"Lá»—i: {ex.Message}", "Red"); }
        }
    }
}

