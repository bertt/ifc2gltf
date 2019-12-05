# ifc2gltf

Experimental tool, converts from IFC geometries to glTF 2.0

## Parameters

-i : input file

-o : output file

Sample:

```
$ ifc2gltf -i spaces_all.ifc -o spaces_all.glb

tool ifc2gltf
Input file: spaces_all.ifc
Output file: spaces_all.glb
Start converting to glTF...
Number of geometries: 2507
Converting to glTF finished.
Elapsed: 00:00:07.5423802
```

## Dependencies

- SharpGLTF

- XBim.Essentials

- XBim.Geometry (only works on Windows)

- CommandLineParser

