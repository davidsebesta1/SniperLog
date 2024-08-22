
namespace SniperLog.ViewModels.Manufacturers.FirearmManufacturers
{
    public partial class FirearmManuPageViewModel : ManuPageViewModelBase
    {
        public override string ManufacturerTypeNameInternal => "Firearm";

        public FirearmManuPageViewModel(DataCacherService<Manufacturer> manufacturersCacher, DataCacherService<ManufacturerType> manTypeCacher) : base(manufacturersCacher, manTypeCacher)
        {
            PageTitle = "Firearm\nManufacturers";
        }
    }
}