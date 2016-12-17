# Unity 3D metaballs
Unity 3D implementation of marching cubes algorithm for rendering metaballs. Based on [implementation by Dario Zubovic](github.com/dario-zubovic).

![](https://github.com/dario-zubovic/metaballs/raw/master/gif2.gif)

Metaball functions are evaluated on GPU. Falloff functions can be chosen in the compute shader with a define statements.

## TODO list:
* Improve GPU memory and compute by using geometry shaders to instantiate triangles
* implement smooth normals

## References:
* William E. Lorensen and Harvey E. Cline. 1987. Marching cubes: A high resolution 3D surface construction algorithm. In Proceedings of the 14th annual conference on Computer graphics and interactive techniques (SIGGRAPH '87), Maureen C. Stone (Ed.). ACM, New York, NY, USA, 163-169.
* Marching cubes lookup tables taken from here: http://www.paulsprojects.net/opengl/metaballs/metaballs.html
* Brian Wyvill and Geoff Wyvill. Field functions for implicit surfaces. The Visual Computer, 5:75–82, December 1989.
