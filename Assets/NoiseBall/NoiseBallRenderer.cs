using UnityEngine;
using RayTracingMode = UnityEngine.Experimental.Rendering.RayTracingMode;

[ExecuteInEditMode]
sealed class NoiseBallRenderer : MonoBehaviour
{
    #region Editable attributes

    [SerializeField] NoiseBallParameters _parameters = NoiseBallParameters.Default();
    [SerializeField] Material _material = null;

    #endregion

    #region Project asset reference

    [SerializeField, HideInInspector] ComputeShader _compute = null;

    #endregion

    #region MonoBehaviour implementation

    NoiseBallMeshBuilder _builder;
    MeshFilter _filter;
    MeshRenderer _renderer;

    void OnDisable() => OnDestroy();

    void OnDestroy()
    {
        _builder?.Dispose();
        _builder = null;
    }

    void Update()
    {
        // Mesh builder initialization/update
        if (_builder == null)
            _builder = new NoiseBallMeshBuilder(_parameters, _compute);
        else
            _builder.Update(_parameters);

        // Mesh filter component lazy initialization and update
        if (_filter == null)
        {
            _filter = gameObject.AddComponent<MeshFilter>();
            _filter.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
        }

        _filter.sharedMesh = _builder.DynamicMesh;

        // Mesh renderer component lazy initialization and update
        if (_renderer == null)
        {
            _renderer = gameObject.AddComponent<MeshRenderer>();
            _renderer.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
            //_renderer.rayTracingMode = RayTracingMode.DynamicGeometry;
        }

        _renderer.sharedMaterial = _material;
    }

    #endregion
}
