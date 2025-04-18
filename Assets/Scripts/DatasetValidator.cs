using System;
using System.Collections.Generic;
using UnityEngine;
using SFB; // Standalone File Browser (Windows)
using System.IO;
using System.Linq;
using System.Collections;
using Zenject;
using UnityEngine.Events;
using UnityEditor;


[Serializable]
public class CategoryInfo
{
    [Tooltip("Категория изображений")]
    public string name;

    [Tooltip("Количество изображений")]
    public int count;
}

// [Serializable]
public class ImageData
{
    [Tooltip("Путь до изображения.")]
    public string path;

    [Tooltip("Индекс истиной категории этого изображения.")]
    public int y;

    [Tooltip("Название категории.")]
    public string categoryName;
}

/// <summary>
/// Проверяет, корректно ли организован датасет изображений для классификации.
/// </summary>
[AddComponentMenu("Custom/Dataset Validator (Проверка датасета)")]
public class DatasetValidator : MonoBehaviour
{
    [Tooltip("Контроллер всплывающих панелей.")]
    [Inject] PopUpPanelsController popUpPanelsController;

    [Header("Прогресс загрузки:")]
    [Range(0, 1)] public float loadProgress;

    /* ---------------------- Общая информация ----------------------*/
    [Header("Общая информация:")]
    [Tooltip("Названия категорий (общие для train и test).")]
    public List<string> categoryNames;

    [Tooltip("Изображение и индекс его категории.")]
    public List<ImageData> trainImagesPaths; // path, номер категории, название категории   
    public List<ImageData> testImagesPaths;

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
    [Tooltip("Разрешение изображений.")]
    public Vector2Int imageSize;

    [Tooltip("Вердикт проверки.")]
    [TextArea]
    public string verdict;

    [Tooltip("Можно ли использовать датасет.")]
    public bool isValid = false;

    [Tooltip("Готовность датасета к использованию.")]
    public UnityAction OnReady;


    [ContextMenu("Загрузить датасет")]
    public void LoadDataset()
    {
        ResetAllData();

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

        popUpPanelsController.ShowPanel("Загрузка датасета");
        // Проверка правильности датасета:
        StartCoroutine(Validate(trainPath, testPath));
    }


    #region ВАЛИДАЦИЯ
    /// <summary>
    /// Проверяет правильность датасета.
    /// </summary>
    IEnumerator Validate(string trainPath, string testPath)
    {
        yield return ValidateCategories(trainPath, testPath);
        yield return ValidateImageDistribution(trainPath, testPath);
        yield return ValidateImageSizesAndCollectPaths(trainPath, testPath);

        verdict += "Датасет пригоден для использования.";
        isValid = true;
        OnReady?.Invoke();
    }

    /// <summary>
    /// Проверяет названия папок в обучающей и тестовой выборках.
    /// </summary>
    IEnumerator ValidateCategories(string trainPath, string testPath)
    {
        var trainCategoriesPaths = Directory.GetDirectories(trainPath);
        var testCategoriesPaths = Directory.GetDirectories(testPath);

        // Извлекаем имена категорий, используя Path.GetFileName
        List<string> trainCategories = trainCategoriesPaths
            .Select(path => Path.GetFileName(path))
            .ToList();

        List<string> testCategories = testCategoriesPaths
            .Select(path => Path.GetFileName(path))
            .ToList();

        // Проверяем, совпадают ли категории
        if (!trainCategories.SequenceEqual(testCategories))
        {
            stopValidation("Категории изображений обучающей выборки не совпадают с категориями тестовых изображений");
        }

        categoryNames = trainCategories;

        yield return null;
    }

