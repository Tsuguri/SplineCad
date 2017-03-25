using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using System.Windows.Forms.Integration;
using System.Windows.Forms;
using SplineCAD.Rendering;

namespace SplineCAD
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		GLControl renderingSurface;
		RenderingContext context;
		public MainWindow()
		{
			OpenTK.Toolkit.Init();

			InitializeComponent();
		}

		private void SurfaceInitialized(object sender, EventArgs e)
		{
			var flags = GraphicsContextFlags.Default;

			renderingSurface = new GLControl(new GraphicsMode(32, 24), 2, 0, flags);
			context= new RenderingContext(renderingSurface);

			renderingSurface.MakeCurrent();
			renderingSurface.Paint += context.Render; ;
			renderingSurface.Dock = DockStyle.Fill;
			(sender as WindowsFormsHost).Child = renderingSurface;
		}


	}
}
