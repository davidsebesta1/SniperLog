using SniperLog.Services;

namespace SniperLog
{
    public abstract class BasePage : ContentPage
    {
        public abstract void Build();

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            Build();
#if (DEBUG)
            HotReloadService.UpdateApplicationEvent += ReloadUI;
#endif
        }

        protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
        {
            base.OnNavigatedFrom(args);
            Build();
#if (DEBUG)
            HotReloadService.UpdateApplicationEvent -= ReloadUI;
#endif

        }

        private void ReloadUI(Type[]? obj)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Build();
            });
        }
    }
}
