using CommunityToolkit.Mvvm.Input;
using SniperLog.Extensions;
using System.Globalization;

namespace SniperLog.ViewModels.SRanges.Subranges
{
    [QueryProperty(nameof(Subrange), "Subrange")]
    [QueryProperty(nameof(OwningRange), "SRange")]
    public partial class SubRangeAddOrEditPageViewModel : BaseViewModel
    {
        private readonly ValidatorService _validatorService;

        public string HeadlineText => Subrange != null ? "Edit subrange" : "New subrange";
        public string CreateOrEditButtonText => Subrange != null ? "Edit" : "Create";

        [ObservableProperty]
        public string _rangeInMeters;

        [ObservableProperty]
        public string _altitude;

        [ObservableProperty]
        public string _directionToNorthDegrees;

        [ObservableProperty]
        public string _verticalFiringOffsetDegrees;

        [ObservableProperty]
        private string _prefix;

        [ObservableProperty]
        private string _notes;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HeadlineText))]
        [NotifyPropertyChangedFor(nameof(CreateOrEditButtonText))]
        private SubRange _subrange;

        [ObservableProperty]
        private ShootingRange _owningRange;

        public SubRangeAddOrEditPageViewModel(ValidatorService validator) : base()
        {
            _validatorService = validator;
        }

        partial void OnSubrangeChanged(SubRange value)
        {
            RefreshEntries();
        }

        [RelayCommand]
        private async Task RefreshEntries()
        {
            RangeInMeters = Subrange?.RangeInMeters.ToString() ?? string.Empty;
            Altitude = Subrange?.Altitude.ToString() ?? string.Empty;
            DirectionToNorthDegrees = Subrange?.DirectionToNorthDegrees.ToString() ?? string.Empty;
            VerticalFiringOffsetDegrees = Subrange?.VerticalFiringOffsetDegrees.ToString() ?? string.Empty;
            Prefix = Subrange?.Prefix.ToString() ?? (await OwningRange.GetNextPrefix()).ToString();
            Notes = Subrange?.NotesText ?? string.Empty;
        }

        [RelayCommand]
        private async Task CancelCreation()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task CreateSubrange()
        {
            _validatorService.RevalidateAll();
            if (!_validatorService.AllValid)
            {
                await Shell.Current.DisplayAlert("Validation", "Please fix any invalid fields and try again", "Okay");
                return;
            }

            if (Subrange == null)
            {
                string notes = Notes;
                Subrange = new SubRange(OwningRange.ID, int.Parse(RangeInMeters, CultureInfo.InvariantCulture), ParseExtensions.ParseOrNullDouble(Altitude), ParseExtensions.ParseOrNullInteger(DirectionToNorthDegrees), ParseExtensions.ParseOrNullInteger(VerticalFiringOffsetDegrees), Prefix[0], string.Empty);
                await Subrange.SaveAsync();
                await Subrange.SaveNotesAsync(notes);
            }
            else
            {
                Subrange.RangeInMeters = int.Parse(RangeInMeters, CultureInfo.InvariantCulture);
                Subrange.Altitude = string.IsNullOrEmpty(Altitude) ? null : double.Parse(Altitude, CultureInfo.InvariantCulture);
                Subrange.DirectionToNorthDegrees = string.IsNullOrEmpty(DirectionToNorthDegrees) ? null : int.Parse(DirectionToNorthDegrees, CultureInfo.InvariantCulture);
                Subrange.VerticalFiringOffsetDegrees = string.IsNullOrEmpty(VerticalFiringOffsetDegrees) ? null : int.Parse(VerticalFiringOffsetDegrees, CultureInfo.InvariantCulture);
                Subrange.Prefix = Prefix[0];

                if (Notes != Subrange.NotesText)
                {
                    await Subrange.SaveNotesAsync(Notes);
                }

                await Subrange.SaveAsync();
            }

            await Shell.Current.GoToAsync("..");
        }
    }
}
