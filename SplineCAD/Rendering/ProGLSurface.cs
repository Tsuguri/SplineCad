using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SplineCAD.Rendering
{
	class ProGLSurface : GLSurface
	{

		public ProGLSurface() : base(new GraphicsMode(ColorFormat.Empty, 24, 0, 1), 3, 3, GraphicsContextFlags.Default)
		{
		}

		protected override void ProcessInput(float dt)
		{
			
		}

		protected override void Render(float dt)
		{

			//GL.ClearColor(Color4.Azure);
			GL.ClearColor(0.1f, 0.2f, 0.3f, 1.0f);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			GL.Viewport(this.Size);
		}
	}
}
