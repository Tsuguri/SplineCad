using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SplineCAD.Data;
using SplineCAD.Rendering;

namespace SplineCAD.Objects
{
	public class Surface : Rendering.Renderable
	{
		private IPoint[,] points;

		private MainDataContext sceneData;

		private SelfActualizingMesh mesh;

		public Surface(MainDataContext data)
		{
			this.sceneData = data;
			points = new IPoint[10, 7];

			for (int i = 0; i < 10; i++)
				for (int j = 0; j < 7; j++)
				{

					points[i, j] = sceneData.CreatePoint();
					points[i, j].Position = new Vector3(i, (float)Math.Sin(0.5 * i), j);
				}
			int xDiv = points.GetLength(0);
			int yDiv = points.GetLength(1);

			var vertices = new List<IPoint>(xDiv*yDiv);
			var indices = new uint[((xDiv - 1) * (yDiv - 1) * 2 + xDiv + yDiv - 2) * 2];
			for (int i = 0; i < points.GetLength(0); i++)
				for (int j = 0; j < points.GetLength(1); j++)
				{
					vertices.Add(points[i, j]);
				}



			mesh = new SelfActualizingMesh(vertices, indices, BeginMode.Lines);
		}

		public override void Render()
		{

		}
	}
}
