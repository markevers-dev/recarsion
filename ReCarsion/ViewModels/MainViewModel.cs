using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Windows.Input;
using System.IO;
using System.Reflection;
using MLModelInterface;
using System.Windows;
using ReCarsion.Helpers;

namespace ReCarsion.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly string modelsDirectory = "Models";
        private IMLModel? _selectedModel;
        public ObservableCollection<IMLModel> AvailableModels { get; set; } = [];
        public ObservableCollection<string> SelectedFiles { get; set; } = [];
        public ObservableCollection<PredictionResult> Predictions { get; set; } = [];
        public IMLModel? SelectedModel
        {
            get => _selectedModel;
            set => SetProperty(ref _selectedModel, value);
        }

        public ICommand OpenFileCommand { get; }
        public ICommand LoadModelsCommand { get; }
        public ICommand PredictCommand { get; }
        public ICommand ShowPreviewCommand { get; }

        private FileSystemWatcher? _modelWatcher;

        private int _predictionsLoaded = 0;
        public int PredictionsLoaded
        {
            get => _predictionsLoaded;
            set => SetProperty(ref _predictionsLoaded, value);
        }

        private bool _isPredicting = false;
        public bool IsPredicting
        {
            get => _isPredicting;
            set => SetProperty(ref _isPredicting, value);
        }

        private bool _isPreviewVisible = false;
        public bool IsPreviewVisible
        {
            get => _isPreviewVisible;
            set => SetProperty(ref _isPreviewVisible, value);
        }

        public MainViewModel()
        {
            OpenFileCommand = new RelayCommand(async () => await OpenFileDialog());
            LoadModelsCommand = new RelayCommand(async () => await LoadModels());
            PredictCommand = new RelayCommand(async () => await Predict());
            ShowPreviewCommand = new RelayCommand(async showPreview =>
            {
                if (bool.TryParse(showPreview?.ToString(), out bool show))
                {
                    await TogglePreview(show);
                }
            });
            _ = LoadModels();

            InitializeFileWatcher();
        }

        private async Task TogglePreview(bool showPreview)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                IsPreviewVisible = showPreview;
            });
        }

        private void InitializeFileWatcher()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var parentDir = Directory.GetParent(baseDirectory)?.Parent?.Parent?.Parent;
            System.Diagnostics.Debug.WriteLine(parentDir);

            if (parentDir == null)
            {
                Console.WriteLine("Could not determine the parent directory for models.");
                return;
            }

            string modelsPath = Path.Combine(parentDir.FullName, modelsDirectory);

            if (!Directory.Exists(modelsPath))
            {
                Console.WriteLine("Models directory does not exist.");
                return;
            }

            _modelWatcher = new FileSystemWatcher(modelsPath, "*.dll")
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
            };

            _modelWatcher.Created += OnModelFileChanged;
            _modelWatcher.Changed += OnModelFileChanged;
            _modelWatcher.Deleted += OnModelFileChanged;
            _modelWatcher.Renamed += OnModelFileChanged;

            _modelWatcher.EnableRaisingEvents = true;
        }

        private async Task OpenFileDialog()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var parentDir = Directory.GetParent(baseDirectory)?.Parent?.Parent?.Parent;

            if (parentDir == null)
            {
                System.Diagnostics.Debug.WriteLine("Could not determine the parent directory for models.");
                return;
            }

            string initialDirectory = Path.Combine(parentDir.FullName, "CarPictures");

            if (!Directory.Exists(initialDirectory))
            {
                System.Diagnostics.Debug.WriteLine("Models directory does not exist.");
                return;
            }

            try
            {
                OpenFolderDialog openFolderDialog = new()
                {
                    Title = "Select a folder containing images",
                    InitialDirectory = initialDirectory,
                    Multiselect = false
                };

                if (openFolderDialog.ShowDialog() == true)
                {
                    List<string> imageFiles = [.. Directory.GetFiles(openFolderDialog.FolderName, "*.*", SearchOption.TopDirectoryOnly)
                                              .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                                          f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                                          f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                                          f.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))];

                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        SelectedFiles.Clear();
                        foreach (var file in imageFiles)
                        {
                            SelectedFiles.Add(file);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private SemaphoreSlim _loadModelsSemaphore = new(1, 1);
        private CancellationTokenSource _debounceCts = new();

        private void OnModelFileChanged(object sender, FileSystemEventArgs e)
        {
            _debounceCts.Cancel();
            _debounceCts = new CancellationTokenSource();

            Task.Delay(1000, _debounceCts.Token)
                .ContinueWith(async t =>
                {
                    if (!t.IsCanceled)
                    {
                        await LoadModels();
                    }
                }, TaskScheduler.Default);
        }

        private async Task LoadModels()
        {
            if (!await _loadModelsSemaphore.WaitAsync(0))
            {
                return;
            }

            try
            {
                var baseDirectory = AppContext.BaseDirectory;
                var parentDir = Directory.GetParent(baseDirectory)?.Parent?.Parent?.Parent;

                if (parentDir == null)
                {
                    Console.WriteLine("Could not determine the parent directory for models.");
                    return;
                }

                string modelsPath = Path.Combine(parentDir.FullName, modelsDirectory);

                if (!Directory.Exists(modelsPath))
                {
                    System.Diagnostics.Debug.WriteLine("Models directory does not exist");
                    return;
                }

                var modelFiles = Directory.GetFiles(modelsPath, "*.dll");
                if (modelFiles.Length == 0)
                {
                    System.Diagnostics.Debug.WriteLine("No model DLLs found");
                    await Application.Current.Dispatcher.InvokeAsync(() => AvailableModels.Clear());
                    return;
                }

                var newModels = new List<IMLModel>();
                var newModelNames = new HashSet<string>();

                foreach (var file in modelFiles)
                {
                    try
                    {
                        Assembly assembly = Assembly.LoadFrom(file);
                        var modelTypes = assembly.GetTypes()
                            .Where(t => typeof(IMLModel).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                        foreach (var type in modelTypes)
                        {
                            if (Activator.CreateInstance(type) is not IMLModel modelInstance)
                            {
                                System.Diagnostics.Debug.WriteLine($"Failed to instantiate model from type: {type.FullName}");
                                continue;
                            }

                            if (modelInstance != null && newModelNames.Add(modelInstance.Name))
                            {
                                newModels.Add(modelInstance);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"An issue occurred while loading a Model: {ex.Message}");
                    }
                }

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    var modelsToRemove = AvailableModels.Where(m => !newModelNames.Contains(m.Name)).ToList();
                    foreach (var model in modelsToRemove)
                    {
                        AvailableModels.Remove(model);
                    }

                    foreach (var model in newModels)
                    {
                        if (!AvailableModels.Any(m => m.Name == model.Name))
                        {
                            AvailableModels.Add(model);
                        }
                    }
                });
            }
            finally
            {
                _loadModelsSemaphore.Release();
            }
        }

        private async Task Predict()
        {
            if (_selectedModel == null)
            {
                Console.WriteLine("No model selected.");
                return;
            }

            Predictions.Clear();
            IsPredicting = true;
            PredictionsLoaded = 0;

            foreach (var file in SelectedFiles)
            {
                try
                {
                    string result = await Task.Run(() => _selectedModel.Predict(file));

                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        if (Predictions.FirstOrDefault(x => x.Label == result) != null)
                            Predictions.FirstOrDefault(x => x.Label == result)?.AssociatedImagePaths.Add(file);
                        else
                            Predictions.Add(new PredictionResult
                            {
                                Label = result,
                                AssociatedImagePaths = [file]
                            });
                        PredictionsLoaded++;
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error predicting for {file}: {ex.Message}");
                }
            }

            IsPredicting = false;
            PredictionsLoaded = 0;
        }
    }
}
