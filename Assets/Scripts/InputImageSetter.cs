
using Zenject;
using UnityEngine;
using SFB;

/// <summary>
/// Устанавливает изображение на входной слой нейросети.
/// </summary>
class InputImageSetter : MonoBehaviour
{
    [Inject] Network network;
    [Inject] NetworkVizualizer networkVizualizer;
    [Inject] ImageProcessor imageProcessor; // Sinleton
    [Inject] CategoryManager categoryManager;

    /// <summary>
    /// Устанавливает изображение на вход нейросети.
    /// </summary>
    public void SetInputImg()
    {
        string path = GetImgPath();

        Matrix inputVector = imageProcessor.ConvertImageToInputMatrix(path, network.h[0].Columns);
        network.h[0] = inputVector;

        network.ForwardPropogation();
        Matrix lastLayer = network.t[^1];  // последний слой, к которому не применялась функция активации.
        
        // Получаем вектор вероятностей.
        Matrix z = MatrixUtils.SoftMax(lastLayer);
        int predicted_Y = Numpy.argmax(z);  // Индекс категории, предсказанный нейросетью
        networkVizualizer.Vizualize();

        Debug.Log($"Предсказано: {categoryManager.GetName(predicted_Y)}");
    }

    string GetImgPath()
    {
        // Открываем окно выбора изображения
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Выбери изображение", "", new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg", "bmp")
        }, false);

        if (paths.Length == 0 || string.IsNullOrEmpty(paths[0]))
        {
            Debug.LogWarning("Изображение не выбрано.");
            return string.Empty;
        }

        string path = paths[0];
        return path;
    }
}