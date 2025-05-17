using System;
using System.IO;
using System.Threading;
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
    [Inject] CategoryManager categoryManager;

    private CancellationTokenSource cancellationTokenSource;

    private void Awake()
    {
        cancellationTokenSource = new CancellationTokenSource();
    }

    [ContextMenu("Полное обучение.")]
    public async void TrainAll()
    {
        if (!datasetValidator.isValid) {
            Debug.LogWarning("Сначала валидируйте датасет");
            return;
        }

        if (!network.isReady)
        {
            Debug.LogWarning("Сначала укажите число слоёв и нейронов.");
            return;
        }

        // При каждом запуске создаём новый источник
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
        cancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = cancellationTokenSource.Token;

        int count = 0;
        try
        {
            foreach (ImageData imgData in datasetValidator.trainImagesPaths)
            {
                if (!Application.isPlaying) return;
                token.ThrowIfCancellationRequested();   // проверка на остановку обучения

                string path = imgData.path;
                int y = imgData.y;

                Matrix inputVector = imageProcessor.ConvertImageToInputMatrix(path, network.h[0].Columns);
                network.h[0] = inputVector;

                PredictionResult res = await Task.Run(() => network.Fit(y), token); // передаём token в Task.Run
                networkVizualizer.Vizualize();

                Debug.Log($"[{count++}/{datasetValidator.trainImagesPaths.Count}] Ошибка: {res.Error} | Истинная категория: {res.TrueLabelIndex} | Предсказано: {categoryManager.GetName(res.PredictedCategoryIndex)}");
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Асинхронная операция отменена.");
        }
    }


    [ContextMenu("Прекратить обучение.")]
    public void StopTraining()
    {
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }
    }
}