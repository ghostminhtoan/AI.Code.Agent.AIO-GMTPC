// =======================================================================
// MainWindow.TabTool.cs
// AI Summary: 2026-05-11 - Added centralized helpers for the consolidated
//             Tool tab so selection actions can target the remaining checkboxes
// =======================================================================
using System.Collections.Generic;
using System.Windows.Controls;

namespace AICodeAgentAIOGMTPC
{
    public partial class MainWindow
    {
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
    }
}
