// AI Summary: 2026-05-02 - Moved the archive download/extract/launch/cleanup flow into a dedicated reusable system file.
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace AICodeAgentAIOGMTPC
{
    public partial class MainWindow
    {
        private const string VENTOY_EXTRACT_ROOT = @"C:\Ventoy";
        private const string VENTOY_FINAL_ROOT = @"C:\";
        private const string VENTOY_GITHUB_RELEASES_API_URL = "https://api.github.com/repos/ventoy/Ventoy/releases/latest";

        private async Task InstallVentoyAsync()
        {
            try
            {
                Directory.CreateDirectory(VENTOY_EXTRACT_ROOT);

                UpdateStatus("Äang probe Ventoy release má»›i nháº¥t trÃªn GitHub...", "Cyan");
                Tuple<string, string, string> ventoyReleaseInfo = await GetLatestVentoyReleaseAssetAsync();
                if (ventoyReleaseInfo == null || string.IsNullOrEmpty(ventoyReleaseInfo.Item1) || string.IsNullOrEmpty(ventoyReleaseInfo.Item2))
                {
                    throw new InvalidOperationException("KhÃ´ng tÃ¬m tháº¥y Ventoy release má»›i nháº¥t.");
                }

                string latestVentoyTag = ventoyReleaseInfo.Item1;
                string ventoyZipDownloadUrl = ventoyReleaseInfo.Item2;
                string zipFileName = ventoyReleaseInfo.Item3;
                string latestVersionName = latestVentoyTag;
                string latestVersionFolderName = latestVentoyTag.TrimStart('v');
                UpdateStatus($"ÄÃ£ chá»n Ventoy {latestVersionName}", "Green");
                UpdateStatus($"ÄÃ£ tÃ¬m tháº¥y file: {zipFileName}", "Cyan");

                string versionFolderPath = Path.Combine(VENTOY_EXTRACT_ROOT, latestVersionFolderName);
                string zipPath = Path.Combine(VENTOY_EXTRACT_ROOT, zipFileName);
                string finalVentoyFolderName = string.Empty;
                string finalVentoyFolderPath = string.Empty;

                if (Directory.Exists(versionFolderPath))
                {
                    try
                    {
                        Directory.Delete(versionFolderPath, true);
                    }
                    catch (Exception ex)
                    {
                        UpdateStatus($"KhÃ´ng xÃ³a Ä‘Æ°á»£c folder Ventoy cÅ©: {ex.Message}", "Orange");
                    }
                }

                UpdateStatus("Äang táº£i Ventoy windows.zip vÃ o C:\\Ventoy...", "Cyan");
                await DownloadWithProgressAsync(ventoyZipDownloadUrl, zipPath, "Ventoy");

                UpdateStatus("Äang giáº£i nÃ©n Ventoy...", "Cyan");
                Directory.CreateDirectory(versionFolderPath);
                ZipFile.ExtractToDirectory(zipPath, versionFolderPath);

                string ventoySourceFolderPath = FindVentoyPayloadFolder(versionFolderPath);
                if (string.IsNullOrEmpty(ventoySourceFolderPath))
                {
                    throw new InvalidOperationException("KhÃ´ng tÃ¬m tháº¥y folder Ventoy sau khi giáº£i nÃ©n.");
                }

                finalVentoyFolderName = Path.GetFileName(ventoySourceFolderPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                finalVentoyFolderPath = Path.Combine(VENTOY_FINAL_ROOT, finalVentoyFolderName);

                if (Directory.Exists(finalVentoyFolderPath))
                {
                    try
                    {
                        Directory.Delete(finalVentoyFolderPath, true);
                    }
                    catch (Exception ex)
                    {
                        UpdateStatus($"KhÃ´ng xÃ³a Ä‘Æ°á»£c folder Ventoy cÅ© á»Ÿ á»• C: {ex.Message}", "Orange");
                    }
                }

                UpdateStatus($"Äang chuyá»ƒn folder Ventoy ra á»• C:\\{finalVentoyFolderName}...", "Cyan");
                Directory.Move(ventoySourceFolderPath, finalVentoyFolderPath);

                string ventoyExePath = FindVentoy2DiskExe(finalVentoyFolderPath);
                if (string.IsNullOrEmpty(ventoyExePath))
                {
                    throw new InvalidOperationException("KhÃ´ng tÃ¬m tháº¥y ventoy2disk.exe sau khi chuyá»ƒn folder.");
                }

                UpdateStatus("Äang má»Ÿ Ventoy2Disk vá»›i quyá»n administrator...", "Cyan");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = ventoyExePath,
                    WorkingDirectory = Path.GetDirectoryName(ventoyExePath),
                    UseShellExecute = true,
                    Verb = "runas"
                };

                Process process = Process.Start(startInfo);
                if (process != null)
                {
                    UpdateStatus("Ventoy2Disk Ä‘Ã£ Ä‘Æ°á»£c má»Ÿ!", "Green");
                    UpdateStatus("Äá»£i Ventoy2Disk táº¯t Ä‘á»ƒ dá»n file táº¡m...", "Cyan");
                    await WaitForProcessExitAsync(process);
                }

                UpdateStatus("Äang xÃ³a file zip Ventoy...", "Cyan");
                TryDeleteFile(zipPath);

                UpdateStatus("Äang dá»n folder táº¡m Ventoy...", "Cyan");
                TryDeleteDirectory(versionFolderPath);

                UpdateStatus("Äang xÃ³a C:\\Ventoy...", "Cyan");
                TryDeleteDirectory(VENTOY_EXTRACT_ROOT);
            }
            catch (System.ComponentModel.Win32Exception ex) when (ex.NativeErrorCode == 1223)
            {
                UpdateStatus("ÄÃ£ há»§y má»Ÿ Ventoy2Disk.", "Yellow");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lá»—i khi cÃ i Ä‘áº·t Ventoy: {ex.Message}", "Red");
            }
        }

        private async Task<Tuple<string, string, string>> GetLatestVentoyReleaseAssetAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("AI-Code-Agent-AIO-GMTPC");
                client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
                string json = await client.GetStringAsync(VENTOY_GITHUB_RELEASES_API_URL);

                Match tagMatch = Regex.Match(json ?? string.Empty, @"""tag_name""\s*:\s*""(?<tag>v\d+\.\d+\.\d+)""", RegexOptions.IgnoreCase);
                if (!tagMatch.Success)
                {
                    return null;
                }

                Match assetMatch = Regex.Match(
                    json ?? string.Empty,
                    @"""name""\s*:\s*""(?<name>ventoy-(?<ver>\d+\.\d+\.\d+)-windows\.zip)""[\s\S]*?""browser_download_url""\s*:\s*""(?<url>https:\/\/github\.com\/ventoy\/Ventoy\/releases\/download\/[^""]+)""",
                    RegexOptions.IgnoreCase);
                if (!assetMatch.Success)
                {
                    return null;
                }

                return Tuple.Create(tagMatch.Groups["tag"].Value, assetMatch.Groups["url"].Value, assetMatch.Groups["name"].Value);
            }
        }

        private static Task WaitForProcessExitAsync(Process process)
        {
            if (process == null)
            {
                return Task.CompletedTask;
            }

            if (process.HasExited)
            {
                return Task.CompletedTask;
            }

            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => tcs.TrySetResult(null);

            if (process.HasExited)
            {
                tcs.TrySetResult(null);
            }

            return tcs.Task;
        }

        private static string FindVentoyPayloadFolder(string versionFolderPath)
        {
            if (string.IsNullOrEmpty(versionFolderPath) || !Directory.Exists(versionFolderPath))
            {
                return null;
            }

            string[] ventoyFolders = Directory.GetDirectories(versionFolderPath, "*", SearchOption.TopDirectoryOnly);
            if (ventoyFolders != null && ventoyFolders.Length > 0)
            {
                foreach (string folder in ventoyFolders)
                {
                    string folderName = Path.GetFileName(folder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                    if (Regex.IsMatch(folderName ?? string.Empty, @"^ventoy-\d+\.\d+\.\d+$", RegexOptions.IgnoreCase))
                    {
                        return folder;
                    }
                }
            }

            string[] subDirectories = Directory.GetDirectories(versionFolderPath, "*", SearchOption.TopDirectoryOnly);
            if (subDirectories != null && subDirectories.Length == 1)
            {
                return subDirectories[0];
            }

            if (File.Exists(Path.Combine(versionFolderPath, "ventoy2disk.exe")) || File.Exists(Path.Combine(versionFolderPath, "Ventoy2Disk.exe")))
            {
                return versionFolderPath;
            }

            return null;
        }

        private static string FindVentoy2DiskExe(string rootFolder)
        {
            if (string.IsNullOrEmpty(rootFolder) || !Directory.Exists(rootFolder))
            {
                return null;
            }

            string[] candidates = Directory.GetFiles(rootFolder, "ventoy2disk.exe", SearchOption.AllDirectories);
            if (candidates != null && candidates.Length > 0)
            {
                return candidates[0];
            }

            candidates = Directory.GetFiles(rootFolder, "Ventoy2Disk.exe", SearchOption.AllDirectories);
            if (candidates != null && candidates.Length > 0)
            {
                return candidates[0];
            }

            return null;
        }

        private static void TryDeleteFile(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch
                {
                }
            }
        }

        private static void TryDeleteDirectory(string path)
        {
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path, true);
                }
                catch
                {
                }
            }
        }
    }
}

