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

    [ContextMenu("Полное обучение")]
    public async void TrainAll()
    {
        foreach (ImageData imgData in datasetValidator.trainImagesPaths)
        {
            if (!Application.isPlaying) return; // достаточно лишь одной этой проверки, чтобы после завершения программы, всё остановилось? 


            string path = imgData.path;
            int y = imgData.y;

            Matrix inputVector = imageProcessor.ConvertImageToInputMatrix(path, network.h[0].GetLength(1));
            network.h[0] = inputVector;

            PredictionResult res = await Task.Run(() => network.Fit(y));    // Ждём завершения

            Debug.Log($"Ошибка: {res.Error} | Истинная категория: {res.TrueLabelIndex} | Предсказано: {res.PredictedCategoryIndex}");

            networkVizualizer.Vizualize();
        }
    }
}