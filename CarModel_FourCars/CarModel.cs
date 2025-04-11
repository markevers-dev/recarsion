using Microsoft.ML.Data;
using Microsoft.ML;
using MLModelInterface;

namespace CarModel_FourCars
{
    public class CarModel : IMLModel
    {
        public string Name => "Car Model - Four Cars";

        public string Predict(string imagePath)
        {
            if (!File.Exists(imagePath))
                return "Invalid file path.";

            byte[] imageBytes = File.ReadAllBytes(imagePath);
            var input = new CarModel_FourCars.ModelInput { ImageSource = imageBytes };

            var predictionResult = CarModel_FourCars.PredictAllLabels(input);

            return predictionResult.FirstOrDefault().Key;
        }

        public List<string> GetLabels()
        {
            DataViewSchema schema = CarModel_FourCars.PredictEngine.Value.OutputSchema;

            VBuffer<ReadOnlyMemory<char>> labelBuffer = new();

            DataViewSchema.Column? labelColumn = schema.GetColumnOrNull("Label");
            if (labelColumn == null)
                return [];

            labelColumn.Value.GetKeyValues(ref labelBuffer);

            List<string> _categoryLabels = [.. labelBuffer.DenseValues().Select(x => x.ToString())];

            return _categoryLabels;
        }
    }
}
