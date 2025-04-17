using System.IO;
using UnityEngine;
/// <summary>
/// Класс для обработки изображений, предназначенный для преобразования изображений в матрицы данных.
/// Он может быть использован для различных операций над изображениями, таких как изменение размера,
/// преобразование в серый масштаб и нормализация.
/// </summary>
public class ImageProcessor
{
    /// <summary>
    /// Преобразует изображение из файла в входную матрицу для нейросети.
    /// Используется для преобразования изображений в данные для обучения.
    /// </summary>
    /// <param name="path">Путь к файлу изображения.</param>
    /// <param name="matrixWidth">Ширина входной матрицы, которая соответствует количеству признаков для каждого изображения.</param>
    /// <returns>Матрица, представляющая изображение.</returns>
    public Matrix ConvertImageToInputMatrix(string path, int matrixWidth)
    {
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D imgTexture = new Texture2D(2, 2);
        imgTexture.LoadImage(fileData);

        // Создаем матрицу для хранения пикселей
        Matrix inputImageMatrix = Numpy.Zeros(1, matrixWidth);
        for (int x = 0; x < imgTexture.width; x++)
        {
            for (int y = 0; y < imgTexture.height; y++)
            {
                int i = y * imgTexture.width + x;
                Color pixel = imgTexture.GetPixel(x, y);
                inputImageMatrix[0, i] = (pixel.r + pixel.g + pixel.b) / 3f; // Преобразование в серый цвет (яркость)
            }
        }

        return inputImageMatrix;
    }

    /// <summary>
    /// Изменяет размер изображения до заданных размеров.
    /// </summary>
    /// <param name="texture">Текстура изображения для изменения размера.</param>
    /// <param name="newWidth">Новая ширина изображения.</param>
    /// <param name="newHeight">Новая высота изображения.</param>
    /// <returns>Измененная текстура.</returns>
    public Texture2D ResizeImage(Texture2D texture, int newWidth, int newHeight)
    {
        Texture2D resizedTexture = new Texture2D(newWidth, newHeight);
        // Используем метод для изменения размера изображения
        resizedTexture.SetPixels(texture.GetPixels(0, 0, texture.width, texture.height));
        resizedTexture.Apply();
        return resizedTexture;
    }

    /// <summary>
    /// Преобразует изображение в черно-белое (градации серого).
    /// </summary>
    /// <param name="texture">Текстура изображения.</param>
    /// <returns>Черно-белая текстура.</returns>
    public Texture2D ConvertToGrayscale(Texture2D texture)
    {
        Texture2D grayTexture = new Texture2D(texture.width, texture.height);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                Color color = texture.GetPixel(x, y);
                float gray = (color.r + color.g + color.b) / 3f;
                grayTexture.SetPixel(x, y, new Color(gray, gray, gray));
            }
        }
        grayTexture.Apply();
        return grayTexture;
    }

    /// <summary>
    /// Нормализует изображение в диапазон от 0 до 1.
    /// </summary>
    /// <param name="texture">Текстура изображения.</param>
    /// <returns>Нормализованная текстура.</returns>
    public Texture2D NormalizeImage(Texture2D texture)
    {
        Texture2D normalizedTexture = new Texture2D(texture.width, texture.height);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                Color color = texture.GetPixel(x, y);
                // Нормализуем значение цвета
                color.r = color.r / 255f;
                color.g = color.g / 255f;
                color.b = color.b / 255f;
                normalizedTexture.SetPixel(x, y, color);
            }
        }
        normalizedTexture.Apply();
        return normalizedTexture;
    }

    #region Закомментированные потенциально полезные функции

    // /// <summary>
    // /// Изменяет яркость изображения на заданное значение.
    // /// </summary>
    // /// <param name="texture">Текстура изображения.</param>
    // /// <param name="brightness">Яркость от -1 (темнее) до 1 (ярче).</param>
    // /// <returns>Изображение с измененной яркостью.</returns>
    // public Texture2D AdjustBrightness(Texture2D texture, float brightness)
    // {
    //     Texture2D adjustedTexture = new Texture2D(texture.width, texture.height);
    //     for (int x = 0; x < texture.width; x++)
    //     {
    //         for (int y = 0; y < texture.height; y++)
    //         {
    //             Color color = texture.GetPixel(x, y);
    //             color.r = Mathf.Clamp01(color.r + brightness);
    //             color.g = Mathf.Clamp01(color.g + brightness);
    //             color.b = Mathf.Clamp01(color.b + brightness);
    //             adjustedTexture.SetPixel(x, y, color);
    //         }
    //     }
    //     adjustedTexture.Apply();
    //     return adjustedTexture;
    // }

    // /// <summary>
    // /// Применяет фильтр размытия к изображению.
    // /// </summary>
    // /// <param name="texture">Текстура изображения.</param>
    // /// <param name="radius">Радиус размытия.</param>
    // /// <returns>Размытую текстуру.</returns>
    // public Texture2D ApplyBlur(Texture2D texture, int radius)
    // {
    //     Texture2D blurredTexture = new Texture2D(texture.width, texture.height);
    //     // Применение алгоритма размытия (например, гауссового фильтра)
    //     blurredTexture.Apply();
    //     return blurredTexture;
    // }
    #endregion
}