    /// <summary>
    /// Совпадает ли кол-во изображений каждой категории в обучающей выборке.
    /// </summary>
    IEnumerator ValidateImageDistribution(string trainPath, string testPath)
    {
        // Обучающая выборка:
        var categories = Directory.GetDirectories(trainPath);

        foreach (var category in categories)
        {
            string categoryName = Path.GetFileName(category);
            string[] images = Directory.GetFiles(category)
                .Where(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg"))
                .ToArray();
            trainCategories.Add(new CategoryInfo { name = categoryName, count = images.Length });

            // Заодно ссчитаем сколько картинок в обучающей выборке.
            trainTotal += images.Length;
        }

        // Проверяем, если в trainCategories есть разные значения в count
        int expectedCount = trainCategories.First().count;
        bool isBalanced = trainCategories.All(c => c.count == expectedCount);
        verdict = "В обучающей выборке в каждой категории разное количество изображений. ";




        // Тестовая выборка:
        categories = Directory.GetDirectories(testPath);
        foreach (var category in categories)
        {
            string categoryName = Path.GetFileName(category);
            string[] images = Directory.GetFiles(category)
                .Where(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg"))
                .ToArray();
            testCategories.Add(new CategoryInfo { name = categoryName, count = images.Length });

            // Заодно ссчитаем сколько картинок в обучающей выборке.
            testTotal += images.Length;
        }

        yield return null;
    }

    /// <summary>
    /// Проверяет размерность всех изображений и одновременно формирует словари:
    /// trainImagesPaths и testImagesPaths.
    /// </summary>
    IEnumerator ValidateImageSizesAndCollectPaths(string trainPath, string testPath)
    {
        trainImagesPaths = new List<ImageData>();
        testImagesPaths = new List<ImageData>();
        int imageCount = 0;

        // Проверяем train и test выборки
        yield return ProcessSet(trainPath, trainImagesPaths);
        yield return ProcessSet(testPath, testImagesPaths);

        // Внутренняя функция проверки и сбора путей
        IEnumerator ProcessSet(string rootPath, List<ImageData> imageList)
        {
            for (int categoryIndex = 0; categoryIndex < categoryNames.Count; categoryIndex++)
            {
                string categoryName = categoryNames[categoryIndex];
                string categoryPath = Path.Combine(rootPath, categoryName);

                string[] images = Directory.GetFiles(categoryPath)
                    .Where(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg"))
                    .ToArray();

                foreach (var imgPath in images)
                {
                    // Проверка размеров
                    byte[] data = File.ReadAllBytes(imgPath);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(data);

                    if (imageSize == Vector2Int.zero)
                        imageSize = new Vector2Int(tex.width, tex.height);
                    else if (imageSize.x != tex.width || imageSize.y != tex.height)
                    {
                        stopValidation("Разрешения изображений не стандартизированы.");
                        yield break;
                    }

                    // Добавляем путь, перемешивая датасет.
                    int randomIndex = UnityEngine.Random.Range(0, imageList.Count + 1);
                    imageList.Insert(randomIndex, new ImageData
                    {
                        path = imgPath,
                        y = categoryIndex,
                        categoryName = categoryName
                    });

                    // Обновление прогресса
                    imageCount++;
                    loadProgress = (float)imageCount / (trainTotal + testTotal);

                    // Отпускание кадра:
                    if (imageCount % 40 == 0)
                        yield return null;
                }
            }
        }
    }
    #endregion


    /// <summary>
    /// Экстренное завершение валидации.
    /// </summary>
    void stopValidation(string text)
    {
        verdict = text;
        StopAllCoroutines();
        OnReady?.Invoke();
    }

    /// <summary>
    /// Получить индекс категории по её названию.
    /// </summary>
    public int GetCategoryIndexByName(string categoryName)
    {
        if (categoryNames == null) throw new ArgumentNullException(nameof(categoryNames));
        int index = categoryNames.IndexOf(categoryName);
        if (index == -1)
            throw new ArgumentException($"Категория с именем \"{categoryName}\" не найдена.");
        return index;
    }

    /// <summary>
    /// Получить название категории по её индексу.
    /// </summary>
    public string GetCategoryNameByIndex(int index)
    {
        if (categoryNames == null) throw new ArgumentNullException(nameof(categoryNames));
        if (index < 0 || index >= categoryNames.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"Индекс {index} вне диапазона категорий.");
        return categoryNames[index];
    }

    /// <summary>
    /// Полностью очищает все данные валидатора.
    /// </summary>
    public void ResetAllData()
    {
        loadProgress = 0f;

        // Общая информация
        categoryNames = new List<string>();
        trainImagesPaths = new List<ImageData>();
        testImagesPaths = new List<ImageData>();

        // Обучающая выборка
        trainTotal = 0;
        trainCategories = new List<CategoryInfo>();

        // Тестовая выборка
        testTotal = 0;
        testCategories = new List<CategoryInfo>();

        // Итог
        imageSize = Vector2Int.zero;
        verdict = string.Empty;
        isValid = false;
    }
}


