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
    }
}
