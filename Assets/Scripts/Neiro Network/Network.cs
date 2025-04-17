using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

/// <summary>
/// Нейросеть обучаемая.
/// </summary>
public class Network : MonoBehaviour
{
    [Inject] DatasetValidator datasetValidator;

    [Tooltip("Готовность нейросети к использованию.")]
    public UnityAction onReady;

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
        if (datasetValidator.isValid)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            InitIntermediateLayers();
            InitActivatedLayers();
            InitWeights();
            InitBiases();

            stopwatch.Stop();
            UnityEngine.Debug.Log($"Создание матриц Numpy: {stopwatch.Elapsed.TotalSeconds} секунд");

            onReady?.Invoke();
        }
    }

    /// <summary>
    /// Инициализирует промежуточные слои.
    /// </summary>
    void InitIntermediateLayers()
    {
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

    #region ДОПФУНКЦИИ
    Matrix Sigmoid(Matrix t)
    {
        Matrix res = 1 / (1 + (Numpy.Exp(-t)));
        return res;
    }
    /// <summary>
    /// Произовдная от sigmoid.
    /// </summary>
    /// <returns>Вектор, к каждому элементу которого применена произовдная</returns>
    Matrix Sigmoid_deriv(Matrix t)
    {
        Matrix sigmoid = 1 / (1 + Numpy.Exp(-t));
        return sigmoid * (1 - sigmoid);
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
        return -Math.Log((z[0, y]));
    }

    /// <summary>
    /// One-hot encoding
    /// </summary>
    /// <param name="y">Индекс истиной категории изображения.</param>
    /// <returns>Вектор правильного ответа. Нулевой вектор, с единицей по индексу истиной категории изображения.</returns>
    Matrix to_full(int y)
    {
        int num_classes = t[t.Count - 1].GetLength(1);
        Matrix y_full = Numpy.Zeros(1, num_classes);
        y_full[0, y] = 1;
        return y_full;

    }

    /// <summary>
    /// Применяет шаг градиентного спуска к весам указанного слоя.
    /// </summary>
    /// <param name="layer">Индекс слоя, к которому применяется обновление.</param>
    void ApplyGradientStep(Matrix dE_dW, Matrix dE_dB, int layer)
    {
        W[layer] -= 0.001 * dE_dW;
        B[layer] -= 0.001 * dE_dB;
    }
    #endregion




    /// <summary>
    /// Истиный индекс категории изображения.
    /// </summary>
    int y = 2;

    /// <summary>
    /// Вектор предположения сети. Нормализованный.
    /// </summary>
    Matrix z;
    void ForwardPropogation()
    {   
        // Проход:
        for (int i = 0; i < t.Count-1; i++)
        {
            t[i+1] = h[i] ^ W[i] + B[i];    // Осторожно, h[0] должен быть таким же как t[0], это входной слой.
            h[i+1] = Sigmoid(t[i + 1]);
        }
        Matrix lastLayer = t[t.Count - 1];  // последний слой, к которому не применялась функция активации.
        z = SoftMax(lastLayer);

        // Вычисление ошибки:
        Double Error = CrossEntropy(z, y);
    }

    
    void BackPropogation()
    {
        // One hot encoding
        Matrix y_full = to_full(y);

        // Последний слой:
        Matrix dE_dt = z - y_full;
        Matrix dE_dW = h[t.Count - 2].T() ^ dE_dt;
        Matrix dE_dB = dE_dt;
        ApplyGradientStep(dE_dW, dE_dB, t.Count-2);
        Matrix dE_dh = dE_dt ^ W[t.Count - 2].T();
        dE_dt = dE_dh * Sigmoid_deriv(t[t.Count - 2]);

        //
        for (int i = t.Count-3; i >= 1; i--)
        {
            dE_dW = h[i].T() ^ dE_dt;
            dE_dB = dE_dt;
            ApplyGradientStep(dE_dW, dE_dB, i);
            dE_dh = dE_dt ^ W[i].T();
            dE_dt = dE_dh * Sigmoid_deriv(t[i]);
        }

        // Первый слой немного отличается:
        dE_dW = h[0].T() ^ dE_dt;
        dE_dB = dE_dt;
        ApplyGradientStep(dE_dW, dE_dB, 0);
    }


}
