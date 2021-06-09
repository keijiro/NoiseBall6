using UnityEngine;

namespace NoiseBall {

// Serializable parameter aggregate
[System.Serializable]
public struct NoiseBallParameters
{
    public int TriangleCount;
    public float TriangleExtent;
    public float NoiseFrequency;
    public float NoiseAmplitude;
    public Vector3 NoiseAnimation;

    public static NoiseBallParameters Default()
    {
        var s = new NoiseBallParameters();
        s.TriangleCount = 10000;
        s.TriangleExtent = 0.3f;
        s.NoiseFrequency = 2.2f;
        s.NoiseAmplitude = 0.85f;
        s.NoiseAnimation = new Vector3(0, 0.13f, 0.51f);
        return s;
    }
}

} // namespace NoiseBall
