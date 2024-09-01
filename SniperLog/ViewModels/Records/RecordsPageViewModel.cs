using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Xml.Linq;

namespace SniperLog.ViewModels.Records
{
    public partial class RecordsPageViewModel : BaseViewModel
    {
        private readonly DataCacherService<ShootingRange> _shootingRangesCacher;
        private readonly DataCacherService<SubRange> _subrangeCacher;
        private readonly DataCacherService<Firearm> _firearmsCacher;
        private readonly DataCacherService<ShootingRecord> _shootingRecordsCacher;

        private readonly ValidatorService _validatorService;

        [ObservableProperty]
        private ObservableCollection<ShootingRange> _shootingRanges;

        [ObservableProperty]
        private ObservableCollection<SubRange> _subRanges;

        [ObservableProperty]
        private ObservableCollection<Firearm> _firearms;

        [ObservableProperty]
        private ObservableCollection<ShootingRecord> _records;

        [ObservableProperty]
        private ShootingRange _selectedRange;

        [ObservableProperty]
        private SubRange _selectedSubRange;

        [ObservableProperty]
        private Firearm _selectedFirearm;

        [ObservableProperty]
        private int _elevationClicks;

        [ObservableProperty]
        private int _windageClicks;

        [ObservableProperty]
        private int _distanceMeters;

        [ObservableProperty]
        private string _imgPath;

        [ObservableProperty]
        private string _notes;

        [ObservableProperty]
        private DateTime? _dateSearchVal;

        public RecordsPageViewModel(DataCacherService<ShootingRange> shootingRangesCacher, DataCacherService<SubRange> subrangeCacher, DataCacherService<Firearm> firearmsCacher, DataCacherService<ShootingRecord> shootingRecordsCacher, ValidatorService validatorService)
        {
            PageTitle = "Records";

            _shootingRangesCacher = shootingRangesCacher;
            _subrangeCacher = subrangeCacher;
            _firearmsCacher = firearmsCacher;
            _shootingRecordsCacher = shootingRecordsCacher;
            _validatorService = validatorService;
        }

        private async Task<IEnumerable<ShootingRecord>> GetAllRecords()
        {
            if (SelectedFirearm == null)
            {
                return null;
            }
            ObservableCollection<ShootingRecord> collection = await _shootingRecordsCacher.GetAll(); // all records for selected firearm

            if (SelectedRange != null) // filter for selected range
            {
                IEnumerable<ShootingRecord> tmp = collection.Where(n => n.ShootingRange_ID == SelectedRange.ID);

                if (SelectedSubRange != null) // filter for selected subrange
                {
                    tmp = tmp.Where(n => n.SubRange_ID == SelectedSubRange.ID);
                }

                return tmp.ToObservableCollection();
            }

            return collection;
        }

        [RelayCommand]
        private async Task CreateNew()
        {
            _validatorService.RevalidateAll();
            if (!_validatorService.AllValid)
            {
                await Shell.Current.DisplayAlert("Validation", "Please fix any invalid fields and try again", "Okay");
                return;
            }

            

            await Firearm.SaveAsync();
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task SearchRecords(DateTime? date)
        {
            IEnumerable<ShootingRecord> records = await GetAllRecords();

            if (date == null)
            {
                Records = records.ToObservableCollection();
                return;
            }

            Records = records.Where(n => n.Date.Date == date.Value.Date).ToObservableCollection();
        }
    }
}
