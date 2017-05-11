using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace SplineCAD.Rendering
{
	public class ExplicitMesh<TVertType> : Mesh<TVertType> where TVertType : struct, IVertex
	{
		public ExplicitMesh(TVertType[] vertices, uint[] indices, BeginMode mode = BeginMode.Triangles)
		{
			Initialize(vertices,indices,mode);
		}
	}
}
