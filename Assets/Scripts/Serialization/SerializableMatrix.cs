using System.Collections.Generic;


/*

Твоя проблема в том, что JsonUtility не сериализует массивы, вложенные в нестандартные классы, если они не помечены как public или [SerializeField]. Твой класс Matrix, скорее всего, выглядит как:

csharp
Копировать
Редактировать
public class Matrix
{
    public int rows;
    public int columns;
    private float[,] data; // <-- не сериализуется!
}
Именно data (или аналогичное поле) содержит значения, но оно не попадает в JSON, потому что:

float[,] не поддерживается JsonUtility. А вот List<List<float>> data спокойно сериализуется.
🔧 Решение
Тебе нужно создать сериализуемый класс SerializableMatrix, который будет использоваться при сохранении/загрузке:
 
 
 */


/// <summary>
/// Сериализуемая обёртка для матрицы, предназначенная для экспорта и импорта через JSON.
/// Хранит значения матрицы в виде одномерного списка serializedData (построчно).
/// </summary>
[System.Serializable]
public class SerializableMatrix
{
    public int rows;
    public int columns;
    public List<float> serializedData;

    public SerializableMatrix(Matrix matrix)
    {
        rows = matrix.Rows;
        columns = matrix.Columns;
        serializedData = new List<float>(rows * columns);
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
                serializedData.Add(matrix[i, j]);
    }

    /// <summary>
    /// Преобразовывает SerializebleMatrix в обычную Matrix.
    /// </summary>
    public Matrix ToMatrix()
    {
        Matrix m = new Matrix(rows, columns);   // нет такого конструктора
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
                m[i, j] = serializedData[i * columns + j];
        return m;
    }
}
