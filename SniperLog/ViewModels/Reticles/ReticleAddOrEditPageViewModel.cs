using CommunityToolkit.Mvvm.Input;
using SniperLog.Extensions.WrapperClasses;

namespace SniperLog.ViewModels.Reticles
{
    [QueryProperty(nameof(Reticle), "Reticle")]
    public partial class ReticleAddOrEditPageViewModel : BaseViewModel
    {
        public string HeadlineText => Reticle != null ? "Edit reticle" : "New reticle";
        public string CreateOrEditButtonText => Reticle != null ? "Edit" : "Create";

        private readonly ValidatorService _validatorService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HeadlineText))]
        [NotifyPropertyChangedFor(nameof(CreateOrEditButtonText))]
        private SightReticle _reticle;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private DrawableImagePaths _tmpImgPath;

        public ReticleAddOrEditPageViewModel(ValidatorService validatorService)
        {
            _validatorService = validatorService;
        }

        partial void OnReticleChanged(SightReticle value)
        {
            Name = Reticle?.Name ?? string.Empty;
            TmpImgPath = Reticle?.BackgroundImgPathFull ?? string.Empty;
        }

        [RelayCommand]
        private async Task CancelCreation()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task CreateReticle()
        {
            _validatorService.RevalidateAll();
            if (!_validatorService.AllValid)
            {
                await Shell.Current.DisplayAlert("Validation", "Please fix any invalid fields and try again", "Okay");
                return;
            }

            if (Reticle == null)
            {
                Reticle = new SightReticle(Name);
            }
            else
            {
                Reticle.Name = Name;
            }

            if (TmpImgPath != Reticle.BackgroundImgPathFull)
            {
                await Reticle.SaveImageAsync(TmpImgPath);
            }

            await Reticle.SaveAsync();
            await Shell.Current.GoToAsync("..");
        }
    }
}