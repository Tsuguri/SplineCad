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
	class TSplineSurface : Surface
	{
		private class UComparer : IComparer<PointWrapper>
		{
			public int Compare(PointWrapper x, PointWrapper y)
			{
				return x.U.CompareTo(y.U);
			}
		}

		private class VComparer : IComparer<PointWrapper>
		{
			public int Compare(PointWrapper x, PointWrapper y)
			{
				return x.V.CompareTo(y.V);
			}
		}

		private class PointWrapper
		{
			private readonly IPoint<Vector4> point;
			private readonly float u;
			private readonly float v;
			private readonly Line uLine;
			private readonly Line vLine;
			private TSplineSurface surface;
			public Vector4 Position => point.Position;

			public Vector4 UDistances { get; private set; }
			public Vector4 VDistances { get; private set; }


			public float U => u;
			public float V => v;

			public PointWrapper(IPoint<Vector4> point, float u, float v, Line uLine, Line vLine, TSplineSurface surface)
			{
				this.u = u;
				this.v = v;
				this.uLine = uLine;
				this.vLine = vLine;
				this.point = point;
				this.surface = surface;
			}

			public void RecalculateKnots()
			{
				UDistances = surface.GetPointKnots(this, true);
				VDistances = surface.GetPointKnots(this, false);
			}

		}

		private class Line
		{
			private readonly SortedSet<PointWrapper> points;

			public float From { get; }

			public float To { get; }

			public float Value { get; }

			public Line(float value, float from, float to, IComparer<PointWrapper> comparer)
			{
				this.points = new SortedSet<PointWrapper>(comparer);
				this.From = from;
				this.To = to;
				this.Value = value;
			}

			public void AddPoint(PointWrapper point)
			{
				points.Add(point);
			}

		}



		private IPoint<Vector4>[,] points;

		private List<PointWrapper> tsplinePoints;
		private List<Line> uLines;
		private List<Line> vLines;

		private MainDataContext sceneData;

		private readonly Vector4RectangesPolygonMesh mesh;
		private SurfaceMesh surfaceMesh;

		private readonly int patchesX;
		private readonly int patchesY;

		private readonly Shader surfaceShader;
		private readonly Shader polygonShader;

		private bool divChanged;

		public class FloatWrapper : BindableObject
		{
			private float val;
			public float Value
			{
				get => val;
				set
				{
					val = value;
					OnPropertyChanged();
				}
			}

			internal FloatWrapper(float value)
			{
				Value = value;
			}
		}

		private Vector4 GetPointKnots(PointWrapper point, bool uKnots)
		{
			FillSurface = false;
			var l = uKnots ? uLines : vLines;
			var pos = uKnots ? point.U : point.V;
			var second = uKnots ? point.V : point.U;
			Vector4 vec = new Vector4(0);

			var p = l.FirstOrDefault(x => Math.Abs(x.Value - pos) < float.Epsilon);
			if (p == null)
				throw new Exception("Bad u value");
			var pIndex = l.IndexOf(p);
			var tmp = pIndex;
			do
			{
				tmp--;

			} while (tmp >= 0 && (l[tmp].From > second || l[tmp].To < second));
			vec.Y = tmp < 0 ? -0.001f : l[tmp].Value;
			tmp--;
			while (tmp >= 0 && (l[tmp].From > second || l[tmp].To < second))
				tmp--;
			vec.X = tmp < 0 ? -0.001f * Math.Abs(tmp) : l[tmp].Value;

			tmp = pIndex + 1;
			while (tmp < l.Count && (l[tmp].From > second || l[tmp].To < second))
				tmp++;
			vec.Z = tmp >= l.Count ? 1.001f : l[tmp].Value;
			tmp++;
			while (tmp < l.Count && (l[tmp].From > second || l[tmp].To < second))
				tmp++;
			vec.W = tmp >= l.Count ? 1.0f + 0.001f * Math.Abs(tmp - l.Count + 1) : l[tmp].Value;

			return vec;
		}

		protected override void PatchDivChanged()
		{
			base.PatchDivChanged();
			divChanged = true;
		}

		public TSplineSurface(MainDataContext data, Shader surfaceShader, Shader polygonShader, IPoint<Vector4>[,] controlPoints)
		{
			this.sceneData = data;
			this.surfaceShader = surfaceShader;
			this.polygonShader = polygonShader;
			this.points = controlPoints;

			var ptsX = controlPoints.GetLength(0);
			var ptsY = controlPoints.GetLength(1);

			var uDiv = 1.0 / (ptsX - 1);
			var vDiv = 1.0 / (ptsY - 1);
			uLines = new List<Line>(ptsX);
			vLines = new List<Line>(ptsY);
			tsplinePoints = new List<PointWrapper>(ptsX * ptsY);

			for (int i = 0; i < ptsX; i++)
			{
				uLines.Add(new Line((float)(i * uDiv), 0, 1, new VComparer()));
			}
			for (int i = 0; i < ptsY; i++)
			{
				vLines.Add(new Line((float)(i * vDiv), 0, 1, new UComparer()));
			}

			for (int i = 0; i < ptsX; i++)
				for (int j = 0; j < ptsY; j++)
				{
					var uline = uLines[i];
					var vline = vLines[j];
					var pt = new PointWrapper(controlPoints[i, j], (float)(i * uDiv), (float)(j * vDiv), uline, vline, this);
					uline.AddPoint(pt);
					vline.AddPoint(pt);
					tsplinePoints.Add(pt);
				}

			var one = uLines[0];
			var two = uLines[1];
			var v = (vLines[0].Value + vLines[1].Value) / 2;
			
			var pt1 = data.CreateRationalPoint();
			var pt2 = data.CreateRationalPoint();

			var edgu = new Line(v, one.Value, two.Value, new UComparer());

			var pt21 = new PointWrapper(pt1, one.Value, v, one, edgu, this);
			var pt22 = new PointWrapper(pt2, two.Value, v, two, edgu, this);

			tsplinePoints.Add(pt21);
			one.AddPoint(pt21);
			two.AddPoint(pt22);
			edgu.AddPoint(pt21);
			edgu.AddPoint(pt22);
			vLines.Add(edgu);
			tsplinePoints.Add(pt22);

			RecalculateKnots();

			mesh = new Vector4RectangesPolygonMesh(points);
			surfaceMesh = new SurfaceMesh((uint)PatchDivX, (uint)PatchDivY);
		}

		public void InsertPoint(float u, float v)
		{

		}

		private void RecalculateKnots()
		{
			uLines.Sort((x, y) => x.Value.CompareTo(y.Value));
			vLines.Sort((x, y) => x.Value.CompareTo(y.Value));

			foreach (var tsplinePoint in tsplinePoints)
			{
				tsplinePoint.RecalculateKnots();
			}
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

			for (var i = 0; i < tsplinePoints.Count; i++)
			{
				var tsplinePoint = tsplinePoints[i];
				var loc = $"functions[{i}].";
				var controlPoint = surfaceShader.GetUniformLocation(loc + "controlPoint");
				var uStart = surfaceShader.GetUniformLocation(loc + "uStart");
				var uDistances = surfaceShader.GetUniformLocation(loc + "uDistances");
				var vStart = surfaceShader.GetUniformLocation(loc + "vStart");
				var vDistances = surfaceShader.GetUniformLocation(loc + "vDistances");

				var vDistancesVal = tsplinePoint.VDistances;
				var vStartVal = vDistancesVal.X;
				vDistancesVal.X = vDistancesVal.Y;
				vDistancesVal.Y = tsplinePoint.V;

				var uDistancesVal = tsplinePoint.UDistances;
				var uStartVal = uDistancesVal.X;
				uDistancesVal.X = uDistancesVal.Y;
				uDistancesVal.Y = tsplinePoint.U;

				surfaceShader.Bind(uStart, uStartVal);
				surfaceShader.Bind(vStart, vStartVal);
				surfaceShader.Bind(uDistances, uDistancesVal);
				surfaceShader.Bind(vDistances, vDistancesVal);
				surfaceShader.Bind(controlPoint, tsplinePoint.Position);
			}
			var ptsCount = surfaceShader.GetUniformLocation("usedPoints");
			surfaceShader.Bind(ptsCount, tsplinePoints.Count);

			var size = surfaceShader.GetUniformLocation("size");
			surfaceShader.Bind(size, new Vector2(1, 1));
			surfaceMesh.Render();
		}
	}
}