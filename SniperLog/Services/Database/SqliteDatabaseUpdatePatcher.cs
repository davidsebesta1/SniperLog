using System.Collections.ObjectModel;

namespace SniperLog.Services.Database
{
    public static class SqliteDatabaseUpdatePatcher
    {
        private static IVersionTracking _versionTracking => VersionTracking.Default;

        private static readonly ReadOnlyCollection<DatabasePatch> _patches = new ReadOnlyCollection<DatabasePatch>
        (
            [
                new DatabasePatch(new Version(0,0,2), "ALTER TABLE ShootingRange ADD RelativeImagePathFromAppdata VARCHAR(100);")
            ]
        );

        public static List<Version> AppliedPatchesVersions => ApplicationConfigService.Config.AppliedPatches;

        public static void CheckForUpdates()
        {
            Version curVersion = Version.Parse(_versionTracking.CurrentVersion);

            bool anyApplied = false;
            foreach (var patch in _patches)
            {
                Version patchVersion = patch.Version;
                if (curVersion >= patchVersion && !AppliedPatchesVersions.Contains(patchVersion))
                {
                    patch.ApplyPatch();
                    AppliedPatchesVersions.Add(patchVersion);
                    anyApplied = true;
                }
            }
            if (anyApplied) ApplicationConfigService.SaveConfig();
        }
    }
}