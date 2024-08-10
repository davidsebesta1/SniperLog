namespace SniperLog.ViewModels
{
    public abstract partial class BaseViewModel : ObservableObject
    {
        #region Properties

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool _isBusy = false;

        public bool IsNotBusy => !IsBusy;

        [ObservableProperty]
        private string _pageTitle;

        #endregion

        #region Ctor

        public BaseViewModel()
        {

        }

        #endregion
    }
}
