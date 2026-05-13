// =======================================================================
// MainWindow.TabCode.cs
// Tab "Vibe coding" - Visual Studio Code, Visual Studio 2026, Git, and Node.js checkboxes
// =======================================================================
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AICodeAgentAIOGMTPC
{
    public partial class MainWindow
    {
        // ===================================================================
        // TabCode - Checkbox Click Handlers
        // TabItem Header: "Vibe coding"
        // ===================================================================
        private void ChkVisualStudioCode_Click(object sender, RoutedEventArgs e)
        {
            if (ChkVisualStudioCode.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Visual Studio Code", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Visual Studio Code", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private void ChkVisualStudio2026_Click(object sender, RoutedEventArgs e)
        {
            if (ChkVisualStudio2026.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Visual Studio 2026", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Visual Studio 2026", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private void ChkGit_Click(object sender, RoutedEventArgs e)
        {
            if (ChkGit.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Git", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Git", "Yellow");
            }

            UpdateInstallButtonState();
        }

        private void ChkNodeJS_Click(object sender, RoutedEventArgs e)
        {
            if (ChkNodeJS.IsChecked == true)
            {
                UpdateStatus("Đã chọn: Node.js", "Green");
            }
            else
            {
                UpdateStatus("Đã hủy chọn: Node.js", "Yellow");
            }

            UpdateInstallButtonState();
        }

        // ===================================================================
        // TabCode - Install Methods
        // ===================================================================
        private async Task InstallVisualStudioCodeAsync()
        {
            await InstallDownloadedSetupAsync(
                displayName: "Visual Studio Code",
                downloadUrl: VISUAL_STUDIO_CODE_DOWNLOAD_URL,
                localFileName: "VisualStudioCodeSetup.exe",
                arguments: VISUAL_STUDIO_CODE_INSTALL_ARGUMENTS,
                runningMessage: "Đang chạy Visual Studio Code installer (silent)...",
                successMessage: "Cài đặt Visual Studio Code hoàn tất!"
            );
        }

        private async Task InstallVisualStudio2026Async()
        {
            await InstallDownloadedSetupAsync(
                displayName: "Visual Studio 2026",
                downloadUrl: VISUAL_STUDIO_2026_DOWNLOAD_URL,
                localFileName: "VisualStudio2026Community.exe",
                arguments: VISUAL_STUDIO_2026_INSTALL_ARGUMENTS,
                runningMessage: "Đang chạy Visual Studio 2026 installer...",
                successMessage: "Cài đặt Visual Studio 2026 hoàn tất!"
            );
        }

        private async Task InstallGitAsync()
        {
            await InstallDownloadedSetupAsync(
                displayName: "Git",
                downloadUrl: GIT_DOWNLOAD_URL,
                localFileName: "Git-2.54.0-64-bit.exe",
                arguments: GIT_INSTALL_ARGUMENTS,
                runningMessage: "Đang chạy Git installer (/silent)...",
                successMessage: "Cài đặt Git hoàn tất!"
            );
        }

        private async Task InstallNodeJSAsync()
        {
            await InstallDownloadedSetupAsync(
                displayName: "Node.js",
                downloadUrl: NODEJS_DOWNLOAD_URL,
                localFileName: "node-v25.8.0-x64.msi",
                arguments: NODEJS_INSTALL_ARGUMENTS,
                runningMessage: "Đang chạy Node.js installer (/passive)...",
                successMessage: "Cài đặt Node.js hoàn tất!"
            );
        }

        private async Task InstallDownloadedSetupAsync(
            string displayName,
            string downloadUrl,
            string localFileName,
            string arguments,
            string runningMessage,
            string successMessage)
        {
            string installerPath = Path.Combine(GetGMTPCFolder(), localFileName);

            try
            {
                UpdateStatus($"Đang tải {displayName}...", "Cyan");
                await DownloadWithProgressAsync(downloadUrl, installerPath, displayName);

                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                UpdateStatus(runningMessage, "Yellow");

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = installerPath,
                    UseShellExecute = true
                };

                if (!string.IsNullOrWhiteSpace(arguments))
                {
                    startInfo.Arguments = arguments;
                }

                Process process = Process.Start(startInfo);
                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus(successMessage, "Green");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi khi cài đặt {displayName}: {ex.Message}", "Red");
            }
            finally
            {
                if (File.Exists(installerPath))
                {
                    try
                    {
                        File.Delete(installerPath);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
