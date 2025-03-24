using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Image;
using Microsoft.ML.Vision;
using System;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        string datasetPath = @"C:\path\to\dataset";
        string modelPath = "model.zip";

        MLContext mlContext = new MLContext();

        var data = mlContext.Data.LoadFromEnumerable(
            Directory.GetFiles(datasetPath, "*", SearchOption.AllDirectories)
                .Select(path => new ImageData { ImagePath = path, Label = Path.GetFileName(Path.GetDirectoryName(path)) })
        );

        var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", "Label")
            .Append(mlContext.Transforms.LoadImages("Image", datasetPath, nameof(ImageData.ImagePath)))
            .Append(mlContext.Transforms.ResizeImages("Image", ImageClassificationTrainer.Architecture.ResNet18, 224, 224))
            .Append(mlContext.Transforms.ExtractPixels("Image"))
            .Append(mlContext.Model.ImageClassification("Image", "Label", validationSetFraction: 0.2));

        var model = pipeline.Fit(data);

        mlContext.Model.Save(model, data.Schema, modelPath);
        Console.WriteLine($"Model saved to {modelPath}");
    }
}

public class ImageData
{
    public string ImagePath { get; set; }
    public string Label { get; set; }
}

