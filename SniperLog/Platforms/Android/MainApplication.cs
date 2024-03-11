using Android;
using Android.App;
using Android.Gms.Maps;
using Android.Health.Connect.DataTypes;
using Android.Runtime;
using Java.Util.Jar;
using Xamarin.Google.Crypto.Tink.Integration.Android;

namespace SniperLog
{

    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership): base(handle, ownership)
        {
           
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}