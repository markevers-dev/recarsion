namespace MLModelInterface
{
    public interface IMLModel
    {
        string Name { get; }
        string Predict(string imagePath);
    }
}
