using Compunet.YoloSharp;
using Compunet.YoloSharp.Data;
using Microsoft.ML.OnnxRuntime;
using SniperLog.Extensions;

namespace SniperLog.Services.AI;

/// <summary>
/// Service used to detect bullet holes inside of target images using YOLOv8 trained model. 
/// Predictor is lazily initialized.
/// </summary>
public class BulletHoleDetectionService
{
    /// <summary>
    /// Target image width to detect the holes in.
    /// </summary>
    public const int TargetImageHeight = 640;

    /// <summary>
    /// Target imaeg height to detect the holes in.
    /// </summary>
    public const int TargetImageWidth = 640;

    private YoloPredictor _predictor;

    /// <summary>
    /// Constructor.
    /// </summary>
    public BulletHoleDetectionService()
    {

    }

    /// <summary>
    /// Runs YOLOv8 detection on an input image.
    /// </summary>
    /// <param name="imagePath">Path to the image file.</param>
    /// <returns>Array of detected objects.</returns>
    public async Task<YoloResult<Detection>> DetectObjects(string imagePath)
    {
        if (_predictor == null)
            await InitYolo();

        return await _predictor.DetectAsync(imagePath);
    }

    private async Task InitYolo()
    {
        SessionOptions sessionOptions = new SessionOptions();

#if ANDROID
        sessionOptions.AppendExecutionProvider_Nnapi(); //Nnapi to improve performance on mobile devices
#endif

        YoloPredictorOptions options = new YoloPredictorOptions
        {
            SessionOptions = sessionOptions
        };

        _predictor = new YoloPredictor(await MauiExtensions.ReadBytesAsync("HoleDetect.onnx"), options);
    }
}

