// =======================================================================
// MainWindow.SystemInstallMultipart.cs
// Chá»©c nÄƒng: CÆ¡ cháº¿ cÃ i Ä‘áº·t Multi-part - táº£i nhiá»u split file, tá»± Ä‘á»™ng ghÃ©p vÃ  xÃ³a
// Cáº­p nháº­t gáº§n Ä‘Ã¢y:
//   - 2026-04-16: Cáº­p nháº­t UpdateStatus hiá»ƒn thá»‹ "Ä‘ang táº£i x/y parts"
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
        // ===================== SystemInstallMultipart - CÃ i Ä‘áº·t Multi-part =====================
        /// <summary>
        /// CÆ¡ cháº¿ cÃ i Ä‘áº·t Multi-part: Táº£i nhiá»u split file (.001, .002, .003...)
        /// Sau khi táº£i xong Báº®T BUá»˜C Tá»° Äá»˜NG Gá»˜P Láº I thÃ nh 1 file
        /// Sau khi Gá»˜P XONG THÃŒ XÃ“A TOÃ€N Bá»˜ SPLIT FILE
        /// </summary>
        /// <param name="downloadUrls">Máº£ng cÃ¡c link táº£i vá» (theo thá»© tá»± .001, .002, .003...)</param>
        /// <param name="outputFilePath">ÄÆ°á»ng dáº«n file hoÃ n chá»‰nh sau khi ghÃ©p</param>
        /// <param name="displayName">TÃªn hiá»ƒn thá»‹ (vÃ­ dá»¥: "Ghost of Tsushima", "Win 10 ISO")</param>
        /// <param name="runAfterMerge">CÃ³ cháº¡y file sau khi ghÃ©p khÃ´ng (default: true)</param>
        /// <param name="deleteAfterMerge">CÃ³ xÃ³a split file sau khi ghÃ©p khÃ´ng (default: true - Báº®T BUá»˜C)</param>
        protected async Task InstallWithMultipartAsync(string[] downloadUrls, string outputFilePath, string displayName, bool runAfterMerge = true, bool deleteAfterMerge = true)
        {
            if (downloadUrls == null || downloadUrls.Length == 0)
            {
                UpdateStatus($"Lá»—i: KhÃ´ng cÃ³ link táº£i cho {displayName}", "Red");
                return;
            }

            try
            {
                // Step 1: Download all parts
                string[] partPaths = new string[downloadUrls.Length];
                string tempFolder = Path.Combine(GetGMTPCFolder(), $"{displayName}_Temp_{DateTime.Now:yyyyMMdd_HHmmss}");
                Directory.CreateDirectory(tempFolder);

                for (int i = 0; i < downloadUrls.Length; i++)
                {
                    string partFileName = Path.GetFileName(downloadUrls[i]);
                    partPaths[i] = Path.Combine(tempFolder, partFileName);
                    
                    string partInfo = $"Part {i + 1}/{downloadUrls.Length} - {displayName}";
                    Dispatcher.Invoke(() => PartInfoTextBlock.Text = partInfo);
                    await DownloadWithProgressAsync(downloadUrls[i], partPaths[i], partInfo);
                    
                    // Reset progress bar between parts
                    Dispatcher.Invoke(() =>
                    {
                        DownloadProgressBar.Value = 0;
                        ProgressTextBlock.Text = "";
                    });
                }

                // Step 2: Merge all parts (Báº®T BUá»˜C)
                UpdateStatus($"Táº£i xong {downloadUrls.Length} pháº§n! Äang gá»™p file...", "Cyan");
                await MergeSplitFilesAsync(partPaths, outputFilePath);

                // Step 3: Delete all split files (Báº®T BUá»˜C)
                if (deleteAfterMerge)
                {
                    UpdateStatus("Äang xÃ³a cÃ¡c file split...", "Gray");
                    foreach (string partPath in partPaths)
                    {
                        if (File.Exists(partPath))
                        {
                            try
                            {
                                File.Delete(partPath);
                                UpdateStatus($"ÄÃ£ xÃ³a {Path.GetFileName(partPath)}", "Gray");
                            }
                            catch { /* Ignore delete errors */ }
                        }
                    }
                    
                    // Delete temp folder
                    try
                    {
                        if (Directory.Exists(tempFolder))
                        {
                            Directory.Delete(tempFolder, true);
                            UpdateStatus($"ÄÃ£ xÃ³a folder táº¡m {tempFolder}", "Gray");
                        }
                    }
                    catch { /* Ignore delete errors */ }
                    
                    UpdateStatus($"ÄÃ£ xÃ³a toÃ n bá»™ {downloadUrls.Length} file split!", "Green");
                }

                // Step 4: Run file after merge (if requested)
                if (runAfterMerge && File.Exists(outputFilePath))
                {
                    UpdateStatus($"Äang má»Ÿ {displayName}...", "Green");
                    Process.Start(new ProcessStartInfo 
                    { 
                        FileName = outputFilePath, 
                        UseShellExecute = true 
                    });
                }

                UpdateStatus($"HoÃ n táº¥t cÃ i Ä‘áº·t {displayName}!", "Green");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lá»—i khi cÃ i {displayName}: {ex.Message}", "Red");
                throw;
            }
        }

        /// <summary>
        /// Gá»™p nhiá»u split files thÃ nh 1 file hoÃ n chá»‰nh
        /// </summary>
        protected async Task MergeSplitFilesAsync(string[] partPaths, string outputPath)
        {
            using (var outputFs = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                foreach (string partPath in partPaths)
                {
                    using (var inputFs = new FileStream(partPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await inputFs.CopyToAsync(outputFs);
                    }
                }
            }
            UpdateStatus($"Gá»™p file thÃ nh cÃ´ng: {Path.GetFileName(outputPath)}", "Green");
        }

        /// <summary>
        /// CÆ¡ cháº¿ cÃ i Ä‘áº·t Multi-part vá»›i retry logic
        /// </summary>
        protected async Task InstallWithMultipartAndRetryAsync(string[] downloadUrls, string outputFilePath, string displayName, int maxRetries = 3)
        {
            int retryCount = 0;
            while (retryCount < maxRetries)
            {
                try
                {
                    await InstallWithMultipartAsync(downloadUrls, outputFilePath, displayName);
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

