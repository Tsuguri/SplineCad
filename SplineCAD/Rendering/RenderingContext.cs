using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SplineCAD.Data;


namespace SplineCAD.Rendering
{
	public class RenderingContext
	{
		GLControl renderingSurface;

		public MainDataContext MainData { get; set; }
		public RenderingContext(GLControl surface)
		{
			renderingSurface = surface;
		}

		public void Render()
		{
			GL.Viewport(0,0,renderingSurface.Width,renderingSurface.Height);
			GL.ClearColor(Color4.Azure);


			GL.Clear(
				ClearBufferMask.ColorBufferBit |
				ClearBufferMask.DepthBufferBit |
				ClearBufferMask.StencilBufferBit);
			MainData?.Render();



			renderingSurface.SwapBuffers();
		}
	}
}
