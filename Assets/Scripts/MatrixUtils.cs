using System;

/// <summary>
/// Класс для использования функций CrossEntropy, SoftMax... для нашего Matrix класса.
/// </summary>
public static class MatrixUtils
{
    #region ФункцииАктивации
    public static Matrix Sigmoid(Matrix t)
    {
        return 1 / (1 + Numpy.Exp(-t));
    }

    /// <summary>
    /// Производная от sigmoid.
    /// </summary>
    /// <param name="t">Входная матрица</param>
    /// <returns>Матрица с применённой производной</returns>
    public static Matrix SigmoidDeriv(Matrix t)
    {
        Matrix sigmoid = Sigmoid(t);
        return sigmoid * (1 - sigmoid);
    }
    #endregion
    
    /// <summary>
    /// Нормализует значение выходного вектора.
    /// </summary>
    /// <param name="t">Входная матрица</param>
    /// <returns>Вектор вероятности отношения изображения к определённой категории.</returns>
    public static Matrix SoftMax(Matrix t)
    {
        Matrix exp = Numpy.Exp(t);
        return exp / Numpy.Sum(exp);
    }

    /// <summary>
    /// Вычисляет величину ошибки (sparse cross-entropy).
    /// </summary>
    /// <param name="z">Вектор вероятностей</param>
    /// <param name="y">Индекс правильной категории</param>
    /// <returns>Значение ошибки</returns>
    public static float CrossEntropy(Matrix z, int y)
    {
        return -MathF.Log(z[0, y]);
    }

    /// <summary>
    /// One-hot encoding
    /// </summary>
    /// <param name="y">Индекс истинной категории изображения</param>
    /// <param name="numClasses">Кол-во нейронов на выходе нейросети.</param>
    /// <returns>One-hot вектор</returns>
    public static Matrix ToFull(int y, int numClasses)
    {
        Matrix yFull = Numpy.Zeros(1, numClasses);
        yFull[0, y] = 1;
        return yFull;
    }
}
