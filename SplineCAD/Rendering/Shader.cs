using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace SplineCAD.Rendering
{
	/// <summary>
	/// Do not use this manually withoud using(ShaderPart ... ) {}
	/// ShaderPart is part of shader program, eg. VertexShader, FragmentShader, etc.
	/// </summary>
	public class ShaderPart : IDisposable
	{
		public int ShaderPartId
		{ get; set; }

		public ShaderType Type { get; private set; }
		public bool IDisposed { get => iDisposed; set => iDisposed = value; }

		private void CreateFromFile(string filename, ShaderType type)
		{
			string code;

			try
			{
				code = File.ReadAllText(filename);
			}
			catch (Exception e)
			{
				throw new Exception("Loading shader failed: ", e);
			}
			CreateFromCode(code, type);
		}

		private void CreateFromCode(string code, ShaderType type)
		{
			int result;

			ShaderPartId = GL.CreateShader(type);
			GL.ShaderSource(ShaderPartId, code);
			GL.CompileShader(ShaderPartId);

			GL.GetShader(ShaderPartId, ShaderParameter.CompileStatus, out result);
			if (result == 0)
			{
				string log = GL.GetShaderInfoLog(ShaderPartId);
				GL.DeleteShader(ShaderPartId);
				throw new Exception("Shader could not be compiled: " + log);
			}
		}

		#region IDispisable

		private bool iDisposed;

		public ShaderPart(string fileName, ShaderType type)
		{
			Type = type;
			CreateFromFile(fileName, type);
		}

		public void Dispose()
		{
			Dispose(true);

		}

		protected virtual void Dispose(bool disposing)
		{
			if (IDisposed)
				return;
			IDisposed = true;

			if (disposing)
			{
				//sth
			}

			Free();



		}

		private void Free()
		{
			if (ShaderPartId != 0)
			{
				GL.DeleteShader(ShaderPartId);
				ShaderPartId = 0;
			}
		}

		~ShaderPart()
		{
			Dispose(false);
		}

		#endregion
	}

	/// <summary>
	/// Represents instance of shader program - linked parts, ready to invoke.
	/// </summary>
	public class Shader
	{
		public delegate void OnActivate(Shader shader);

		public event OnActivate OnActivateMethod;

		/// <summary>
		/// OpenGl ID of this shader
		/// </summary>
		public int ProgramId { get; private set; }

		public void Activate()
		{
			GL.UseProgram(ProgramId);

			OnActivateMethod?.Invoke(this);
		}

		#region StaticFactory

		/// <summary>
		/// Creates shader automatically disposing shader parts (with using(){}).
		/// </summary>
		public static Shader CreateShader(string vertexFileName, string fragmentFileName)
		{
			using (ShaderPart vertexPart = new ShaderPart(vertexFileName, ShaderType.VertexShader), fragmentPart = new ShaderPart(fragmentFileName, ShaderType.FragmentShader))
			{
				return new Shader(vertexPart, fragmentPart);
			}
		}

		#endregion

		#region Constructors

		public Shader(params ShaderPart[] shaderParts)
		{
			int result;
			ProgramId = GL.CreateProgram();

			foreach (var shaderPart in shaderParts)
			{
				GL.AttachShader(ProgramId, shaderPart.ShaderPartId);
			}

			GL.LinkProgram(ProgramId);

			GL.GetProgram(ProgramId, GetProgramParameterName.LinkStatus, out result);
			if (result == 0)
			{
				var log = GL.GetProgramInfoLog(ProgramId);
				GL.DeleteProgram(ProgramId);

				throw new Exception("Shader could not be linked: " + log);
			}
		}

		#endregion

		#region Binding

		public int GetUniformLocation(string location)
		{
			int loc = GL.GetUniformLocation(ProgramId, location);
			if (loc == -1)
				throw new Exception("Location does not exist");
			return loc;
		}

		public void Bind(int location, float value)
		{
			GL.Uniform1(location,value);
		}

		public void Bind(int location, Vector2 vector)
		{
			GL.Uniform2(location,vector);
		}

		public void Bind(int location, Vector3 vector)
		{
			GL.Uniform3(location, vector);
		}


		public void Bind(int location, Vector4 vector)
		{
			GL.Uniform4(location,vector);
		}

		public void Bind(int location, Matrix4 matrix)
		{
			GL.UniformMatrix4(location, false, ref matrix);
		}

		#endregion

		#region IDisposableImplementation

		private bool iDisposed;

		private void Free()
		{
			if (ProgramId != 0)
			{
				GL.DeleteProgram(ProgramId);
				ProgramId = 0;
			}
		}

		/// <summary>
		/// Deletes shader from GPU memory. Use with caution.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (iDisposed)
				return;

			if (disposing)
			{
				//if anything managed
			}

			Free();

			iDisposed = true;
		}

		~Shader()
		{
			Dispose(false);
		}

		#endregion
	}
}
