using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
namespace SplineCAD.Rendering
{
	public class RenderingContext
	{
		GLControl renderingSurface;
		public RenderingContext(GLControl surface)
		{
			renderingSurface = surface;
		}

		public void Render(object sender, PaintEventArgs e)
		{
			GL.ClearColor(Color4.Azure);
			GL.Clear(
				ClearBufferMask.ColorBufferBit |
				ClearBufferMask.DepthBufferBit |
				ClearBufferMask.StencilBufferBit);

			renderingSurface.SwapBuffers();
		}
	}
}
