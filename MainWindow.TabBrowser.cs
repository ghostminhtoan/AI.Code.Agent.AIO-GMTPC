// AI Summary: 2026-04-25 - Added Brave browser install handler and Browser-tab selection/status wiring; removed Edge workspace button hook
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

    }
}

