using SniperLog.ViewModels.Records;
using System.Reflection;
using System.Xml.Linq;

namespace SniperLog.Pages.Records
{
    public partial class RecordsPage : ContentPage
    {
        private readonly ValidatorService _validatorService;

        public RecordsPage(RecordsPageViewModel vm, ValidatorService validatorService)
        {
            InitializeComponent();
            BindingContext = vm;
            _validatorService = validatorService;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}