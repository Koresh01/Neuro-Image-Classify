﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Отрисовывает активированные нейроны.
/// </summary>
public class ActivatedLayersGenerator : MonoBehaviour
{
    [Inject] Network network;
    [Inject] GradientColorPicker colorPicker;
    [Inject] DatasetValidator datasetValidator;

    [Header("Материал для пикселей")]
    [SerializeField] Material pixelMaterial;

    [Header("Размер пикселя (plane):")]
    [Range(0.1f, 2f)] public float pixelSize = 1f;

    [Header("Расстояние между пикселями:")]
    [Range(1f, 2f)] public float pixelSpacing = 1.2f;

    [Header("Расстояние между слоями нейросети:")]
    public float pixelSpacing_Z = 50;

    [NonSerialized] public List<Vector3> pixelPositions = new();
    [NonSerialized] public List<Color> pixelColors = new();

    public void DrawLayers()
    {
        pixelPositions.Clear();
        pixelColors.Clear();

        Vector2Int imgSize = datasetValidator.imageSize;

        // Входной слой
        AddInputLayer(network.h[0], imgSize);

        // Скрытые слои
        for (int i = 1; i < network.h.Count - 1; i++)
            AddHiddenLayer(network.h[i], i, imgSize);

        // Выходной слой
        Matrix lastLayer = network.t[^1];  // последний слой, к которому не применялась функция активации.
        Matrix z = MatrixUtils.SoftMax(lastLayer);  // вектор вероятностей
        AddOutputLayer(z, network.t.Count - 1, imgSize);
    }

    private void AddInputLayer(Matrix matrix, Vector2Int imgSize)
    {
        for (int y = 0; y < imgSize.y; y++)
        {
            for (int x = 0; x < imgSize.x; x++)
            {
                Vector3 pos = new(
                    x * pixelSpacing,
                    y * pixelSpacing,
                    0);

                // Центрирование:
                pos -= new Vector3(imgSize.x / 2f, imgSize.y / 2f, 0) * pixelSpacing;

                AddPixel(pos, (float)matrix[0, y * imgSize.x + x]);
            }
        }
    }

    private void AddHiddenLayer(Matrix matrix, int layerIndex, Vector2Int imgSize)
    {
        int count = matrix.Columns;
        int side = Mathf.CeilToInt(Mathf.Sqrt(count));

        for (int i = 0; i < count; i++)
        {
            int x = i % side;
            int y = i / side;

            Vector3 pos = new(
                x * pixelSpacing,
                y * pixelSpacing,
                layerIndex * pixelSpacing_Z);

            pos -= new Vector3(side / 2f, side / 2f, 0) * pixelSpacing;

            AddPixel(pos, (float)matrix[0, i]);
        }
    }

    private void AddOutputLayer(Matrix matrix, int layerIndex, Vector2Int imgSize)
    {
        int count = matrix.Columns;

        for (int x = 0; x < count; x++)
        {
            Vector3 pos = new(
                x * pixelSpacing,
                0,
                layerIndex * pixelSpacing_Z);

            pos -= new Vector3(count / 2f, +1, 0) * pixelSpacing;

            AddPixel(pos, (float)matrix[0, x]);
        }
    }

    private void AddPixel(Vector3 pos, float value)
    {
        pixelPositions.Add(pos);
        pixelColors.Add(colorPicker.GetActivatedColor(value));
    }

    private void OnRenderObject()
    {
        if (pixelPositions.Count == 0)
            return;

        pixelMaterial.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.QUADS);

        for (int i = 0; i < pixelPositions.Count; i++)
        {
            GL.Color(pixelColors[i]);
            DrawQuad(pixelPositions[i], pixelSize);
        }

        GL.End();
        GL.PopMatrix();
    }

    private void DrawQuad(Vector3 center, float size)
    {
        float half = size / 2f;

        Vector3 right = Vector3.right * half;
        Vector3 up = Vector3.up * half;

        // Меняем порядок вершин на противоположный (против часовой стрелки)
        GL.Vertex(center - right + up);
        GL.Vertex(center + right + up);
        GL.Vertex(center + right - up);
        GL.Vertex(center - right - up);
    }
}
