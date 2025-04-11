using Microsoft.ML.Data;
using Microsoft.ML;
using MLModelInterface;

namespace CarModel_ThreeCars
{
    public class CarModel : IMLModel
    {
        public string Name => "Car Model - Three Cars";

        public string Predict(string imagePath)
        {
            if (!File.Exists(imagePath))
                return "Invalid file path.";

            byte[] imageBytes = File.ReadAllBytes(imagePath);
            var input = new CarModel_ThreeCars.ModelInput { ImageSource = imageBytes };

            var predictionResult = CarModel_ThreeCars.PredictAllLabels(input);

            return predictionResult.FirstOrDefault().Key;
        }

        public List<string> GetLabels()
        {
            DataViewSchema schema = CarModel_ThreeCars.PredictEngine.Value.OutputSchema;

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
