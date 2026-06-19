using Dalamud.Interface.Components;
using ECommons.Funding;
using ECommons.GameHelpers;
using ECommons.Logging;
using ECommons.LanguageHelpers;
using System.IO;

namespace JustBackup;

internal class ConfigWindow : Window
{
    private JustBackup p;
    private string newIgnoredFile = string.Empty;

    public ConfigWindow(JustBackup p) : base("JustBackup configuration".Loc())
    {
        this.p = p;
    }

    void Settings()
    {
        ImGuiEx.LineCentered("restore", () =>
        {
            ImGuiEx.WithTextColor(ImGuiColors.DalamudOrange, delegate
            {
                if (ImGui.Button("Read how to restore a backup".Loc()))
                {
                    ShellStart("https://github.com/NightmareXIV/JustBackup/blob/master/README.md#restoring-a-backup");
                }
            });
        });
        ImGuiEx.Text("Custom backup path (by default: %localappdata%\\JustBackup):".Loc());
        ImGui.SetNextItemWidth(400f);
        ImGui.InputText("##PathToBkp", ref p.config.BackupPath, 100);
        ImGuiEx.Text("Custom temporary files path (by default: %temp%):".Loc());
        ImGui.SetNextItemWidth(400f);
        ImGui.InputText("##PathToTmp", ref p.config.TempPath, 100);
        ImGui.Checkbox("Automatically remove old backups".Loc(), ref p.config.DeleteBackups);
        if (p.config.DeleteBackups)
        {
            ImGui.SetNextItemWidth(50f);
            ImGui.DragInt("Delete backups older than, days".Loc(), ref p.config.DaysToKeep, 0.1f, 3, 730);
            if (p.config.DaysToKeep < 3) p.config.DaysToKeep = 3;
            if (p.config.DaysToKeep > 730) p.config.DaysToKeep = 730;
            ImGui.Checkbox("Delete to recycle bin, if available.".Loc(), ref p.config.DeleteToRecycleBin);
            ImGui.SetNextItemWidth(50f);
            ImGui.DragInt("Always keep at least this number of backup regardless of their date".Loc(), ref p.config.BackupsToKeep, 0.1f, 10, 100000);
            if (p.config.BackupsToKeep < 0) p.config.BackupsToKeep = 0;
            ImGui.Separator();
        }
        ImGui.Checkbox("Include plugin configurations".Loc(), ref p.config.BackupPluginConfigs);
        ImGui.Checkbox("Include ALL files inside FFXIV's data folder into backup".Loc(), ref p.config.BackupAll);
        ImGuiEx.Text("  (otherwise only config files will be saved, screenshots, logs, etc will be skipped)".Loc());
        ImGui.Checkbox("Exclude replays from backup".Loc(), ref p.config.ExcludeReplays);
        ImGui.Checkbox("Use built-in zip method instead of 7-zip".Loc(), ref p.config.UseDefaultZip);
        if (p.config.UseDefaultZip) ImGuiEx.Text(ImGuiColors.DalamudRed, "7-zip archives are taking up to 15 times less space!".Loc());
        ImGui.Checkbox("Do not restrict amount of resources 7-zip can use".Loc(), ref p.config.NoThreadLimit);
        ImGui.SetNextItemWidth(100f);
        ImGui.SliderInt("Minimal interval between backups, minutes".Loc(), ref p.config.MinIntervalBetweenBackups, 0, 60);
        ImGuiComponents.HelpMarker("Backup will not be created if previous backup was created less than this amount of minutes. Note that only successfully completed backups will update interval.".Loc());
    }

    void Tools()
    {
        if (ImGui.Button("Open backup folder".Loc()))
        {
            ShellStart(p.GetBackupPath());
        }
        ImGuiEx.WithTextColor(ImGuiColors.DalamudOrange, delegate
        {
            if (ImGui.Button("Read how to restore a backup".Loc()))
            {
                ShellStart("https://github.com/NightmareXIV/JustBackup/blob/master/README.md#restoring-a-backup");
            }
        });
        if (ImGui.Button("Open FFXIV configuration folder".Loc()))
        {
            ShellStart(p.GetFFXIVConfigFolder());
        }
        if (ImGui.Button("Open plugins configuration folder".Loc()))
        {
            ShellStart(JustBackup.GetPluginsConfigDir().FullName);
        }
        if (Svc.ClientState.LocalPlayer != null)
        {
            if (ImGui.Button("Open current character's config directory".Loc()))
            {
                ShellStart(Path.Combine(p.GetFFXIVConfigFolder(), $"FFXIV_CHR{Svc.ClientState.LocalContentId:X16}"));
            }
            if (ImGui.Button("Add identification info".Loc()))
            {
                Safe(() =>
                {
                    var fname = Path.Combine(p.GetFFXIVConfigFolder(), $"FFXIV_CHR{Svc.ClientState.LocalContentId:X16}",
                        $"_{Player.NameWithWorld}.dat");
                    File.Create(fname).Dispose();
                    Notify.Success("Added identification info for current character".Loc());
                }, (e) =>
                {
                    Notify.Error("Error while adding identification info for current character:\n".Loc() + e);
                });
            }
            ImGuiEx.Tooltip("Adds an empty file into character's config directory\ncontaining character's name and home world".Loc());
        }
    }


    void Ignored()
    {
        var id = 0;
        foreach (var file in p.config.Ignore.ToArray())
        {
            if (ImGui.SmallButton($"x##{id++}"))
            {
                p.config.Ignore.Remove(file);
            }
            ImGui.SameLine();
            ImGui.Text(file);
        }

        if (ImGui.SmallButton("+"))
        {
            if (!p.config.Ignore.Contains(newIgnoredFile, StringComparer.InvariantCultureIgnoreCase))
            {
                p.config.Ignore.Add(newIgnoredFile);
                newIgnoredFile = string.Empty;
            }
        }
        ImGui.SameLine();

        ImGui.InputText("Ignored (partial) Path".Loc(), ref newIgnoredFile, 512);
    }

    void Expert()
    {
        ImGuiEx.Text("Override game configuration folder path:".Loc());
        ImGuiEx.SetNextItemFullWidth();
        ImGui.InputText("##pathGame", ref p.config.OverrideGamePath, 2000);
        ImGui.SetNextItemWidth(150f);
        ImGui.InputInt("Maximum threads".Loc(), ref p.config.MaxThreads.ValidateRange(1, 99), 1, 99);
        ImGui.SetNextItemWidth(100f);
        ImGui.SliderInt("Throttle copying, ms".Loc(), ref p.config.CopyThrottle.ValidateRange(0, 50), 0, 5);
        ImGuiComponents.HelpMarker("The higher this value, the longer backup creation will take but the less loaded your SSD/HDD will be. Increase this value if you're experiencing lag during backup process.".Loc());
    }

    public override void Draw()
    {
        PatreonBanner.DrawRight();
        ImGuiEx.EzTabBar("default", PatreonBanner.Text,
            ("Settings".Loc(), Settings, null, true),
            ("Tools".Loc(), Tools, null, true),
            ("Ignored pathes (beta)".Loc(), Ignored, null, true),
            ("Expert options".Loc(), Expert, null, true),
            InternalLog.ImGuiTab()
            );
                   
    }

    public override void OnClose()
    {
        Svc.PluginInterface.SavePluginConfig(p.config);
        Notify.Success("Configuration saved".Loc());
        base.OnClose();
    }
}
