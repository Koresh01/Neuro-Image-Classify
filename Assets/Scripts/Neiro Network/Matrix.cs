using System;

/// <summary>
/// Класс матрицы, который представляет из себя все объекты линейной алгебры.
/// </summary>
[Serializable]
public class Matrix
{
    public double[,] data;
    public int Rows { get; }
    public int Columns { get; }

    // Конструктор для создания матрицы
    public Matrix(double[,] initialData)
    {
        data = initialData;
        Rows = initialData.GetLength(0);
        Columns = initialData.GetLength(1);
    }

    // Перегрузка оператора умножения
    public static Matrix operator *(Matrix a, Matrix b)
    {
        if (a.Columns != b.Rows)
            throw new ArgumentException("Invalid matrix multiplication dimensions");

        double[,] result = new double[a.Rows, b.Columns];
        for (int i = 0; i < a.Rows; i++)
        {
            for (int j = 0; j < b.Columns; j++)
            {
                for (int k = 0; k < a.Columns; k++)
                {
                    result[i, j] += a.data[i, k] * b.data[k, j];
                }
            }
        }
        return new Matrix(result);
    }

    // Метод для транспонирования матрицы
    public Matrix T()
    {
        double[,] result = new double[Columns, Rows];
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                result[j, i] = data[i, j];
            }
        }
        return new Matrix(result);
    }

    // Перегрузка оператора сложения
    public static Matrix operator +(Matrix a, Matrix b)
    {
        if (a.Rows != b.Rows || a.Columns != b.Columns)
            throw new ArgumentException("Matrix dimensions must agree.");

        double[,] result = new double[a.Rows, a.Columns];
        for (int i = 0; i < a.Rows; i++)
        {
            for (int j = 0; j < a.Columns; j++)
            {
                result[i, j] = a.data[i, j] + b.data[i, j];
            }
        }
        return new Matrix(result);
    }

    // Метод для вывода матрицы в консоль (для удобства)
    public override string ToString()
    {
        var result = "";
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                result += data[i, j] + "\t";
            }
            result += "\n";
        }
        return result;
    }
}

/*
// Пример использования
class Program
{
    static void Main()
    {
        Matrix x = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
        Matrix W = new Matrix(new double[,] { { 5, 6 }, { 7, 8 } });
        Matrix b = new Matrix(new double[,] { { 1, 1 }, { 1, 1 } });

        Matrix result = x * W.T() + b;
        Console.WriteLine(result);
    }
}
*/
