using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace SplineCAD.Rendering
{
	public interface IMesh
	{
		void Render();
		void Dispose();
	}


	public abstract class Mesh<TVertType> : IMesh where TVertType : struct,IVertex
	{
		private int vao;
		private int vbo;
		private int ibo;

		private int indicesCount;
		private BeginMode mode;

		protected void Initialize(TVertType[] vertices, uint[] indices, BeginMode mode = BeginMode.Triangles)
		{
			this.mode = mode;
			if (vao == 0)
			{
				vao = GL.GenVertexArray();
				vbo = GL.GenBuffer();
				ibo = GL.GenBuffer();
			}


			indicesCount = indices.Length;

			GL.BindVertexArray(vao);

			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Marshal.SizeOf<TVertType>()), vertices, BufferUsageHint.StaticDraw);

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
			GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsageHint.StaticDraw);

			vertices[0].SetGlAttributes();

			GL.BindVertexArray(0);
		}

		public virtual void Render()
		{
			GL.BindVertexArray(vao);
			//GL.PointSize oraz BeginMode.Vector3Points
			GL.DrawElements(mode, indicesCount, DrawElementsType.UnsignedInt, 0);
		}

		public virtual void Dispose()
		{
			if (ibo != 0)
			{
				GL.DeleteBuffer(ibo);
				ibo = 0;
			}

			if (vbo != 0)
			{
				GL.DeleteBuffer(vbo);
				vbo = 0;
			}

			if (vao != 0)
			{
				GL.DeleteVertexArray(vao);
				vao = 0;
			}
		}
	}
}
