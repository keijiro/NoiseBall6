using UnityEngine;

namespace NoiseBall {

[ExecuteInEditMode]
public sealed class NoiseBallController : MonoBehaviour
{
    #region Editable attributes

    public NoiseBallParameters parameters = NoiseBallParameters.Default();

    #endregion

    #region Project asset reference

    [SerializeField, HideInInspector] ComputeShader _compute = null;

    #endregion

    #region MonoBehaviour implementation

    MeshBuilder _builder;
    MeshFilter _filter;

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
            _builder = new MeshBuilder(parameters, _compute);
        else
            _builder.Update(parameters);

        // Mesh filter component lazy initialization and update
        if (_filter == null)
        {
            _filter = gameObject.AddComponent<MeshFilter>();
            _filter.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
        }

        _filter.sharedMesh = _builder.DynamicMesh;
    }

    #endregion
}

} // namespace NoiseBall
