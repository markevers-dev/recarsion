using MLModelInterface;

namespace CarModel_TwoCars
{
    public class CarModel : IMLModel
    {
        public string Name => "Car Model - Two Cars";

        public string Predict(string imagePath)
        {
            if (!File.Exists(imagePath))
                return "Invalid file path.";

            byte[] imageBytes = File.ReadAllBytes(imagePath);
            var input = new CarModel_TwoCars.ModelInput { ImageSource = imageBytes };

            var predictionResult = CarModel_TwoCars.PredictAllLabels(input);

            return predictionResult.FirstOrDefault().Key;
        }
    }
}
