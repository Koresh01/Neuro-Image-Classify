using UnityEngine;

[AddComponentMenu("Custom/PixelColorSetter (���� ���������� ���� ������)")] // �� �� ������ ���� ��������
[RequireComponent(typeof(MeshRenderer))]
public class PixelColorSetter : MonoBehaviour
{
    private MaterialPropertyBlock _mpb;
    private MeshRenderer _renderer;

    void Awake()
    {
        _mpb = new MaterialPropertyBlock();
        _renderer = GetComponent<MeshRenderer>();
    }

    public void SetColor(Color color)
    {
        _mpb.SetColor("_Color", color);
        _renderer.SetPropertyBlock(_mpb);
    }
}
