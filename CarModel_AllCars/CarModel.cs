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
            var input = new CarModel_AllCars.ModelInput { ImageSource = imageBytes };

            var predictionResult = CarModel_AllCars.PredictAllLabels(input);

            return predictionResult.FirstOrDefault().Key;
        }
    }
}
