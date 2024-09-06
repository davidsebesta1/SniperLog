using SniperLog.ViewModels;

namespace SniperLog.Extensions.CustomXamlComponents.ViewModels
{
    public partial class CustomImageEditorPopupViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _backgroundImage;

        public CustomImagePickerEntry Entry;

        public CustomImageEditorPopupViewModel() : base()
        {

        }
    }
}
