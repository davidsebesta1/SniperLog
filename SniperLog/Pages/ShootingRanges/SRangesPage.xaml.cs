using SniperLog.ViewModels.SRanges;

namespace SniperLog.Pages.ShootingRanges
{
    public partial class SRangesPage : ContentPage
    {
        public SRangesPage(SRangesPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;


        }

        public FirearmType ReferencedFirearmType
        {
            get
            {
                return ServicesHelper.GetService<DataCacherService<FirearmType>>().GetFirstBy(n => n.ID == 1).GetAwaiter().GetResult();
            }
        }
    }
}