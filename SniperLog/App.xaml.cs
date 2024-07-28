using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Handlers;
using SniperLog.Pages;

namespace SniperLog
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            EntryHandler.Mapper.AppendToMapping("CursorColor", (handler, view) =>
            {
#if __ANDROID__
        handler.PlatformView.TextCursorDrawable.SetTint(Colors.White.ToInt());
#endif
            });

            EditorHandler.Mapper.AppendToMapping("RemoveEditorUnderline", (handler, view) =>
            {
#if __ANDROID__
                var nativeEditText = handler.PlatformView;
                if (nativeEditText != null)
                {
                    nativeEditText.Background.SetTint(Colors.Transparent.ToInt());
                }
#endif
            });

            MainPage = new AppShell();
        }
    }
}
