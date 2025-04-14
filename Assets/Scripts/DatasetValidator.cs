using System;
using System.Collections.Generic;
using UnityEngine;
using SFB; // Standalone File Browser (Windows)
using System.IO;
using System.Linq;
using System.Collections;
using Zenject;
using UnityEngine.Events;

/// <summary>
/// Пара "Категория - Кол-во изображений".
/// </summary>
[Serializable]
public class CategoryInfo
{
    [Tooltip("Категория изображений")]
    public string name;

    [Tooltip("Количество изображений")]
    public int count;
}

/// <summary>
/// Проверяет, корректно ли организован датасет изображений для классификации.
/// </summary>
[AddComponentMenu("Custom/Dataset Validator (Проверка датасета)")]
public class DatasetValidator : MonoBehaviour
{
    [Tooltip("Контроллер всплывающих панелей.")]
    [Inject] PopUpPanelsController popUpPanelsController;
    /* ---------------------- Общая информация ----------------------*/
    [Header("Общая информация:")]
    [Tooltip("Названия категорий (общие для train и test).")]
    public List<string> categoryNames;

    [Header("Прогресс загрузки:")]
    [Range(0, 1)] public float loadProgress;

    /* ---------------------- Обучающая выборка ----------------------*/
    [Header("Обучающая выборка:")]
    [Tooltip("Кол-во изображений в обучающей выборке.")]
    public int trainTotal;

    [Tooltip("Количество изображений в каждой категории (обучающая выборка).")]
    public List<CategoryInfo> trainCategories;

    /* ---------------------- Тестовая выборка ----------------------*/
    [Header("Тестовая выборка:")]
    [Tooltip("Кол-во изображений в тестовой выборке.")]
    public int testTotal;

    [Tooltip("Количество изображений в каждой категории (тестовая выборка).")]
    public List<CategoryInfo> testCategories;

    /* ---------------------- Итог ----------------------*/
    [Header("Итог:")]
    [Tooltip("Общее разрешение изображений.")]
    public Vector2Int imageSize;

    [Tooltip("Вердикт проверки.")]
    [TextArea]
    public string verdict;

    [Tooltip("Можно ли использовать датасет.")]
    public bool isValid = false;

    [Tooltip("Готовность датасета к использованию.")]
    public UnityAction OnReady;

    [ContextMenu("Проверить датасет")]
    public void ValidateDataset()
    {
        var paths = StandaloneFileBrowser.OpenFolderPanel("Выбор папки с датасетом", "", false);
        if (paths.Length == 0 || string.IsNullOrEmpty(paths[0])) return;

        string datasetPath = paths[0];

        string trainPath = Directory.GetDirectories(datasetPath).FirstOrDefault(p => Path.GetFileName(p).ToLower() == "train");
        string testPath = Directory.GetDirectories(datasetPath).FirstOrDefault(p => Path.GetFileName(p).ToLower() == "test");

        if (trainPath == null || testPath == null)
        {
            verdict = "В папке должны быть подпапки 'train' и 'test'.";
            isValid = false;
            return;
        }

        StartCoroutine(Validate(trainPath, testPath));
    }

    /// <summary>
    /// Проверяем обучающий и тестовый наборы фотографий.
    /// </summary>
    private IEnumerator Validate(string trainPath, string testPath)
    {
        popUpPanelsController.ShowPanel("Загрузка датасета");
        loadProgress = 0;

        int trainImageCount = CountImages(trainPath);
        int testImageCount = CountImages(testPath);
        int totalImages = trainImageCount + testImageCount;

        float trainWeight = (float)trainImageCount / totalImages;
        float testWeight = (float)testImageCount / totalImages;

        var trainResult = new CategoryAnalysisResult();
        yield return StartCoroutine(AnalyzeCategory(trainPath, progress => loadProgress = progress * trainWeight, trainResult));

        var testResult = new CategoryAnalysisResult();
        yield return StartCoroutine(AnalyzeCategory(testPath, progress => loadProgress = trainWeight + progress * testWeight, testResult));

        if (trainResult.sizeMismatch || testResult.sizeMismatch)
        {
            verdict = "Невозможно использовать этот датасет. Все изображения должны быть одинакового разрешения!";
            isValid = false;
            OnReady?.Invoke();
            yield break;
        }

        imageSize = trainResult.imageSize;
        trainCategories = trainResult.categories;
        trainTotal = trainResult.totalCount;

        testCategories = testResult.categories;
        testTotal = testResult.totalCount;

        categoryNames = trainCategories.Select(c => c.name).ToList();

        if (!Enumerable.SequenceEqual(categoryNames, testCategories.Select(c => c.name)))
        {
            verdict = "Невозможно использовать этот датасет. Категории в папках train и test не совпадают!";
            isValid = false;
            OnReady?.Invoke();
            yield break;
        }

        bool balanced = trainCategories.Select(c => c.count).Distinct().Count() == 1;

        isValid = true;
        verdict = balanced
            ? "Датасет корректен и сбалансирован."
            : "Датасет допустим к использованию, однако категории содержат неодинаковое количество изображений.";

        OnReady?.Invoke();
    }
    private IEnumerator AnalyzeCategory(string path, Action<float> onProgress, CategoryAnalysisResult result)
    {
        var categories = Directory.GetDirectories(path);
        int totalImages = categories.Sum(cat => Directory.GetFiles(cat)
            .Count(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg")));

        int processedImages = 0;

        foreach (var category in categories)
        {
            string catName = Path.GetFileName(category);
            string[] images = Directory.GetFiles(category)
                .Where(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg"))
                .ToArray();

            result.categories.Add(new CategoryInfo { name = catName, count = images.Length });
            result.totalCount += images.Length;

            foreach (var imgPath in images)
            {
                byte[] data = File.ReadAllBytes(imgPath);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(data);

                if (result.imageSize == Vector2Int.zero)
                    result.imageSize = new Vector2Int(tex.width, tex.height);
                else if (result.imageSize.x != tex.width || result.imageSize.y != tex.height)
                    result.sizeMismatch = true;

                UnityEngine.Object.Destroy(tex);

                processedImages++;
                onProgress?.Invoke((float)processedImages / totalImages);

                if (processedImages % 65 == 0)
                    yield return null;
            }
        }

        onProgress?.Invoke(1.0f);
    }
    private int CountImages(string path)
    {
        return Directory.GetDirectories(path)
            .Sum(cat => Directory.GetFiles(cat)
                .Count(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg")));
    }


    private class CategoryAnalysisResult
    {
        public List<CategoryInfo> categories = new();
        public int totalCount = 0;
        public Vector2Int imageSize = Vector2Int.zero;
        public bool sizeMismatch = false;
    }
}



