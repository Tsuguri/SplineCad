using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SplineCAD.Objects;

namespace SplineCAD.Rendering
{
	class Vector3SelfActualizingMesh : Mesh<PositionVertex>
	{
		private readonly List<IPoint<Vector3>> points;
		private readonly uint[] indices;
		private readonly BeginMode mode;

		private bool changed = false;

		public Vector3SelfActualizingMesh(List<IPoint<Vector3>> points, uint[] indices, BeginMode mode)
		{
			this.points = points;
			this.indices = indices;
			this.mode = mode;
			var p = points.Select(x => new PositionVertex(x.Position)).ToArray();
			points.ForEach(x => x.OnChanged += OnIPointChanged);
			Initialize(p, indices, mode);
			changed = true;

		}

		private void OnIPointChanged(IPoint<Vector3> point)
		{
			changed = true;
		}

		public override void Render()
		{
			if (changed)
			{
				changed = false;
				Dispose();
				var p = points.Select(x => new PositionVertex(x.Position)).ToArray();
				points.ForEach(x => x.OnChanged += OnIPointChanged);
				Initialize(p, indices, mode);
			}
			base.Render();
		}
	}

	class Vector4SelfActualizingMesh : Mesh<RationalVertex>
	{
		private readonly List<IPoint<Vector4>> points;
		private readonly uint[] indices;
		private readonly BeginMode mode;

		private bool changed = false;

		public Vector4SelfActualizingMesh(List<IPoint<Vector4>> points, uint[] indices, BeginMode mode)
		{
			this.points = points;
			this.indices = indices;
			this.mode = mode;
			var p = points.Select(x => new RationalVertex(x.Position)).ToArray();
			points.ForEach(x => x.OnChanged += OnIPointChanged);
			Initialize(p, indices, mode);
			changed = true;

		}

		private void OnIPointChanged(IPoint<Vector4> point)
		{
			changed = true;
		}

		public override void Render()
		{
			if (changed)
			{
				changed = false;
				Dispose();
				var p = points.Select(x => new RationalVertex(x.Position)).ToArray();
				points.ForEach(x => x.OnChanged += OnIPointChanged);
				Initialize(p, indices, mode);
			}
			base.Render();
		}
	}
}
