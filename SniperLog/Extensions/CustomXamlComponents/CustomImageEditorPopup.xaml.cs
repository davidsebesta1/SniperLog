using Mopups.Pages;
using SniperLog.Extensions.CustomXamlComponents.ViewModels;

namespace SniperLog.Extensions.CustomXamlComponents
{
    /// <summary>
    /// A custom popup for editing images.
    /// </summary>
    public partial class CustomImageEditorPopup : PopupPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomImageEditorPopup"/> class.
        /// </summary>
        /// <param name="vm">The view model for the image editor popup.</param>
        public CustomImageEditorPopup(CustomImageEditorPopupViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        /// <summary>
        /// Called when the popup is disappearing.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}
