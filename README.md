# Sniper Log

## About
Sniper Log is an Android app that allows you to keep track of guns, ranges, rifle scopes and ammunition, as well as record target shots with corrections, pictures and notes. The data is analyzed with a ballistic calculator and sight correction algorithms that take into account atmospheric conditions and the shooter's experience.

The app automatically records environmental conditions such as temperature, pressure, humidity, wind speed and direction when connected to the internet but is also capable of operating completely offline. 
AI hit detection enables automatic detection and evaluation of holes in the target, making it easier to analyze shooting accuracy.

AI hit detection is done using YOLOv11 and it's jupyter notebook with information and python code for training can be found [here](https://colab.research.google.com/drive/1OZlh3nJkv5w-3_o3jchv7XCoZKgQrnhW?usp=sharing).

<table>
  <tr>
    <td><img src="https://github.com/user-attachments/assets/f4c3e6df-dc51-40f7-b40c-6aade6fa99a4" width="230"/></td>
    <td><img src="https://github.com/user-attachments/assets/630fb23e-440e-4d86-9723-092f0d4609e4" width="230"/></td>
    <td><img src="https://github.com/user-attachments/assets/f5f7eaa2-9d72-4b1d-8342-103d1e87153a" width="230"/></td>
    <td><img src="https://github.com/user-attachments/assets/ed2231bf-c089-42f5-a94a-1db1a6ceb79f" width="230"/></td>
  </tr>
</table>

## Installation
### Windows
#### Requirements
- Windows 10.0.17763
- .NET 8 runtime

#### Installation
Steps:
- Download the Windows Build in the [releases](https://github.com/davidsebesta1/SniperLog/releases).<br>
- Start using .exe file.
### Android
#### Requirements
- Android 5.0 (API 24) Minimum
  
#### Installation
Steps:
- Download the Android signed .apk file in [releases](https://github.com/davidsebesta1/SniperLog/releases)
- Install it on your local android device.

## Bug Reporting
Use the [issues page](https://github.com/davidsebesta1/SniperLog/issues) to report any bugs.

## License
See [LICENSE](https://github.com/davidsebesta1/SniperLog/blob/master/LICENSE)

## Tech stack & Tools
Used framework [.NET MAUI](https://dotnet.microsoft.com/en-us/apps/maui)<br>
Designed in [figma](https://www.figma.com/design/2ca9TYEWmeJG5Sesapl2VD/SniperLog?node-id=0-1&t=CFNcEpz8xl3NrZd5-1)<br>
Ballistic calculator by [gehtsoft-usa](https://github.com/gehtsoft-usa/BallisticCalculator1)<br>
Labelling images for AI using [LabelImg](https://github.com/HumanSignal/labelImg)<br>
Bullet hole detection AI powered by [YOLO11](https://docs.ultralytics.com/models/yolo11/)<br>
YOLOv8 usage NuGet via [YoloSharp](https://github.com/dme-compunet/YoloSharp)<br>
Configuration using YAML Nuget from [YamlDotNet](https://github.com/aaubry/YamlDotNet/wiki)<br>
Popup NuGet from [Mopups](https://github.com/LuckyDucko/Mopups)<br>
Charts UI via [LiveCharts2](https://github.com/beto-rodriguez/LiveCharts2)<br>
Weather API from [OpenWeatherMap](https://openweathermap.org/)

## Contributing
If you have suggestions, feature requests, or want to contribute, feel free to open a pull request or an issue.
