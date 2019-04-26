using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public sealed class Scatter : MonoBehaviour
{
    [SerializeField] Mesh _sourceMesh;
    [SerializeField] float _extent = 1;
    [Range(1, 40000)]
    [SerializeField] int _instanceCount = 100;
    [SerializeField] Color _color = Color.white;
    [SerializeField] float _scale = 1;
    [SerializeField] [Range(0, 1)] int _shape;

    [SerializeField, HideInInspector] Shader _shader;

    ComputeBuffer IndirectShaderData;

    public Material _material;
    MaterialPropertyBlock _sheet;


    void OnValidate()
    {
        _instanceCount = Mathf.Max(1, _instanceCount);
    }

    void OnDisable()
    {
        if (IndirectShaderData != null)
        {
            IndirectShaderData.Release();
            IndirectShaderData = null;
        }
    }

    void OnDestroy()
    {
        if (_material)
        {
            if (Application.isPlaying)
                Destroy(_material);
            else
                DestroyImmediate(_material);
        }
    }

    void Update()
    {
        if (_sourceMesh == null) return;

        if (IndirectShaderData == null)
            IndirectShaderData = new ComputeBuffer(
                1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);

        if (_material == null) _material = new Material(_shader);
        if (_sheet == null) _sheet = new MaterialPropertyBlock();

        var bounds = new Bounds(
            transform.position,
            new Vector3(_extent, _sourceMesh.bounds.size.magnitude, _extent)
        );

        _sheet.SetColor("_Color", _color);
        _sheet.SetFloat("_Scale", _scale);
        _sheet.SetFloat("_Area", _extent);
        _sheet.SetInt("_Shape", _shape);
        _sheet.SetFloat("_Time", Application.isPlaying ? Time.time : 0);
        _sheet.SetMatrix("_LocalToWorld", transform.localToWorldMatrix);
        _sheet.SetMatrix("_WorldToLocal", transform.worldToLocalMatrix);

        IndirectShaderData.SetData(new uint[5] {
            _sourceMesh.GetIndexCount(0), (uint)_instanceCount, 0, 0, 0 });

        Graphics.DrawMeshInstancedIndirect(
            _sourceMesh, 0, _material, bounds, IndirectShaderData, 0, _sheet
        );

        
    }
  private void OnDrawGizmos()
    {
        if(_shape == 0)
        {
            Handles.color = Color.blue;
            Handles.matrix = this.transform.localToWorldMatrix;
            Handles.DrawWireCube(Vector3.zero, new Vector3(_extent, 0, _extent));
        }
        if(_shape == 1)
        {
            Handles.color = Color.blue;
            Handles.matrix = this.transform.localToWorldMatrix;
            Handles.DrawWireDisc(Vector3.zero, Vector3.up, _extent / 2);
        }
    }
}
