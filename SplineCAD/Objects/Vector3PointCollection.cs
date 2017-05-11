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

	public class PointCollection<T> : IEnumerable<IPoint<T>> where T : struct
	{
		#region Types

		protected class Point<T> : IPoint<T>
		{

			private int ID { get; set; }

			private T position;

			public T Position
			{
				get => position;
				set
				{
					if (position.Equals(value))
						return;
					position = value;
					OnChanged?.Invoke(this);
				}
			}

			public event ChangedHandler<T> OnChanged;
		}

		#endregion

		protected readonly List<Point<T>> points = new List<Point<T>>();

		public IPoint<T> CreatePoint()
		{
			var p = new Point<T>();
			points.Add(p);
			return p;
		}

		public void RemovePoint(IPoint<T> point)
		{
			var p = point as Point<T>;
			if (p != null)
			{
				points.Remove(p);
			}
			else
				throw new ArgumentException("given point does not originate from this kind of collection");
		}


		public IEnumerator<IPoint<T>> GetEnumerator()
		{
			return points.Cast<IPoint<T>>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public class Vector3PointCollection : PointCollection<Vector3>
	{


		#region Fields
		
		private readonly ExplicitMesh<PositionVertex> mesh;


		#endregion

		#region Constructors

		public Vector3PointCollection()
		{
			var indices = new uint[] { 0 };
			var vertices = new[] { new PositionVertex(0, 0, 0) };
			mesh = new ExplicitMesh<PositionVertex>(vertices, indices, BeginMode.Points);
			GL.PointSize(6);
		}

		#endregion

		public void Render(Shader shader)
		{
			var positionLoc = shader.GetUniformLocation("pointPosition");

			foreach (var point in points)
			{
				shader.Bind(positionLoc,point.Position);
				mesh.Render();
			}
			shader.Bind(positionLoc,new Vector3(0,0,0));

		}
    }

	public class Vector4PointCollection : PointCollection<Vector4>
	{
		#region Fields

		private readonly ExplicitMesh<PositionVertex> mesh;


		#endregion

		#region Constructors

		public Vector4PointCollection()
		{
			var indices = new uint[] { 0 };
			var vertices = new[] { new PositionVertex(0, 0, 0) };
			mesh = new ExplicitMesh<PositionVertex>(vertices, indices, BeginMode.Points);
			GL.PointSize(6);
		}

		#endregion

		public void Render(Shader shader)
		{
			var positionLoc = shader.GetUniformLocation("rationalPointPosition");

			foreach (var point in points)
			{
				shader.Bind(positionLoc, point.Position);
				mesh.Render();
			}
			shader.Bind(positionLoc, new Vector4(0, 0, 0, 1));


		}
	}
}
