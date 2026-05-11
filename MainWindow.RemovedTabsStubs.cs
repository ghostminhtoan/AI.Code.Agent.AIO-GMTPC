// =======================================================================
// MainWindow.RemovedTabsStubs.cs
// AI Summary: 2026-05-11 - Added stub controls for removed tabs and panels
//             so code-behind can keep compiling after the UI was consolidated
//             into the Tool tab
// =======================================================================
using System.Windows.Controls;

namespace AICodeAgentAIOGMTPC
{
    public partial class MainWindow
    {
        // Removed tabs that are still referenced by shared layout / button logic.
        private readonly WrapPanel CheckBoxPanel = new WrapPanel();
        private readonly WrapPanel OfficePanel = new WrapPanel();
        private readonly WrapPanel SubtitlePanel = new WrapPanel();
        private readonly WrapPanel MultimediaPanel = new WrapPanel();
        private readonly WrapPanel SystemPanel = new WrapPanel();
        private readonly WrapPanel PartitionPanel = new WrapPanel();
        private readonly WrapPanel GamingPanel = new WrapPanel();
        private readonly WrapPanel DriverPanel = new WrapPanel();
        private readonly WrapPanel BrowserPanel = new WrapPanel();
        private readonly WrapPanel RemoteDesktopPanel = new WrapPanel();
        private readonly WrapPanel WindowsPanel = new WrapPanel();
        private readonly WrapPanel WindowsToolsPanel = new WrapPanel();
        private readonly WrapPanel WindowsSetupPanel = new WrapPanel();

        // System Information header / text fields.
        private readonly StackPanel HardwareHeaderPanel = new StackPanel();
        private readonly TextBlock TbGPU = new TextBlock();
        private readonly TextBlock TbRAM = new TextBlock();
        private readonly TextBlock TbMainboard = new TextBlock();
        private readonly TextBlock TbCPU = new TextBlock();

        // Popular / Office / Multimedia / Browser / Remote Desktop checkboxes.
        private readonly CheckBox ChkInstallWinRAR = new CheckBox();
        private readonly CheckBox ChkInstallBID = new CheckBox();
        private readonly CheckBox ChkActivateWindows = new CheckBox();
        private readonly CheckBox ChkPauseWindowsUpdate = new CheckBox();
        private readonly CheckBox ChkDirectX = new CheckBox();
        private readonly CheckBox ChkJava = new CheckBox();
        private readonly CheckBox ChkOpenAL = new CheckBox();
        private readonly CheckBox ChkRevoUninstaller = new CheckBox();
        private readonly CheckBox ChkInstallZalo = new CheckBox();
        private readonly CheckBox ChkActivateOffice = new CheckBox();
        private readonly CheckBox ChkOfficeToolPlus = new CheckBox();
        private readonly CheckBox ChkOfficeSoftmaker = new CheckBox();
        private readonly CheckBox ChkGMTPCFonts = new CheckBox();
        private readonly CheckBox ChkPotPlayer = new CheckBox();
        private readonly CheckBox ChkFoxit = new CheckBox();
        private readonly CheckBox ChkBandiview = new CheckBox();
        private readonly CheckBox ChkAdvancedCodecPack = new CheckBox();
        private readonly CheckBox ChkUltraviewer = new CheckBox();
        private readonly CheckBox ChkTeamViewerQS = new CheckBox();
        private readonly CheckBox ChkTeamViewerFull = new CheckBox();
        private readonly CheckBox ChkAnyDesk = new CheckBox();
        private readonly CheckBox ChkEdge = new CheckBox();
        private readonly CheckBox ChkBrave = new CheckBox();

        // System / tools checkboxes that are still referenced elsewhere.
        private readonly CheckBox ChkMMTApps = new CheckBox();
        private readonly CheckBox ChkDISMPP = new CheckBox();
        private readonly CheckBox ChkFolderSize = new CheckBox();
        private readonly CheckBox ChkGoogleDrive = new CheckBox();
        private readonly CheckBox ChkNetLimiter = new CheckBox();

        // Legacy tabs that were fully removed.
        private readonly CheckBox ChkSubtitleEdit = new CheckBox();
        private readonly CheckBox ChkVidCoder = new CheckBox();
        private readonly CheckBox ChkBoilsoftVideoSplitter = new CheckBox();
        private readonly CheckBox ChkVibe = new CheckBox();
        private readonly CheckBox ChkMKVToolNix = new CheckBox();
        private readonly CheckBox ChkSubtitleDraftGMTPC = new CheckBox();
        private readonly CheckBox ChkDownloadSampleVideo = new CheckBox();
        private readonly CheckBox ChkProcessLasso = new CheckBox();
        private readonly CheckBox ChkThrottlestop = new CheckBox();
        private readonly CheckBox ChkMSIAfterburner = new CheckBox();
        private readonly CheckBox ChkLeagueOfLegends = new CheckBox();
        private readonly CheckBox ChkPorofessor = new CheckBox();
        private readonly CheckBox ChkSamuraiMaiden = new CheckBox();
        private readonly CheckBox ChkGhostOfTsushima = new CheckBox();
        private readonly CheckBox ChkJumpForce = new CheckBox();
        private readonly CheckBox Chk3DPChip = new CheckBox();
        private readonly CheckBox Chk3DPNet = new CheckBox();
        private readonly CheckBox ChkAomeiPartitionAssistant = new CheckBox();
        private readonly CheckBox ChkDiskGenius = new CheckBox();
        private readonly CheckBox ChkVentoy = new CheckBox();
        private readonly CheckBox ChkWintoHDD = new CheckBox();
        private readonly CheckBox ChkWin11_26H1 = new CheckBox();
        private readonly CheckBox ChkWin10LtscIot21H2 = new CheckBox();
        private readonly CheckBox ChkWin10_22H2_2024_December = new CheckBox();

        private readonly Button BtnWinPEToHDD = new Button();
        private readonly TabControl WindowsTabControl = new TabControl();
    }
}
