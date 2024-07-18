using SniperLog.Pages;

namespace SniperLog
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("CursorColor", (handler, view) =>
            {
#if __ANDROID__
        handler.PlatformView.TextCursorDrawable.SetTint(Colors.White.ToInt());
#endif
            });

            MainPage = new AppShell();
        }
    }
}
