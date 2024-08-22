
namespace SniperLog.ViewModels.Manufacturers.SightManufacturers
{
    public partial class SightManuPageViewModel : ManuPageViewModelBase
    {
        public override string ManufacturerTypeNameInternal => "Sight";

        public SightManuPageViewModel(DataCacherService<Manufacturer> manufacturersCacher, DataCacherService<ManufacturerType> manTypeCacher) : base(manufacturersCacher, manTypeCacher)
        {
            PageTitle = "Sight\nManufacturers";
        }
    }
}
