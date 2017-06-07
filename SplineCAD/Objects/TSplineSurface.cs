using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SplineCAD.Data;
using SplineCAD.Rendering;
using SplineCAD.Utilities;
using System.Windows.Media;

namespace SplineCAD.Objects
{
	class TSplineSurface : Surface
	{
		#region Types

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
			private TSplineSurface surface;
			public Vector4 Position => point.Position;
			public IPoint<Vector4> Point => point;

			public Vector4 UDistances { get; private set; }
			public Vector4 VDistances { get; private set; }


			public float U => u;
			public float V => v;

			public PointWrapper(IPoint<Vector4> point, float u, float v, TSplineSurface surface)
			{
				this.u = u;
				this.v = v;
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

			public float From { get; set; }

			public float To { get; set; }

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

			public IEnumerable<PointWrapper> GetPoints()
			{
				return points;
			}

		}

		#endregion


		private static int count = 0;

		private List<PointWrapper> tsplinePoints;

		/// <summary>
		/// Lines with constant u value;
		/// </summary>
		private List<Line> uLines;

		/// <summary>
		/// Lines with constant v value;
		/// </summary>
		private List<Line> vLines;

		private MainDataContext sceneData;

		private Vector4SelfActualizingMesh selfMesh;
		private SurfaceMesh surfaceMesh;

		private readonly int patchesX;
		private readonly int patchesY;

		private readonly Shader surfaceShader;
		private readonly Shader polygonShader;

		private bool divChanged;
		private bool pointsChanged;

		#region EdgesInsertionUI

		private bool u;

		public bool U
		{
			get => u;
			set
			{
				u = value;
				OnPropertyChanged();
			}
		}

		private float valFrom, valTo, val;

		public float ValFrom
		{
			get => valFrom;
			set
			{
				if (value < ValTo)
				{
					valFrom = value;
					OnPropertyChanged();
				}
			}
		}

		public float ValTo
		{
			get => valTo;
			set
			{
				if (value > ValFrom)
				{
					valTo = value;
					OnPropertyChanged();
				}
			}
		}

		public float Val
		{
			get => val;
			set
			{
				if (value > 0 && value < 1)
					val = value;
				OnPropertyChanged();
			}
		}

		private ICommand addEdge;
		public ICommand AddEdge => addEdge ?? (addEdge = new CommandHandler(AddEdgeUi));

		private void AddEdgeUi()
		{
			InsertEdge(ValFrom, ValTo, Val, U);
		}

		#endregion



		protected override void PatchDivChanged()
		{
			base.PatchDivChanged();
			divChanged = true;
		}

		public TSplineSurface(MainDataContext data, Shader surfaceShader, Shader polygonShader, IPoint<Vector4>[,] controlPoints, Color clr)
		{
			this.sceneData = data;
			this.surfaceShader = surfaceShader;
			this.polygonShader = polygonShader;

			this.Name = "T-Spline " + (++count).ToString();
			SurfaceColor = clr;

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
					var pt = new PointWrapper(controlPoints[i, j], (float)(i * uDiv), (float)(j * vDiv), this);
					uline.AddPoint(pt);
					vline.AddPoint(pt);
					tsplinePoints.Add(pt);
				}
			pointsChanged = true;

			surfaceMesh = new SurfaceMesh((uint)PatchDivX, (uint)PatchDivY);
		}

		private void CreatePolygonMesh()
		{
			var vertices = tsplinePoints.Select(x => x.Point).ToList();
			var indices = new List<uint>();
			Dictionary<PointWrapper, uint> indexMap = new Dictionary<PointWrapper, uint>();

			uint i = 0;
			foreach (var tsplinePoint in tsplinePoints)
			{
				indexMap.Add(tsplinePoint, i);
				i++;
			}

			foreach (var uLine in uLines.Concat(vLines))
			{
				var first = uLine.GetPoints().First();
				foreach (var pointWrapper in uLine.GetPoints().Skip(1))
				{
					indices.Add(indexMap[first]);
					indices.Add(indexMap[pointWrapper]);
					first = pointWrapper;
				}
			}
			selfMesh?.Dispose();
			selfMesh = new Vector4SelfActualizingMesh(vertices, indices.ToArray(), BeginMode.Lines);

		}

