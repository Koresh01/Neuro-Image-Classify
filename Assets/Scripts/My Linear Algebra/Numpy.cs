using System;

/// <summary>
/// Математические функции, имитирующие функциональность NumPy.
/// </summary>
public static class Numpy
{
    /// <summary>
    /// Возвращает индекс максимального значения в расплющенной матрице.
    /// </summary>
    public static int argmax(Matrix data)
    {
        // Flattened argmax
        int maxIndex = 0;
        double maxValue = data[0, 0];

        int rows = data.GetLength(0);
        int cols = data.GetLength(1);

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
            {
                if (data[i, j] > maxValue)
                {
                    maxValue = data[i, j];
                    maxIndex = i * cols + j;
                }
            }
        return maxIndex;
    }

    /// <summary>
    /// Создаёт новый массив, заполненный нулями. 
    /// </summary>
    public static Matrix Zeros(int rows, int cols)
    {
        double[,] data = new double[rows, cols]; // все значения уже 0
        return new Matrix(data);
    }

    /// <summary>
    /// Создаёт новый массив, заполненный единицами.
    /// </summary>
    public static Matrix Ones(int rows, int cols)
    {
        double[,] data = new double[rows, cols];
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                data[i, j] = 1.0;

        return new Matrix(data);
    }

    /// <summary>
    /// Возвращает новую матрицу, где к каждому элементу исходной матрицы применяется экспонента (e^x).
    /// </summary>
    /// <param name="data">Исходная матрица</param>
    /// <returns>Новая матрица с применённой функцией exp к каждому элементу</returns>
    public static Matrix Exp(Matrix data)
    {
        int rows = data.GetLength(0);
        int cols = data.GetLength(1);
        double[,] result = new double[rows, cols];

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                result[i, j] = Math.Exp(data[i, j]);

        return new Matrix(result);
    }

    /// <summary>
    /// Возвращает новую матрицу, в которой к каждому элементу исходной матрицы применяется натуральный логарифм (ln).
    /// </summary>
    /// <returns>Новая матрица с применённой функцией логарифма</returns>
    public static Matrix Log(Matrix data)
    {
        int rows = data.GetLength(0);
        int cols = data.GetLength(1);
        double[,] result = new double[rows, cols];

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                result[i, j] = Math.Log(data[i, j]);

        return new Matrix(result);
    }

    /// <summary>
    /// Возвращает сумму всех элементов в матрице.
    /// </summary>
    /// <returns>Double значение суммы всех элементов</returns>
    public static double Sum(Matrix data)
    {
        int rows = data.GetLength(0);
        int cols = data.GetLength(1);
        double sum = 0.0;

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                sum += data[i, j];

        return sum;
    }

    /// <summary>
    /// Возвращает новую матрицу, в которой каждый элемент равен максимуму между исходным значением и порогом.
    /// </summary>
    /// <param name="data">Исходная матрица</param>
    /// <param name="threshold">Пороговое значение</param>
    /// <returns>Новая матрица после применения функции maximum</returns>
    public static Matrix Maximum(Matrix data, double threshold)
    {
        int rows = data.GetLength(0);
        int cols = data.GetLength(1);
        double[,] result = new double[rows, cols];

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                result[i, j] = Math.Max(data[i, j], threshold);

        return new Matrix(result);
    }


    public static class Random
    {
        private static System.Random rand = new System.Random();

        /// <summary>
        /// Создаёт матрицу размера (rows,cols), заполненную значениями из нормального распределения.
        /// </summary>
        public static Matrix Randn(int rows, int cols)
        {
            double[,] data = new double[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    data[i, j] = Gaussian();

            return new Matrix(data);

            // Гаусово распределение.
            double Gaussian()
            {
                double u1 = 1.0 - rand.NextDouble();
                double u2 = 1.0 - rand.NextDouble();
                return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
            }
        }
    }
}
