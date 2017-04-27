using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using SplineCAD.Objects;

namespace SplineCAD.Rendering
{
	class SelfActualizingMesh : Mesh
	{
		private readonly List<IPoint> points;
		private readonly uint[] indices;
		private readonly BeginMode mode;

		private bool changed = false;

		public SelfActualizingMesh(List<IPoint> points, uint[] indices, BeginMode mode)
		{
			this.points = points;
			this.indices = indices;
			this.mode = mode;
			var p = points.Select(x => new PositionVertex(x.Position)).ToArray();
			points.ForEach(x => x.OnChanged += OnIPointChanged);
			Initialize(p, indices, mode);
			changed = true;

		}

		private void OnIPointChanged(IPoint point)
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
}
