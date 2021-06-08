using UnityEngine;

static class Util
{
    // "Please safely destroy this object in any situation!"
    public static void DestroyObjectSafe(Object o)
    {
        if (o == null) return;
        if (Application.isPlaying)
            Object.Destroy(o);
        else
            Object.DestroyImmediate(o);
    }
}

static class ComputeShaderExtensions
{
    // Execute a compute shader with specifying a minimum number of thread
    // count not by a thread GROUP count.
    public static void DispatchThreads
      (this ComputeShader compute, int kernel, int count)
    {
        uint x, y, z;
        compute.GetKernelThreadGroupSizes(kernel, out x, out y, out z);
        var groups = (count + (int)x - 1) / (int)x;
        compute.Dispatch(kernel, groups, 1, 1);
    }
}
