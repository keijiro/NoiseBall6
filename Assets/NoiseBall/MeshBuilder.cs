using UnityEngine;
using UnityEngine.Rendering;

namespace NoiseBall {

//
// GPU compute-based noise ball mesh builder
//
sealed class MeshBuilder : System.IDisposable
{
    #region Public members

    // Mesh object reference
    public Mesh DynamicMesh { private set; get; }

    // Public constructor
    public MeshBuilder
      (in NoiseBallParameters param, ComputeShader compute)
    {
        _compute = compute;

        // Create a Mesh object as internal temporary.
        DynamicMesh = new Mesh();
        DynamicMesh.hideFlags = HideFlags.HideAndDontSave;

        // We want GraphicsBuffer access as Raw (ByteAddress) buffers.
        DynamicMesh.indexBufferTarget |= GraphicsBuffer.Target.Raw;
        DynamicMesh.vertexBufferTarget |= GraphicsBuffer.Target.Raw;

        // Mesh initialization
        ResetMesh(param);
        BuildMesh(param);
        UpdateBounds(param);
    }

    // IDisposable implementation
    public void Dispose()
    {
        _vertexBuffer?.Dispose();
        _vertexBuffer = null;

        _indexBuffer?.Dispose();
        _indexBuffer = null;

        Util.DestroyObjectSafe(DynamicMesh);
        DynamicMesh = null;
    }

    // Step and update method
    public void Update(in NoiseBallParameters param)
    {
        // Reset the mesh object if the triangle count has been changed.
        if (param.TriangleCount * 3 != DynamicMesh.vertexCount)
            ResetMesh(param);

        // Time step
        if (Application.isPlaying)
            _noiseOffset += param.NoiseAnimation * Time.deltaTime;

        // Mesh update
        BuildMesh(param);
        UpdateBounds(param);
    }

    #endregion

    #region Private methods

    ComputeShader _compute;
    GraphicsBuffer _vertexBuffer;
    GraphicsBuffer _indexBuffer;
    Vector3 _noiseOffset;

    // Mesh object initialization/reset
    void ResetMesh(in NoiseBallParameters param)
    {
        // Dispose previous references.
        _vertexBuffer?.Dispose();
        _indexBuffer?.Dispose();

        // Vertex position: float32 x 3
        var vp = new VertexAttributeDescriptor
          (VertexAttribute.Position, VertexAttributeFormat.Float32, 3);

        // Vertex normal: float32 x 3
        var vn = new VertexAttributeDescriptor
          (VertexAttribute.Normal, VertexAttributeFormat.Float32, 3);

        // Vertex/index buffer formats
        var vertexCount = param.TriangleCount * 3;
        DynamicMesh.SetVertexBufferParams(vertexCount, vp, vn);
        DynamicMesh.SetIndexBufferParams(vertexCount, IndexFormat.UInt32);

        // Submesh initialization
        DynamicMesh.SetSubMesh(0, new SubMeshDescriptor(0, vertexCount),
                               MeshUpdateFlags.DontRecalculateBounds);

        // GraphicsBuffer references
        _vertexBuffer = DynamicMesh.GetVertexBuffer(0);
        _indexBuffer = DynamicMesh.GetIndexBuffer();
    }

    // Execute the compute shader to build the mesh.
    void BuildMesh(in NoiseBallParameters param)
    {
        _compute.SetInt("TriangleCount", param.TriangleCount);
        _compute.SetFloat("TriangleExtent", param.TriangleExtent);
        _compute.SetFloat("NoiseFrequency", param.NoiseFrequency);
        _compute.SetFloat("NoiseAmplitude", param.NoiseAmplitude);
        _compute.SetVector("NoiseOffset", _noiseOffset);

        _compute.SetBuffer(0, "Vertices", _vertexBuffer);
        _compute.DispatchThreads(0, param.TriangleCount);

        _compute.SetBuffer(1, "Indices", _indexBuffer);
        _compute.DispatchThreads(1, param.TriangleCount);
    }

    void UpdateBounds(in NoiseBallParameters param)
      => DynamicMesh.bounds = new Bounds
           (Vector3.zero, Vector3.one * (1 + param.NoiseAmplitude * 2));

    #endregion
}

} // namespace NoiseBall
