using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace NoiseBall {

public sealed class Configurator : MonoBehaviour
{
    [CreateProperty] public float Density { get; set; } = 0.25f;

    [CreateProperty] public int TriangleCount
      => (int)(100 * Mathf.Pow(2, Density * 15));

    [CreateProperty] public int Fps
      => (int)(1 / _deltaTime);

    public float TriangleExtent
      => 4.0f / (1 + 100 * Density * Density);

    float _deltaTime;

    void Start()
      => GetComponent<UIDocument>().rootVisualElement.dataSource = this;

    void Update()
    {
        var target = FindFirstObjectByType<NoiseBallController>();
        target.parameters.TriangleCount = TriangleCount;
        target.parameters.TriangleExtent = TriangleExtent;
        _deltaTime = Mathf.Lerp(_deltaTime, Time.deltaTime, 0.01f);
    }

    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadMethod]
    #else
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    #endif
    public static void RegisterConverters()
    {
        var grp = new ConverterGroup("Thousands Separator");
        grp.AddConverter((ref int v) => $"{v:n0}");
        ConverterGroups.RegisterConverterGroup(grp);
    }
}

} // namespace NoiseBall
