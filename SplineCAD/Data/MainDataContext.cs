﻿using System;
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
		private ICommand createBsplineCommand;
		private ICommand createNurbsCommand;

		private bool changed;


		private readonly ObservableCollection<Model> sceneObjects = new ObservableCollection<Model>();


		private Model selectedModel;



		private Vector3PointCollection vector3Points;
		private Vector4PointCollection vector4Points;

		private Camera camera;

		#endregion

		#region Properties

		private Dictionary<string, Shader> Shaders { get; set; }

		private Dictionary<string, IMesh> Meshes { get; set; }

		public ICommand TestButtonCommand => testButtonCommand ?? (testButtonCommand = new CommandHandler(TestButtonAction));

		public ICommand RemoveSelectedCommand => removeSelectedCommand ??
												 (removeSelectedCommand = new CommandHandler(RemoveSelected));

		public ICommand CreateBsplineCommand => createBsplineCommand ??
		                                        (createBsplineCommand = new CommandHandler(CreateBSplineMesh));

		public ICommand CreateNurbsCommand => createNurbsCommand ??
		                                        (createNurbsCommand = new CommandHandler(CreateNurbsMesh));

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

        public Vector3PointCollection Vector3Points => vector3Points;

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



			var pt1 = vector3Points.CreatePoint();
			pt1.Position = new Vector3(1.0f, 1.0f, 1.0f);

			var pt2 = vector3Points.CreatePoint();
			pt2.Position = new Vector3(-1.0f, -1.0f, 1.0f);

			var pt3 = vector3Points.CreatePoint();
			pt3.Position = new Vector3(1.0f, -1.0f, 1.0f);

			var pt4 = vector3Points.CreatePoint();
			pt4.Position = new Vector3(-1.0f, 1.0f, 1.0f);

			var pt5 = vector3Points.CreatePoint();
			pt5.Position = new Vector3(1.0f, 1.0f, -1.0f);

			var pt6 = vector3Points.CreatePoint();
			pt6.Position = new Vector3(-1.0f, -1.0f, -1.0f);

			var pt7 = vector3Points.CreatePoint();
			pt7.Position = new Vector3(1.0f, -1.0f, -1.0f);

			var pt8 = vector3Points.CreatePoint();
			pt8.Position = new Vector3(-1.0f, 1.0f, -1.0f);



			camera = new Camera(new Vector3(0.0f, 0.0f, 5.0f));

			CreateBSplineMesh();
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
				{"RationalLineShader", Shader.CreateShader("Shaders\\RationalLineShader.vert","Shaders\\RationalLineShader.frag") },
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
			Shaders["RationalLineShader"].OnActivateMethod += StandardShaderDelegate;
			Shaders["BsplineShader"].OnActivateMethod += StandardShaderDelegate;


		}

		private void InitializeMeshes()
		{
			Meshes = new Dictionary<string, IMesh>
			{
				{"cubeMesh", new Cube() }
			};
			vector3Points = new Vector3PointCollection();
			vector4Points = new Vector4PointCollection();
		}

		#endregion

		#region ObjectCreation

		public IPoint<Vector3> CreatePoint()
		{
			return vector3Points.CreatePoint();
		}

		public IPoint<Vector4> CreateRationalPoint()
		{
			return vector4Points.CreatePoint();
		}

		private void CreateBSplineMesh()
		{
			var points = new IPoint<Vector3>[4 + 3, 4 + 3];



			for (int i = 0; i < 4 + 3; i++)
			for (int j = 0; j < 4 + 3; j++)
			{

				points[i, j] = CreatePoint();
				points[i, j].Position = new Vector3(i, (float)Math.Sin(0.5 * i), j);
			}
			var bspline = new BSplineSurface(this, Shaders["BsplineShader"], Shaders["LineShader"], points);
			SceneObjects.Add(bspline);

		}

		private void CreateNurbsMesh()
		{
			var points = new IPoint<Vector4>[4 + 3, 4 + 3];
			for (int i = 0; i < 4 + 3; i++)
			for (int j = 0; j < 4 + 3; j++)
			{

				points[i, j] = CreateRationalPoint();
				points[i, j].Position = new Vector4(i, (float)Math.Sin(0.5 * i), j,1);
			}
			var bspline = new NurbsSurface(this, Shaders["BsplineShader"], Shaders["RationalLineShader"], points);
			SceneObjects.Add(bspline);
		}

		#endregion

		#region ObjectManipulation

		private void RemoveSelected()
		{
			var selected = SceneObjects.Where(x => x.IsSelected).ToList();
			selected.ForEach(x => x.CleanUp());
			selected.ForEach(x => SceneObjects.Remove(x));
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

			vector3Points.Render(ptShader);
			vector4Points.Render(ptShader);

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
