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
        float maxValue = data[0, 0];

        int rows = data.Rows;
        int cols = data.Columns;

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
        float[,] data = new float[rows, cols]; // все значения уже 0
        return new Matrix(data);
    }

    /// <summary>
    /// Создаёт новый массив, заполненный единицами.
    /// </summary>
    public static Matrix Ones(int rows, int cols)
    {
        float[,] data = new float[rows, cols];
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                data[i, j] = 1.0f;

        return new Matrix(data);
    }

    /// <summary>
    /// Возвращает новую матрицу, где к каждому элементу исходной матрицы применяется экспонента (e^x).
    /// </summary>
    /// <param name="data">Исходная матрица</param>
    /// <returns>Новая матрица с применённой функцией exp к каждому элементу</returns>
    public static Matrix Exp(Matrix data)
    {
        int rows = data.Rows;
        int cols = data.Columns;
        float[,] result = new float[rows, cols];

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                result[i, j] = MathF.Exp(data[i, j]);

        return new Matrix(result);
    }

    /// <summary>
    /// Возвращает новую матрицу, в которой к каждому элементу исходной матрицы применяется натуральный логарифм (ln).
    /// </summary>
    /// <returns>Новая матрица с применённой функцией логарифма</returns>
    public static Matrix Log(Matrix data)
    {
        int rows = data.Rows;
        int cols = data.Columns;
        float[,] result = new float[rows, cols];

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                result[i, j] = MathF.Log(data[i, j]);

        return new Matrix(result);
    }

    /// <summary>
    /// Возвращает сумму всех элементов в матрице.
    /// </summary>
    /// <returns>float значение суммы всех элементов</returns>
    public static float Sum(Matrix data)
    {
        int rows = data.Rows;
        int cols = data.Columns;
        float sum = 0.0f;

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                sum += data[i, j];

        return sum;
    }

    /// <summary>
    /// Возвращает минимальное значение в матрице (как в numpy.min).
    /// </summary>
    public static float Min(Matrix data)
    {
        int rows = data.Rows;
        int cols = data.Columns;

        float minValue = data[0, 0];
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                if (data[i, j] < minValue)
                    minValue = data[i, j];

        return minValue;
    }

    /// <summary>
    /// Возвращает максимальное значение в матрице (как в numpy.max).
    /// </summary>
    public static float Max(Matrix data)
    {
        int rows = data.Rows;
        int cols = data.Columns;

        float maxValue = data[0, 0];
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                if (data[i, j] > maxValue)
                    maxValue = data[i, j];

        return maxValue;
    }

    public static class Random
    {
        private static System.Random rand = new System.Random();

        /// <summary>
        /// Создаёт матрицу размера (rows,cols), заполненную значениями из нормального распределения.
        /// </summary>
        public static Matrix Randn(int rows, int cols)
        {
            float[,] data = new float[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    data[i, j] = Gaussian();

            return new Matrix(data);

            // Гаусово распределение.
            float Gaussian()
            {
                float u1 = 1.0f - (float) rand.NextDouble();
                float u2 = 1.0f - (float) rand.NextDouble();
                return MathF.Sqrt(-2.0f * MathF.Log(u1)) * MathF.Cos(2.0f * MathF.PI * u2);
            }
        }
    }
}
