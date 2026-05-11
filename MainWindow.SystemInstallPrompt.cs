// =======================================================================
// MainWindow.SystemInstallPrompt.cs
// Chá»©c nÄƒng: CÆ¡ cháº¿ cÃ i Ä‘áº·t cÃ³ há»™p thoáº¡i Yes/No - chá»n cÃ i tá»± Ä‘á»™ng hoáº·c má»Ÿ bÃ¬nh thÆ°á»ng
// Cáº­p nháº­t gáº§n Ä‘Ã¢y:
//   - 2026-03-17: Táº¡o má»›i theo yÃªu cáº§u phÃ¢n loáº¡i cÆ¡ cháº¿ cÃ i Ä‘áº·t (giá»‘ng MMT Apps)
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
        // ===================== SystemInstallPrompt - CÃ i Ä‘áº·t cÃ³ há»™p thoáº¡i Yes/No =====================
        /// <summary>
        /// CÆ¡ cháº¿ cÃ i Ä‘áº·t cÃ³ há»™p thoáº¡i Yes/No
        /// Sau khi táº£i xong, há»i ngÆ°á»i dÃ¹ng cÃ³ muá»‘n cÃ i tá»± Ä‘á»™ng khÃ´ng
        /// Chá»n Yes: cháº¡y vá»›i lá»‡nh /s Ä‘á»ƒ cÃ i tá»± Ä‘á»™ng
        /// Chá»n No: má»Ÿ file bÃ¬nh thÆ°á»ng
        /// </summary>
        /// <param name="downloadUrl">Link táº£i vá»</param>
        /// <param name="filePath">ÄÆ°á»ng dáº«n lÆ°u file</param>
        /// <param name="silentArguments">Tham sá»‘ cÃ i tá»± Ä‘á»™ng (vÃ­ dá»¥: /s, /silent)</param>
        /// <param name="displayName">TÃªn hiá»ƒn thá»‹ (vÃ­ dá»¥: "MMT Apps", "Office Tool")</param>
        protected async Task InstallWithPromptAsync(string downloadUrl, string filePath, string silentArguments, string displayName)
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

                // Step 2: Show Yes/No dialog
                UpdateStatus($"Äang chá» ngÆ°á»i dÃ¹ng chá»n cÃ¡ch cÃ i Ä‘áº·t {displayName}...", "Yellow");
                
                bool installSilently = false;
                Dispatcher.Invoke(() =>
                {
                    MessageBoxResult result = MessageBox.Show(
                        $"Báº¡n cÃ³ muá»‘n cÃ i Ä‘áº·t {displayName} tá»± Ä‘á»™ng khÃ´ng?\n\n" +
                        $"Yes: CÃ i tá»± Ä‘á»™ng (silent mode)\n" +
                        $"No: Má»Ÿ file Ä‘á»ƒ cÃ i thá»§ cÃ´ng",
                        $"CÃ i Ä‘áº·t {displayName}",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);
                    
                    installSilently = (result == MessageBoxResult.Yes);
                });

                // Step 3: Run installer based on user choice
                if (installSilently)
                {
                    // User chose Yes - install silently
                    UpdateStatus($"Äang cÃ i Ä‘áº·t {displayName} tá»± Ä‘á»™ng ( {silentArguments} )...", "Yellow");
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = filePath,
                        Arguments = silentArguments,
                        UseShellExecute = true
                    };
                    Process process = Process.Start(startInfo);
                    if (process != null)
                    {
                        await Task.Run(() => process.WaitForExit());
                        UpdateStatus($"CÃ i Ä‘áº·t {displayName} tá»± Ä‘á»™ng hoÃ n táº¥t.", "Green");
                    }
                }
                else
                {
                    // User chose No - open file normally
                    UpdateStatus($"Äang má»Ÿ {displayName}...", "Yellow");
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    };
                    Process.Start(startInfo);
                    UpdateStatus($"ÄÃ£ má»Ÿ {displayName}. Vui lÃ²ng cÃ i thá»§ cÃ´ng.", "Green");
                }

                // Step 4: Delete installer (only if installed silently)
                if (installSilently && File.Exists(filePath))
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
        /// CÆ¡ cháº¿ cÃ i Ä‘áº·t cÃ³ há»™p thoáº¡i Yes/No vá»›i retry logic
        /// </summary>
        protected async Task InstallWithPromptAndRetryAsync(string downloadUrl, string filePath, string silentArguments, string displayName, int maxRetries = 3)
        {
            int retryCount = 0;
            while (retryCount < maxRetries)
            {
                try
                {
                    await InstallWithPromptAsync(downloadUrl, filePath, silentArguments, displayName);
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

