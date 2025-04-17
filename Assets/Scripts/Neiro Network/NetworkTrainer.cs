using System.IO;
using System.Threading.Tasks;
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
    [Inject] ImageProcessor imageProcessor; // Sinleton, т.к. он не наследуется от MonoBehaviour

    private int currentImageIndex = 0;


    [ContextMenu("Один шаг обучения")]
    public async void Fit()
    {
        if (currentImageIndex >= datasetValidator.trainImagesPaths.Count)
        {
            Debug.LogWarning("Обучение завершено. Все изображения обработаны.");
            return;
        }

        // Получаем данные для изображения
        ImageData imgData = datasetValidator.trainImagesPaths[currentImageIndex];
        int trueLabel = imgData.y;
        string path = imgData.path;

        // Подготовка входных данных
        int width = network.h[0].GetLength(1);
        network.h[0] = imageProcessor.ConvertImageToInputMatrix(path, width);

        // Асинхронное выполнение обучения нейросети
        PredictionResult res = await Task.Run(() => network.Fit(trueLabel));

        // Логирование результата
        Debug.Log($"[{currentImageIndex + 1}/{datasetValidator.trainImagesPaths.Count}] " +
                  $"Ошибка: {res.Error} | Истинная категория: {res.TrueLabelIndex} | Предсказано: {res.PredictedCategoryIndex}");

        // Визуализация после обучения
        networkVizualizer.Vizualize();

        // Переход к следующему изображению
        currentImageIndex++;
    }

    // [ContextMenu("Полное обучение")]
    async void TrainAll()
    {
        currentImageIndex = 0;

        while (currentImageIndex < datasetValidator.trainImagesPaths.Count)
        {
            // Дожидаемся завершения каждого шага обучения
            await Task.Run(() => Fit());
        }

        Debug.Log("Полное обучение завершено.");
    }

    // Пока не используется
    void TestNetwork()
    {

    }
}