		private const double Eps = 1e-10;

		private Vector4 GetPointKnots(PointWrapper point, bool uKnots)
		{
			var l = uKnots ? uLines : vLines;
			var pos = uKnots ? point.U : point.V;
			var second = uKnots ? point.V : point.U;
			Vector4 vec = new Vector4(0);

			var p = l.FirstOrDefault(x => Math.Abs(x.Value - pos) < float.Epsilon && x.From - Eps < second && x.To + Eps > second);
			if (p == null)
				throw new Exception("Bad value value");
			var pIndex = l.IndexOf(p);
			var tmp = pIndex;
			do
			{
				tmp--;

			} while (tmp >= 0 && (l[tmp].From - Eps > second || l[tmp].To + Eps < second));
			vec.Y = tmp < 0 ? -0.001f : l[tmp].Value;
			tmp--;
			while (tmp >= 0 && (l[tmp].From - Eps > second || l[tmp].To + Eps < second))
				tmp--;
			vec.X = tmp < 0 ? -0.001f * Math.Abs(tmp) : l[tmp].Value;

			tmp = pIndex + 1;
			while (tmp < l.Count && (l[tmp].From - Eps > second || l[tmp].To + Eps < second))
				tmp++;
			vec.Z = tmp >= l.Count ? 1.001f : l[tmp].Value;
			tmp++;
			while (tmp < l.Count && (l[tmp].From - Eps > second || l[tmp].To + Eps < second))
				tmp++;
			vec.W = tmp >= l.Count ? 1.0f + 0.001f * Math.Abs(tmp - l.Count + 1) : l[tmp].Value;

			return vec;
		}


		public void InsertEdge(float from, float to, float value, bool u)
		{
			var fLines = u ? uLines : vLines;
			fLines.Sort((x, y) => x.Value.CompareTo(y.Value));
			var sLines = u ? vLines : uLines;
			var sameVal = fLines.Where(x => Math.Abs(x.Value - value) < Eps).ToList();

			var line = NewEdge(from, to, value, u);

			if (sameVal.Count > 0)
			{
				var overlapping = sameVal.FirstOrDefault(x => x.From < from && x.To > to);
				if (overlapping != null)
					return;

				var pre = sameVal.FirstOrDefault(x => Math.Abs(x.To - from) < Eps);


				var post = sameVal.FirstOrDefault(x => Math.Abs(x.From - to) < Eps);

				if (pre != null)
				{
					line = MergeEdges(pre, line, u);
				}
				else
				{
					var left = sameVal.Where(x => x.To < from).ToList();
					if (left.Count > 0)
					{
						var lefter = left.Max(x => x.To);
						var edge = left.FirstOrDefault(x => Math.Abs(x.To - lefter) < Eps);
						if (edge != null)
						{
							var middle = sLines
								.Count(x => x.Value > edge.To && x.Value < line.From && x.From > line.Value &&
														   x.To < line.Value);
							if (middle == 0)
							{
								line = MergeEdges(edge, line, u, true);
							}
						}

					}
				}
				if (post != null)
				{
					line = MergeEdges(line, post, u);
				}
				else
				{
					var right = sameVal.Where(x => x.From > to).ToList();
					if (right.Count > 0)
					{
						var righter = right.Min(x => x.From);
						var edge = right.FirstOrDefault(x => Math.Abs(x.From - righter) < Eps);
						if (edge != null)
						{
							var middle = sLines
								.Count(x => x.Value < edge.From && x.Value > line.To && x.From > line.Value &&
											x.To < line.Value);
							if (middle == 0)
							{
								line = MergeEdges(line, edge, u, true);
							}
						}

					}
				}

			}

			var prev = sLines.Where(x => x.Value < line.From - Eps).ToList();
			var prevCount = prev.Count;
			var inters = sLines.Skip(prevCount).Where(x => x.Value < line.To + Eps && line.Value < x.To + Eps && line.Value > x.From - Eps).OrderBy(x => x.Value).ToList();

			if (Math.Abs(inters.First().Value - line.From) > Eps)
				line.From = inters.First().Value;
			var last = inters.Last();
			if (Math.Abs(last.Value - line.To) > Eps)
				line.To = last.Value;

			foreach (var inter in inters)
			{
				if (line.GetPoints().FirstOrDefault(x => Math.Abs(x.U - value) < Eps && Math.Abs(x.V - inter.Value) < Eps) == null)
				{
					var pt = sceneData.CreateRationalPoint();
					var wrap = new PointWrapper(pt, u ? value : inter.Value, u ? inter.Value : value, this);
					PointWrapper previous, after;
					float t = 0;
					if (u)
					{
						after = inter.GetPoints().FirstOrDefault(x => x.U > value);
						previous = inter.GetPoints().Reverse().FirstOrDefault(x => x.U < value);
						t = (value - previous.U) / (after.U - previous.U);
					}
					else
					{
						after = inter.GetPoints().FirstOrDefault(x => x.V > value);
						previous = inter.GetPoints().Reverse().FirstOrDefault(x => x.V < value);
						t = (value - previous.V) / (after.V - previous.V);

					}
					pt.Position = after.Position * t + (1 - t) * previous.Position;

					line.AddPoint(wrap);
					inter.AddPoint(wrap);
					tsplinePoints.Add(wrap);
				}
			}


			pointsChanged = true;
			int z = 0;


		}

