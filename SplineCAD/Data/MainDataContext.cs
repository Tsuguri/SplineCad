using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SplineCAD.Objects;
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
		private ICommand removeSelectedCommand;

		private bool changed;


		private readonly ObservableCollection<Model> sceneObjects = new ObservableCollection<Model>();


		private Model selectedModel;



		private PointCollection points;

		private Camera camera;

		#endregion

		#region Properties

		private Dictionary<string, Shader> Shaders { get; set; }

		private Dictionary<string, Mesh> Meshes { get; set; }

		public ICommand TestButtonCommand => testButtonCommand ?? (testButtonCommand = new CommandHandler(TestButtonAction));

		public ICommand RemoveSelectedCommand => removeSelectedCommand ??
												 (removeSelectedCommand = new CommandHandler(RemoveSelected));

		public ObservableCollection<Model> SceneObjects => sceneObjects;

		public Model SelectedModel
		{
			get => selectedModel;
			set
			{
				selectedModel = value;
				OnPropertyChanged();
			}
		}

		public Camera MainCamera => camera;

		#endregion

		private void TestButtonAction()
		{
			changed = !changed;
		}


		#region Initialization

		public void InitializeDataContext()
		{
			InitializeShaders();
			InitializeMeshes();



			var pt1 = points.CreatePoint();
			pt1.Position = new Vector3(1.0f, 1.0f, 1.0f);

			var pt2 = points.CreatePoint();
			pt2.Position = new Vector3(-1.0f, -1.0f, 1.0f);

			var pt3 = points.CreatePoint();
			pt3.Position = new Vector3(1.0f, -1.0f, 1.0f);

			var pt4 = points.CreatePoint();
			pt4.Position = new Vector3(-1.0f, 1.0f, 1.0f);

			var pt5 = points.CreatePoint();
			pt5.Position = new Vector3(1.0f, 1.0f, -1.0f);

			var pt6 = points.CreatePoint();
			pt6.Position = new Vector3(-1.0f, -1.0f, -1.0f);

			var pt7 = points.CreatePoint();
			pt7.Position = new Vector3(1.0f, -1.0f, -1.0f);

			var pt8 = points.CreatePoint();
			pt8.Position = new Vector3(-1.0f, 1.0f, -1.0f);



			camera = new Camera(new Vector3(0.0f, 0.0f, 5.0f));

			var surface = new Surface(this, Shaders["BsplineShader"], Shaders["LineShader"]);

			sceneObjects.Add(surface);
		}

		private void InitializeShaders()
		{
			Shaders = new Dictionary<string, Shader>
			{
				//here add shaders
				//eg:
				{"testShader", Shader.CreateShader("Shaders\\test.vert", "Shaders\\test.frag")},
				{"pointShader", Shader.CreateShader("Shaders\\pointShader.vert","Shaders\\pointShader.frag") },
				{"LineShader", Shader.CreateShader("Shaders\\LineShader.vert","Shaders\\LineShader.frag") },
				{"BsplineShader",Shader.CreateShader("Shaders\\BsplineSurface.vert","Shaders\\BsplineSurface.frag") }
			};

			void StandardShaderDelegate(Shader shader)
			{
				shader.Bind(shader.GetUniformLocation("viewMatrix"), camera.ViewMatrix);
				shader.Bind(shader.GetUniformLocation("projMatrix"), camera.ProjectionMatrix);
			}

			Shaders["testShader"].OnActivateMethod += StandardShaderDelegate;

			Shaders["pointShader"].OnActivateMethod += StandardShaderDelegate;
			Shaders["LineShader"].OnActivateMethod += StandardShaderDelegate;
			Shaders["BsplineShader"].OnActivateMethod += StandardShaderDelegate;


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

		#region ObjectCreation

		public IPoint CreatePoint()
		{
			return points.CreatePoint();
		}

		#endregion

		#region ObjectManipulation

		private void RemoveSelected()
		{
			var selected = SceneObjects.Where(x => x.IsSelected).ToList();
			selected.ForEach(x => x.CleanUp());
			selected.ForEach(x => SceneObjects.Remove(x));
		}

		private void CreateBSplineMesh()
		{
			var bspline = new Surface(this, Shaders["BsplineShader"], Shaders["LineShader"]);
			SceneObjects.Add(bspline);

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
			var lineShader = Shaders["LineShader"];
			shader.Activate();
			mesh.Render();

			ptShader.Activate();

			points.Render(ptShader);

			//lineShader.Activate();
			foreach (var sceneObject in sceneObjects)
			{
				//jakieś bindowanie uniformów.

				sceneObject.Render();
			}

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
