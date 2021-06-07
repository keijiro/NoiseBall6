using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
sealed class NoiseBall : MonoBehaviour
{
    #region Editable attributes

    [SerializeField] int _triangleCount = 1000;
    [SerializeField] float _triangleExtent = 0.1f;
    [SerializeField] float _noiseFrequency = 1;
    [SerializeField] float _noiseAmplitude = 0.5f;
    [SerializeField] Vector3 _noiseAnimation = Vector3.one;
    [SerializeField] Material _material = null;

    #endregion

    #region Project asset reference

    [SerializeField, HideInInspector] ComputeShader _compute = null;

    #endregion

    #region Mesh object

    Mesh _mesh;
    GraphicsBuffer _vertexBuffer;
    GraphicsBuffer _indexBuffer;

    int VertexCount => _triangleCount * 3;

    void ResetMesh()
    {
        // Dispose previous references.
        if (_vertexBuffer != null) _vertexBuffer.Dispose();
        if (_vertexBuffer != null) _indexBuffer.Dispose();

        // Vertex position: float32 x 3
        var vp = new VertexAttributeDescriptor
          (VertexAttribute.Position, VertexAttributeFormat.Float32, 3);

        // Vertex normal: float32 x 3
        var vn = new VertexAttributeDescriptor
          (VertexAttribute.Normal, VertexAttributeFormat.Float32, 3);

        // Vertex/index buffer formats
        _mesh.SetVertexBufferParams(VertexCount, vp, vn);
        _mesh.SetIndexBufferParams(VertexCount, IndexFormat.UInt32);

        // Submesh initialization
        _mesh.SetSubMesh(0, new SubMeshDescriptor(0, VertexCount),
                         MeshUpdateFlags.DontRecalculateBounds);

        // Bounds (1000x1000x1000)
        _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);

        // GraphicsBuffer references
        _vertexBuffer = _mesh.GetVertexBuffer(0);
        _indexBuffer = _mesh.GetIndexBuffer();
    }

    #endregion

    #region MonoBehaviour implementation

    Vector3 _noiseOffset;

    void OnValidate()
      => _triangleCount = Mathf.Max(1, _triangleCount);

    void OnDisable()
    {
        OnDestroy();
    }

    void OnDestroy()
    {
        if (_vertexBuffer != null) _vertexBuffer.Dispose();
        if (_indexBuffer != null) _indexBuffer.Dispose();

        if (_mesh != null)
        {
            if (Application.isPlaying)
                Destroy(_mesh);
            else
                DestroyImmediate(_mesh);
        }

        _vertexBuffer = null;
        _indexBuffer = null;
        _mesh = null;
    }

    void Update()
    {
        if (_mesh == null)
        {
            _mesh = new Mesh();
            _mesh.hideFlags = HideFlags.HideAndDontSave;

            // We want GraphicsBuffer access as Raw (ByteAddress) buffers.
            _mesh.indexBufferTarget |= GraphicsBuffer.Target.Raw;
            _mesh.vertexBufferTarget |= GraphicsBuffer.Target.Raw;
        }

        if (VertexCount != _mesh.vertexCount) ResetMesh();

        var groupCount = (_triangleCount + 63) / 64;

        if (Application.isPlaying)
            _noiseOffset += _noiseAnimation * Time.deltaTime;

        var param = new Vector3
          (_noiseFrequency, _noiseAmplitude, _triangleExtent);

        _compute.SetInt("TriangleCount", _triangleCount);
        _compute.SetVector("Parameters", param);
        _compute.SetVector("NoiseOffset", _noiseOffset);

        _compute.SetBuffer(0, "Vertices", _vertexBuffer);
        _compute.Dispatch(0, groupCount, 1, 1);

        _compute.SetBuffer(1, "Indices", _indexBuffer);
        _compute.Dispatch(1, groupCount, 1, 1);

        Graphics.DrawMesh
          (_mesh, transform.localToWorldMatrix, _material, gameObject.layer);
    }

    #endregion
}
