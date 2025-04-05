using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SniperLog.Config;
using SniperLog.Extensions.WrapperClasses;
using SniperLog.Pages.Firearms.MuzzleVelocities;
using SniperLog.Pages.Records;
using SniperLog.Services.Ballistics;
using SniperLogNetworkLibrary;
using System.Collections.ObjectModel;

namespace SniperLog.ViewModels.Records;

/// <summary>
/// Viewmodel for handling <see cref="RecordsPage"/>.
/// </summary>
public partial class RecordsPageViewModel : BaseViewModel
{
    #region Services

    private readonly DataCacherService<ShootingRange> _shootingRangesCacher;
    private readonly DataCacherService<SubRange> _subrangeCacher;
    private readonly DataCacherService<Firearm> _firearmsCacher;
    private readonly DataCacherService<ShootingRecord> _shootingRecordsCacher;
    private readonly DataCacherService<Ammunition> _ammunitonsCacher;

    private readonly ValidatorService _validatorService;

    #endregion

    #region Properties

    #region Trajectories

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
    }];

    /// <summary>
    /// Y axis of the graph.
    /// </summary>
    [ObservableProperty]
    private List<Axis> _yAxisesWindage = [new Axis()
    {
        NamePaint = new SolidColorPaint(SKColors.White),
        LabelsPaint = new SolidColorPaint(SKColors.White),
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

    #endregion

    #region Selectable shooting ranges and firearms

    /// <summary>
    /// Collection of available ammunition.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<Ammunition> _ammunitions;

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

    #endregion

    #region New record inputs
    /// <summary>
    /// User input for the a new record.
    /// </summary>
    [ObservableProperty]
    private Ammunition _selectedAmmunition;

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

    #endregion

    #region Table

    /// <summary>
    /// User input for the a new record.
    /// </summary>
    [ObservableProperty]
    private DateTime? _dateSearchVal;

    #endregion

    #endregion

    #region Ctor

    public RecordsPageViewModel(DataCacherService<ShootingRange> shootingRangesCacher, DataCacherService<SubRange> subrangeCacher, DataCacherService<Firearm> firearmsCacher, DataCacherService<ShootingRecord> shootingRecordsCacher, DataCacherService<Ammunition> ammoCacher, ValidatorService validatorService)
    {
        PageTitle = "Records";

        _shootingRangesCacher = shootingRangesCacher;
        _subrangeCacher = subrangeCacher;
        _firearmsCacher = firearmsCacher;
        _shootingRecordsCacher = shootingRecordsCacher;
        _ammunitonsCacher = ammoCacher;

        _validatorService = validatorService;
    }

    #endregion

    #region Commands & Methods

    /// <summary>
    /// Refreshes the elevation and windage chart.
    /// </summary>
    public async Task RefreshChart()
    {
        try
        {
            if (SelectedFirearm is null)
                return;

            ObservableCollection<FirearmSightSetting> baseSightSettings = await SelectedFirearm.ReferencedFirearmSight.GetBaseSightSettingsAsync();
            var ord = baseSightSettings.OrderBy(static n => n.Distance);
            IEnumerable<int> distances = ord.Select(static n => n.Distance);

            XAxises[0].Labels.Clear();

            foreach (int item in distances)
                XAxises[0].Labels.Add(item.ToString());

            NetworkAccess accessType = Connectivity.Current.NetworkAccess;
            if (accessType == NetworkAccess.None)
                throw new IOException("Unable to load weather offsets. No internet.");

            WeatherResponseMessage msg = SelectedRange == null ? default : await SelectedRange.GetCurrentWeather();

            List<ClickOffset?>? weatherOffsets = await GetNearestWeatherClicksFromBase(distances, msg);
            ElevationSeries.Clear();

            // Base clicks
            ElevationSeries.Add(new LineSeries<int>
            {
                Values = ord.Select(n => n.ElevationValue).ToList(),
                Name = "Base",
                Stroke = new SolidColorPaint(SKColor.Parse("#6281EE")),
                GeometryStroke = new SolidColorPaint(SKColor.Parse("#6281EE")),
                Fill = null,
                LineSmoothness = 0,

            });

            // Weather offsets
            if (weatherOffsets != null)
            {
                ElevationSeries.Add(new LineSeries<int?>
                {
                    Values = weatherOffsets.Where(n => n.HasValue && n.Value.VerticalClicks != null).Select(n => n.Value.VerticalClicks).ToList(),
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
                    Values = weatherOffsets.Where(n => n.HasValue && n.Value.WindageClicks != null).Select(n => n.Value.WindageClicks).ToList(),
                    Name = "Base",
                    Stroke = new SolidColorPaint(SKColor.Parse("#EE8100")),
                    GeometryStroke = new SolidColorPaint(SKColor.Parse("#EE8100")),
                    Fill = null,
                    LineSmoothness = 0,
                });
            }

            // Calculated offsets
            if (SelectedRange != null)
            {
                BallisticCalculatorService ballisticCalculatorService = new BallisticCalculatorService();

                List<ClickOffset> offset = null;
                try
                {
                    offset = await ballisticCalculatorService.CalculateOffset(SelectedFirearm, SelectedAmmunition, msg, 100, 300, 50, SelectedFirearm.ReferencedFirearmSight.OneClickValue);
                }
                catch (ArgumentException e)
                {
                    bool goToSettings = await Shell.Current.DisplayAlert("Warning", e.Message, "Go to settings", "Okay");

                    if (goToSettings)
                        await Shell.Current.GoToAsync(nameof(MuzzleVelocitiesPage), new Dictionary<string, object>(1) { { "Firearm", SelectedFirearm } });

                    return;
                }

                ElevationSeries.Add(new LineSeries<int?>
                {
                    Values = offset.Where(n => distances.Any(x => x == n.Distance)).Select(static n => n.VerticalClicks).ToList(),
                    Name = "Base",
                    Stroke = new SolidColorPaint(SKColor.Parse("#FF0000")),
                    GeometryStroke = new SolidColorPaint(SKColor.Parse("#FF0000")),
                    Fill = null,
                    LineSmoothness = 0,
                });

                WindageSeries.Add(new LineSeries<int?>
                {
                    Values = offset.Where(n => distances.Any(x => x == n.Distance)).Select(static n => n.WindageClicks).ToList(),
                    Name = "Base",
                    Stroke = new SolidColorPaint(SKColor.Parse("#FF0000")),
                    GeometryStroke = new SolidColorPaint(SKColor.Parse("#FF0000")),
                    Fill = null,
                    LineSmoothness = 0,
                });
            }
        }
        catch (TimeoutException timeoutEx)
        {
            IToast toast = Toast.Make("Unable to retrive weather info", ToastDuration.Long);
            await toast.Show();
        }
        catch (Exception e)
        {
            await Shell.Current.DisplayAlert("Error", e.Message, "Okay");
        }
    }

    public async Task<List<ClickOffset?>> GetNearestWeatherClicksFromBase(IEnumerable<int> distances, WeatherResponseMessage msg)
    {
        if (SelectedFirearm == null)
            return null;

        if (SelectedRange == null)
            return null;

        if (msg.Equals(default(WeatherResponseMessage)))
            return null;

        ObservableCollection<ShootingRecord> relatedRecords = await _shootingRecordsCacher.GetAllBy(n => n.ReferencedFirearm == SelectedFirearm);
        int count = distances.Count();

        List<ClickOffset?> list = new List<ClickOffset?>(count);

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

        return new ClickOffset(shootingRecord.ElevationClicksOffset, shootingRecord.WindageClicksOffset, related.First().Distance);
    }

    [RelayCommand]
    private async Task RefreshEntries(bool resetSubRange = false)
    {
        Ammunitions = await _ammunitonsCacher.GetAll();
        ShootingRanges = await _shootingRangesCacher.GetAll();
        SubRanges = await _subrangeCacher.GetAllBy(n => SelectedRange != null && n.ShootingRange_ID == SelectedRange.ID);

        if (resetSubRange)
            SelectedSubRange = null;

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
    private async Task Appearing()
    {
        PreferencesConfig config = ApplicationConfigService.GetConfig<PreferencesConfig>();

        Firearm? lastSaved = await _firearmsCacher.GetFirstBy(n => n.ID == config.LastSelectedFirearm);

        if (lastSaved != SelectedFirearm)
        {
            SelectedFirearm = lastSaved;
        }

        await RefreshChart();
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
            await RefreshChart();
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

        ShootingRecord record = new ShootingRecord(SelectedRange.ID, SelectedSubRange.ID, SelectedFirearm.ID, SelectedAmmunition.ID, weather?.ID, ElevationClicks, WindageClicks, DistanceMeters, DateTime.Now.ToBinary());
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
        ApplicationConfigService.GetConfig<PreferencesConfig>().SetLastSelectedFirearm(SelectedFirearm);
        SelectedFirearm.SetLastSelectedAmmunition(SelectedAmmunition);

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
        await Shell.Current.GoToAsync(nameof(RecordDetailsPage), new Dictionary<string, object>(1) { { "Record", record } });
    }

    async partial void OnSelectedFirearmChanged(Firearm? value)
    {
        await SearchRecords(null);
        await RefreshChart();

        if (value != null)
            SelectedAmmunition = await value.GetLastSelectedAmmunition();
        else
            SelectedAmmunition = null;
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
