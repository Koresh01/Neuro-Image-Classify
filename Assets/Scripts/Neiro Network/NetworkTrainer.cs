using System.IO;
using UnityEngine;
using Zenject;

/// <summary>
/// Компонент для пошагового и полного обучения нейросети на датасете изображений.
/// </summary>
[AddComponentMenu("Custom/NetworkTrainer (Тренер нейросети)")]
class NetworkTrainer : MonoBehaviour
{
    [Inject] DatasetValidator datasetValidator;
    [Inject] NetworkVizualizer networkVizualizer;
    [Inject] Network network;

    // Индекс текущего изображения для поэтапного обучения
    private int currentImageIndex = 0;

    /// <summary>
    /// Преобразует изображение из файла в входной вектор (матрицу) для нейросети.
    /// </summary>
    Matrix ConvertImageToInputMatrix(string path)
    {
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D imgTexture = new Texture2D(2, 2);
        imgTexture.LoadImage(fileData);

        Matrix inputImageMatrix = Numpy.Zeros(1, network.h[0].GetLength(1));
        for (int x = 0; x < imgTexture.width; x++)
        {
            for (int y = 0; y < imgTexture.height; y++)
            {
                int i = y * imgTexture.width + x;

                Color pixel = imgTexture.GetPixel(x, y);
                inputImageMatrix[0, i] = (pixel.r + pixel.g + pixel.b) / 3f;
            }
        }

        return inputImageMatrix;
    }

    /// <summary>
    /// Выполняет один шаг обучения на следующем изображении из датасета.
    /// Можно вызывать через контекстное меню в редакторе Unity.
    /// </summary>
    [ContextMenu("Один шаг обучения")]
    void Fit()
    {
        if (currentImageIndex >= datasetValidator.trainImagesPaths.Count)
        {
            Debug.LogWarning("Обучение завершено. Все изображения обработаны.");
            return;
        }

        ImageData imgData = datasetValidator.trainImagesPaths[currentImageIndex];
        int trueLabel = imgData.y;
        string path = imgData.path;

        // Подготовка входных данных
        network.h[0] = ConvertImageToInputMatrix(path);

        // Обучение
        PredictionResult res = network.Fit(trueLabel);
        Debug.Log($"[{currentImageIndex + 1}/{datasetValidator.trainImagesPaths.Count}] " +
                  $"Ошибка: {res.Error} | Истинная категория: {res.TrueLabelIndex} | Предсказано: {res.PredictedCategoryIndex}");

        // Визуализация
        networkVizualizer.Vizualize();

        currentImageIndex++;
    }

    /// <summary>
    /// Запускает полное обучение на всех изображениях из датасета.
    /// </summary>
    // [ContextMenu("Полное обучение")]
    void TrainAll()
    {
        currentImageIndex = 0;

        while (currentImageIndex < datasetValidator.trainImagesPaths.Count)
        {
            Fit();
        }

        Debug.Log("Полное обучение завершено.");
    }

    // Пока не используется
    void TestNetwork()
    {

    }
}
