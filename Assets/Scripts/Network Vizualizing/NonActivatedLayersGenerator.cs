﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Отрисовывает неактивированные слои.
/// </summary>
public class NonActivatedLayersGenerator : MonoBehaviour
{
    [Inject] Network network;
    [Inject] GradientColorPicker colorPicker;
    [Inject] DatasetValidator datasetValidator;

    public float offset = -5f;

    [Header("Материал для пикселей")]
    [SerializeField] Material pixelMaterial;

    [Header("Размер пикселя (plane):")]
    [Range(0.1f, 2f)] public float pixelSize = 1f;

    [Header("Расстояние между пикселями:")]
    [Range(1f, 2f)] public float pixelSpacing = 1.2f;

    [NonSerialized] public List<Vector3> pixelPositions = new();
    [NonSerialized] public List<Color> pixelColors = new();

    public void DrawLayers()
    {
        pixelPositions.Clear();
        pixelColors.Clear();

        Vector2Int imgSize = datasetValidator.imageSize;

        // Входной слой
        AddInputLayer(network.t[0], imgSize);

        // Скрытые слои
        for (int i = 1; i < network.h.Count; i++)
            AddHiddenLayer(network.t[i], i, imgSize);

        // Выходной слой
        //Matrix lastLayer = network.t[^1];  // последний слой, к которому не применялась функция активации.
        //Matrix z = MatrixUtils.SoftMax(lastLayer);  // вектор вероятностей
        //AddOutputLayer(z, network.t.Count - 1, imgSize);
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
                layerIndex * Mathf.Max(imgSize.x, imgSize.y));

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
                layerIndex * Mathf.Max(imgSize.x, imgSize.y));

            pos -= new Vector3(count / 2f, 0, 0) * pixelSpacing;

            AddPixel(pos, (float)matrix[0, x]);
        }
    }

    private void AddPixel(Vector3 pos, float value)
    {
        pixelPositions.Add(pos + new Vector3(0,0,offset));  // смещаем неактивированные пиксели
        pixelColors.Add(colorPicker.GetNonActivatedColor(value));
    }

    private void OnRenderObject()
    {
        if (pixelPositions.Count == 0)
            return;

        pixelMaterial.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.QUADS);

        int firstPixel = datasetValidator.imageSize.x * datasetValidator.imageSize.y;
        for (int i = firstPixel; i < pixelPositions.Count; i++)
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
