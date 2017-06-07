using System;
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
using System.Windows.Forms;
using System.Windows.Media;

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
		private ICommand createTsplineCommand;
        private ICommand exportSceneCommand;

		private bool changed;


		private readonly ObservableCollection<Model> sceneObjects = new ObservableCollection<Model>();


		private Model selectedModel;



		private Vector3PointCollection vector3Points;
		private Vector4PointCollection vector4Points;

		private Camera camera;

        private Vector3 lightpos = new Vector3(0.0f, 5.0f, 0.0f);

		#endregion

		#region Properties

		public Dictionary<string, Shader> Shaders { get; private set; }

		private Dictionary<string, IMesh> Meshes { get; set; }

		public ICommand TestButtonCommand => testButtonCommand ?? (testButtonCommand = new CommandHandler(TestButtonAction));

		public ICommand RemoveSelectedCommand => removeSelectedCommand ??
												 (removeSelectedCommand = new CommandHandler(RemoveSelected));

		public ICommand CreateBsplineCommand => createBsplineCommand ??
		                                        (createBsplineCommand = new CommandHandler(CreateBSplineMesh));

		public ICommand CreateNurbsCommand => createNurbsCommand ??
		                                        (createNurbsCommand = new CommandHandler(CreateNurbsMesh));

		public ICommand CreateTsplineCommand => createTsplineCommand ??
		                                        (createTsplineCommand = new CommandHandler(CreateTsplineMesh));

        public ICommand ExportSceneCommand => exportSceneCommand ??
                                                (exportSceneCommand = new CommandHandler(ExportScene));

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

        public Vector3 LightPos => lightpos;

        public Vector3PointCollection Vector3Points => vector3Points;
        public Vector4PointCollection Vector4Points => vector4Points;

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

			camera = new Camera(new Vector3(0.0f, 0.0f, 5.0f));
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
				{"BsplineShader",Shader.CreateShader("Shaders\\BsplineSurface.vert","Shaders\\BsplineSurface.frag") },
				{"NurbsShader",Shader.CreateShader("Shaders\\NurbsSurface.vert","Shaders\\NurbsSurface.frag") },
				{"TsplineShader", Shader.CreateShader("Shaders\\TsplineSurface.vert","Shaders\\TsplineSurface.frag")}
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
			Shaders["NurbsShader"].OnActivateMethod += StandardShaderDelegate;
			Shaders["TsplineShader"].OnActivateMethod += StandardShaderDelegate;


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

		public IPoint<Vector4> CreateRationalPoint(Vector4 pos)
		{
			return vector4Points.CreatePoint(pos);
		}

		private void CreateBSplineMesh()
		{
            SurfacePopup popup = new SurfacePopup();
            popup.ShowDialog();
            if (popup.cancelled)
                return;

            Vector3 position = new Vector3((float)popup.UD_posX.Value, (float)popup.UD_posY.Value, (float)popup.UD_posZ.Value);
            Color clr = (Color)popup.clrPicker.SelectedColor;
            float width = (float)popup.UD_width.Value;
            float height = (float)popup.UD_height.Value;
            int lenU = (int)popup.UD_patchesU.Value + 3;
            int lenV = (int)popup.UD_patchesV.Value + 3;

            var points = new IPoint<Vector3>[lenU, lenV];
            for (int i = 0; i < lenU; i++)
            {
                float u = -width / 2 + (i*width) / lenU;
                for (int j = 0; j < lenV; j++)
                {
                    float v = -height / 2 + (j * height) / lenV;
                    points[i, j] = CreatePoint();
                    points[i, j].Position = new Vector3(position.X + u, position.Y, position.Z + v);
                }
            }			    
			var bspline = new BSplineSurface(this, Shaders["BsplineShader"], Shaders["LineShader"], points, clr);
			SceneObjects.Add(bspline);

		}

		private void CreateNurbsMesh()
		{
            SurfacePopup popup = new SurfacePopup();
            popup.ShowDialog();
            if (popup.cancelled)
                return;

            Vector3 position = new Vector3((float)popup.UD_posX.Value, (float)popup.UD_posY.Value, (float)popup.UD_posZ.Value);
            Color clr = (Color)popup.clrPicker.SelectedColor;
            float width = (float)popup.UD_width.Value;
            float height = (float)popup.UD_height.Value;
            int lenU = (int)popup.UD_patchesU.Value + 3;
            int lenV = (int)popup.UD_patchesV.Value + 3;

            var points = new IPoint<Vector4>[lenU, lenV];
            for (int i = 0; i < lenU; i++)
            {
                float u = -width / 2 + (i * width) / lenU;
                for (int j = 0; j < lenV; j++)
                {
                    float v = -height / 2 + (j * height) / lenV;
                    points[i, j] = CreateRationalPoint();
                    points[i, j].Position = new Vector4(position.X + u, position.Y, position.Z + v, 1);
                }
            }
			var bspline = new NurbsSurface(this, Shaders["NurbsShader"], Shaders["RationalLineShader"], points, clr);
			SceneObjects.Add(bspline);
		}

		private void CreateTsplineMesh()
		{
            SurfacePopup popup = new SurfacePopup();
            popup.ShowDialog();
            if (popup.cancelled)
                return;

            Vector3 position = new Vector3((float)popup.UD_posX.Value, (float)popup.UD_posY.Value, (float)popup.UD_posZ.Value);
            Color clr = (Color)popup.clrPicker.SelectedColor;
            float width = (float)popup.UD_width.Value;
            float height = (float)popup.UD_height.Value;        
            int lenU = (int)popup.UD_patchesU.Value + 3;
            int lenV = (int)popup.UD_patchesV.Value + 3;

            var points = new IPoint<Vector4>[lenU, lenV];
            for (int i = 0; i < lenU; i++)
            {
                float u = -width / 2 + (i * width) / lenU;
                for (int j = 0; j < lenV; j++)
                {
                    float v = -height / 2 + (j * height) / lenV;

                    points[i, j] = CreateRationalPoint();
                    points[i, j].Position = new Vector4(position.X + u, position.Y, position.Z + v, 1);
                }
            }
			var tspline = new TSplineSurface(this, Shaders["TsplineShader"], Shaders["RationalLineShader"], points, clr);
			SceneObjects.Add(tspline);
		}

		#endregion

		#region ObjectManipulation

		public void AddModel(Model model)
		{
			sceneObjects.Add(model);
		}

		private void RemoveSelected()
		{
			var selected = SceneObjects.Where(x => x.IsSelected).ToList();
			selected.ForEach(x => x.CleanUp());
			selected.ForEach(x => SceneObjects.Remove(x));
		}



        #endregion

        #region Export

        private void ExportScene()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "IGES files (*.igs)|*.igs";
	        if (dialog.ShowDialog() == DialogResult.Cancel) return;
            if (!FileManager.ExportToIGS(SceneObjects.ToList(), dialog.FileName,this))
                MessageBox.Show("Export failed.");
            else MessageBox.Show("Export successful.");
            
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

            MainCamera.HandleKeyboardMovement(0.15f);

            //var shader = Shaders["testShader"];
            //var mesh = Meshes["cubeMesh"];
            //shader.Activate();
            //mesh.Render();

            var ptShader = Shaders["pointShader"];

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
