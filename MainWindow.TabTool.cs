// =======================================================================
// MainWindow.TabTool.cs
// AI Summary: 2026-05-11 - Consolidated all MainWindow.Tab*.cs logic into a
//             single TabTool partial so selection, install, and tab handlers
//             live in one place after the UI tab consolidation
// =======================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;

namespace AICodeAgentAIOGMTPC
{
    public partial class MainWindow
    {
        // -----------------------------------------------------------------------
        // SOURCE: MainWindow.TabBrowser.cs
        // -----------------------------------------------------------------------
        private async Task InstallCocCocAsync()
        {
            UpdateStatus("Äang táº£i Cá»‘c Cá»‘c...", "Cyan");
            string cocCocInstallerPath = Path.Combine(GetGMTPCFolder(), "coccoc_standalone_vi.exe");
            try
            {
                await DownloadWithProgressAsync(COCCOC_DOWNLOAD_URL, cocCocInstallerPath, "Cá»‘c Cá»‘c");

                // Reset progress UI after download
                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus("Äang cháº¡y Cá»‘c Cá»‘c installer ( " + COCCOC_INSTALL_ARGUMENTS + " )...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = cocCocInstallerPath,
                    Arguments = COCCOC_INSTALL_ARGUMENTS,
                    UseShellExecute = true
                };

                Process process = Process.Start(startInfo);
                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    if (process.ExitCode == 0)
                    {
                        UpdateStatus("CÃ i Ä‘áº·t Cá»‘c Cá»‘c hoÃ n táº¥t.", "Green");
                    }
                    else
                    {
                        UpdateStatus($"Cá»‘c Cá»‘c installer káº¿t thÃºc vá»›i mÃ£ {process.ExitCode}", "Yellow");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                UpdateStatus("Táº£i Cá»‘c Cá»‘c bá»‹ há»§y.", "Red");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lá»—i khi táº£i hoáº·c cÃ i Ä‘áº·t Cá»‘c Cá»‘c: {ex.Message}", "Red");
            }
            finally
            {
                if (File.Exists(cocCocInstallerPath))
                {
                    try { File.Delete(cocCocInstallerPath); }
                    catch { }
                }
            }
        }


        private async Task InstallEdgeAsync()
        {
            try
            {
                UpdateStatus("Äang táº£i Microsoft Edge...", "Cyan");
                string edgePath = Path.Combine(GetGMTPCFolder(), "MicrosoftEdgeSetup.exe");
                await DownloadWithProgressAsync(EDGE_DOWNLOAD_URL, edgePath, "Microsoft Edge");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus("Äang cháº¡y Microsoft Edge installer ( " + EDGE_INSTALL_ARGUMENTS + " )...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = edgePath,
                    Arguments = EDGE_INSTALL_ARGUMENTS,
                    UseShellExecute = true
                };
                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("Microsoft Edge Ä‘Ã£ hoÃ n táº¥t.", "Green");
                }

                if (File.Exists(edgePath)) File.Delete(edgePath);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lá»—i khi cÃ i Microsoft Edge: {ex.Message}", "Red");
            }
        }

        private async Task InstallBraveAsync()
        {
            try
            {
                UpdateStatus("Äang táº£i Brave...", "Cyan");
                string bravePath = Path.Combine(GetGMTPCFolder(), "BraveSetup.exe");
                await DownloadWithProgressAsync(BRAVE_DOWNLOAD_URL, bravePath, "Brave");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus("Äang cháº¡y Brave installer...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = bravePath,
                    Arguments = BRAVE_INSTALL_ARGUMENTS,
                    UseShellExecute = true
                };

                Process process = Process.Start(startInfo);
                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("Brave Ä‘Ã£ hoÃ n táº¥t.", "Green");
                }

                if (File.Exists(bravePath)) File.Delete(bravePath);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lá»—i khi cÃ i Brave: {ex.Message}", "Red");
            }
        }


