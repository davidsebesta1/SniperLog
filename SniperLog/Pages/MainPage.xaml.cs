using SniperLog.ViewModels;

namespace SniperLog.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {

        }
    }
}