using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SplineCAD.Rendering;
using System.Collections;

namespace SplineCAD.Objects
{
	public class PointCollection : IEnumerable
	{
		#region Types

		private class Point : IPoint
		{

			private int ID { get; set; }

			private Vector3 position;

			public Vector3 Position
			{
				get => position;
				set
				{
					if(position==value)
						return;
					position = value;
					OnChanged?.Invoke(this);
				}
			}

			public event ChangedHandler OnChanged;
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
			var indices = new uint[] { 0 };
			var vertices = new[] { new PositionVertex(0, 0, 0) };
			mesh = new ExplicitMesh(vertices, indices, BeginMode.Points);
			GL.PointSize(6);
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

		public void Render(Shader shader)
		{
			var positionLoc = shader.GetUniformLocation("pointPosition");

			foreach (var point in points)
			{
				shader.Bind(positionLoc,point.Position);
				mesh.Render();
			}

		}

        public IEnumerator GetEnumerator()
        {
            return points.GetEnumerator();
        }
    }
}
