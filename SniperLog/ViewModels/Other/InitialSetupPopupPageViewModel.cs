using CommunityToolkit.Mvvm.Input;
using SniperLog.Config;
using SniperLog.Extensions;
using SniperLog.Services.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SniperLog.ViewModels.Other
{
    public partial class InitialSetupPopupPageViewModel : BaseViewModel
    {
        private readonly CsvProcessor _csvProcessor;

        public InitialSetupPopupPageViewModel(CsvProcessor processor) : base()
        {
            _csvProcessor = processor;
        }

        [RelayCommand]
        private async Task LoadInitialDatabase()
        {
            await SqLiteDatabaseConnection.Instance.ExecuteNonQueryAsync(await MauiAssetHelper.ReadTextFileAsync("SETUPQUERY.sql"));
        }

        [RelayCommand]
        private async Task LoadInitialData()
        {
            await _csvProcessor.LoadToDatabase<Country>(new StreamReader(await FileSystem.Current.OpenAppPackageFileAsync("Countries.csv")));
            await _csvProcessor.LoadToDatabase<SightClickType>(new StreamReader(await FileSystem.Current.OpenAppPackageFileAsync("SightClickTypes.csv")));
            await _csvProcessor.LoadToDatabase<FirearmType>(new StreamReader(await FileSystem.Current.OpenAppPackageFileAsync("FirearmTypes.csv")));
            await _csvProcessor.LoadToDatabase<FirearmCaliber>(new StreamReader(await FileSystem.Current.OpenAppPackageFileAsync("FirearmCalibers.csv")));

            ShootingRange s1 = new ShootingRange("Oleško", "Oleško, Litoměřice", 50.486238d, 14.201412d, false, string.Empty);
            ShootingRange s2 = new ShootingRange("TestFav", "Fav", 1d, 2d, true, string.Empty);

            await s1.SaveAsync();
            await s2.SaveAsync();

            SubRange testSub = new SubRange(s1.ID, false, 300, 123, 46, 0, 'A', string.Empty);
            await testSub.SaveAsync();
            await testSub.SaveNotesAsync("test notes markiplier");
        }

        [RelayCommand]
        private void FinalizeInitialLoad()
        {
            VersionControl tracking = ApplicationConfigService.GetConfig<VersionControl>();
            tracking.FirstLaunchEver = false;
            ApplicationConfigService.SaveConfig(tracking);
        }
    }
}