        private void ChkChrome_Click(object sender, RoutedEventArgs e)
        {
            if (ChkChrome.IsChecked == true)
            {
                UpdateStatus("ÄÃ£ chá»n: Google Chrome", "Green");
            }
            else
            {
                UpdateStatus("ÄÃ£ há»§y chá»n: Google Chrome", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkCocCoc_Click(object sender, RoutedEventArgs e)
        {
            if (ChkCocCoc.IsChecked == true)
            {
                UpdateStatus("ÄÃ£ chá»n: Cá»‘c Cá»‘c", "Green");
            }
            else
            {
                UpdateStatus("ÄÃ£ há»§y chá»n: Cá»‘c Cá»‘c", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkEdge_Click(object sender, RoutedEventArgs e)
        {
            if (ChkEdge.IsChecked == true)
            {
                UpdateStatus("ÄÃ£ chá»n: Microsoft Edge", "Green");
            }
            else
            {
                UpdateStatus("ÄÃ£ há»§y chá»n: Microsoft Edge", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private void ChkBrave_Click(object sender, RoutedEventArgs e)
        {
            if (ChkBrave.IsChecked == true)
            {
                UpdateStatus("ÄÃ£ chá»n: Brave", "Green");
            }
            else
            {
                UpdateStatus("ÄÃ£ há»§y chá»n: Brave", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private async Task InstallChromeAsync()
        {
            UpdateStatus("Äang táº£i Google Chrome...", "Cyan");
            string chromeInstallerPath = Path.Combine(GetGMTPCFolder(), "ChromeSetup.exe");
            try
            {
                await DownloadWithProgressAsync(CHROME_DOWNLOAD_URL, chromeInstallerPath, "Google Chrome");

                // Reset progress UI after download
                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus("Äang cháº¡y Chrome installer ( " + CHROME_INSTALL_ARGUMENTS + " )...", "Yellow");

                // Run installer with arguments
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = chromeInstallerPath,
                    Arguments = CHROME_INSTALL_ARGUMENTS,
                    UseShellExecute = true
                };

                Process process = Process.Start(startInfo);
                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    if (process.ExitCode == 0)
                    {
                        UpdateStatus("CÃ i Ä‘áº·t Google Chrome hoÃ n táº¥t.", "Green");
                    }
                    else
                    {
                        UpdateStatus($"Chrome installer káº¿t thÃºc vá»›i mÃ£ {process.ExitCode}", "Yellow");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                UpdateStatus("Táº£i Chrome bá»‹ há»§y.", "Red");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lá»—i khi táº£i hoáº·c cÃ i Ä‘áº·t Chrome: {ex.Message}", "Red");
            }
            finally
            {
                if (File.Exists(chromeInstallerPath))
                {
                    try { File.Delete(chromeInstallerPath); }
                    catch { }
                }
            }
        }

// -----------------------------------------------------------------------
// SOURCE: MainWindow.TabDriver.cs
// -----------------------------------------------------------------------
// ===================================================================
        // TabDriver â€” Checkbox Click Handlers
        // TabItem Header: "Driver"
        // Checkboxes: Chk3DPChip, Chk3DPNet
        // ===================================================================
        private void Chk3DPChip_Click(object sender, RoutedEventArgs e)
        {
            if (Chk3DPChip.IsChecked == true)
            {
                UpdateStatus("ÄÃ£ chá»n: 3DP Chip (all driver trá»« internet)", "Green");
            }
            else
            {
                UpdateStatus("ÄÃ£ há»§y chá»n: 3DP Chip (all driver trá»« internet)", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private void Chk3DPNet_Click(object sender, RoutedEventArgs e)
        {
            if (Chk3DPNet.IsChecked == true)
            {
                UpdateStatus("ÄÃ£ chá»n: 3DP Net - driver internet", "Green");
            }
            else
            {
                UpdateStatus("ÄÃ£ há»§y chá»n: 3DP Net - driver internet", "Yellow");
            }

            UpdateInstallButtonState();
        }

        // ===================================================================
        // TabDriver â€” Install Methods
        // ===================================================================
        private Task Run3DPChipAsync()
        {
            Btn3DPChip_Click(null, null);
            return Task.CompletedTask;
        }

        private Task Install3DPNetAsync()
        {
            Btn3DPNet_Click(null, null);
            return Task.CompletedTask;
        }

        // ===================================================================
        // TabDriver â€” Button Click Handlers (actual implementation)
        // ===================================================================
        private async void Btn3DPChip_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Äang cháº¡y 3DP Chip - all driver trá»« internet...", "Cyan");
            string driverChipPath = Path.Combine(@"R:\HDD R\ZC SYMLINK\USERS\Downloads\Programs", "3DP_Chip_v2510.exe");
            if (File.Exists(driverChipPath))
            {
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo { FileName = driverChipPath, UseShellExecute = true };
                    Process process = Process.Start(startInfo);
                    if (process != null) { await Task.Run(() => process.WaitForExit()); UpdateStatus(process.ExitCode == 0 ? "3DP Chip hoÃ n táº¥t!" : $"MÃ£ lá»—i: {process.ExitCode}", process.ExitCode == 0 ? "Green" : "Red"); }
                }
                catch (Exception ex) { UpdateStatus($"Lá»—i: {ex.Message}", "Red"); }
            }
            else { UpdateStatus("KhÃ´ng tÃ¬m tháº¥y file 3DP_Chip_v2510.exe", "Red"); }
        }

        private async void Btn3DPNet_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Äang táº£i 3DP Net - driver internet...", "Cyan");
            string driverNetPath = Path.Combine(GetGMTPCFolder(), "3DP_Net_v2101.exe");
            try
            {
                await DownloadWithProgressAsync("https://github.com/ghostminhtoan/MMT/releases/download/v1.0/3DP.Net.exe", driverNetPath, "3DP Net Driver Installer");
                UpdateStatus("Äang cháº¡y 3DP Net vá»›i lá»‡nh /y...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo { FileName = driverNetPath, Arguments = "/y", UseShellExecute = true };
                Process process = Process.Start(startInfo);
                if (process != null) { await Task.Run(() => process.WaitForExit()); UpdateStatus(process.ExitCode == 0 ? "3DP Net hoÃ n táº¥t!" : $"MÃ£ lá»—i: {process.ExitCode}", process.ExitCode == 0 ? "Green" : "Red"); }
                if (File.Exists(driverNetPath)) File.Delete(driverNetPath);
            }
            catch (Exception ex) { UpdateStatus($"Lá»—i: {ex.Message}", "Red"); }
        }

// -----------------------------------------------------------------------
// SOURCE: MainWindow.TabGaming.cs
// -----------------------------------------------------------------------
private async Task InstallMSIAfterburnerAsync()
        {
            try
            {
                UpdateStatus("Đang tải MSI Afterburner...", "Cyan");
                string msiAfterburnerPath = Path.Combine(GetGMTPCFolder(), "MSIAfterburner.exe");
                await DownloadWithProgressAsync("https://github.com/ghostminhtoan/MMT/releases/download/v1.0/MSI.Afterburner.exe", msiAfterburnerPath, "MSI Afterburner");

                UpdateStatus("Đang cài đặt MSI Afterburner...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = msiAfterburnerPath,
                    UseShellExecute = true
                };
                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("Cài đặt MSI Afterburner hoàn tất!", "Green");
                }

                if (File.Exists(msiAfterburnerPath))
                {
                    File.Delete(msiAfterburnerPath);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi cài đặt MSI Afterburner: {ex.Message}", "Red");
            }
        }

        private async Task InstallHibitUninstallerAsync()
        {
            try
            {
                UpdateStatus("Đang tải Revo Uninstaller...", "Cyan");
                string RevoPath = Path.Combine(GetGMTPCFolder(), "RevoUninstaller-setup.exe");
                await DownloadWithProgressAsync("https://github.com/ghostminhtoan/MMT/releases/download/v1.0/Revo.Uninstaller.Pro.exe", RevoPath, "Revo Uninstaller Installer");

                // Reset progress UI after download
                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ConnectionTraceGrid.Children.Clear();
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus("Đang chạy Revo Uninstaller với lệnh /S /I...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = RevoPath,
                    Arguments = "/S /I",
                    UseShellExecute = true
                };
                Process process = Process.Start(startInfo);
                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    if (process.ExitCode == 0)
                    {
                        UpdateStatus("Cài đặt Revo Uninstaller thành công!", "Green");
                    }
                    else
                    {
                        UpdateStatus($"Cài đặt Revo Uninstaller thất bại. Mã lỗi: {process.ExitCode}", "Red");
                    }
                }

                if (File.Exists(RevoPath))
                {
                    File.Delete(RevoPath);
                    UpdateStatus("Đã xóa file Revo Uninstaller installer tạm thời", "Cyan");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi tải hoặc cài đặt Revo Uninstaller: {ex.Message}", "Red");
            }
        }

        private void ChkProcessLasso_Click(object sender, RoutedEventArgs e)
        {
            if (ChkProcessLasso.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Process Lasso", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Process Lasso", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkThrottlestop_Click(object sender, RoutedEventArgs e)
        {
            if (ChkThrottlestop.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Throttlestop", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Throttlestop", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkMSIAfterburner_Click(object sender, RoutedEventArgs e)
        {
            if (ChkMSIAfterburner.IsChecked == true)
            {
                UpdateStatus("Đã chọn: MSI Afterburner", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: MSI Afterburner", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkLeagueOfLegends_Click(object sender, RoutedEventArgs e)
        {
            if (ChkLeagueOfLegends.IsChecked == true)
            {
                UpdateStatus("Đã chọn: League of Legends VN", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: League of Legends VN", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkPorofessor_Click(object sender, RoutedEventArgs e)
        {
            if (ChkPorofessor.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Porofessor", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Porofessor", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkSamuraiMaiden_Click(object sender, RoutedEventArgs e)
        {
            if (ChkSamuraiMaiden.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Samurai Maiden", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Samurai Maiden", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkGhostOfTsushima_Click(object sender, RoutedEventArgs e)
        {
            if (ChkGhostOfTsushima.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Ghost of Tsushima", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Ghost of Tsushima", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private void ChkJumpForce_Click(object sender, RoutedEventArgs e)
        {
            if (ChkJumpForce.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Jump Force (11 parts)", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Jump Force", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private async Task InstallProcessLassoAsync()
        {
            try
            {
                UpdateStatus("Đang tải Process Lasso...", "Cyan");
                string processLassoPath = Path.Combine(GetGMTPCFolder(), "ProcessLasso.exe");
                await DownloadWithProgressAsync("https://github.com/ghostminhtoan/MMT/releases/download/v1.0/Process.Lasso.exe", processLassoPath, "Process Lasso");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                MessageBoxResult result = MessageBox.Show("Yes = Cài đặt tự động vào ổ C\nNo = Cài vào ổ khác", "Cài đặt Process Lasso", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = processLassoPath,
                    UseShellExecute = true
                };

                if (result == MessageBoxResult.Yes)
                {
                    startInfo.Arguments = "/s";
                    UpdateStatus("Cài đặt Process Lasso vào ổ C...", "Yellow");
                }
                else if (result == MessageBoxResult.No)
                {
                    UpdateStatus("Cài Process Lasso vào ổ khác...", "Yellow");
                }
                else
                {
                    UpdateStatus("Đã hủy cài đặt Process Lasso", "Yellow");
                    if (File.Exists(processLassoPath))
                    {
                        File.Delete(processLassoPath);
                    }
                    return;
                }

                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("Cài đặt Process Lasso hoàn tất!", "Green");
                }

                if (File.Exists(processLassoPath))
                {
                    File.Delete(processLassoPath);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi cài đặt Process Lasso: {ex.Message}", "Red");
            }
        }


        private async Task InstallThrottlestopAsync()
        {
            try
            {
                UpdateStatus("Đang tải Throttlestop...", "Cyan");
                string throttlestopPath = Path.Combine(GetGMTPCFolder(), "Throttlestop.exe");
                await DownloadWithProgressAsync("https://github.com/ghostminhtoan/MMT/releases/download/v1.0/Throttlestop.exe", throttlestopPath, "Throttlestop");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                MessageBoxResult result = MessageBox.Show("Yes = Cài đặt tự động vào ổ C\nNo = Cài vào ổ khác", "Cài đặt Throttlestop", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = throttlestopPath,
                    UseShellExecute = true
                };

                if (result == MessageBoxResult.Yes)
                {
                    startInfo.Arguments = "/s";
                    UpdateStatus("Cài đặt Throttlestop vào ổ C...", "Yellow");
                }
                else if (result == MessageBoxResult.No)
                {
                    UpdateStatus("Cài Throttlestop vào ổ khác...", "Yellow");
                }
                else
                {
                    UpdateStatus("Đã hủy cài đặt Throttlestop", "Yellow");
                    if (File.Exists(throttlestopPath))
                    {
                        File.Delete(throttlestopPath);
                    }
                    return;
                }

                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("Cài đặt Throttlestop hoàn tất!", "Green");
                }

                if (File.Exists(throttlestopPath))
                {
                    File.Delete(throttlestopPath);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi cài đặt Throttlestop: {ex.Message}", "Red");
            }
        }


        private async Task InstallLeagueOfLegendsVNAsync()
        {
            try
            {
                UpdateStatus("Đang tải League of Legends VN...", "Cyan");
                string lolPath = Path.Combine(GetGMTPCFolder(), "LeagueOfLegendsVN.exe");
                await DownloadWithProgressAsync("https://lol.secure.dyn.riotcdn.net/channels/public/x/installer/current/live.vn2.exe", lolPath, "League of Legends VN");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus("Đang cài đặt League of Legends VN...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = lolPath,
                    UseShellExecute = true
                };
                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("Cài đặt League of Legends VN hoàn tất!", "Green");
                }

                if (File.Exists(lolPath))
                {
                    File.Delete(lolPath);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi cài đặt League of Legends VN: {ex.Message}", "Red");
            }
        }


        private async Task InstallPorofessorAsync()
        {
            try
            {
                UpdateStatus("Đang tải Porofessor...", "Cyan");
                string porofessorPath = Path.Combine(GetGMTPCFolder(), "Porofessor.exe");
                await DownloadWithProgressAsync("https://github.com/ghostminhtoan/MMT/releases/download/v1.0/Porofessor.exe", porofessorPath, "Porofessor");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus("Đang chạy Porofessor...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = porofessorPath,
                    UseShellExecute = true
                };
                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("Porofessor đã hoàn tất!", "Green");
                }

                if (File.Exists(porofessorPath))
                {
                    File.Delete(porofessorPath);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi cài đặt Porofessor: {ex.Message}", "Red");
            }
        }


        private async Task InstallSamuraiMaidenAsync()
        {
            try
            {
                string gmtFolder = GetGMTPCFolder();
                string part1Path = Path.Combine(gmtFolder, "SAMURAI.MAIDEN_LinkNeverDie.Com.part1.exe");
                string part2Path = Path.Combine(gmtFolder, "SAMURAI.MAIDEN_LinkNeverDie.Com.part2.rar");
                string part3Path = Path.Combine(gmtFolder, "SAMURAI.MAIDEN_LinkNeverDie.Com.part3.rar");
                string part4Path = Path.Combine(gmtFolder, "SAMURAI.MAIDEN_LinkNeverDie.Com.part4.rar");

                // Download part 1
                UpdateStatus("Đang tải Samurai Maiden - Part 1...", "Cyan");
                await DownloadWithProgressAsync(
                    "https://github.com/ghostminhtoan/MMT/releases/download/game/SAMURAI.MAIDEN_LinkNeverDie.Com.part1.exe",
                    part1Path, "Samurai Maiden - Part 1");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                // Download part 2
                UpdateStatus("Đang tải Samurai Maiden - Part 2...", "Cyan");
                await DownloadWithProgressAsync(
                    "https://github.com/ghostminhtoan/MMT/releases/download/game/SAMURAI.MAIDEN_LinkNeverDie.Com.part2.rar",
                    part2Path, "Samurai Maiden - Part 2");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                // Download part 3
                UpdateStatus("Đang tải Samurai Maiden - Part 3...", "Cyan");
                await DownloadWithProgressAsync(
                    "https://github.com/ghostminhtoan/MMT/releases/download/game/SAMURAI.MAIDEN_LinkNeverDie.Com.part3.rar",
                    part3Path, "Samurai Maiden - Part 3");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                // Download part 4
                UpdateStatus("Đang tải Samurai Maiden - Part 4...", "Cyan");
                await DownloadWithProgressAsync(
                    "https://github.com/ghostminhtoan/MMT/releases/download/game/SAMURAI.MAIDEN_LinkNeverDie.Com.part4.rar",
                    part4Path, "Samurai Maiden - Part 4");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                // Run part1.exe
                UpdateStatus("Đang chạy SAMURAI.MAIDEN_LinkNeverDie.Com.part1.exe...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = part1Path,
                    UseShellExecute = true,
                    WorkingDirectory = gmtFolder
                };
                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("Samurai Maiden đã hoàn tất!", "Green");
                }

                // Delete all parts after installation
                if (File.Exists(part1Path)) File.Delete(part1Path);
                if (File.Exists(part2Path)) File.Delete(part2Path);
                if (File.Exists(part3Path)) File.Delete(part3Path);
                if (File.Exists(part4Path)) File.Delete(part4Path);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi cài đặt Samurai Maiden: {ex.Message}", "Red");
            }
        }

        private async Task InstallGhostOfTsushimaAsync()
        {
            try
            {
                // Use default temp folder in GMTPC folder
                string tempFolder = Path.Combine(GetGMTPCFolder(), "GhostOfTsushima_Temp_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }

                UpdateStatus($"Đang tải về folder: {tempFolder}", "Cyan");

                string part01Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part01.exe");
                string part02Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part02.rar");
                string part03Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part03.rar");
                string part04Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part04.rar");
                string part05Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part05.rar");
                string part06Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part06.rar");
                string part07Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part07.rar");
                string part08Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part08.rar");
                string part09Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part09.rar");
                string part10Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part10.rar");
                string part11Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part11.rar");
                string part12Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part12.rar");
                string part13Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part13.rar");
                string part14Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part14.rar");
                string part15Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part15.rar");
                string part16Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part16.rar");
                string part17Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part17.rar");
                string part18Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part18.rar");
                string part19Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part19.rar");
                string part20Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part20.rar");
                string part21Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part21.rar");
                string part22Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part22.rar");
                string part23Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part23.rar");
                string part24Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part24.rar");
                string part25Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part25.rar");
                string part26Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part26.rar");
                string part27Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part27.rar");
                string part28Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part28.rar");
                string part29Path = Path.Combine(tempFolder, "Ghost.of.Tsushima_LinkNeverDie.Com.part29.rar");

                // Download all 29 parts
                UpdateStatus("Đang tải Ghost of Tsushima - Part 1/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART01_URL, part01Path, "Ghost of Tsushima - Part 1");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 2/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART02_URL, part02Path, "Ghost of Tsushima - Part 2");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 3/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART03_URL, part03Path, "Ghost of Tsushima - Part 3");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 4/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART04_URL, part04Path, "Ghost of Tsushima - Part 4");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 5/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART05_URL, part05Path, "Ghost of Tsushima - Part 5");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 6/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART06_URL, part06Path, "Ghost of Tsushima - Part 6");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 7/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART07_URL, part07Path, "Ghost of Tsushima - Part 7");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 8/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART08_URL, part08Path, "Ghost of Tsushima - Part 8");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 9/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART09_URL, part09Path, "Ghost of Tsushima - Part 9");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 10/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART10_URL, part10Path, "Ghost of Tsushima - Part 10");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 11/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART11_URL, part11Path, "Ghost of Tsushima - Part 11");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 12/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART12_URL, part12Path, "Ghost of Tsushima - Part 12");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 13/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART13_URL, part13Path, "Ghost of Tsushima - Part 13");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 14/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART14_URL, part14Path, "Ghost of Tsushima - Part 14");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 15/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART15_URL, part15Path, "Ghost of Tsushima - Part 15");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 16/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART16_URL, part16Path, "Ghost of Tsushima - Part 16");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 17/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART17_URL, part17Path, "Ghost of Tsushima - Part 17");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 18/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART18_URL, part18Path, "Ghost of Tsushima - Part 18");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 19/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART19_URL, part19Path, "Ghost of Tsushima - Part 19");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 20/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART20_URL, part20Path, "Ghost of Tsushima - Part 20");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 21/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART21_URL, part21Path, "Ghost of Tsushima - Part 21");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 22/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART22_URL, part22Path, "Ghost of Tsushima - Part 22");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 23/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART23_URL, part23Path, "Ghost of Tsushima - Part 23");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 24/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART24_URL, part24Path, "Ghost of Tsushima - Part 24");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 25/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART25_URL, part25Path, "Ghost of Tsushima - Part 25");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 26/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART26_URL, part26Path, "Ghost of Tsushima - Part 26");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 27/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART27_URL, part27Path, "Ghost of Tsushima - Part 27");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 28/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART28_URL, part28Path, "Ghost of Tsushima - Part 28");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                UpdateStatus("Đang tải Ghost of Tsushima - Part 29/29...", "Cyan");
                await DownloadWithProgressAsync(GHOST_OF_TSUSHIMA_PART29_URL, part29Path, "Ghost of Tsushima - Part 29");
                Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });

                // Run part01.exe
                UpdateStatus("Đang chạy Ghost.of.Tsushima_LinkNeverDie.Com.part01.exe...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = part01Path,
                    UseShellExecute = true,
                    WorkingDirectory = tempFolder
                };
                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("Ghost of Tsushima đã hoàn tất!", "Green");
                }

                // Delete temp folder after installation
                UpdateStatus("Đang xóa file tạm thời...", "Cyan");
                try
                {
                    if (Directory.Exists(tempFolder))
                    {
                        Directory.Delete(tempFolder, true);
                        UpdateStatus($"Đã xóa folder tạm: {tempFolder}", "Green");
                    }
                }
                catch (Exception exDelete)
                {
                    UpdateStatus($"Không thể xóa folder tạm: {exDelete.Message}", "Yellow");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi cài đặt Ghost of Tsushima: {ex.Message}", "Red");
            }
        }

        // ===================================================================
        // Jump Force - 11 parts multi-part installer
        // ===================================================================
        private async Task InstallJumpForceAsync()
        {
            string gmtPCFolder = GetGMTPCFolder();
            string tempFolder = Path.Combine(gmtPCFolder, "JumpForce_Temp_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));

            UpdateStatus("Đang tải về 11 file parts...", "Cyan");

            try
            {
                // Create temp folder
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }

                // Download all 11 parts sequentially with progress
                string[] partUrls = new string[]
                {
                    JUMP_FORCE_PART01_URL, JUMP_FORCE_PART02_URL, JUMP_FORCE_PART03_URL,
                    JUMP_FORCE_PART04_URL, JUMP_FORCE_PART05_URL, JUMP_FORCE_PART06_URL,
                    JUMP_FORCE_PART07_URL, JUMP_FORCE_PART08_URL, JUMP_FORCE_PART09_URL,
                    JUMP_FORCE_PART10_URL, JUMP_FORCE_PART11_URL
                };

                string[] partPaths = new string[11];
                for (int i = 0; i < partUrls.Length; i++)
                {
                    string partFileName = $"JUMP.FORCE_LinkNeverDie.Com.part{(i + 1):D2}.{(i == 0 ? "exe" : "rar")}";
                    partPaths[i] = Path.Combine(tempFolder, partFileName);

                    UpdateStatus($"Đang tải phần {i + 1}/11...", "Cyan");
                    await DownloadWithProgressAsync(partUrls[i], partPaths[i], $"Jump Force - Part {i + 1}");
                    Dispatcher.Invoke(() => { DownloadProgressBar.Value = 0; ProgressTextBlock.Text = ""; SpeedTextBlock.Text = ""; });
                }

                // Run part01.exe
                UpdateStatus("Đang chạy JUMP.FORCE_LinkNeverDie.Com.part01.exe...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = partPaths[0],
                    UseShellExecute = true,
                    WorkingDirectory = tempFolder
                };
                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("Jump Force đã hoàn tất!", "Green");
                }

                // Delete temp folder after installation
                UpdateStatus("Đang xóa file tạm thời...", "Cyan");
                try
                {
                    if (Directory.Exists(tempFolder))
                    {
                        Directory.Delete(tempFolder, true);
                        UpdateStatus($"Đã xóa folder tạm: {tempFolder}", "Green");
                    }
                }
                catch (Exception exDelete)
                {
                    UpdateStatus($"Không thể xóa folder tạm: {exDelete.Message}", "Yellow");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi cài đặt Jump Force: {ex.Message}", "Red");
            }
        }

// -----------------------------------------------------------------------
// SOURCE: MainWindow.TabInfo.cs
// -----------------------------------------------------------------------
private DispatcherTimer _systemInfoRefreshTimer;
        private string _cpuInfoTemplate;
        private uint _cpuBaseClockMhz;
        private uint _cpuTurboClockMhz;
        private bool _cpuClockRefreshInProgress;
        private static readonly HttpClient _cpuSpecClient = CreateCpuSpecClient();

        private void PopulateSystemInfo()
        {
            try
            {
                TbGPU.Text = BuildGpuInfo();
                InitializeCpuInfoCache();
                TbRAM.Text = BuildRamInfo();
                TbMainboard.Text = BuildMainboardInfo();
                EnsureSystemInfoRefreshTimer();
            }
            catch (Exception ex)
            {
                // don't crash UI
                try { TbMainboard.Text = "Error: " + ex.Message; } catch { }
            }
        }

        private string BuildGpuInfo()
        {
            StringBuilder sb = new StringBuilder();
            int index = 1;

            foreach (ManagementObject gpu in QueryWmi("Win32_VideoController"))
            {
                AppendLine(sb, $"=== GPU {index} ===");
                AppendLine(sb, "Tên", GetValue(gpu, "Name"));
                AppendLine(sb, "Driver Version", GetValue(gpu, "DriverVersion"));
                AppendLine(sb, "Display memory (VRAM)", FormatGpuMemory(gpu));
                AppendLine(sb, "Video Processor", GetValue(gpu, "VideoProcessor"));
                AppendGpuCodecInfo(sb, GetValue(gpu, "Name"), GetValue(gpu, "VideoProcessor"));
                AppendLine(sb, "Độ phân giải", FormatResolution(gpu));
                AppendLine(sb, "Refresh Rate", AppendUnit(GetValue(gpu, "CurrentRefreshRate"), "Hz"));
                AppendLine(sb, "Bit Depth", AppendUnit(GetValue(gpu, "CurrentBitsPerPixel"), "bit"));
                AppendLine(sb, "Trạng thái", GetValue(gpu, "Status"));
                AppendLine(sb, "PNP Device ID", GetValue(gpu, "PNPDeviceID"));
                index++;
            }

            return sb.Length > 0 ? sb.ToString().TrimEnd() : "Unknown";
        }

        private void AppendGpuCodecInfo(StringBuilder sb, string gpuName, string videoProcessor)
        {
            string name = $"{gpuName} {videoProcessor}".ToLowerInvariant();
            string encoder = "Unknown";
            string decoder = "Unknown";
            string quickSync = "No";
            string nvidiaEncode = "No";
            string amdEncode = "No";

            if (name.Contains("nvidia") || name.Contains("geforce") || name.Contains("quadro") || name.Contains("rtx") || name.Contains("gtx"))
            {
                encoder = "NVIDIA NVENC";
                decoder = "NVIDIA NVDEC / PureVideo";
                nvidiaEncode = "Yes";
            }
            else if (name.Contains("intel") || name.Contains("uhd") || name.Contains("iris") || name.Contains("arc"))
            {
                encoder = "Intel Quick Sync Video";
                decoder = "Intel Quick Sync Video";
                quickSync = "Yes";
            }
            else if (name.Contains("amd") || name.Contains("radeon") || name.Contains("rx ") || name.Contains("vega"))
            {
                encoder = "AMD AMF / VCE / VCN";
                decoder = "AMD UVD / VCN";
                amdEncode = "Yes";
            }

            AppendLine(sb, "Encode Support", encoder == "Unknown" ? "Unknown" : "Yes");
            AppendLine(sb, "Decode Support", decoder == "Unknown" ? "Unknown" : "Yes");
            AppendLine(sb, "Hardware Encoder", encoder);
            AppendLine(sb, "Hardware Decoder", decoder);
            AppendLine(sb, "Intel QuickSync Support", quickSync);
            AppendLine(sb, "NVIDIA Encode", nvidiaEncode);
            AppendLine(sb, "AMD Encode", amdEncode);
        }

        private string BuildCpuInfoTemplate()
        {
            StringBuilder sb = new StringBuilder();
            int index = 1;

            foreach (ManagementObject cpu in QueryWmi("Win32_Processor"))
            {
                AppendLine(sb, $"=== CPU {index} ===");
                AppendLine(sb, "Tên", GetValue(cpu, "Name"));
                AppendLine(sb, "Hãng sản xuất", GetValue(cpu, "Manufacturer"));
                AppendLine(sb, "Kiến trúc", GetArchitectureName(GetValue(cpu, "Architecture")));
                AppendLine(sb, "Socket", GetValue(cpu, "SocketDesignation"));
                AppendLine(sb, "Số nhân", GetValue(cpu, "NumberOfCores"));
                AppendLine(sb, "Số luồng", GetValue(cpu, "NumberOfLogicalProcessors"));
                AppendLine(sb, "Base clock", "{BASE_CLOCK}");
                AppendLine(sb, "Turbo clock", "{TURBO_CLOCK}");
                AppendLine(sb, "Xung hiện tại", "{CURRENT_CLOCK}");
                AppendLine(sb, "Cache L2", AppendUnit(GetValue(cpu, "L2CacheSize"), "KB"));
                AppendLine(sb, "Cache L3", AppendUnit(GetValue(cpu, "L3CacheSize"), "KB"));
                AppendLine(sb, "Virtualization", GetValue(cpu, "VirtualizationFirmwareEnabled"));
                AppendLine(sb, "Processor ID", GetValue(cpu, "ProcessorId"));
                AppendLine(sb, "");
                index++;
            }

            return sb.Length > 0 ? sb.ToString().TrimEnd() : "Unknown";
        }

        private void InitializeCpuInfoCache()
        {
            _cpuBaseClockMhz = GetCpuBaseClockMhz();
            _cpuTurboClockMhz = 0;
            _cpuInfoTemplate = BuildCpuInfoTemplate();
            TbCPU.Text = RenderCpuInfoTemplate(GetCurrentCpuClockMhz());
            _ = LoadCpuTurboClockAsync();
        }

        private string RenderCpuInfoTemplate(uint currentClockMhz)
        {
            if (string.IsNullOrWhiteSpace(_cpuInfoTemplate))
            {
                return "Unknown";
            }

            return _cpuInfoTemplate
                .Replace("{BASE_CLOCK}", AppendUnit(_cpuBaseClockMhz.ToString(), "MHz"))
                .Replace("{TURBO_CLOCK}", _cpuTurboClockMhz > 0 ? AppendUnit(_cpuTurboClockMhz.ToString(), "MHz") : "Loading...")
                .Replace("{CURRENT_CLOCK}", AppendUnit(currentClockMhz.ToString(), "MHz"));
        }

        private void EnsureSystemInfoRefreshTimer()
        {
            if (_systemInfoRefreshTimer != null)
            {
                return;
            }

            _systemInfoRefreshTimer = new DispatcherTimer(DispatcherPriority.Background)
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _systemInfoRefreshTimer.Tick += SystemInfoRefreshTimer_Tick;
            _systemInfoRefreshTimer.Start();
        }

        private async void SystemInfoRefreshTimer_Tick(object sender, EventArgs e)
        {
            if (_cpuClockRefreshInProgress)
            {
                return;
            }

            _cpuClockRefreshInProgress = true;
            try
            {
                uint currentClockMhz = await Task.Run(() => GetCurrentCpuClockMhz());
                if (currentClockMhz == 0)
                {
                    currentClockMhz = _cpuBaseClockMhz;
                }

                if (!string.IsNullOrWhiteSpace(_cpuInfoTemplate))
                {
                    TbCPU.Text = RenderCpuInfoTemplate(currentClockMhz);
                }
            }
            catch
            {
            }
            finally
            {
                _cpuClockRefreshInProgress = false;
            }
        }

        private uint GetCpuBaseClockMhz()
        {
            try
            {
                ManagementObject cpu = FirstOrDefault(QueryWmi("Win32_Processor"));
                uint value = (uint)GetUlongValue(cpu, "MaxClockSpeed");
                if (value > 0)
                {
                    return value;
                }
            }
            catch
            {
            }

            return 0;
        }

        private async Task LoadCpuTurboClockAsync()
        {
            try
            {
                string cpuName = GetCpuModelName();
                if (string.IsNullOrWhiteSpace(cpuName))
                {
                    return;
                }

                uint turboClock = await Task.Run(() => TryResolveTurboClockFromWeb(cpuName));
                if (turboClock > 0)
                {
                    _cpuTurboClockMhz = turboClock;
                    if (!string.IsNullOrWhiteSpace(_cpuInfoTemplate))
                    {
                        Dispatcher.Invoke(() => TbCPU.Text = RenderCpuInfoTemplate(GetCurrentCpuClockMhz()));
                    }
                }
            }
            catch
            {
            }
        }

        private string GetCpuModelName()
        {
            try
            {
                ManagementObject cpu = FirstOrDefault(QueryWmi("Win32_Processor"));
                string name = GetValue(cpu, "Name");
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name;
                }
            }
            catch
            {
            }

            return null;
        }

        private uint TryResolveTurboClockFromWeb(string cpuName)
        {
            try
            {
                string slug = BuildChaynikamCpuSlug(cpuName);
                if (string.IsNullOrWhiteSpace(slug))
                {
                    return 0;
                }

                string url = $"https://www.chaynikam.info/en/{slug}.html";
                string html = _cpuSpecClient.GetStringAsync(url).GetAwaiter().GetResult();
                if (string.IsNullOrWhiteSpace(html))
                {
                    return 0;
                }

                Match match = Regex.Match(html, @"Turbo Boost\s*</td>\s*<td class=""tdc2"">\s*([0-9]+)\s*MHz", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (match.Success && uint.TryParse(match.Groups[1].Value, out uint turboMhz))
                {
                    return turboMhz;
                }

                match = Regex.Match(html, @"Turbo Boost\s*</td>\s*<td class=""tdc2"">\s*([0-9]+(?:\.[0-9]+)?)\s*GHz", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (match.Success && double.TryParse(match.Groups[1].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double turboGhz))
                {
                    return (uint)Math.Round(turboGhz * 1000.0);
                }
            }
            catch
            {
            }

            return 0;
        }

        private string BuildChaynikamCpuSlug(string cpuName)
        {
            if (string.IsNullOrWhiteSpace(cpuName))
            {
                return null;
            }

            string cleaned = cpuName;
            cleaned = Regex.Replace(cleaned, @"\s*@\s*[0-9.]+\s*GHz", "", RegexOptions.IgnoreCase);
            cleaned = cleaned.Replace("(R)", "").Replace("(TM)", "");
            cleaned = Regex.Replace(cleaned, @"\bCPU\b", "", RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();

            if (cleaned.StartsWith("Intel ", StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Substring(6).Trim();
            }
            else if (cleaned.StartsWith("AMD ", StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Substring(4).Trim();
            }

            cleaned = cleaned.Replace(' ', '_');
            cleaned = cleaned.Replace("(", "").Replace(")", "");
            cleaned = Regex.Replace(cleaned, @"_+", "_");
            cleaned = cleaned.Trim('_');
            return cleaned;
        }

        private uint GetCurrentCpuClockMhz()
        {
            try
            {
                uint baseClock = _cpuBaseClockMhz > 0 ? _cpuBaseClockMhz : GetCpuBaseClockMhz();
                if (baseClock == 0)
                {
                    return 0;
                }

                double processorPerformance = GetProcessorPerformancePercent();
                if (processorPerformance <= 0)
                {
                    return baseClock;
                }

                return (uint)Math.Round(baseClock * (processorPerformance / 100.0));
            }
            catch
            {
                return 0;
            }
        }

        private double GetProcessorPerformancePercent()
        {
            try
            {
                using (PerformanceCounter counter = new PerformanceCounter("Processor Information", "% Processor Performance", "_Total", true))
                {
                    counter.NextValue();
                    Thread.Sleep(100);
                    return counter.NextValue();
                }
            }
            catch
            {
                return 0;
            }
        }

        private string BuildRamInfo()
        {
            StringBuilder sb = new StringBuilder();
            ulong totalRamBytes = 0;
            int slot = 1;

            foreach (ManagementObject ram in QueryWmi("Win32_PhysicalMemory"))
            {
                ulong capacity = GetUlongValue(ram, "Capacity");
                totalRamBytes += capacity;

                AppendLine(sb, $"=== RAM Slot {slot} ===");
                AppendLine(sb, "Dung lượng", FormatBytes(capacity));
                AppendLine(sb, "Hãng sản xuất", GetValue(ram, "Manufacturer"));
                AppendLine(sb, "Part Number", GetValue(ram, "PartNumber"));
                AppendLine(sb, "Serial", GetValue(ram, "SerialNumber"));
                AppendLine(sb, "Bank", GetValue(ram, "BankLabel"));
                AppendLine(sb, "Vị trí", GetValue(ram, "DeviceLocator"));
                AppendLine(sb, "Tốc độ", AppendUnit(GetValue(ram, "Speed"), "MHz"));
                AppendLine(sb, "Configured Speed", AppendUnit(GetValue(ram, "ConfiguredClockSpeed"), "MHz"));
                AppendLine(sb, "Form Factor", GetMemoryFormFactor(GetValue(ram, "FormFactor")));
                AppendLine(sb, "");
                AppendLine(sb, "");
                slot++;
            }

            if (totalRamBytes > 0)
            {
                sb.Insert(0, $"Tổng RAM vật lý: {FormatBytes(totalRamBytes)}{Environment.NewLine}{Environment.NewLine}");
            }

            return sb.Length > 0 ? sb.ToString().TrimEnd() : "Unknown";
        }

        private string BuildMainboardInfo()
        {
            StringBuilder sb = new StringBuilder();
            ManagementObject board = FirstOrDefault(QueryWmi("Win32_BaseBoard"));
            ManagementObject bios = FirstOrDefault(QueryWmi("Win32_BIOS"));
            ManagementObject system = FirstOrDefault(QueryWmi("Win32_ComputerSystem"));

            AppendLine(sb, "Hãng sản xuất", GetValue(board, "Manufacturer"));
            AppendLine(sb, "Model", GetValue(board, "Product"));
            AppendLine(sb, "Serial", GetValue(board, "SerialNumber"));
            AppendLine(sb, "Version", GetValue(board, "Version"));
            AppendLine(sb, "Trạng thái", GetValue(board, "Status"));
            AppendLine(sb, "");
            AppendLine(sb, "BIOS", GetValue(bios, "Manufacturer"));
            AppendLine(sb, "BIOS Version", GetValue(bios, "SMBIOSBIOSVersion"));
            AppendLine(sb, "Ngày phát hành", FormatWmiDate(GetValue(bios, "ReleaseDate")));
            AppendLine(sb, "");
            AppendLine(sb, "System", GetValue(system, "Manufacturer"));
            AppendLine(sb, "System Model", GetValue(system, "Model"));
            AppendLine(sb, "System Type", GetValue(system, "SystemType"));

            return sb.Length > 0 ? sb.ToString().TrimEnd() : "Unknown";
        }

        private IEnumerable<ManagementObject> QueryWmi(string wmiClass)
        {
            List<ManagementObject> results = new List<ManagementObject>();

            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT * FROM {wmiClass}"))
                {
                    foreach (ManagementObject item in searcher.Get())
                    {
                        results.Add(item);
                    }
                }
            }
            catch { }

            return results;
        }

        private ManagementObject FirstOrDefault(IEnumerable<ManagementObject> items)
        {
            foreach (ManagementObject item in items)
            {
                return item;
            }

            return null;
        }

        private string GetValue(ManagementObject item, string propertyName)
        {
            try
            {
                if (item == null || item[propertyName] == null) return "Unknown";
                string value = item[propertyName].ToString().Trim();
                return string.IsNullOrWhiteSpace(value) ? "Unknown" : value;
            }
            catch
            {
                return "Unknown";
            }
        }

        private ulong GetUlongValue(ManagementObject item, string propertyName)
        {
            try
            {
                if (item == null || item[propertyName] == null) return 0;
                return Convert.ToUInt64(item[propertyName]);
            }
            catch
            {
                return 0;
            }
        }

        private void AppendLine(StringBuilder sb, string text)
        {
            if (text == null) return;
            sb.AppendLine(text);
        }

        private void AppendLine(StringBuilder sb, string label, string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value == "Unknown") return;
            sb.AppendLine($"{label}: {value}");
        }

        private string AppendUnit(string value, string unit)
        {
            if (string.IsNullOrWhiteSpace(value) || value == "Unknown") return "Unknown";
            return $"{value} {unit}";
        }

        private string FormatResolution(ManagementObject gpu)
        {
            string width = GetValue(gpu, "CurrentHorizontalResolution");
            string height = GetValue(gpu, "CurrentVerticalResolution");
            if (width == "Unknown" || height == "Unknown") return "Unknown";
            return $"{width} x {height}";
        }

        private string FormatWmiDate(string value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value) || value == "Unknown") return "Unknown";
                DateTime date = ManagementDateTimeConverter.ToDateTime(value);
                return date.ToString("dd/MM/yyyy");
            }
            catch
            {
                return "Unknown";
            }
        }

        private string GetArchitectureName(string value)
        {
            switch (value)
            {
                case "0": return "x86";
                case "1": return "MIPS";
                case "2": return "Alpha";
                case "3": return "PowerPC";
                case "5": return "ARM";
                case "6": return "Itanium";
                case "9": return "x64";
                case "12": return "ARM64";
                default: return value;
            }
        }

        private string GetMemoryFormFactor(string value)
        {
            switch (value)
            {
                case "8": return "DIMM";
                case "12": return "SODIMM";
                case "13": return "SRIMM";
                default: return value;
            }
        }

        private string GetDirectXVersion()
        {
            try
            {
                // Most modern Windows (10/11) strictly use DirectX 12
                // We can also check the registry for a broad version string
                string versionStr = "";
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\DirectX"))
                {
                    versionStr = key?.GetValue("Version") as string ?? "";
                }

                // Map common internal version strings to friendly names
                if (versionStr.StartsWith("4.09.00.0904"))
                {
                    // This is the common string for DirectX 9.0c, but also often remains on Win10/11
                    // Detection by OS version is more reliable for modern DX
                    if (IsWindows10Or11()) return "DirectX 12";
                    return "DirectX 9.0c";
                }

                // Detailed mapping if needed
                switch (versionStr)
                {
                    case "4.09.00.0904": return "DirectX 9.0c";
                    case "4.09.00.0902": return "DirectX 9.0b";
                    case "4.09.00.0900": return "DirectX 9.0";
                    case "4.08.01.0881": return "DirectX 8.1";
                    case "4.08.00.0400": return "DirectX 8.0";
                    case "4.07.00.0700": return "DirectX 7.0";
                }

                if (IsWindows10Or11()) return "DirectX 12";
                if (IsWindows8Or81()) return "DirectX 11.1/11.2";
                if (IsWindows7()) return "DirectX 11";

                return !string.IsNullOrEmpty(versionStr) ? $"DirectX {versionStr}" : "Unknown";
            }
            catch { return "Unknown"; }
        }

        private bool IsWindows10Or11()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                {
                    string buildStr = key?.GetValue("CurrentBuild")?.ToString() ?? "";
                    if (int.TryParse(buildStr, out int build))
                    {
                        return build >= 10240; // Windows 10 build 10240 is the first release
                    }
                }
            }
            catch { }
            return Environment.OSVersion.Version.Major >= 10;
        }

        private bool IsWindows8Or81()
        {
            return Environment.OSVersion.Version.Major == 6 && (Environment.OSVersion.Version.Minor == 2 || Environment.OSVersion.Version.Minor == 3);
        }

        private bool IsWindows7()
        {
            return Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1;
        }

        private string GetWmiSingleValue(string wmiClass, string property)
        {
            try
            {
                var searcher = new ManagementObjectSearcher($"select {property} from {wmiClass}");
                foreach (ManagementObject mo in searcher.Get())
                {
                    if (mo[property] != null)
                    {
                        return mo[property].ToString();
                    }
                }
            }
            catch { }
            return null;
        }

        private string FormatBytes(ulong bytes)
        {
            double gb = bytes / (1024.0 * 1024.0 * 1024.0);
            return $"{gb:F2} GB";
        }

        private string FormatGpuMemory(ManagementObject gpu)
        {
            ulong registryBytes = GetGpuDedicatedMemoryFromRegistry(gpu);
            if (registryBytes > 0)
            {
                return FormatBytes(registryBytes);
            }

            ulong adapterBytes = GetUlongValue(gpu, "AdapterRAM");
            if (adapterBytes > 0)
            {
                return FormatBytes(adapterBytes);
            }

            return "Unknown";
        }

        private ulong GetGpuDedicatedMemoryFromRegistry(ManagementObject gpu)
        {
            try
            {
                string pnpDeviceId = GetValue(gpu, "PNPDeviceID");
                if (string.IsNullOrWhiteSpace(pnpDeviceId))
                {
                    return 0;
                }

                using (RegistryKey classRoot = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}"))
                {
                    if (classRoot == null)
                    {
                        return 0;
                    }

                    foreach (string subKeyName in classRoot.GetSubKeyNames())
                    {
                        try
                        {
                            using (RegistryKey subKey = classRoot.OpenSubKey(subKeyName))
                            {
                                if (subKey == null)
                                {
                                    continue;
                                }

                                string driverDesc = subKey.GetValue("DriverDesc") as string;
                                string matchingDeviceId = subKey.GetValue("MatchingDeviceId") as string;
                                if (!RegistryValueMatchesGpu(pnpDeviceId, driverDesc, matchingDeviceId))
                                {
                                    continue;
                                }

                                object memoryValue = subKey.GetValue("HardwareInformation.qwMemorySize");
                                if (memoryValue is long longValue && longValue > 0)
                                {
                                    return (ulong)longValue;
                                }

                                if (memoryValue is byte[] bytes && bytes.Length >= 8)
                                {
                                    return BitConverter.ToUInt64(bytes, 0);
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
            }

            return 0;
        }

        private bool RegistryValueMatchesGpu(string pnpDeviceId, string driverDesc, string matchingDeviceId)
        {
            if (!string.IsNullOrWhiteSpace(driverDesc) && pnpDeviceId.IndexOf(driverDesc, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(matchingDeviceId) && pnpDeviceId.IndexOf(matchingDeviceId, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            string pnpPrefix = pnpDeviceId.Split(new[] { '&' }, 2)[0];
            return !string.IsNullOrWhiteSpace(driverDesc) && driverDesc.IndexOf(pnpPrefix, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static HttpClient CreateCpuSpecClient()
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(15);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AI-Code-Agent-AIO-GMTPC/1.0");
            return client;
        }

        private Button _btnRestartBios;

        private void InitRestartBiosButton()
        {
            _btnRestartBios = new Button
            {
                Content = "Restart to BIOS",
                Width = 120,
                Height = 24,
                Background = Brushes.DarkRed,
                Foreground = Brushes.White,
                Margin = new Thickness(20, 0, 0, 0)
            };
            _btnRestartBios.Click += BtnRestartBios_Click;
        }

        public void SetupRestartBiosButton(StackPanel hardwareHeaderPanel)
        {
            if (_btnRestartBios == null) InitRestartBiosButton();
            hardwareHeaderPanel.Orientation = Orientation.Horizontal;
            hardwareHeaderPanel.Children.Add(_btnRestartBios);
        }

        private void BtnRestartBios_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Restart into BIOS! Yes or No?",
                "Restart to BIOS",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "shutdown",
                        Arguments = "/r /fw /t 0",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    Process.Start(psi);
                }
                catch { }
            }
        }

// -----------------------------------------------------------------------
// SOURCE: MainWindow.TabMultimedia.cs
// -----------------------------------------------------------------------
private async Task InstallPotPlayerAsync()
        {
            try
            {
                UpdateStatus("Đang tải PotPlayer...", "Cyan");
                string potPath = Path.Combine(GetGMTPCFolder(), "PotPlayerSetup64.exe");
                await DownloadWithProgressAsync("https://t1.daumcdn.net/potplayer/PotPlayer/Version/Latest/PotPlayerSetup64.exe", potPath, "PotPlayer");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus("Đang chạy PotPlayer installer (silent)...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = potPath,
                    Arguments = "/S",
                    UseShellExecute = true
                };
                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("PotPlayer đã hoàn tất.", "Green");
                }

                if (File.Exists(potPath)) File.Delete(potPath);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi cài PotPlayer: {ex.Message}", "Red");
            }
        }


        private async Task InstallFastStoneAsync()
        {
            try
            {
                UpdateStatus("Đang tải FastStone Capture...", "Cyan");
                string fsPath = Path.Combine(GetGMTPCFolder(), "FastStone.Capture.exe");
                await DownloadWithProgressAsync("https://github.com/ghostminhtoan/MMT/releases/download/v1.0/FastStone.Capture.exe", fsPath, "FastStone Capture");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus("Đang chạy FastStone Capture installer (silent)...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = fsPath,
                    Arguments = "/silent",
                    UseShellExecute = true
                };
                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("FastStone Capture đã hoàn tất.", "Green");
                }

                if (File.Exists(fsPath)) File.Delete(fsPath);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi cài FastStone Capture: {ex.Message}", "Red");
            }
        }


        private async Task InstallFoxitAsync()
        {
            try
            {
                UpdateStatus("Đang tải Foxit PDF Reader...", "Cyan");
                string foxitPath = Path.Combine(GetGMTPCFolder(), "FoxitPDFReaderSetup.exe");
                await DownloadWithProgressAsync("https://cdn01.foxitsoftware.com/product/reader/desktop/win/2025.2.0/FoxitPDFReader20252_L10N_Setup_Prom_x64.exe", foxitPath, "Foxit PDF Reader");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus("Đang chạy Foxit installer (quiet)...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = foxitPath,
                    Arguments = "/quiet",
                    UseShellExecute = true
                };
                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("Foxit đã cài xong, đang khởi chạy Foxit...", "Green");
                }

                // Run Foxit after install
                string foxitExe = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Foxit Software", "Foxit PDF Reader", "foxitPDFReader.exe");
                try
                {
                    if (File.Exists(foxitExe))
                    {
                        Process.Start(foxitExe);
                        MessageBox.Show("Nếu thấy chữ 'register' thì chọn 'Not now', sau đó ấn 'Next' liên tục để hoàn tất.", "Lưu ý", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        UpdateStatus("Không tìm thấy file foxitPDFReader.exe để khởi chạy sau cài đặt.", "Yellow");
                    }
                }
                catch { }

                if (File.Exists(foxitPath)) File.Delete(foxitPath);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi cài Foxit PDF Reader: {ex.Message}", "Red");
            }
        }


        private async Task InstallBandiviewAsync()
        {
            try
            {
                UpdateStatus("Đang tải Bandiview...", "Cyan");
                string bPath = Path.Combine(GetGMTPCFolder(), "Bandiview.exe");
                await DownloadWithProgressAsync("https://github.com/ghostminhtoan/MMT/releases/download/v1.0/Bandiview.exe", bPath, "Bandiview");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus("Đang chạy Bandiview installer (silent)...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = bPath,
                    Arguments = "/silent",
                    UseShellExecute = true
                };
                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("Bandiview đã hoàn tất.", "Green");
                }

                if (File.Exists(bPath)) File.Delete(bPath);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi cài Bandiview: {ex.Message}", "Red");
            }
        }

        private void ChkPotPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (ChkPotPlayer.IsChecked == true)
            {
                UpdateStatus("Đã chọn: PotPlayer", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: PotPlayer", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkFastStone_Click(object sender, RoutedEventArgs e)
        {
            if (ChkFastStone.IsChecked == true)
            {
                UpdateStatus("Đã chọn: FastStone Capture", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: FastStone Capture", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkFoxit_Click(object sender, RoutedEventArgs e)
        {
            if (ChkFoxit.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Foxit PDF Reader", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Foxit PDF Reader", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkBandiview_Click(object sender, RoutedEventArgs e)
        {
            if (ChkBandiview.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Bandiview (Picture viewer)", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Bandiview (Picture viewer)", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private void ChkAdvancedCodecPack_Click(object sender, RoutedEventArgs e)
        {
            if (ChkAdvancedCodecPack.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Advanced Codec Pack", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Advanced Codec Pack", "Yellow");
            }

            UpdateInstallButtonState();
        }

// -----------------------------------------------------------------------
// SOURCE: MainWindow.TabOffice.cs
// -----------------------------------------------------------------------
/*
         * AI Summary:
         * Date: 2026-04-30
         * - Switched Office Tool Plus to probe latest Office_Tool_with_runtime_*_x64.zip from GitHub Releases, extract to C:\, create desktop shortcut, and open Office Tool Plus.exe
         * Date: 2026-04-17
         * - Added 3 download buttons for Subtitle Edit: Vietnamese Profile, Multiple Replace Template, and Shortcut MMT XML
         * Date: 2026-03-28
         * - Added ChkSubtitleEdit_Click and InstallSubtitleEditAsync
         * Date: 2026-04-22
         * - Replaced Gouenji Fansub Fonts with GMTPC Fonts and delete-on-exit cleanup
         * Date: 2026-03-08
         * - Added ChkNotepadPlusPlus_Click and InstallNotepadPlusPlusAsync
         */

        private async void BtnActivateOffice_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Đã chọn: Tự động kích hoạt Office", "Green");
            await Task.Run(() => ActivateOffice());
        }


        // ===================== Chức năng kích hoạt Office =====================
        private void ActivateOffice()
        {
            try
            {
                UpdateStatus("Đang kích hoạt Office...", "Cyan");
                string activateOfficeCmdPath = Path.Combine(GetGMTPCFolder(), "ACTIVATE.OFFICE.cmd");

                // Tải file ACTIVATE.OFFICE.cmd
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile("https://github.com/ghostminhtoan/MMT/releases/download/activate/ACTIVATE.OFFICE.cmd", activateOfficeCmdPath);
                }
                UpdateStatus("Đã tải file ACTIVATE.OFFICE.cmd", "Cyan");

                // Chạy script với quyền admin
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = activateOfficeCmdPath,
                    UseShellExecute = true,
                    Verb = "runas"
                };

                Process.Start(startInfo);
                UpdateStatus("Đã mở cửa sổ kích hoạt Office", "Green");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi kích hoạt Office: {ex.Message}", "Red");
            }
        }


        private void ChkActivateOffice_Click(object sender, RoutedEventArgs e)
        {
            if (ChkActivateOffice.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Tự động kích hoạt Office", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Tự động kích hoạt Office", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkOfficeToolPlus_Click(object sender, RoutedEventArgs e)
        {
            if (ChkOfficeToolPlus.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Office Tool Plus", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Office Tool Plus", "Yellow");
            }

            UpdateInstallButtonState();
        }


        // Helper methods for installation tasks to be called from BtnInstall_Click
        private async Task InstallOfficeToolPlusAsync()
        {
            try
            {
                string officeToolPlusRootFolder = @"C:\Office Tool Plus";
                string officeToolPlusDownloadFolder = Path.Combine(officeToolPlusRootFolder, "Download");

                if (Directory.Exists(officeToolPlusRootFolder))
                {
                    try
                    {
                        Directory.Delete(officeToolPlusRootFolder, true);
                    }
                    catch (Exception ex)
                    {
                        UpdateStatus($"Không xóa được thư mục Office Tool Plus cũ: {ex.Message}", "Orange");
                    }
                }

                Directory.CreateDirectory(officeToolPlusDownloadFolder);
                Directory.CreateDirectory(officeToolPlusRootFolder);

                UpdateStatus("Đang lấy link Office Tool Plus mới nhất từ GitHub Releases...", "Cyan");
                string officeToolPlusZipUrl = await GetLatestOfficeToolPlusRuntimeZipUrlAsync();
                string officeToolPlusZipName = Path.GetFileName(new Uri(officeToolPlusZipUrl).LocalPath);
                string officeToolPlusZipPath = Path.Combine(officeToolPlusDownloadFolder, officeToolPlusZipName);

                UpdateStatus($"Đang tải {officeToolPlusZipName}...", "Cyan");
                await DownloadWithProgressAsync(officeToolPlusZipUrl, officeToolPlusZipPath, "Office Tool Plus");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus("Đang giải nén Office Tool Plus vào ổ C...", "Cyan");
                ZipFile.ExtractToDirectory(officeToolPlusZipPath, officeToolPlusRootFolder);

                if (File.Exists(officeToolPlusZipPath))
                {
                    File.Delete(officeToolPlusZipPath);
                    UpdateStatus("Đã xóa file zip Office Tool Plus sau khi giải nén", "Cyan");
                }

                string officeToolPlusExePath = FindOfficeToolPlusExePath(officeToolPlusRootFolder);
                if (string.IsNullOrEmpty(officeToolPlusExePath))
                {
                    throw new FileNotFoundException("Không tìm thấy Office Tool Plus.exe trong thư mục đã giải nén.");
                }

                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string shortcutPath = Path.Combine(desktopPath, "Office Tool Plus.lnk");
                if (File.Exists(shortcutPath))
                {
                    File.Delete(shortcutPath);
                }

                try
                {
                    Type shellType = Type.GetTypeFromProgID("WScript.Shell");
                    if (shellType != null)
                    {
                        object shell = Activator.CreateInstance(shellType);
                        object shortcut = shellType.InvokeMember("CreateShortcut", System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { shortcutPath });
                        shellType.InvokeMember("TargetPath", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { officeToolPlusExePath });
                        shellType.InvokeMember("WorkingDirectory", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { Path.GetDirectoryName(officeToolPlusExePath) });
                        shellType.InvokeMember("Description", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { "Office Tool Plus" });
                        shellType.InvokeMember("Save", System.Reflection.BindingFlags.InvokeMethod, null, shortcut, null);
                        UpdateStatus("Đã tạo shortcut Office Tool Plus trên Desktop", "Green");
                    }
                }
                catch (Exception ex)
                {
                    UpdateStatus($"Không thể tạo shortcut Office Tool Plus: {ex.Message}", "Orange");
                }

                UpdateStatus("Đang mở Office Tool Plus...", "Green");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = officeToolPlusExePath,
                    UseShellExecute = true,
                    WorkingDirectory = Path.GetDirectoryName(officeToolPlusExePath)
                };

                Process process = Process.Start(startInfo);
                if (process != null)
                {
                    UpdateStatus("Office Tool Plus đã được mở!", "Green");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi tải hoặc cài đặt Office Tool Plus: {ex.Message}", "Red");
            }
        }

        private async Task<string> GetLatestOfficeToolPlusRuntimeZipUrlAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("AI-Code-Agent-AIO-GMTPC");
                    client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");

                    string json = await client.GetStringAsync(OFFICE_TOOL_PLUS_RELEASES_API_URL);
                    Match match = Regex.Match(
                        json,
                        "\"name\"\\s*:\\s*\"(?<name>Office_Tool_with_runtime_v(?<version>[^\"]+?)_x64\\.zip)\".*?\"browser_download_url\"\\s*:\\s*\"(?<url>[^\"]+)\"",
                        RegexOptions.Singleline);

                    if (match.Success)
                    {
                        string assetName = match.Groups["name"].Value;
                        string version = match.Groups["version"].Value;
                        string downloadUrl = match.Groups["url"].Value.Replace("\\/", "/");
                        UpdateStatus($"Đã tìm thấy Office Tool Plus {version}: {assetName}", "Green");
                        return downloadUrl;
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Không lấy được link Office Tool Plus mới nhất: {ex.Message}", "Yellow");
            }

            throw new InvalidOperationException("Không tìm thấy gói Office Tool Plus x64 mới nhất trên GitHub Releases.");
        }

        private string FindOfficeToolPlusExePath(string searchRoot)
        {
            if (!Directory.Exists(searchRoot))
            {
                return string.Empty;
            }

            string[] matches = Directory.GetFiles(searchRoot, "Office Tool Plus.exe", SearchOption.AllDirectories);
            if (matches != null && matches.Length > 0)
            {
                return matches[0];
            }

            return string.Empty;
        }


        // Thêm phương thức xử lý sự kiện Click cho Office Softmaker
        private void ChkOfficeSoftmaker_Click(object sender, RoutedEventArgs e)
        {
            if (ChkOfficeSoftmaker.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Office Softmaker", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Office Softmaker", "Yellow");
            }

            UpdateInstallButtonState();
        }


        // Thêm phương thức cài đặt Office Softmaker
        private async Task InstallOfficeSoftmakerAsync()
        {
            try
            {
                UpdateStatus("Đang tải Office Softmaker...", "Cyan");
                string officeSoftmakerPath = Path.Combine(GetGMTPCFolder(), "Office.Softmaker.exe");
                await DownloadWithProgressAsync("https://github.com/ghostminhtoan/MMT/releases/download/v1.0/Office.Softmaker.exe", officeSoftmakerPath, "Office Softmaker Installer");

                // Đảm bảo progress bar reset sau khi tải
                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                // Hiển thị popup để hỏi người dùng chọn cài đặt
                MessageBoxResult result = MessageBox.Show("Yes = Cài đặt tự động vào ổ C\nNo = Cài vào ổ khác", "Cài đặt tự động Office Softmaker", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = officeSoftmakerPath,
                    UseShellExecute = true
                };

                if (result == MessageBoxResult.Yes) // Cài đặt tự động vào ổ C
                {
                    startInfo.Arguments = "/passive"; // Sử dụng /passive như yêu cầu
                    UpdateStatus("1 = Cài đặt tự động vào ổ C", "Yellow");
                }
                else if (result == MessageBoxResult.No) // Cài vào ổ khác
                {
                    UpdateStatus("2 = Cài vào ổ khác", "Yellow");
                }
                else // Hủy
                {
                    UpdateStatus("Đã hủy cài đặt Office Softmaker", "Yellow");
                    if (File.Exists(officeSoftmakerPath))
                    {
                        File.Delete(officeSoftmakerPath);
                    }
                    return;
                }

                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());

                    if (process.ExitCode == 0)
                    {
                        UpdateStatus("Cài đặt Office Softmaker thành công!", "Green");
                    }
                    else
                    {
                        UpdateStatus($"Cài đặt Office Softmaker thất bại. Mã lỗi: {process.ExitCode}", "Red");
                    }
                }

                // Xóa file sau khi cài đặt xong
                if (File.Exists(officeSoftmakerPath))
                {
                    File.Delete(officeSoftmakerPath);
                    UpdateStatus("Đã xóa file Office.Softmaker.exe", "Cyan");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi cài đặt Office Softmaker: {ex.Message}", "Red");
            }
        }

        // ===================================================================
        // TabOffice — GMTPC Fonts
        // ===================================================================
        private void ChkGMTPCFonts_Click(object sender, RoutedEventArgs e)
        {
            if (ChkGMTPCFonts.IsChecked == true)
            {
                UpdateStatus("Đã chọn: GMTPC Fonts", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: GMTPC Fonts", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private async Task InstallGMTPCFontsAsync()
        {
            try
            {
                UpdateStatus("Đang tải GMTPC Fonts...", "Cyan");
                string fontsPath = Path.Combine(GetGMTPCFolder(), "GMTPC-FONTS.exe");
                RegisterDownloadedFileForDeleteOnExit(fontsPath);
                await DownloadWithProgressAsync(GMTPC_FONTS_DOWNLOAD_URL, fontsPath, "GMTPC Fonts");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus("Đang chạy GMTPC Fonts...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = fontsPath,
                    UseShellExecute = true
                };
                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    UpdateStatus("Đã chạy GMTPC Fonts. File tải về sẽ được xóa khi tắt app.", "Green");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi chạy GMTPC Fonts: {ex.Message}", "Red");
            }
        }

        // ===================================================================
        // TabOffice — Notepad++
        // ===================================================================
        private void ChkNotepadPlusPlus_Click(object sender, RoutedEventArgs e)
        {
            if (ChkNotepadPlusPlus.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Notepad++", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Notepad++", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private async Task InstallNotepadPlusPlusAsync()
        {
            try
            {
                UpdateStatus("Đang tải Notepad++...", "Cyan");
                string notepadPlusPlusPath = Path.Combine(GetGMTPCFolder(), "npp.8.9.2.Installer.x64.msi");
                await DownloadWithProgressAsync(NOTEPAD_PLUS_PLUS_DOWNLOAD_URL, notepadPlusPlusPath, "Notepad++");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus("Đang cài đặt Notepad++ (passive)...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "msiexec",
                    Arguments = $"/i \"{notepadPlusPlusPath}\" /passive",
                    UseShellExecute = true
                };
                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("Cài đặt Notepad++ hoàn tất!", "Green");
                }

                if (File.Exists(notepadPlusPlusPath))
                {
                    File.Delete(notepadPlusPlusPath);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi cài đặt Notepad++: {ex.Message}", "Red");
            }
        }

        // ===================================================================
        // TabOffice — Subtitle Edit
        // ===================================================================
        private void ChkSubtitleEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ChkSubtitleEdit.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Subtitle Edit", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Subtitle Edit", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private async Task InstallSubtitleEditAsync()
        {
            try
            {
                UpdateStatus("Đang tải Subtitle Edit...", "Cyan");
                string subtitleEditPath = Path.Combine(GetGMTPCFolder(), "Subtitle.Edit.exe");
                await DownloadWithProgressAsync(SUBTITLE_EDIT_DOWNLOAD_URL, subtitleEditPath, "Subtitle Edit");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                // Hiển thị popup để hỏi người dùng chọn cài đặt
                MessageBoxResult result = MessageBox.Show("Yes = Cài đặt tự động (silent)\nNo = Cài đặt thủ công (GUI)", "Cài đặt Subtitle Edit", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Cancel)
                {
                    UpdateStatus("Đã hủy cài đặt Subtitle Edit", "Yellow");
                    if (File.Exists(subtitleEditPath))
                    {
                        File.Delete(subtitleEditPath);
                    }
                    return;
                }

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = subtitleEditPath,
                    UseShellExecute = true
                };

                if (result == MessageBoxResult.Yes)
                {
                    // Cài đặt tự động
                    startInfo.Arguments = SUBTITLE_EDIT_INSTALL_ARGUMENTS;
                    UpdateStatus("Đang cài đặt Subtitle Edit (silent)...", "Yellow");
                }
                else
                {
                    // Cài đặt thủ công
                    UpdateStatus("Đang mở Subtitle Edit installer (thủ công)...", "Yellow");
                }

                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    // Đợi người dùng tắt installer
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("Cài đặt Subtitle Edit hoàn tất!", "Green");
                }

                // Xóa file installer sau khi cài đặt xong
                if (File.Exists(subtitleEditPath))
                {
                    File.Delete(subtitleEditPath);
                    UpdateStatus("Đã xóa file Subtitle.Edit.exe", "Cyan");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi cài đặt Subtitle Edit: {ex.Message}", "Red");
            }
        }

        // ===================================================================
        // Subtitle Edit Download Buttons
        // ===================================================================
        private async void BtnDownloadVietnameseProfile_Click(object sender, RoutedEventArgs e)
        {
            await DownloadSubtitleFileAsync("https://github.com/ghostminhtoan/Subtitle-draft-GMTPC/releases/download/subtitle.materials/vietnamese.profile.profile", "vietnamese.profile.profile");
        }

        private async void BtnDownloadMultipleReplace_Click(object sender, RoutedEventArgs e)
        {
            await DownloadSubtitleFileAsync("https://github.com/ghostminhtoan/Subtitle-draft-GMTPC/releases/download/subtitle.materials/multiple_replace.template", "multiple_replace.template");
        }

        private async void BtnDownloadShortcutMMT_Click(object sender, RoutedEventArgs e)
        {
            await DownloadSubtitleFileAsync("https://github.com/ghostminhtoan/Subtitle-draft-GMTPC/releases/download/subtitle.materials/shortcut.MMT.xml", "shortcut.MMT.xml");
        }

        private async Task DownloadSubtitleFileAsync(string url, string fileName)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string filePath = Path.Combine(desktopPath, fileName);

                UpdateStatus($"Đang tải {fileName} về Desktop...", "Cyan");

                using (WebClient client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(url, filePath);
                }

                UpdateStatus($"{fileName} đã tải về Desktop!", "Green");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi tải {fileName}: {ex.Message}", "Red");
            }
        }

// -----------------------------------------------------------------------
// SOURCE: MainWindow.TabPartition.cs
// -----------------------------------------------------------------------
// Event handler stubs for missing CheckBoxes
        private void ChkDiskGenius_Click(object sender, RoutedEventArgs e)
        {
            if (ChkDiskGenius.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Disk Genius", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Disk Genius", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkAomeiPartitionAssistant_Click(object sender, RoutedEventArgs e)
        {
            if (ChkAomeiPartitionAssistant.IsChecked == true)
            {
                UpdateStatus("Đã chọn: AOMEI Partition Assistant", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: AOMEI Partition Assistant", "Yellow");
            }

            UpdateInstallButtonState();
        }


        // InstallDiskGeniusAsync() -> Moved to MainWindow.SystemArguments.cs
        // (có MessageBox.Show + /s argument)

        // InstallAomeiPartitionAssistantAsync() -> Moved to MainWindow.SystemArguments.cs
        // (có MessageBox.Show + /passive argument)

// -----------------------------------------------------------------------
// SOURCE: MainWindow.TabPopular.cs
// -----------------------------------------------------------------------
/// <summary>
        /// Kiểm tra Windows đang dùng Dark Theme hay Light Theme
        /// Trả về true nếu đang dùng Dark Theme, false nếu đang dùng Light Theme
        /// </summary>
        private bool IsDarkThemeEnabled()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("AppsUseLightTheme");
                        if (value != null)
                        {
                            int appsUseLightTheme = Convert.ToInt32(value);
                            // AppsUseLightTheme = 0: Dark Theme
                            // AppsUseLightTheme = 1: Light Theme
                            return appsUseLightTheme == 0;
                        }
                    }
                }
                return false; // Default là Light Theme
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Bật/tắt Dark Theme và restart Explorer
        /// </summary>
        private async Task ToggleThemeAsync()
        {
            try
            {
                bool isDarkTheme = IsDarkThemeEnabled();
                int newValue = isDarkTheme ? 1 : 0; // Nếu đang Dark thì đổi thành Light (1), ngược lại

                // Ghi registry
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", true))
                {
                    if (key != null)
                    {
                        key.SetValue("AppsUseLightTheme", newValue, RegistryValueKind.DWord);
                        key.SetValue("SystemUsesLightTheme", newValue, RegistryValueKind.DWord);
                    }
                }

                UpdateStatus(isDarkTheme ? "Đang chuyển sang Light Theme..." : "Đang chuyển sang Dark Theme...", "Cyan");
                await Task.Delay(500);

                // Restart Explorer
                UpdateStatus("Đang restart Explorer...", "Yellow");
                
                var explorerProcesses = Process.GetProcessesByName("explorer");
                foreach (var proc in explorerProcesses)
                {
                    try
                    {
                        proc.Kill();
                    }
                    catch { }
                }

                // Start lại Explorer
                await Task.Delay(1000);
                Process.Start("explorer.exe");

                UpdateStatus($"Đã chuyển sang {(isDarkTheme ? "Light" : "Dark")} Theme và restart Explorer", "Green");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi đổi theme: {ex.Message}", "Red");
            }
        }

        /// <summary>
        /// Event handler khi click vào Theme Toggle Button
        /// </summary>
        private async void TglTheme_Click(object sender, RoutedEventArgs e)
        {
            await ToggleThemeAsync();
            
            // Cập nhật lại trạng thái toggle button sau khi đổi theme
            await Task.Delay(1500);
            UpdateThemeToggleButtonState();
        }

        /// <summary>
        /// Cập nhật trạng thái của Theme Toggle Button dựa trên theme hiện tại
        /// </summary>
        private void UpdateThemeToggleButtonState()
        {
            try
            {
                if (TglTheme != null)
                {
                    bool isDarkTheme = IsDarkThemeEnabled();
                    TglTheme.IsChecked = isDarkTheme;
                }
            }
            catch { }
        }

        private void ChkInstallIDM_Click(object sender, RoutedEventArgs e)
        {
            if (ChkInstallIDM.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Internet Download Manager", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Internet Download Manager", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkInstallNeatDM_Click(object sender, RoutedEventArgs e)
        {
            if (ChkInstallNeatDM.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Neat Download Manager", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Neat Download Manager", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkInstallWinRAR_Click(object sender, RoutedEventArgs e)
        {
            if (ChkInstallWinRAR.IsChecked == true)
            {
                UpdateStatus("Đã chọn: WinRAR", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: WinRAR", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkInstallBID_Click(object sender, RoutedEventArgs e)
        {
            if (ChkInstallBID.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Bulk Image Downloader", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Bulk Image Downloader", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkActivateWindows_Click(object sender, RoutedEventArgs e)
        {
            if (ChkActivateWindows.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Tự động kích hoạt Windows", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Tự động kích hoạt Windows", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkPauseWindowsUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (ChkPauseWindowsUpdate.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Pause Windows Update", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Pause Windows Update", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkVcredist_Click(object sender, RoutedEventArgs e)
        {
            if (ChkVcredist.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Vcredist 2005-2022", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Vcredist 2005-2022", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkDirectX_Click(object sender, RoutedEventArgs e)
        {
            if (ChkDirectX.IsChecked == true)
            {
                UpdateStatus("Đã chọn: DirectX", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: DirectX", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkJava_Click(object sender, RoutedEventArgs e)
        {
            if (ChkJava.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Java", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Java", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkOpenAL_Click(object sender, RoutedEventArgs e)
        {
            if (ChkOpenAL.IsChecked == true)
            {
                UpdateStatus("Đã chọn: OpenAL", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: OpenAL", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private void ChkRevoUninstaller_Click(object sender, RoutedEventArgs e)
        {
            if (ChkRevoUninstaller.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Revo Uninstaller", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Revo Uninstaller", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkInstallZalo_Click(object sender, RoutedEventArgs e)
        {
            if (ChkInstallZalo.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Zalo", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Zalo", "Yellow");
            }

            UpdateInstallButtonState();
        }

// -----------------------------------------------------------------------
// SOURCE: MainWindow.TabRemoteDesktop.cs
// -----------------------------------------------------------------------
private void ChkUltraviewer_Click(object sender, RoutedEventArgs e)
        {
            if (ChkUltraviewer.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Ultraviewer", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Ultraviewer", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkTeamViewerQS_Click(object sender, RoutedEventArgs e)
        {
            if (ChkTeamViewerQS.IsChecked == true)
            {
                UpdateStatus("Đã chọn: TeamViewer QuickSupport", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: TeamViewer QuickSupport", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkTeamViewerFull_Click(object sender, RoutedEventArgs e)
        {
            if (ChkTeamViewerFull.IsChecked == true)
            {
                UpdateStatus("Đã chọn: TeamViewer Full", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: TeamViewer Full", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private void ChkAnyDesk_Click(object sender, RoutedEventArgs e)
        {
            if (ChkAnyDesk.IsChecked == true)
            {
                UpdateStatus("Đã chọn: AnyDesk", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: AnyDesk", "Yellow");
            }

            UpdateInstallButtonState();
        }


        private async Task InstallUltraviewerAsync()
        {
            try
            {
                UpdateStatus("Đang tải Ultraviewer...", "Cyan");
                string ultraviewerPath = Path.Combine(GetGMTPCFolder(), "UltraViewer_setup.exe");
                await DownloadWithProgressAsync("https://dl2.ultraviewer.net/UltraViewer_setup_6.6_vi.exe", ultraviewerPath, "Ultraviewer");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus("Đang chạy Ultraviewer...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = ultraviewerPath,
                    Arguments = "/silent",
                    UseShellExecute = true
                };
                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("Ultraviewer đã hoàn tất!", "Green");
                }

                if (File.Exists(ultraviewerPath))
                {
                    File.Delete(ultraviewerPath);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi cài đặt Ultraviewer: {ex.Message}", "Red");
            }
        }


        private async Task InstallTeamViewerQuickSupportAsync()
        {
            try
            {
                UpdateStatus("Đang tải TeamViewer QuickSupport...", "Cyan");
                string tvqsPath = Path.Combine(GetGMTPCFolder(), "TeamViewerQS_x64.exe");
                await DownloadWithProgressAsync("https://dl.teamviewer.com/download/TeamViewerQS_x64.exe", tvqsPath, "TeamViewer QuickSupport");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus("Đang chạy TeamViewer QuickSupport...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = tvqsPath,
                    UseShellExecute = true
                };
                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("TeamViewer QuickSupport đã hoàn tất.", "Green");
                }

                if (File.Exists(tvqsPath)) File.Delete(tvqsPath);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi cài TeamViewer QuickSupport: {ex.Message}", "Red");
            }
        }


        private async Task InstallTeamViewerFullAsync()
        {
            try
            {
                UpdateStatus("Đang tải TeamViewer Full...", "Cyan");
                string tvFullPath = Path.Combine(GetGMTPCFolder(), "TeamViewer_Setup.exe");
                await DownloadWithProgressAsync("https://tinyurl.com/teamviewerlatest", tvFullPath, "TeamViewer Full");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus("Đang chạy TeamViewer Full (silent)...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = tvFullPath,
                    Arguments = "/S /V/qn",
                    UseShellExecute = true
                };
                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("TeamViewer Full đã hoàn tất.", "Green");
                }

                if (File.Exists(tvFullPath)) File.Delete(tvFullPath);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi cài TeamViewer Full: {ex.Message}", "Red");
            }
        }

        private async Task InstallAnyDeskAsync()
        {
            try
            {
                UpdateStatus("Đang tải AnyDesk...", "Cyan");
                string anydeskPath = Path.Combine(GetGMTPCFolder(), "AnyDesk.exe");
                await DownloadWithProgressAsync("https://tinyurl.com/anydesk621", anydeskPath, "AnyDesk");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus("Đang chạy AnyDesk...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = anydeskPath,
                    UseShellExecute = true
                };
                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("AnyDesk đã hoàn tất.", "Green");
                }

                if (File.Exists(anydeskPath)) File.Delete(anydeskPath);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi cài AnyDesk: {ex.Message}", "Red");
            }
        }


        private void ChkVMWare162Lite_Click(object sender, RoutedEventArgs e)
        {
            if (ChkVMWare162Lite.IsChecked == true)
            {
                UpdateStatus("Đã chọn: VMWare 16.2 lite", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: VMWare 16.2 lite", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private async Task InstallVMWare162LiteAsync()
        {
            try
            {
                UpdateStatus("Đang tải VMWare 16.2 lite...", "Cyan");
                string vmwarePath = Path.Combine(GetGMTPCFolder(), "VMware_Workstation_16.2.2_Lite.exe");
                await DownloadWithProgressAsync("https://github.com/ghostminhtoan/MMT/releases/download/v1.0/VMware_Workstation_16.2.2_Lite_Eng_._Rus.exe", vmwarePath, "VMWare 16.2 lite");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus("Đang chạy VMWare 16.2 lite (silent)...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = vmwarePath,
                    Arguments = "",
                    UseShellExecute = true
                };
                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus("VMWare 16.2 lite đã hoàn tất.", "Green");
                }

                if (File.Exists(vmwarePath)) File.Delete(vmwarePath);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi cài VMWare 16.2 lite: {ex.Message}", "Red");
            }
        }

// -----------------------------------------------------------------------
// SOURCE: MainWindow.TabSubtitle.cs
// -----------------------------------------------------------------------
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

// -----------------------------------------------------------------------
// SOURCE: MainWindow.TabSystem.cs
// -----------------------------------------------------------------------
private void ChkTeraCopy_Click(object sender, RoutedEventArgs e)
        {
            if (ChkTeraCopy.IsChecked == true)
            {
                UpdateStatus("Đã chọn: TeraCopy", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: TeraCopy", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private void ChkVPN1111_Click(object sender, RoutedEventArgs e)
        {
            if (ChkVPN1111.IsChecked == true)
            {
                UpdateStatus("Đã chọn: VPN 1111 (Cloudflare)", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: VPN 1111 (Cloudflare)", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private void ChkMemReduct_Click(object sender, RoutedEventArgs e)
        {
            if (ChkMemReduct.IsChecked == true)
            {
                UpdateStatus("Đã chọn: MemReduct", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: MemReduct", "Yellow");
            }

            UpdateInstallButtonState();
        }

// -----------------------------------------------------------------------
// SOURCE: MainWindow.TabTool.cs
// -----------------------------------------------------------------------
private IEnumerable<CheckBox> GetToolTabCheckBoxes()
        {
            yield return ChkInstallIDM;
            yield return ChkInstallNeatDM;
            yield return ChkVcredist;
            yield return ChkNotepadPlusPlus;
            yield return ChkFastStone;
            yield return ChkComfortClipboardPro;
            yield return ChkPowerISO;
            yield return ChkMemReduct;
            yield return ChkTeraCopy;
            yield return ChkVPN1111;
            yield return ChkChrome;
            yield return ChkCocCoc;
            yield return ChkVMWare162Lite;
        }

        private void SetToolTabCheckBoxes(bool isChecked)
        {
            foreach (var checkBox in GetToolTabCheckBoxes())
            {
                if (checkBox != null)
                {
                    checkBox.IsChecked = isChecked;
                }
            }
        }

// -----------------------------------------------------------------------
// SOURCE: MainWindow.TabWindows.cs
// -----------------------------------------------------------------------


// -----------------------------------------------------------------------
// SOURCE: MainWindow.TabWindowsMicrosoft.cs
// -----------------------------------------------------------------------
// ===================================================================
        // TabWindowsMicrosoft - Checkbox Click Handlers
        // TabItem Header: "Windows - Microsoft"
        // Checkboxes: ChkWin11_26H1
        // ===================================================================
        private void ChkWin11_26H1_Click(object sender, RoutedEventArgs e)
        {
            if (ChkWin11_26H1.IsChecked == true)
            {
                UpdateStatus("ÄÃ£ chá»n: Win 11 - 26H1 - 2026 Feb - server archive.org", "Green");
            }
            else
            {
                UpdateStatus("ÄÃ£ há»§y chá»n: Win 11 - 26H1 - 2026 Feb - server archive.org", "Yellow");
            }

            UpdateInstallButtonState();
        }

        // ===================================================================
        // TabWindowsMicrosoft - Install Methods
        // ===================================================================
        private async Task InstallWin11_26H1Async()
        {
            try
            {
                UpdateStatus("Äang táº£i Win 11 - 26H1 - 2026 Feb...", "Cyan");
                string win11Path = Path.Combine(GetGMTPCFolder(), "Win11_26H1.iso");
                await DownloadWithProgressAsync("https://archive.org/download/microsoft-win11-26h2-february-2026/en-us_windows_11_consumer_editions_version_26h1_x64_dvd_5208fe5b.iso", win11Path, "Win 11 26H1");

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus("Táº£i Win 11 26H1 hoÃ n táº¥t! File ISO Ä‘Ã£ Ä‘Æ°á»£c lÆ°u táº¡i: " + win11Path, "Green");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lá»—i khi táº£i Win 11 26H1: {ex.Message}", "Red");
            }
        }
    }
}
