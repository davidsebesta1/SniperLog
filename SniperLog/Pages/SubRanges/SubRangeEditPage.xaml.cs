using Mopups.Pages;
using Mopups.Services;
using SniperLog.Extensions;
using SniperLog.Extensions.Maui;
using SniperLog.Models;

namespace SniperLog.Pages.SubRanges;

public partial class SubRangeEditPage : PopupPage
{
    private SubRange _subRange;

    private readonly Dictionary<string, bool> _valuesValidation = new Dictionary<string, bool>()
    {
        {"RangeInMeters", false },
        {"Altitude", true },
        {"DirectionToNorth", true },
        {"VerticalFiringOffsetDegrees", true },
        {"NotesRelativePathFromAppData", true }
    };
    public SubRangeEditPage(SubRange subrange)
    {
        InitializeComponent();
        _subRange = subrange;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        RangeInMetersLabel.Text = _subRange.RangeInMeters.ToString();
        AltitudeLabel.Text = _subRange.Altitude.ToString();
        DirectionToNorthLabel.Text = _subRange.DirectionToNorth.ToString();
        VerticalFiringOffsetDegreesLabel.Text = _subRange.VerticalFiringOffsetDegrees.ToString();

        if (!string.IsNullOrEmpty(_subRange.NotesRelativePathFromAppData))
        {
            string path = AppDataFileHelper.GetPathFromAppData(_subRange.NotesRelativePathFromAppData);
            if (Path.Exists(path))
            {
                NotesRelativePathFromAppDataLabel.Text = File.ReadAllText(path);
            }
        }
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            await MopupService.Instance.PopAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.ToString(), "Okay");
        }
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        if (_valuesValidation.Values.Any(n => n == false))
        {
            await Shell.Current.DisplayAlert("Error", "Please fill out mandatory fields and fix any invalid inputs", "Okay");
            return;
        }

        int rangeInMeters;
        double? altitude;
        double? directionToNorth;
        double? verticalFiringOffsetDegrees;
        string? notes;

        try
        {
            rangeInMeters = int.Parse(RangeInMetersLabel.Text);
            altitude = !string.IsNullOrEmpty(AltitudeLabel.Text) ? double.Parse(AltitudeLabel.Text) : 0d;
            directionToNorth = !string.IsNullOrEmpty(DirectionToNorthLabel.Text) ? double.Parse(DirectionToNorthLabel.Text) : 0d;
            verticalFiringOffsetDegrees = !string.IsNullOrEmpty(VerticalFiringOffsetDegreesLabel.Text) ? double.Parse(VerticalFiringOffsetDegreesLabel.Text) : 0d;
            notes = NotesRelativePathFromAppDataLabel.Text;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.ToString(), "Okay");
            return;
        }

        if (!string.IsNullOrEmpty(notes))
        {
            await _subRange.SaveNotes(notes);
        }

        _subRange.RangeInMeters = rangeInMeters;
        _subRange.Altitude = altitude;
        _subRange.DirectionToNorth = directionToNorth;
        _subRange.VerticalFiringOffsetDegrees = verticalFiringOffsetDegrees;
        await _subRange.SaveAsync();
        await MopupService.Instance.PopAsync();
    }

    private void RangeInMetersLabel_TextChanged(object sender, TextChangedEventArgs args)
    {
        InputValidatorHelper.ValidationCheck(args.NewTextValue, RangeInMetersErrText, _valuesValidation, "RangeInMeters", n => double.TryParse(n, out double val) && val >= 0);
    }

    private void AltitudeLabel_TextChanged(object sender, TextChangedEventArgs args)
    {
        InputValidatorHelper.ValidationCheck(args.NewTextValue, AltitudeErrText, _valuesValidation, "Altitude", n =>
        {
            if (double.TryParse(n, out double val))
            {
                return val >= 0d;
            }

            return true;
        });
    }

    private void DirectionToNorthLabel_TextChanged(object sender, TextChangedEventArgs args)
    {
        InputValidatorHelper.ValidationCheck(args.NewTextValue, DirectionToNorthErrText, _valuesValidation, "DirectionToNorth", n =>
        {
            return true;
        });
    }

    private void VerticalFiringOffsetDegreesLabel_TextChanged(object sender, TextChangedEventArgs args)
    {
        InputValidatorHelper.ValidationCheck(args.NewTextValue, VerticalFiringOffsetDegreesErrText, _valuesValidation, "VerticalFiringOffsetDegrees", n =>
        {
            return true;
        });
    }

    private void NotesRelativePathFromAppDataLabel_TextChanged(object sender, TextChangedEventArgs args)
    {
        InputValidatorHelper.ValidationCheck(args.NewTextValue, VerticalFiringOffsetDegreesErrText, _valuesValidation, "VerticalFiringOffsetDegrees", n =>
        {
            return true;
        });
    }

}