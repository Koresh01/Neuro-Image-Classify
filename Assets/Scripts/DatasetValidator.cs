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
    [Range(0, 1)] public float trainProgress;
    [Range(0, 1)] public float testProgress;

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

        // Запускаем анализ параллельно
        StartCoroutine(ValidateAsync(trainPath, testPath));
    }

    /// <summary>
    /// Параллельно проверяем обучающий и тестовый наборы фотографий
    /// </summary>
    private IEnumerator ValidateAsync(string trainPath, string testPath)
    {
        // Отображаем пользователю панель загрузки:
        popUpPanelsController.ShowPanel("Загрузка датасета");

        trainProgress = 0;
        testProgress = 0;

        Vector2Int size = Vector2Int.zero;  // Размерность входных изображений [пикс].
        
        // Несовпадения в размерностях изображений.
        bool sizeMismatch1 = false; // -> в обучающей выборке
        bool sizeMismatch2 = false; // -> в тестовой выборке

        // Индикаторы завершения обоих корутин.
        bool trainDone = false;
        bool testDone = false;

        StartCoroutine(AnalyzeCategoryFolderAsync(trainPath, v => trainProgress = v, (list, total, s, mismatch) =>
        {
            trainCategories = list;
            trainTotal = total;
            size = s;
            sizeMismatch1 = mismatch;
            trainDone = true;
        }));

        StartCoroutine(AnalyzeCategoryFolderAsync(testPath, v => testProgress = v, (list, total, _, mismatch) =>
        {
            testCategories = list;
            testTotal = total;
            sizeMismatch2 = mismatch;
            testDone = true;
        }));

        while (!trainDone || !testDone)
            yield return null;

        // Размер входного изображения:
        if (sizeMismatch1 || sizeMismatch2)
        {
            verdict = "Невозможно использовать этот датасет. Все изображения должны быть одинакового разрешения!";
            isValid = false;
            OnReady?.Invoke();
            yield break;
        }
        imageSize = size;

        // Совпадение по названиям категорий в ОБУЧАЮЩЕЙ и ТЕСТОВОЙ выборках:
        categoryNames = trainCategories.Select(c => c.name).ToList();
        if (!Enumerable.SequenceEqual(categoryNames, testCategories.Select(c => c.name)))
        {
            verdict = "Невозможно использовать этот датасет. Категории в папках train и test не совпадают!";
            isValid = false;
            OnReady?.Invoke();
            yield break;
        }

        // Одинаковое ли количество картинок в каждой категории.
        bool balanced = trainCategories.Select(c => c.count).Distinct().Count() == 1;

        // Вердикт:
        isValid = true;
        verdict = balanced
            ? "Датасет корректен и сбалансирован."
            : "Датасет допустим к использованию, однако категории содержат неодинаковое количество изображений.";

        // Гасим пользователю панель загрузки:
        //popUpPanelsController.ClosePanel("Загрузка датасета");
        OnReady?.Invoke();
    }

    private IEnumerator AnalyzeCategoryFolderAsync(string path, Action<float> onProgress, Action<List<CategoryInfo>, int, Vector2Int, bool> onComplete)
    {
        var result = new List<CategoryInfo>();
        var categories = Directory.GetDirectories(path);
        int totalCount = 0;
        Vector2Int imageSize = Vector2Int.zero;
        bool sizeMismatch = false;

        int totalImages = categories.Sum(cat => Directory.GetFiles(cat)
            .Count(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg")));

        int processedImages = 0;

        foreach (var category in categories)
        {
            string catName = Path.GetFileName(category);
            string[] images = Directory.GetFiles(category)
                .Where(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg"))
                .ToArray();

            result.Add(new CategoryInfo { name = catName, count = images.Length });
            totalCount += images.Length;

            foreach (var imgPath in images)
            {
                byte[] data = File.ReadAllBytes(imgPath);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(data);

                if (imageSize == Vector2Int.zero)
                    imageSize = new Vector2Int(tex.width, tex.height);
                else if (imageSize.x != tex.width || imageSize.y != tex.height)
                    sizeMismatch = true;

                UnityEngine.Object.Destroy(tex);

                processedImages++;
                onProgress?.Invoke((float)processedImages / totalImages);

                // Прочитали 65 изображений и освободили кадр, разрешив перейти в следующему кадру. Если будет не 65, а 1, то лагать не будет, а загружать будет по 1 изображению за 1 кадр. Слишком долго короче.
                if (processedImages % 65 == 0)
                    yield return null;
            }
        }
        onProgress?.Invoke(1.0f);
        onComplete?.Invoke(result, totalCount, imageSize, sizeMismatch);
    }
}
