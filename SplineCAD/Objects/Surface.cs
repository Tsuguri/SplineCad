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
	public class Surface : Renderable
	{
		private IPoint[,] points;

		private MainDataContext sceneData;

		private readonly RectangesPolygonMesh mesh;

		private bool polygonVisible;

		public bool PolygonVisible
		{
			get => polygonVisible;
			set
			{
				polygonVisible = value;
				OnPropertyChanged();
			}
		}


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
			mesh = new RectangesPolygonMesh(points);
		}

		public override void Render()
		{
			mesh.Render();
		}
	}
}
