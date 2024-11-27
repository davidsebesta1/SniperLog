using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using SniperLog.Extensions;
using SniperLog.Extensions.WrapperClasses;
using SniperLogNetworkLibrary;
using System.Collections.ObjectModel;

namespace SniperLog.ViewModels.Records
{
    public partial class RecordsPageViewModel : BaseViewModel
    {
        #region Services

        private readonly DataCacherService<ShootingRange> _shootingRangesCacher;
        private readonly DataCacherService<SubRange> _subrangeCacher;
        private readonly DataCacherService<Firearm> _firearmsCacher;
        private readonly DataCacherService<ShootingRecord> _shootingRecordsCacher;

        private readonly ValidatorService _validatorService;

        #endregion

        #region Properties

        //[ObservableProperty]
        //private LineChart _elevationChart;

        [ObservableProperty]
        private ObservableCollection<ShootingRange> _shootingRanges;

        [ObservableProperty]
        private ObservableCollection<SubRange> _subRanges;

        [ObservableProperty]
        private ObservableCollection<Firearm> _firearms;

        [ObservableProperty]
        private ObservableCollection<ShootingRecord> _records;

        [ObservableProperty]
        private ShootingRange? _selectedRange;

        [ObservableProperty]
        private SubRange? _selectedSubRange;

        [ObservableProperty]
        private Firearm? _selectedFirearm;

        [ObservableProperty]
        private int _elevationClicks;

        [ObservableProperty]
        private int _windageClicks;

        [ObservableProperty]
        private int _distanceMeters;

        [ObservableProperty]
        private DrawableImagePaths _imgPath = new DrawableImagePaths(string.Empty, string.Empty);

        [ObservableProperty]
        private string _notes;

        [ObservableProperty]
        private DateTime? _dateSearchVal;

        #endregion

        #region Ctor

        public RecordsPageViewModel(DataCacherService<ShootingRange> shootingRangesCacher, DataCacherService<SubRange> subrangeCacher, DataCacherService<Firearm> firearmsCacher, DataCacherService<ShootingRecord> shootingRecordsCacher, ValidatorService validatorService)
        {
            PageTitle = "Records";

            _shootingRangesCacher = shootingRangesCacher;
            _subrangeCacher = subrangeCacher;
            _firearmsCacher = firearmsCacher;
            _shootingRecordsCacher = shootingRecordsCacher;
            _validatorService = validatorService;
        }

        #endregion

        #region Commands & Methods

        [RelayCommand]
        private async Task RefreshEntries(bool resetSubRange = false)
        {
            ShootingRanges = await _shootingRangesCacher.GetAll();
            SubRanges = await _subrangeCacher.GetAllBy(n => SelectedRange != null && n.ShootingRange_ID == SelectedRange.ID);

            if (resetSubRange)
            {
                SelectedSubRange = null;
            }
            Firearms = await _firearmsCacher.GetAll();
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

            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                await FinalizeCreatingNewRecord(false);
                return;
            }

            try
            {
                await SelectedRange.TrySendWeatherRequestMessage();
                await FinalizeCreatingNewRecord(true);
            }
            catch (Exception e)
            {
                await FinalizeCreatingNewRecord(false);
                return;
            }
            finally
            {
                ResetForm();
            }

        }

        private async Task FinalizeCreatingNewRecord(bool weatherSuccess)
        {
            Weather? weather = null;
            if (weatherSuccess && !SelectedRange.CurrentWeather.Equals(default(WeatherResponseMessage)))
            {
                weather = new Weather(SelectedRange.CurrentWeather);
                await weather.SaveAsync();
            }

            ShootingRecord record = new ShootingRecord(SelectedRange.ID, SelectedSubRange.ID, SelectedFirearm.ID, weather?.ID, ElevationClicks, WindageClicks, DistanceMeters, DateTime.Now.ToBinary());
            await record.SaveAsync();

            if (!string.IsNullOrEmpty(Notes))
            {
                await record.SaveNotesAsync(Notes);
            }

            if (!string.IsNullOrEmpty(ImgPath))
            {
                await record.SaveImageAsync(ImgPath);
            }

            await SearchRecords(DateSearchVal);
        }

        private void ResetForm()
        {
            ElevationClicks = 0;
            WindageClicks = 0;
            DistanceMeters = 0;
            ImgPath.ImagePath = string.Empty;
            ImgPath.OverDrawPath = string.Empty;
            Notes = string.Empty;
        }

        [RelayCommand]
        private async Task SearchRecords(DateTime? date)
        {
            IEnumerable<ShootingRecord> records = await GetAllRecords();

            if (records == null)
            {
                return;
            }

            if (date == null)
            {
                Records = records.ToObservableCollection();
                return;
            }

            Records = records.Where(n => n.Date.Date == date.Value.Date).ToObservableCollection();
        }

        private async Task RefreshChart()
        {
            if (SelectedFirearm == null)
                return;

            IEnumerable<ShootingRecord> records = await _shootingRecordsCacher.GetAll();

            var wher = records.Where(n => n.Firearm_ID == SelectedFirearm.ID);
            var ord = wher.OrderBy(n => n.Distance);
            /*
            var select = ord.Select(n => new ChartEntry(n.ElevationClicksOffset)
            {
                Label = n.Distance.ToString() + " m",
                ValueLabel = n.ElevationClicksOffset.ToString()
            });
            */
            
        }

        [RelayCommand]
        private async Task GoToDetails(ShootingRecord record)
        {
            await Shell.Current.GoToAsync("Records/RecordDetails", new Dictionary<string, object>(1) { { "Record", record } });
        }

        async partial void OnSelectedFirearmChanged(Firearm? value)
        {
            await SearchRecords(null);
            await RefreshChart();
        }

        async partial void OnSelectedRangeChanged(ShootingRange? value)
        {
            await RefreshEntries(true);
            await SearchRecords(null);
        }

        async partial void OnSelectedSubRangeChanged(SubRange? value)
        {
            await SearchRecords(null);
        }

        #endregion
    }
}
