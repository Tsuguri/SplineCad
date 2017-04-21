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
			renderingSurface.Disposed+= RenderingSurfaceOnDisposed;
			renderingSurface.Dock = DockStyle.Fill;
			imageHost.Child = renderingSurface;
			//	throw new Exception("Application is broken, plz repair.");

			CompositionTarget.Rendering += CompositionTargetOnRendering;
			MainWindowDataContext.InitializeDataContext();

		}

		private void RenderingSurfaceOnDisposed(object sender, EventArgs eventArgs)
		{
			MainWindowDataContext.OnDispose();
		}
	}
}
