NoiseBall6
==========

![photo](https://user-images.githubusercontent.com/343936/121196685-a64da000-c8ab-11eb-9638-6897a1c6c088.jpg)

**NoiseBall6** is a Unity sample project that shows how to access vertex/index buffers directly from compute shaders.

This sample uses the new Mesh API (available from Unity 2021.2a19) that exposes vertex/index buffers from a Mesh object via GraphicsBuffer objects. You can directly read/write them from compute shaders without spending CPU-side resources.

For details of the new API, please check out [the introductory document](https://docs.google.com/document/d/1_YrJafo9_ZsFm4-8K2QlD0k3RgwZ_49tSA84paobfcY/edit#heading=h.cvw3aojqmyd2).

What's inside
-------------

This repository contains a common source code package and multiple projects that work with each render pipeline/feature.

- **Common**: C# scripts & compute shaders
- **DXR**: HDRP + DXR sample project (requires ray-tracing hardware)
- **HDRP**: HDRP sample project (runs on desktop/console)
- **URP**: URP sample project (compatible with all platforms)

The **Common** package is automatically referenced from each project. You can simply open these projects without any special care.
