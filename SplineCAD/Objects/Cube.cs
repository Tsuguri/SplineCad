using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SplineCAD.Rendering;

namespace SplineCAD.Objects
{
	class Cube : Mesh<PositionVertex>
	{

		private int indicesCount;

		public Cube()
		{
			float size = 2;
			float halfsize = size / 2.0f;
			PositionVertex[] vertices = {
				new PositionVertex(-halfsize, -halfsize, -halfsize),
				new PositionVertex(-halfsize, -halfsize, halfsize),
				new PositionVertex(halfsize, -halfsize, -halfsize),
				new PositionVertex(halfsize, -halfsize, halfsize),
				new PositionVertex(-halfsize, halfsize, -halfsize),
				new PositionVertex(-halfsize, halfsize, halfsize),
				new PositionVertex(halfsize, halfsize, -halfsize),
				new PositionVertex(halfsize, halfsize, halfsize),
			};


			uint[] indices = {
				//0,1,3, //down
				//0,3,2,

				//4,5,6, //up
				//5,6,7,

				//1,5,0, //left
				//0,5,4,

				//2,6,3, //right
				//3,6,7,

				//4,0,6, //front
				//0,6,2

                0,1,
                2,3,

                3,1,        
                2,0,

                4,5,
                6,7,

                5,7,                
                4,6,

                1,5,
                0,4,

                2,6,
                3,7,
            };

			Initialize(vertices,indices, BeginMode.Lines);
		}
	}
}