		private Line NewEdge(float from, float to, float val, bool u)
		{
			return u ? NewUEdge(from, to, val) : NewVEdge(from, to, val);
		}

		private Line MergeEdges(Line one, Line two, bool u, bool hard = false)
		{
			if (Math.Abs(one.Value - two.Value) > Eps)
				throw new ArgumentException("Lines are not on the same value!");

			var pre = one.From < two.From ? one : two;
			var post = one.From < two.From ? two : one;

			if (!hard && Math.Abs(pre.To - post.From) > Eps)
				throw new ArgumentException("Lines ends does not meet!");

			var line = NewEdge(pre.From, post.To, pre.Value, u);

			foreach (var pointWrapper in pre.GetPoints().Concat(post.GetPoints()))
			{
				line.AddPoint(pointWrapper);
			}
			var list = u ? uLines : vLines;
			list.Remove(one);
			list.Remove(two);
			return line;
		}

		private Line NewUEdge(float from, float to, float val)
		{
			var line = new Line(val, from, to, new VComparer());
			uLines.Add(line);
			return line;
		}

		private Line NewVEdge(float from, float to, float val)
		{
			var line = new Line(val, from, to, new UComparer());
			vLines.Add(line);
			return line;
		}

		private int LineComparator(Line one, Line two)
		{
			var val = one.Value.CompareTo(two.Value);
			if (val == 0)
				return one.From.CompareTo(two.From);
			return val;
		}

		private void RecalculateKnots()
		{
			uLines.Sort(LineComparator);
			vLines.Sort(LineComparator);

			foreach (var tsplinePoint in tsplinePoints)
			{
				tsplinePoint.RecalculateKnots();
			}
		}



		public override void CleanUp()
		{
			foreach (var p in tsplinePoints)
			{
				sceneData.Vector4Points.RemovePoint(p.Point);
			}

			selfMesh.Dispose();
			surfaceMesh.Dispose();
		}

