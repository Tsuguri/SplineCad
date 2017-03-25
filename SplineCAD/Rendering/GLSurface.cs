using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplineCAD.Rendering
{
	public abstract partial class GLSurface : GLControl
	{
		public GLSurface(GraphicsMode mode, int major, int minor, GraphicsContextFlags flags)
			: base(mode, major, minor, flags)
		{
			InitializeComponent();

			this.Disposed += GLSurface_Disposed;
		}

		private void GLSurface_Disposed(object sender, EventArgs e)
		{
			OnDisposed(e);
		}

		protected virtual void OnDisposed(EventArgs e)
		{

		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			GL.Viewport(0, 0, this.Size.Width, this.Size.Height);
		}

		public void UpdateFrameData(float dt)
		{
			ProcessInput(dt);

			Render(dt);
		}

		protected abstract void Render(float dt);

		protected abstract void ProcessInput(float dt);


		#region Component Designer generated code

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}


		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		}

		#endregion

	}
}
