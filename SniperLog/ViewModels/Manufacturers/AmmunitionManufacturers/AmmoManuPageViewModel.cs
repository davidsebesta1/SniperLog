using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SniperLog.ViewModels.Manufacturers.AmmunitionManufacturers
{
    public partial class AmmoManuPageViewModel : ManuPageViewModelBase
    {
        public override string ManufacturerTypeNameInternal => "Ammunition";

        public AmmoManuPageViewModel(DataCacherService<Manufacturer> manufacturersCacher, DataCacherService<ManufacturerType> manTypeCacher) : base(manufacturersCacher, manTypeCacher)
        {
            PageTitle = "Ammunition\nManufacturers";
        }
    }
}
