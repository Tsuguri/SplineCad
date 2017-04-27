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
using SplineCAD.Objects;

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
        private IPoint caughtPoint;

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
            renderingSurface.MouseUp += RenderingSurface_MouseUp;
            renderingSurface.MouseDown += RenderingSurface_MouseDown;
            renderingSurface.Disposed += RenderingSurfaceOnDisposed;
			renderingSurface.Dock = DockStyle.Fill;
			ImageHost.Child = renderingSurface;
			//	throw new Exception("Application is broken, plz repair.");

			CompositionTarget.Rendering += CompositionTargetOnRendering;
			MainWindowDataContext.InitializeDataContext();

		}

        private void RenderingSurface_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            caughtPoint = null;
        }

        private void RenderingSurface_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                foreach(IPoint p in MainWindowDataContext.Points) //projecting points and calculating distance
                {
                    Vector4 v = new Vector4(p.Position, 1.0f);
                    v = MainWindowDataContext.MainCamera.ProjectionMatrix * MainWindowDataContext.MainCamera.ViewMatrix * v;
                    v = Vector4.Divide(v, v.W);
                    System.Drawing.Point scenePoint = new System.Drawing.Point(
                        (int)((v.X + 1.0f) * renderingSurface.Width / 2.0f),
                        (int)(renderingSurface.Height - (v.Y + 1.0f) * renderingSurface.Height / 2.0f));
                    double dx = scenePoint.X - e.X;
                    double dy = scenePoint.Y - e.Y;
                    if(Math.Sqrt((dx * dx + dy * dy)) < 5 /*tolerance*/)
                    {
                        caughtPoint = p;
                        return;
                    }

                }
            }
        }

        private void RenderingSurface_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            System.Drawing.Point p = e.Location;

            if (e.Button == MouseButtons.Right)
                MainWindowDataContext.MainCamera.Zoom(0.02f * (p.Y - mousePos.Y));
            if (e.Button == MouseButtons.Left)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) && caughtPoint != null) //point movement
                {
                    Vector3 camPos = MainWindowDataContext.MainCamera.Position;
                    //double dx = caughtPoint.Position.X - camPos.X;
                    //double dy = caughtPoint.Position.Y - camPos.Y;
                    //double dz = caughtPoint.Position.Z - camPos.Z;
                    //float distance = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
                    caughtPoint.Position += 0.02f * ((mousePos.Y - p.Y) * MainWindowDataContext.MainCamera.Up +
                                            (p.X - mousePos.X) * MainWindowDataContext.MainCamera.Right);
                }                    
                else
                    MainWindowDataContext.MainCamera.Rotate(mousePos.X - p.X, p.Y - mousePos.Y);
            }
                
                
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
