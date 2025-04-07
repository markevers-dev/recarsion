# 🚗 ReCarsion

This project contains an application capable of classifying car images based on different ML models. The application uses ML.NET for machine learning and allows users to load, apply, and manage models through reflection.

## 📝 Background of the Project

This application was made as the third assignment for the I-NotS WIN 2025 P3 course at the HAN University of Applied Sciences. It received a final grade of **10,0**.

## 🚀 Functions

- **🔧 Machine Learning Models**: The application supports loading different ML models to classify images. The models are stored in a separate folder (``Models``) and can be updated at any time. Thanks to reflection, the application automatically detects new models without the need for manual code changes.
- **🌐 XAML UI (WPF)**:
  - **🎯 Choice of ML Model**: The application provides a dropdown menu that allows users to select the model they want to use for classification. If no model is available, the user will be informed in a user-friendly way about the absence of a model.
  - **🖼️ Image Selection**: The user interface contains a button that allows users to select images. After the files are selected, the application sorts the images based on their classified categories.
 
## 🚗 Trained Car Models

The different models have been trained on two to five of the following cars:
- Audi Q3
- Hyundai Creta
- Mahindra Scorpio
- Rolls Royce Phantom
- Suzuki Swift

Depending on the model, images will be classified into different categories for these cars.
 
## ⚙️ Technical Details

- **💻 MVVM**: The application follows the MVVM (Model-View-ViewModel) pattern to ensure a separation of concerns, making the code easily testable and maintainable. This means there is no Code-Behind.
- **🔍 Reflection**: The project uses ``System.Reflection`` to dynamically load ML models, inspect available types, and instantiate new model instances. This ensures the application can handle different ML models flexibly without modifying the code.
- **🔗 Assemblies and Interfaces**: All ML models implement the same interface, allowing the application to work consistently with different models.

## 📖 How to Use

1. Launch the application and use the button to select images or folders.

2. Choose the model you want to use from the dropdown.

3. The application will automatically sort the images based on their classification by the selected model.
