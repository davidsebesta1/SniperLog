using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace SniperLog.ViewModels.SRanges.Subranges
{
    [QueryProperty(nameof(Range), "ShootingRange")]
    public partial class SubRangesPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ShootingRange _range;

        [ObservableProperty]
        private ObservableCollection<SubRange> _subRanges;

        private readonly DataCacherService<SubRange> _dataCacherService;

        public SubRangesPageViewModel(DataCacherService<SubRange> dataCacherService) : base()
        {
            _dataCacherService = dataCacherService;
        }

        [RelayCommand]
        private async Task RefreshSubRanges()
        {
            SubRanges = await _dataCacherService.GetAllBy(n => n.ShootingRange_ID == Range.ID);
        }

        [RelayCommand]
        private async Task ToggleFavouriteRange()
        {
            Range.IsMarkedAsFavourite = !Range.IsMarkedAsFavourite;
            await Range.SaveAsync();
        }

        [RelayCommand]
        private async Task AddSubrange()
        {
            await Shell.Current.GoToAsync("AddOrEditSubrange", new Dictionary<string, object>(2) { { "Subrange", null }, { "SRange", Range } });
        }

        [RelayCommand]
        private async Task EditSubrange(SubRange range)
        {
            await Shell.Current.GoToAsync("AddOrEditSubrange", new Dictionary<string, object>(2) { { "Subrange", range }, { "SRange", Range } });
        }

        [RelayCommand]
        private async Task DeleteSubrange(SubRange range)
        {
            bool res = await Shell.Current.DisplayAlert("Confirmation", $"Are you sure you want to delete subrange {range.Prefix}? This action cannot be undone!", "Delete", "Cancel");

            if (res)
            {
                await range.DeleteAsync();
                SubRanges.Remove(range);
            }
        }

        [RelayCommand]
        private async Task ReturnToRange()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
