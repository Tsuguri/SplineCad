using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SplineCAD.Rendering;
using SplineCAD.Utilities;

namespace SplineCAD.Data
{
	public class MainDataContext : BindableObject
	{

		#region Types

		#endregion

		#region Fields

		private ICommand testButtonCommand;

		#endregion

		#region Properties

		private bool changed = false;
		public ICommand TestButtonCommand => testButtonCommand ?? (testButtonCommand = new CommandHandler(TestButtonAction));

		private void TestButtonAction()
		{
			changed = !changed;
		}

		#endregion

		#region Rendering

		public void Render()
		{
			if (changed)
				GL.ClearColor(Color4.Beige);
		}

		#endregion
	}
}
