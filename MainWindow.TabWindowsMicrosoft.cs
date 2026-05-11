// AI Summary: 2026-04-30 - Fixed Windows Microsoft tab Vietnamese text encoding and preserved Win11 26H1 download flow
// =======================================================================
// MainWindow.TabWindowsMicrosoft.cs
// Chá»©c nÄƒng: Xá»­ lÃ½ checkbox vÃ  cÃ i Ä‘áº·t cho Tab Windows - Microsoft
// Cáº­p nháº­t: 2026-03-10 - XÃ³a Win 10 20H2 2022 April - onedrive
// =======================================================================
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AICodeAgentAIOGMTPC
{
    public partial class MainWindow
    {
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

