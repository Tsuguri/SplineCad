using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace SplineCAD.Rendering
{
	public class SurfaceMesh : Mesh<PositionVertex>
	{

		public SurfaceMesh(uint xDiv, uint yDiv)
		{
			uint verticesAmount = (xDiv + 1) * (yDiv + 1);
			var vertices = new PositionVertex[verticesAmount];
			var indices = new uint[xDiv * yDiv * 6];

			for (int i = 0; i < xDiv + 1; i++)
				for (int j = 0; j < yDiv + 1; j++)
				{
					vertices[i * (yDiv+1) + j] = new PositionVertex(i / (float)xDiv, j / (float)yDiv, 0);
				}


			for (uint i = 0; i < xDiv; i++)
				for (uint j = 0; j < yDiv; j++)
				{
					indices[(i * yDiv + j) * 6] = i * (yDiv+1) + j;
					indices[(i * yDiv + j) * 6 + 1] = (i + 1) * (yDiv + 1) + j;
					indices[(i * yDiv + j) * 6 + 2] = (i + 1) * (yDiv+1) + j + 1;
					indices[(i * yDiv + j) * 6 + 3] = i * (yDiv+1) + j;
					indices[(i * yDiv + j) * 6 + 4] = (i + 1) * (yDiv+1) + j + 1;
					indices[(i * yDiv + j) * 6 + 5] = i * (yDiv+1) + j + 1;
				}

			Initialize(vertices, indices, BeginMode.Triangles);

		}
	}
}
