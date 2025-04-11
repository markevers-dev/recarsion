namespace MLModelInterface
{
    public interface IMLModel
    {
        string Name { get; }
        string Predict(string imagePath);
        List<string> GetLabels();
    }
}
