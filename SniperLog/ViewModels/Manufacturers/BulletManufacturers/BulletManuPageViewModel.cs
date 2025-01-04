using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SniperLog.ViewModels.Manufacturers.BulletManufacturers
{
    public class BulletManuPageViewModel : ManuPageViewModelBase
    {
        public override string ManufacturerTypeNameInternal => "Bullet";

        public BulletManuPageViewModel(DataCacherService<Manufacturer> manufacturersCacher, DataCacherService<ManufacturerType> manTypeCacher) : base(manufacturersCacher, manTypeCacher)
        {
            PageTitle = "Bullet\nManufacturers";
        }
    }
}
