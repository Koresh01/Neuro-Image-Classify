using System;

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
