namespace SniperLog.ViewModels
{
    /// <summary>
    /// Abstract class as a base for all view models.
    /// </summary>
    public abstract partial class BaseViewModel : ObservableObject
    {
        #region Properties

        /// <summary>
        /// Whether the viewmodel is busy.
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool _isBusy = false;

        /// <summary>
        /// Whethet the viewmodel is not busy. Used by XAML.
        /// </summary>
        public bool IsNotBusy => !IsBusy;

        /// <summary>
        /// The page title.
        /// </summary>
        [ObservableProperty]
        private string _pageTitle;

        #endregion

        #region Ctor

        /// <summary>
        /// Constructor.
        /// </summary>
        public BaseViewModel()
        {
           
        }

        #endregion
    }
}
