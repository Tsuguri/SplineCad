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

using SplineCAD.Rendering;
using System.Diagnostics;

namespace SplineCAD
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		GLSurface renderingSurface;
		Stopwatch timer;

		public MainWindow()
		{
			OpenTK.Toolkit.Init();
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			renderingSurface = new ProGLSurface();
			imageHost.Child = renderingSurface;
			timer = new Stopwatch();
			CompositionTarget.Rendering += RenderGLSurface;
		}

		private void RenderGLSurface(object sender, EventArgs e)
		{
			timer.Stop();

			float timeElapsed = (float)timer.Elapsed.TotalSeconds;
			timer.Reset();
			timer.Start();
			renderingSurface.UpdateFrameData(timeElapsed);
		}
	}
}
