using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace SplineCAD.Rendering
{
	public class ShaderPart : IDisposable
	{
		public int ShaderPartId
		{ get; set; }

		public ShaderType Type { get; private set; }


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
				throw;
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

		private bool iDisposed = false;

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
			if (iDisposed)
				return;
			iDisposed = true;

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

	public class Shader
	{
		public int ProgramId { get; private set; }

		#region StaticFactory

		public static Shader CreateShader(string vertexFileName, string fragmentFileName)
		{
			using (ShaderPart vertexPart = new ShaderPart(vertexFileName, ShaderType.VertexShader), fragmentPart = new ShaderPart(fragmentFileName, ShaderType.FragmentShader))
			{
				return new Shader(vertexPart,fragmentPart);
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
				GL.AttachShader(ProgramId,shaderPart.ShaderPartId);
			}

			GL.LinkProgram(ProgramId);

			GL.GetProgram(ProgramId,GetProgramParameterName.LinkStatus, out result);
			if (result == 0)
			{
				var log = GL.GetProgramInfoLog(ProgramId);
				GL.DeleteProgram(ProgramId);

				throw new Exception("Shader could not be linked: "+log);
			}
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
