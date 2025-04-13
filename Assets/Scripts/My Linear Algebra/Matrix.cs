using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс матрицы, который представляет из себя все объекты линейной алгебры.
/// </summary>
[Serializable]
public class Matrix
{
    double[,] data;

    [Tooltip("Кол-во строк")]
    [SerializeField] private int rows;

    [Tooltip("Кол-во столбцов")]
    [SerializeField] private int columns;

    // Конструктор для создания матрицы
    public Matrix(double[,] initialData)
    {
        data = initialData;
        rows = initialData.GetLength(0);
        columns = initialData.GetLength(1);
    }
    #region OPERATORS
    public static Matrix operator -(Matrix a)
    {
        double[,] result = new double[a.rows, a.columns];
        for (int i = 0; i < a.rows; i++)
            for (int j = 0; j < a.columns; j++)
                result[i, j] = -a.data[i, j];

        return new Matrix(result);
    }


    public static Matrix operator +(Matrix a, double b)
    {
        double[,] result = new double[a.rows, a.columns];
        for (int i = 0; i < a.rows; i++)
            for (int j = 0; j < a.columns; j++)
                result[i, j] = a.data[i, j] + b;

        return new Matrix(result);
    }
    public static Matrix operator +(double a, Matrix b)
    {
        return b + a; // используем предыдущую перегрузку
    }
    public static Matrix operator +(Matrix a, Matrix b)
    {
        if (a.rows != b.rows || a.columns != b.columns)
            throw new ArgumentException("Matrix dimensions must agree.");

        double[,] result = new double[a.rows, a.columns];
        for (int i = 0; i < a.rows; i++)
        {
            for (int j = 0; j < a.columns; j++)
            {
                result[i, j] = a.data[i, j] + b.data[i, j];
            }
        }
        return new Matrix(result);
    }


    public static Matrix operator *(Matrix a, Matrix b)
    {
        if (a.columns != b.rows)
            throw new ArgumentException("Invalid matrix multiplication dimensions");

        double[,] result = new double[a.rows, b.columns];
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
    public static Matrix operator /(Matrix a, double b)
    {
        double[,] result = new double[a.rows, a.columns];
        for (int i = 0; i < a.rows; i++)
            for (int j = 0; j < a.columns; j++)
                result[i, j] = a.data[i, j] / b;

        return new Matrix(result);
    }
    public static Matrix operator /(double b, Matrix a)
    {
        double[,] result = new double[a.rows, a.columns];
        for (int i = 0; i < a.rows; i++)
            for (int j = 0; j < a.columns; j++)
                result[i, j] = b / a.data[i, j];

        return new Matrix(result);
    }


    public double this[int i, int j]
    {
        get => data[i, j];
        set => data[i, j] = value;
    }
    #endregion

    // Метод для транспонирования матрицы
    public Matrix T()
    {
        double[,] result = new double[columns, rows];
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