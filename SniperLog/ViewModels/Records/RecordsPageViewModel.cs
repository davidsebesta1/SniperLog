using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SniperLog.Extensions.WrapperClasses;
using SniperLog.Services.Ballistics;
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

        /// <summary>
        /// X axis for the graph.
        /// </summary>
        [ObservableProperty]
        private List<Axis> _xAxises = [new Axis()
        {
            NamePaint = new SolidColorPaint(SKColors.White),
            LabelsPaint = new SolidColorPaint(SKColors.White),
            Labels = new ObservableCollection<string>(),
            MinStep = 50,
        }];

        /// <summary>
        /// Y axis of the graph.
        /// </summary>
        [ObservableProperty]
        private List<Axis> _yAxises = [new Axis()
        {
            NamePaint = new SolidColorPaint(SKColors.White),
            LabelsPaint = new SolidColorPaint(SKColors.White),
            LabelsDensity = 0.01f,
        }];

        /// <summary>
        /// Y axis of the graph.
        /// </summary>
        [ObservableProperty]
        private List<Axis> _yAxisesWindage = [new Axis()
        {
            NamePaint = new SolidColorPaint(SKColors.White),
            LabelsPaint = new SolidColorPaint(SKColors.White),
            LabelsDensity = 1f,
        }];

        /// <summary>
        /// Series for the elevation clicks.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<ISeries> _elevationSeries = [];

        /// <summary>
        /// Series for the windage clicks.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<ISeries> _windageSeries = [];

        /// <summary>
        /// Collection of available shooting ranges.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<ShootingRange> _shootingRanges;

        /// <summary>
        /// Collection of available shooting ranges based on the selected <see cref="SelectedRange"/>.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<SubRange> _subRanges;

        /// <summary>
        /// Collection of available firearms.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Firearm> _firearms;

        /// <summary>
        /// Collection of available shooting records based on selected <see cref="SelectedFirearm"/>.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<ShootingRecord> _records;

        /// <summary>
        /// Selected <see cref="ShootingRange"/>.
        /// </summary>
        [ObservableProperty]
        private ShootingRange? _selectedRange;

        /// <summary>
        /// Selected <see cref="SubRange"/>.
        /// </summary>
        [ObservableProperty]
        private SubRange? _selectedSubRange;

        /// <summary>
        /// Selected <see cref="Firearm"/>
        /// </summary>
        [ObservableProperty]
        private Firearm? _selectedFirearm;

        /// <summary>
        /// User input for the a new record.
        /// </summary>
        [ObservableProperty]
        private int _elevationClicks;

        /// <summary>
        /// User input for the a new record.
        /// </summary>
        [ObservableProperty]
        private int _windageClicks;

        /// <summary>
        /// User input for the a new record.
        /// </summary>
        [ObservableProperty]
        private int _distanceMeters;

        /// <summary>
        /// User input for the a new record.
        /// </summary>
        [ObservableProperty]
        private DrawableImagePaths _imgPath = new DrawableImagePaths(string.Empty, string.Empty);

        /// <summary>
        /// User input for the a new record.
        /// </summary>
        [ObservableProperty]
        private string _notes;

        /// <summary>
        /// User input for the a new record.
        /// </summary>
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

        /// <summary>
        /// Refreshes the elevation and windage chart
        /// </summary>
        /// <returns></returns>
        public async Task RefreshChart()
        {
            if (SelectedFirearm == null)
                return;

            if (SelectedRange != null)
            {
                WeatherResponseMessage msg = await SelectedRange.GetCurrentWeather();
                BallisticCalculatorService ballisticCalculatorService = new BallisticCalculatorService();
                var offset = ballisticCalculatorService.CalculateOffset(msg, 300, 2700, 0.319, ClickType.MRADs, 0.1d);
            }












            ObservableCollection<FirearmSightSetting> baseSightSettings = await SelectedFirearm.ReferencedFirearmSight.GetBaseSightSettingsAsync();
            var ord = baseSightSettings.OrderBy(n => n.Distance);
            var distances = ord.Select(n => n.Distance);

            XAxises[0].Labels.Clear();

            foreach (var item in distances)
            {
                XAxises[0].Labels.Add(item.ToString());
            }

            List<ClickOffset?> weatherOffsets = await GetNearestWeatherClicksFromBase(distances);
            ElevationSeries.Clear();

            ElevationSeries.Add(new LineSeries<int>
            {
                Values = ord.Select(n => n.ElevationValue).ToList(),
                Name = "Base",
                Stroke = new SolidColorPaint(SKColor.Parse("#6281EE")),
                GeometryStroke = new SolidColorPaint(SKColor.Parse("#6281EE")),
                Fill = null,
                LineSmoothness = 0,

            });

            if (weatherOffsets != null)
            {
                ElevationSeries.Add(new LineSeries<int?>
                {
                    Values = (ICollection<int?>)weatherOffsets.Where(n => n.HasValue && n.Value.VerticalClicks != null).Select(n => n.Value.VerticalClicks).ToList(),
                    Name = "Base",
                    Stroke = new SolidColorPaint(SKColor.Parse("#EE8100")),
                    GeometryStroke = new SolidColorPaint(SKColor.Parse("#EE8100")),
                    Fill = null,
                    LineSmoothness = 0,
                });
            }

            WindageSeries.Clear();

            WindageSeries.Add(new LineSeries<int>
            {
                Values = ord.Select(n => n.WindageValue).ToList(),
                Name = "Base",
                Stroke = new SolidColorPaint(SKColor.Parse("#6281EE")),
                GeometryStroke = new SolidColorPaint(SKColor.Parse("#6281EE")),
                Fill = null,
                LineSmoothness = 0,
            });

            if (weatherOffsets != null)
            {
                WindageSeries.Add(new LineSeries<int?>
                {
                    Values = (ICollection<int?>)weatherOffsets.Where(n => n.HasValue && n.Value.WindageClicks != null).Select(n => n.Value.WindageClicks).ToList(),
                    Name = "Base",
                    Stroke = new SolidColorPaint(SKColor.Parse("#EE8100")),
                    GeometryStroke = new SolidColorPaint(SKColor.Parse("#EE8100")),
                    Fill = null,
                    LineSmoothness = 0,
                });
            }
        }


        public async Task<List<ClickOffset?>> GetNearestWeatherClicksFromBase(IEnumerable<int> distances)
        {
            if (SelectedFirearm == null)
                return null;

            if (SelectedRange == null)
                return null;

            ObservableCollection<ShootingRecord> relatedRecords = await _shootingRecordsCacher.GetAllBy(n => n.ReferencedFirearm == SelectedFirearm);
            int count = distances.Count();

            List<ClickOffset?> list = new List<ClickOffset?>(count);

            WeatherResponseMessage msg;
            try
            {
                NetworkAccess accessType = Connectivity.Current.NetworkAccess;

                if (accessType == NetworkAccess.None)
                {
                    using IToast toast = Toast.Make("Unable to load weather offsets. No internet.", ToastDuration.Long, 14);

                    await toast.Show();
                    return null;
                }

                msg = await SelectedRange.GetCurrentWeather();

            }
            catch (TimeoutException timeoutEx)
            {
                using IToast toast = Toast.Make("Unable to load weather offsets. Can't download weather data.", ToastDuration.Long, 14);

                await toast.Show();
                return null;
            }

            if (msg.Equals(default(WeatherResponseMessage)))
                return null;

            foreach (int distance in distances)
            {
                var rangeRelated = relatedRecords.Where(n => n.Distance - distance == 0);

                ClickOffset? nearest = GetNearestElevationClicksToCurrentWeather(rangeRelated, msg);
                list.Add(nearest);
            }

            return list;
        }

        private ClickOffset? GetNearestElevationClicksToCurrentWeather(IEnumerable<ShootingRecord> related, WeatherResponseMessage weather)
        {
            if (SelectedRange == null)
                return null;

            if (!related.Any())
                return null;


            ShootingRecord shootingRecord = null;
            double bestScoreSoFar = double.MaxValue; // score for best match, lower is better
            foreach (ShootingRecord record in related)
            {
                if (record.ReferencedWeather == null)
                    continue;

                double tempDiff = Math.Abs((double)(weather.Temperature - record.ReferencedWeather.Temperature));
                int pressureDiff = Math.Abs((int)(weather.Pressure - record.ReferencedWeather.Pressure));
                int humidityDiff = Math.Abs((int)(weather.Humidity - record.ReferencedWeather.Humidity));

                double score = tempDiff + pressureDiff + humidityDiff;

                if (score < bestScoreSoFar)
                {
                    bestScoreSoFar = score;
                    shootingRecord = record;
                }
            }

            if (shootingRecord == null)
                return null;

            return new ClickOffset(shootingRecord.ElevationClicksOffset, shootingRecord.WindageClicksOffset);
        }

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

            ShootingRecord record = new ShootingRecord(SelectedRange.ID, SelectedSubRange.ID, SelectedFirearm.ID, 1, weather?.ID, ElevationClicks, WindageClicks, DistanceMeters, DateTime.Now.ToBinary());
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

        [RelayCommand]
        private async Task GoToDetails(ShootingRecord record)
        {
            await Shell.Current.GoToAsync("RecordDetails", new Dictionary<string, object>(1) { { "Record", record } });
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

            await RefreshChart();
        }

        async partial void OnSelectedSubRangeChanged(SubRange? value)
        {
            await SearchRecords(null);
        }

        #endregion

    }
}
