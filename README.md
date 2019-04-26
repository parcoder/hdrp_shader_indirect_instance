# First, Special thanks "Lennard" for help me to understand HDRP
# Contact: pabloromeroanimator@gmail.com


# hdrp_shader_indirect_instance
HDRP custom shader with indirect instance support



This is my experience writing a custom HDRP 5.6.1 in Unity3d 2019.1.0b7


Indirect instance in HDRP is another history. Because in HDRP every objects depend of the relative camera position.

The shader that I share with you needs a custom ".cginc"
In this file I set the position, rotate and scale foreach instance.

In the custom shader HDRP I was add pragma looks like this.
#pragma instancing_options procedural:setup for call the function.

The script Scatter.cs send the buffer at shader between another things shapes dispertion.
For now support circle area and square area.


 

 


