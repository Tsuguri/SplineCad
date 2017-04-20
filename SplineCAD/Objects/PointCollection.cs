using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using SplineCAD.Rendering;

namespace SplineCAD.Objects
{
	public class PointCollection
	{
		#region Types

		private class Point : IPoint
		{

			private int ID { get; set; }


			public Vector3 Position { get; set; }
		}

		#endregion

		#region Fields

		private readonly List<Point> points = new List<Point>();
		private readonly ExplicitMesh mesh;


		#endregion

		#region Properties

		#endregion

		#region Constructors

		public PointCollection()
		{
			var indices = new uint[] {0};
			var vertices = new[] {new PositionVertex(0,0,0)};
			mesh = new ExplicitMesh(vertices,indices);
		}

		#endregion


		public IPoint CreatePoint()
		{
			var p = new Point();
			points.Add(p);
			return p;
		}

		public void RemovePoint(IPoint point)
		{
			var p = point as Point;
			if (p != null)
			{
				points.Remove(p);
			}
			else
				throw new ArgumentException("given point does not originate from this kind of collection");
		}

		public void Render()
		{


			mesh.Render();
		}
	}
}
