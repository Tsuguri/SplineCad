using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SplineCAD.Objects;
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

		private Dictionary<string, Mesh> Meshes { get; set; }

		private List<int> sceneObjects;

		private PointCollection points;

		private Camera camera;

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


			var pt1 = points.CreatePoint();
			pt1.Position = new Vector3(0.5f, 0.5f, 1);
			var pt2 = points.CreatePoint();
			pt2.Position = new Vector3(-0.5f, -0.5f, 1);

			var pt3 = points.CreatePoint();
			pt3.Position = new Vector3(0.5f, -0.5f, 1);

			var pt4 = points.CreatePoint();
			pt4.Position = new Vector3(-0.5f, 0.5f, 1);

			camera = new Camera();

		}

		private void InitializeShaders()
		{
			Shaders = new Dictionary<string, Shader>
			{
				//here add shaders
				//eg:
				{"testShader", Shader.CreateShader("Shaders\\test.vert", "Shaders\\test.frag")},
				{"pointShader", Shader.CreateShader("Shaders\\pointShader.vert","Shaders\\pointShader.frag") }
			};
		}

		private void InitializeMeshes()
		{
			Meshes = new Dictionary<string, Mesh>
			{
				{"cubeMesh", new Cube() }
			};
			points = new PointCollection();
		}

		#endregion

		#region Rendering

		public void Render()
		{
			if (changed)
			{
				GL.ClearColor(Color4.Beige);
				GL.Clear(ClearBufferMask.ColorBufferBit);
			}

			var shader = Shaders["testShader"];
			var mesh = Meshes["cubeMesh"];
			var ptShader = Shaders["pointShader"];

			shader.Activate();
			mesh.Render();

			ptShader.Activate();
			points.Render(ptShader);


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
