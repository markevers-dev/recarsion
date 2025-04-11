using System.Collections.ObjectModel;

namespace ReCarsion
{
    public class PredictionResult
    {
        public required string Label { get; set; }
        public required ObservableCollection<string> AssociatedImagePaths { get; set; }
    }
}
