using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplineCAD.Rendering
{
	public class ExplicitMesh : Mesh
	{
		public ExplicitMesh(PositionVertex[] vertices, uint[] indices)
		{
			Initialize(vertices,indices);
		}
	}
}
