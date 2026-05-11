// =======================================================================
// MainWindow.TabDriver.cs
// Chá»©c nÄƒng: Xá»­ lÃ½ checkbox vÃ  cÃ i Ä‘áº·t cho Tab Driver
// Cáº­p nháº­t: 2026-03-10 - Táº¡o file má»›i cho Tab Driver vá»›i 3DP Chip vÃ  3DP Net
// =======================================================================
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AICodeAgentAIOGMTPC
{
    public partial class MainWindow
    {
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
    }
}

