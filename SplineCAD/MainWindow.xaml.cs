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
using System.Windows.Threading;
using SplineCAD.Data;
using SplineCAD.Rendering;

namespace SplineCAD
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		internal MainDataContext MainWindowDataContext { get; set; }
		GLControl renderingSurface;
		RenderingContext context;
		private DispatcherTimer timer;
        private System.Drawing.Point mousePos;

		public MainWindow()
		{
			OpenTK.Toolkit.Init();
			MainWindowDataContext = new MainDataContext();
			DataContext = MainWindowDataContext;
            
			InitializeComponent();
		}

		//private void SurfaceInitialized(object sender, EventArgs e)
		//{
			
		//}

		private void CompositionTargetOnRendering(object sender, EventArgs eventArgs)
		{
			context.Render();
		}

		private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			var flags = GraphicsContextFlags.Default;

			renderingSurface = new GLControl(new GraphicsMode(32, 24), 2, 0, flags);
			context = new RenderingContext(renderingSurface);
			context.MainData = MainWindowDataContext;
			renderingSurface.MakeCurrent();
            renderingSurface.Resize += RenderingSurfaceResized;
            renderingSurface.MouseMove += RenderingSurface_MouseMove;
			renderingSurface.Disposed += RenderingSurfaceOnDisposed;
			renderingSurface.Dock = DockStyle.Fill;
			imageHost.Child = renderingSurface;
			//	throw new Exception("Application is broken, plz repair.");

			CompositionTarget.Rendering += CompositionTargetOnRendering;
			MainWindowDataContext.InitializeDataContext();

		}

        private void RenderingSurface_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            System.Drawing.Point p = e.Location;

            if (e.Button == MouseButtons.Right)
                MainWindowDataContext.MainCamera.Zoom(0.01f * (p.Y - mousePos.Y));
            if (e.Button == MouseButtons.Left)
                MainWindowDataContext.MainCamera.Rotate(mousePos.X - p.X, p.Y - mousePos.Y);
                
            mousePos = p;
        }

        private void RenderingSurfaceResized(object sender, EventArgs e)
        {
            MainWindowDataContext.MainCamera?.CreateProjection(1.0f, 100.0f, 45.0f, renderingSurface.Width /(float) renderingSurface.Height);
        }

        private void RenderingSurfaceOnDisposed(object sender, EventArgs eventArgs)
		{
			MainWindowDataContext.OnDispose();
		}
	}
}
