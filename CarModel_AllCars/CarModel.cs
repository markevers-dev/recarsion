using Microsoft.ML;
using Microsoft.ML.Data;
using MLModelInterface;

namespace CarModel_AllCars
{
    public class CarModel : IMLModel
    {
        public string Name => "Car Model - All Cars";

        public string Predict(string imagePath)
        {
            if (!File.Exists(imagePath))
                return "Invalid file path.";

            byte[] imageBytes = File.ReadAllBytes(imagePath);
            CarModel_AllCars.ModelInput input = new() { ImageSource = imageBytes };

            IOrderedEnumerable<KeyValuePair<string, float>> predictionResult = CarModel_AllCars.PredictAllLabels(input);

            return predictionResult.FirstOrDefault().Key;
        }

        public List<string> GetLabels()
        {
            DataViewSchema schema = CarModel_AllCars.PredictEngine.Value.OutputSchema;

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
