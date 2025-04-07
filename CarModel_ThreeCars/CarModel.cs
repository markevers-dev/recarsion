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
    }
}
