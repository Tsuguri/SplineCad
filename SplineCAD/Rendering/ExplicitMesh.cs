using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace SplineCAD.Rendering
{
	public class ExplicitMesh : Mesh
	{
		public ExplicitMesh(PositionVertex[] vertices, uint[] indices, BeginMode mode = BeginMode.Triangles)
		{
			Initialize(vertices,indices,mode);
		}
	}
}