		public override void Render()
		{
			if (pointsChanged)
			{
				RecalculateKnots();
				CreatePolygonMesh();
				pointsChanged = false;
			}

			if (divChanged)
			{
				divChanged = false;
				surfaceMesh?.Dispose();
				surfaceMesh = new SurfaceMesh((uint)PatchDivX, (uint)PatchDivY);
			}
			if (PolygonVisible)
			{
				polygonShader.Activate();
				selfMesh.Render();
			}
			GL.PolygonMode(MaterialFace.Front, FillSurface ? PolygonMode.Fill : PolygonMode.Line);
			GL.PolygonMode(MaterialFace.Back, FillSurface ? PolygonMode.Fill : PolygonMode.Line);

			surfaceShader.Activate();

			var camPos = surfaceShader.GetUniformLocation("camPos");
			var lightPos = surfaceShader.GetUniformLocation("lightPos");
			var surfColor = surfaceShader.GetUniformLocation("surfColor");

			surfaceShader.Bind(lightPos, sceneData.LightPos);
			surfaceShader.Bind(camPos, sceneData.MainCamera.Position);
			surfaceShader.Bind(surfColor, (new Vector3(SurfaceColor.R, SurfaceColor.G, SurfaceColor.B)).Normalized());

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

		public NurbsSurface ConvertToNurbs(Shader surfaceShader, Shader polygonShader)
		{
			var vDivs = vLines.Select(x => x.Value).Distinct().ToList();
			var uDivs = uLines.Select(x => x.Value).Distinct().ToList();

			var vFilled = new List<int>();
			foreach (Line t in vLines)
			{
				if (Math.Abs(t.From) < Eps && Math.Abs(t.To - 1) < Eps)
				{
					vFilled.Add(vDivs.IndexOf(t.Value));
				}
			}

			IPoint<Vector4>[,] pts = new IPoint<Vector4>[uDivs.Count, vDivs.Count];

			foreach (var tsplinePoint in tsplinePoints)
			{
				var pt = sceneData.CreateRationalPoint(tsplinePoint.Position);
				var i = uDivs.IndexOf(tsplinePoint.U);
				var j = vDivs.IndexOf(tsplinePoint.V);
				pts[i, j] = pt;
			}
			foreach (var vLine in vFilled)
			{
				int lastFilled = 0;
				int nextFilled = 0;
				for (int i = 0; i < uDivs.Count; i++)
				{
					if (pts[i, vLine] == null)
					{
						if (nextFilled < i)
						{
							nextFilled = i;
							do
							{
								nextFilled++;
							} while (pts[nextFilled, vLine] == null);
						}

						var prev = pts[lastFilled, vLine];
						var next = pts[nextFilled, vLine];
						var prevVal = uDivs[lastFilled];
						var nextVal = uDivs[nextFilled];
						var val = uDivs[i];
						var t = (val - prevVal) / (nextVal - prevVal);
						var pos = prev.Position * (1 - t) + next.Position * t;
						pts[i, vLine] = sceneData.CreateRationalPoint(pos);
					}
					else
					{
						lastFilled = i;
					}
				}
			}

			for (int i = 0; i < uDivs.Count; i++)
			{
				int lastFilled = 0;
				int nextFilled = 0;
				for (int j = 0; j < vDivs.Count; j++)
				{
					if (pts[i, j] == null)
					{
						if (nextFilled < j)
						{
							nextFilled = j;
							do
							{
								nextFilled++;
							} while (pts[i, nextFilled] == null);
						}

						var prev = pts[i, lastFilled];
						var next = pts[i, nextFilled];
						var prevVal = vDivs[lastFilled];
						var nextVal = vDivs[nextFilled];
						float val = vDivs[j];
						var t = (val - prevVal) / (nextVal - prevVal);
						var pos = prev.Position * (1 - t) + next.Position * t;
						pts[i, j] = sceneData.CreateRationalPoint(pos);
					}
					else
					{
						lastFilled = j;
					}
				}
			}

			var p = new NurbsSurface(sceneData, surfaceShader, polygonShader, pts, SurfaceColor, uDivs, vDivs);
			sceneData.AddModel(p);
			return p;
		}
	}
}