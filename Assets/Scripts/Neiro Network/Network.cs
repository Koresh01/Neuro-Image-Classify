using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/*
 
INPUT_DIM

   v

 LAYER 1

   v

   H1 (Relu)
   
   v

 LAYER 2

   v

Softmax()

 */

/// <summary>
/// Нейросеть обучаемая.
/// </summary>
public class Network : MonoBehaviour
{
    [Inject] DatasetValidator datasetValidator;

    [Header("Слои:")]
    public List<Matrix> t;

    [Header("Активированные слои:")]
    public List<Matrix> h;

    [Header("Матрицы весов:")]
    public List<Matrix> W;

    [Header("Нелинейности:")]
    public List<Matrix> B;

    void OnEnable()
    {
        datasetValidator.OnReady += Init;
    }

    private void OnDisable()
    {
        datasetValidator.OnReady -= Init;
    }

    #region ИНИЦИАЛИЗАЦИЯ
    /// <summary>
    /// Датасет валиден, значит указываем конфигурацию нейросети.
    /// </summary>
    void Init()
    {
        InitIntermediateLayers();
        InitActivatedLayers();
        InitWeights();
        InitBiases();
    }

    /// <summary>
    /// Инициализирует промежуточные слои.
    /// </summary>
    void InitIntermediateLayers()
    {
        // s.Clear();

        int inputDim = datasetValidator.imageSize[0] * datasetValidator.imageSize[1];
        int outputDim = datasetValidator.categoryNames.Count;

        // Входной слой
        t[0] = Numpy.Zeros(1, inputDim);

        // Промежуточные слои добавляются через инспектор —> пропускаем

        // Выходной слой
        t[t.Count - 1] = Numpy.Zeros(1, outputDim);
    }

    /// <summary>
    /// Инициализирует слои активационные Relu или Sigmoid.
    /// </summary>
    void InitActivatedLayers()
    {
        h = new List<Matrix>();

        for (int i = 0; i < t.Count; i++)
        {
            h.Add(Numpy.Zeros(1, t[i].GetLength(1))); // Вход и выход не активируем
        }
    }

    /// <summary>
    /// Инициализирует веса.
    /// </summary>
    void InitWeights()
    {
        W.Clear();

        for (int i = 0; i < t.Count-1; i++)
        {
            // Матрицы весов между слоями:
            int curColumns = t[i].GetLength(1);
            int nextColumns = t[i+1].GetLength(1);

            Matrix w = Numpy.Random.Randn(curColumns, nextColumns);
            W.Add(w);
        }
    }

    /// <summary>
    /// Инициализирует нелинейности.
    /// </summary>
    void InitBiases()
    {
        B.Clear();

        for (int i = 0; i < t.Count - 1; i++)
        {
            int to = t[i + 1].GetLength(1);
            B.Add(Numpy.Random.Randn(1, to));
        }
    }
    #endregion

    Matrix Sigmoid(Matrix t)
    {
        Matrix res = 1 / (1 + (Numpy.Exp(-t)));
        return res;
    }

    /// <summary>
    /// Нормализует значение выходного вектора.
    /// <returns>Вектор вероятности отношения изображения к определённой категории.</returns>
    Matrix SoftMax(Matrix t)
    {
        Matrix exp = Numpy.Exp(t);  // Marix недопустим в этом контексте, и = недопустимый термин в выражении
        return exp / Numpy.Sum(exp);
    }

    /// <summary>
    /// Вычисляет величину обшибки.
    /// <param name="z">Вектор вероятностей</param>
    /// <param name="y">Индекс правильной категории</param>
    /// <returns>Double значение величины ошибки.</returns>
    Double CrossEntropy(Matrix z, int y)    // это разреженная крос-энтропия, т.к. здесь y - это индекс, а не вектор.
    {
        return -Math.Log((z[1, y]));
    }

    void ForwardPropogation()
    {   
        // Проход:
        for (int i = 0; i < t.Count; i++)
        {
            t[i+1] = h[i] * W[i] + B[i];    // Осторожно, h[0] должен быть таким же как t[0], это входной слой.
            h[i+1] = Sigmoid(t[i + 1]);
        }
        Matrix lastLayer = t[t.Count - 1];  // последний слой, к которому не применялась функция активации.
        Matrix z = SoftMax(lastLayer);

        // One hot encoding
        int y = 2; // индекс истиной категории поданного на вход изображения

        // Вычисление ошибки:
        Double Error = CrossEntropy(z, y);
    }
}
