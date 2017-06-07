using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SplineCAD.Data;
using SplineCAD.Rendering;
using SplineCAD.Utilities;

namespace SplineCAD.Objects
{
	public class NurbsSurface : Surface
	{
		private IPoint<Vector4>[,] points;

		private MainDataContext sceneData;

		private readonly Vector4RectangesPolygonMesh mesh;
		private SurfaceMesh surfaceMesh;

		private readonly int patchesX;
		private readonly int patchesY;
		private readonly int pointsX;
		private readonly int pointsY;

		private readonly Shader surfaceShader;
		private readonly Shader polygonShader;

		private bool divChanged;



		private readonly ObservableCollection<FloatWrapper> uDivs;
		private readonly ObservableCollection<FloatWrapper> vDivs;

		public ObservableCollection<FloatWrapper> UDivs => uDivs;
		public ObservableCollection<FloatWrapper> VDivs => vDivs;

        public IPoint<Vector4>[,] Points { get => points; }

		protected override void PatchDivChanged()
		{
			base.PatchDivChanged();
			divChanged = true;
		}

		public NurbsSurface(MainDataContext data, Shader surfaceShader, Shader polygonShader, IPoint<Vector4>[,] controlPoints)
		{
			this.sceneData = data;
			this.surfaceShader = surfaceShader;
			this.polygonShader = polygonShader;
			this.points = controlPoints;

			patchesX = controlPoints.GetLength(0) - 3;
			patchesY = controlPoints.GetLength(1) - 3;
			pointsX = patchesX + 3;
			pointsY = patchesY + 3;

			mesh = new Vector4RectangesPolygonMesh(points);
			surfaceMesh = new SurfaceMesh((uint)PatchDivX, (uint)PatchDivY);

			uDivs = new ObservableCollection<FloatWrapper>();
			vDivs = new ObservableCollection<FloatWrapper>();

			float uStep = 1 / (float)(pointsX - 1);
			float vStep = 1 / (float)(pointsY - 1);

			for (int i = 0; i < pointsX; i++)
				uDivs.Add(new FloatWrapper(i * uStep));
			for (int i = 0; i < pointsY; i++)
				vDivs.Add(new FloatWrapper(i * vStep));


		}

		public override void CleanUp()
		{
			mesh.Dispose();
			surfaceMesh.Dispose();
		}

		public override void Render()
		{
			if (divChanged)
			{
				divChanged = false;
				surfaceMesh?.Dispose();
				surfaceMesh = new SurfaceMesh((uint)PatchDivX, (uint)PatchDivY);
			}
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


			var tu1 = surfaceShader.GetUniformLocation("tu1");
			var tu2 = surfaceShader.GetUniformLocation("tu2");
			var tu3 = surfaceShader.GetUniformLocation("tu3");
			var tu4 = surfaceShader.GetUniformLocation("tu4");
			var tu5 = surfaceShader.GetUniformLocation("tu5");
			var tu6 = surfaceShader.GetUniformLocation("tu6");
			var tu7 = surfaceShader.GetUniformLocation("tu7");

			var tv1 = surfaceShader.GetUniformLocation("tv1");
			var tv2 = surfaceShader.GetUniformLocation("tv2");
			var tv3 = surfaceShader.GetUniformLocation("tv3");
			var tv4 = surfaceShader.GetUniformLocation("tv4");
			var tv5 = surfaceShader.GetUniformLocation("tv5");
			var tv6 = surfaceShader.GetUniformLocation("tv6");
			var tv7 = surfaceShader.GetUniformLocation("tv7");

            var camPos = surfaceShader.GetUniformLocation("camPos");
            var lightPos = surfaceShader.GetUniformLocation("lightPos");

            surfaceShader.Bind(lightPos, sceneData.LightPos);
            surfaceShader.Bind(camPos, sceneData.MainCamera.Position);

            var vs = new List<float> { vDivs[0].Value * 3 - vDivs[1].Value*2, vDivs[0].Value * 2 -  vDivs[1].Value }.Concat(vDivs.Select(x=>x.Value))
				.Concat(new List<float>
				{
					vDivs[vDivs.Count - 1].Value * 2 - vDivs[vDivs.Count - 2].Value,
					vDivs[vDivs.Count - 1].Value * 3 - vDivs[vDivs.Count - 2].Value * 2
				})
				.ToList();

			var us = new List<float> { uDivs[0].Value * 2 - uDivs[2].Value, uDivs[0].Value * 2 - uDivs[1].Value }.Concat(uDivs.Select(x => x.Value))
				.Concat(new List<float>
				{
					uDivs[uDivs.Count - 1].Value * 2 - uDivs[uDivs.Count - 2].Value,
					uDivs[uDivs.Count - 1].Value * 2 - uDivs[uDivs.Count - 3].Value
				})
				.ToList();
             
			//draw every patch
			for (int i = 0; i < patchesX; i++)
			{
				surfaceShader.Bind(tu1, us[i + 1] - us[i]);
				surfaceShader.Bind(tu2, us[i + 2] - us[i]);
				surfaceShader.Bind(tu3, us[i + 3] - us[i]);
				surfaceShader.Bind(tu4, us[i + 4] - us[i]);
				surfaceShader.Bind(tu5, us[i + 5] - us[i]);
				surfaceShader.Bind(tu6, us[i + 6] - us[i]);
				surfaceShader.Bind(tu7, us[i + 7] - us[i]);

				for (int j = 0; j < patchesY; j++)
				{
					surfaceShader.Bind(tv1, vs[j + 1] - vs[j]);
					surfaceShader.Bind(tv2, vs[j + 2] - vs[j]);
					surfaceShader.Bind(tv3, vs[j + 3] - vs[j]);
					surfaceShader.Bind(tv4, vs[j + 4] - vs[j]);
					surfaceShader.Bind(tv5, vs[j + 5] - vs[j]);
					surfaceShader.Bind(tv6, vs[j + 6] - vs[j]);
					surfaceShader.Bind(tv7, vs[j + 7] - vs[j]);

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
