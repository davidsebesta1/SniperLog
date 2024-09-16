using Mopups.Pages;
using SniperLog.Extensions.CustomXamlComponents.ViewModels;

namespace SniperLog.Extensions.CustomXamlComponents
{
    public partial class CustomImageEditorPopup : PopupPage
    {
        public CustomImageEditorPopup(CustomImageEditorPopupViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

        }
    }
}