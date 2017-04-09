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
using ShaderType = OpenTK.Graphics.OpenGL4.ShaderType;

namespace SplineCAD.Data
{
	public class MainDataContext : BindableObject
	{

		#region Types

		#endregion

		#region Fields

		private ICommand testButtonCommand;

		private Dictionary<string, Shader> Shaders { get; set; }

		#endregion

		#region Properties

		private bool changed = false;
		public ICommand TestButtonCommand => testButtonCommand ?? (testButtonCommand = new CommandHandler(TestButtonAction));

		private void TestButtonAction()
		{
			changed = !changed;
		}

		#endregion

		#region Initialization

		public MainDataContext()
		{
		}

		public void InitializeDataContext()
		{
			InitializeShaders();
			InitializeMeshes();
			InitializeTextures();
		}

		private void InitializeTextures()
		{
			// nothing here yet;
		}

		private void InitializeShaders()
		{
			Shaders = new Dictionary<string, Shader>
			{
				//here add shaders
				//eg:
				{"testShader", Shader.CreateShader("Shaders\\test.vert", "Shaders\\test.frag")}
			};
		}

		private void InitializeMeshes()
		{
			//nothing here yet;
		}

		#endregion

		#region Rendering

		public void Render()
		{
			if (changed)
				GL.ClearColor(Color4.Beige);
		}

		#endregion

		#region Dispose

		public void OnDispose()
		{
			foreach (var shader in Shaders)
			{
				shader.Value.Dispose();
			}
		}

		#endregion
	}
}
