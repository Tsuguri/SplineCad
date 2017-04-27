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
	public class BSplineSurface : Surface
	{
		private IPoint[,] points;

		private MainDataContext sceneData;

		private readonly RectangesPolygonMesh mesh;
		private SurfaceMesh surfaceMesh;

		private int patchesX;
		private int patchesY;

		private Shader surfaceShader;
		private Shader polygonShader;

		public BSplineSurface(MainDataContext data, Shader surfaceShader, Shader polygonShader, IPoint[,] controlPoints)
		{
			this.sceneData = data;
			this.surfaceShader = surfaceShader;
			this.polygonShader = polygonShader;
			this.points = controlPoints;
			patchesX = controlPoints.GetLength(0) - 3;
			patchesY = controlPoints.GetLength(1) - 3;

			mesh = new RectangesPolygonMesh(points);
			surfaceMesh = new SurfaceMesh(10, 10);

		}

		public override void CleanUp()
		{
			mesh.Dispose();
			surfaceMesh.Dispose();
		}

		public override void Render()
		{
			if (PolygonVisible)
			{
				polygonShader.Activate();
				mesh.Render();
			}
			GL.PolygonMode(MaterialFace.Front, FillSurface ? PolygonMode.Fill : PolygonMode.Line);
			GL.PolygonMode(MaterialFace.Back, FillSurface ? PolygonMode.Fill : PolygonMode.Line);

			surfaceShader.Activate();

			var b00 = surfaceShader.GetUniformLocation("b00");
			var b01 = surfaceShader.GetUniformLocation("b01");
			var b02 = surfaceShader.GetUniformLocation("b02");
			var b03 = surfaceShader.GetUniformLocation("b03");
			var b10 = surfaceShader.GetUniformLocation("b10");
			var b11 = surfaceShader.GetUniformLocation("b11");
			var b12 = surfaceShader.GetUniformLocation("b12");
			var b13 = surfaceShader.GetUniformLocation("b13");
			var b20 = surfaceShader.GetUniformLocation("b20");
			var b21 = surfaceShader.GetUniformLocation("b21");
			var b22 = surfaceShader.GetUniformLocation("b22");
			var b23 = surfaceShader.GetUniformLocation("b23");
			var b30 = surfaceShader.GetUniformLocation("b30");
			var b31 = surfaceShader.GetUniformLocation("b31");
			var b32 = surfaceShader.GetUniformLocation("b32");
			var b33 = surfaceShader.GetUniformLocation("b33");



			//draw every patch
			for (int i = 0; i < patchesX; i++)
			{
				for (int j = 0; j < patchesY; j++)
				{
					surfaceShader.Bind(b00, points[i + 0, j + 0].Position);
					surfaceShader.Bind(b01, points[i + 0, j + 1].Position);
					surfaceShader.Bind(b02, points[i + 0, j + 2].Position);
					surfaceShader.Bind(b03, points[i + 0, j + 3].Position);
					surfaceShader.Bind(b10, points[i + 1, j + 0].Position);
					surfaceShader.Bind(b11, points[i + 1, j + 1].Position);
					surfaceShader.Bind(b12, points[i + 1, j + 2].Position);
					surfaceShader.Bind(b13, points[i + 1, j + 3].Position);
					surfaceShader.Bind(b20, points[i + 2, j + 0].Position);
					surfaceShader.Bind(b21, points[i + 2, j + 1].Position);
					surfaceShader.Bind(b22, points[i + 2, j + 2].Position);
					surfaceShader.Bind(b23, points[i + 2, j + 3].Position);
					surfaceShader.Bind(b30, points[i + 3, j + 0].Position);
					surfaceShader.Bind(b31, points[i + 3, j + 1].Position);
					surfaceShader.Bind(b32, points[i + 3, j + 2].Position);
					surfaceShader.Bind(b33, points[i + 3, j + 3].Position);



					surfaceMesh.Render();
				}
			}

		}
	}
}
