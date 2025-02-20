using SniperLog.Config.Interfaces;

namespace SniperLog.Config
{
    public class PreferencesConfig : IConfig
    {
        public static string Name => "Preferences";

        public int LastSelectedFirearm { get; set; } = 0;

        public Dictionary<int, int> LastSelectedAmmunitionForFirearm { get; set; } = new Dictionary<int, int>();

        /// <summary>
        /// Gets the last selected firearm. Or null.
        /// </summary>
        /// <returns>The last selected firearm or null if there isn't any.</returns>
        public async Task<Firearm?> GetLastSelectedFirearm()
        {
            DataCacherService<Firearm> firearmCacher = ServicesHelper.GetService<DataCacherService<Firearm>>();

            return await firearmCacher.GetFirstBy(n => n.ID == LastSelectedFirearm);
        }

        /// <summary>
        /// Sets the last selected firearm for record preferences.
        /// </summary>
        /// <param name="firearm">The firearm.</param>
        public void SetLastSelectedFirearm(Firearm? firearm)
        {
            if (firearm == null)
                LastSelectedFirearm = 0;
            else
                LastSelectedFirearm = firearm.ID;

            ApplicationConfigService.SaveConfig(this);
        }

        /// <summary>
        /// Gets the last selected ammunition for this firearm for recording entries.
        /// </summary>
        /// <returns>Ammunition that was lastly selected or null.</returns>
        public async Task<Ammunition?> GetLastSelectedAmmunition(Firearm firearm)
        {
            DataCacherService<Ammunition> ammoCacher = ServicesHelper.GetService<DataCacherService<Ammunition>>();

            if (LastSelectedAmmunitionForFirearm.TryGetValue(firearm.ID, out int ammoId))
                return await ammoCacher.GetFirstBy(n => n.ID == ammoId);

            return null;
        }

        /// <summary>
        /// Sets the last selected ammunition for provided firearm and saves the preferences.
        /// </summary>
        /// <param name="firearm">The firearm to this preference is for.</param>
        /// <param name="ammo">The ammunition that has been selected as last.</param>
        public void SetLastSelectedAmmunition(Firearm firearm, Ammunition? ammo)
        {
            if (ammo == null)
                LastSelectedAmmunitionForFirearm.Remove(firearm.ID);
            else
                LastSelectedAmmunitionForFirearm[firearm.ID] = ammo.ID;


            ApplicationConfigService.SaveConfig(this);
        }
    }
}
