// =======================================================================
// MainWindow.SystemInstallDefault.cs
// Chá»©c nÄƒng: CÆ¡ cháº¿ cÃ i Ä‘áº·t cÆ¡ báº£n - táº£i file vÃ  cháº¡y vá»›i argument
// Cáº­p nháº­t gáº§n Ä‘Ã¢y:
//   - 2026-03-17: Táº¡o má»›i theo yÃªu cáº§u phÃ¢n loáº¡i cÆ¡ cháº¿ cÃ i Ä‘áº·t
// =======================================================================
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace AICodeAgentAIOGMTPC
{
    public partial class MainWindow
    {
        // ===================== SystemInstallDefault - CÃ i Ä‘áº·t cÆ¡ báº£n =====================
        /// <summary>
        /// CÆ¡ cháº¿ cÃ i Ä‘áº·t cÆ¡ báº£n: Táº£i file vÃ  cháº¡y vá»›i argument
        /// Sá»­ dá»¥ng cho cÃ¡c checkbox cÃ i Ä‘áº·t pháº§n má»m thÃ´ng thÆ°á»ng
        /// </summary>
        /// <param name="downloadUrl">Link táº£i vá»</param>
        /// <param name="filePath">ÄÆ°á»ng dáº«n lÆ°u file</param>
        /// <param name="installArguments">Tham sá»‘ cÃ i Ä‘áº·t (vÃ­ dá»¥: /s, /silent, /quiet)</param>
        /// <param name="displayName">TÃªn hiá»ƒn thá»‹ (vÃ­ dá»¥: "IDM", "WinRAR")</param>
        protected async Task InstallWithDefaultAsync(string downloadUrl, string filePath, string installArguments, string displayName)
        {
            try
            {
                // Step 1: Download file
                UpdateStatus($"Äang táº£i {displayName}...", "Cyan");
                await DownloadWithProgressAsync(downloadUrl, filePath, displayName);

                // Reset progress bar
                Dispatcher.Invoke(() =>
                {
                    DownloadProgressBar.Value = 0;
                    ProgressTextBlock.Text = "";
                    SpeedTextBlock.Text = "";
                });

                // Step 2: Run installer with arguments
                UpdateStatus($"Äang cÃ i Ä‘áº·t {displayName} ( {installArguments} )...", "Yellow");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    Arguments = installArguments,
                    UseShellExecute = true
                };
                Process process = Process.Start(startInfo);
                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                    UpdateStatus($"CÃ i Ä‘áº·t {displayName} hoÃ n táº¥t.", "Green");
                }

                // Step 3: Delete installer
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    UpdateStatus($"ÄÃ£ xÃ³a file cÃ i Ä‘áº·t {displayName}", "Cyan");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lá»—i khi cÃ i {displayName}: {ex.Message}", "Red");
            }
        }

        /// <summary>
        /// CÆ¡ cháº¿ cÃ i Ä‘áº·t cÆ¡ báº£n vá»›i retry logic
        /// </summary>
        protected async Task InstallWithDefaultAndRetryAsync(string downloadUrl, string filePath, string installArguments, string displayName, int maxRetries = 3)
        {
            int retryCount = 0;
            while (retryCount < maxRetries)
            {
                try
                {
                    await InstallWithDefaultAsync(downloadUrl, filePath, installArguments, displayName);
                    return; // Success
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount >= maxRetries)
                    {
                        UpdateStatus($"Lá»—i sau {maxRetries} láº§n thá»­: {ex.Message}", "Red");
                        throw;
                    }
                    UpdateStatus($"Thá»­ láº¡i láº§n {retryCount}/{maxRetries}...", "Yellow");
                    await Task.Delay(2000); // Wait 2 seconds before retry
                }
            }
        }
    }
}

