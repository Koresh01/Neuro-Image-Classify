using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс матрицы, который представляет из себя все объекты линейной алгебры.
/// </summary>
[Serializable]
public class Matrix
{
    float[,] data;

    [Tooltip("Кол-во строк")]
    [SerializeField] private int rows;

    [Tooltip("Кол-во столбцов")]
    [SerializeField] private int columns;

    // Конструктор для создания матрицы
    public Matrix(float[,] initialData)
    {
        data = initialData;
        rows = initialData.GetLength(0);
        columns = initialData.GetLength(1);
    }
    #region OPERATORS
    public static Matrix operator -(Matrix a)
    {
        float[,] result = new float[a.rows, a.columns];
        for (int i = 0; i < a.rows; i++)
            for (int j = 0; j < a.columns; j++)
                result[i, j] = -a.data[i, j];

        return new Matrix(result);
    }
    public static Matrix operator -(Matrix a, Matrix b)
    {
        if (a.rows != b.rows || a.columns != b.columns)
            throw new ArgumentException("Matrix dimensions must agree.");

        float[,] result = new float[a.rows, a.columns];
        for (int i = 0; i < a.rows; i++)
            for (int j = 0; j < a.columns; j++)
                result[i, j] = a.data[i, j] - b.data[i, j];

        return new Matrix(result);
    }
    public static Matrix operator -(Matrix a, float b)
    {
        float[,] result = new float[a.rows, a.columns];
        for (int i = 0; i < a.rows; i++)
            for (int j = 0; j < a.columns; j++)
                result[i, j] = a.data[i, j] - b;

        return new Matrix(result);
    }
    public static Matrix operator -(float a, Matrix b)
    {
        float[,] result = new float[b.rows, b.columns];
        for (int i = 0; i < b.rows; i++)
            for (int j = 0; j < b.columns; j++)
                result[i, j] = a - b.data[i, j];

        return new Matrix(result);
    }

    public static Matrix operator +(Matrix a, float b)
    {
        float[,] result = new float[a.rows, a.columns];
        for (int i = 0; i < a.rows; i++)
            for (int j = 0; j < a.columns; j++)
                result[i, j] = a.data[i, j] + b;

        return new Matrix(result);
    }
    public static Matrix operator +(float a, Matrix b)
    {
        return b + a; // используем предыдущую перегрузку
    }
    public static Matrix operator +(Matrix a, Matrix b)
    {
        if (a.rows != b.rows || a.columns != b.columns)
            throw new ArgumentException("Matrix dimensions must agree.");

        float[,] result = new float[a.rows, a.columns];
        for (int i = 0; i < a.rows; i++)
        {
            for (int j = 0; j < a.columns; j++)
            {
                result[i, j] = a.data[i, j] + b.data[i, j];
            }
        }
        return new Matrix(result);
    }

    /// <summary>
    /// Матричное перемножение
    /// </summary>
    public static Matrix operator ^(Matrix a, Matrix b)
    {
        if (a.columns != b.rows)
            throw new ArgumentException("Invalid matrix multiplication dimensions");

        float[,] result = new float[a.rows, b.columns];
        for (int i = 0; i < a.rows; i++)
        {
            for (int j = 0; j < b.columns; j++)
            {
                for (int k = 0; k < a.columns; k++)
                {
                    result[i, j] += a.data[i, k] * b.data[k, j];
                }
            }
        }
        return new Matrix(result);
    }
    /// <summary>
    /// Поэлементное перемножение.
    /// </summary>
    public static Matrix operator *(Matrix a, Matrix b)
    {
        if (a.rows != b.rows || a.columns != b.columns)
            throw new ArgumentException("Matrix dimensions must agree.");

        float[,] result = new float[a.rows, a.columns];
        for (int i = 0; i < a.rows; i++)
        {
            for (int j = 0; j < a.columns; j++)
            {
                result[i, j] = a.data[i, j] * b.data[i, j];
            }
        }
        return new Matrix(result);
    }
    public static Matrix operator *(float a, Matrix b)
    {
        float[,] result = new float[b.rows, b.columns];
        for (int i = 0; i < b.rows; i++)
            for (int j = 0; j < b.columns; j++)
                result[i, j] = a * b.data[i, j];

        return new Matrix(result);
    }

    public static Matrix operator /(Matrix a, float b)
    {
        float[,] result = new float[a.rows, a.columns];
        for (int i = 0; i < a.rows; i++)
            for (int j = 0; j < a.columns; j++)
                result[i, j] = a.data[i, j] / b;

        return new Matrix(result);
    }
    public static Matrix operator /(float b, Matrix a)
    {
        float[,] result = new float[a.rows, a.columns];
        for (int i = 0; i < a.rows; i++)
            for (int j = 0; j < a.columns; j++)
                result[i, j] = b / a.data[i, j];

        return new Matrix(result);
    }


    public float this[int i, int j]
    {
        get => data[i, j];
        set => data[i, j] = value;
    }
    #endregion

    // Метод для транспонирования матрицы
    public Matrix T()
    {
        float[,] result = new float[columns, rows];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                result[j, i] = data[i, j];
            }
        }
        return new Matrix(result);
    }

    /// <summary>
    /// Возвращает размерность матрицы.
    /// 0 - вернёт кол-во строк
    /// 1 - вернёт кол-во столбцов
    /// </summary>
    public int GetLength(int wh)
    {
        if (wh == 0) return rows;
        else return columns;
    }
}