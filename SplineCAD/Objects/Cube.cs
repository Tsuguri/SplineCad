using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SplineCAD.Rendering;

namespace SplineCAD.Objects
{
	class Cube : Mesh
	{
		private int ID;
		private int vao;
		private int vbo;
		private int ibo;

		private int indicesCount;

		public Cube()
		{
			vao = GL.GenVertexArray();
			vbo = GL.GenBuffer();
			ibo = GL.GenBuffer();
			float size = 1;
			float halfsize = size / 2.0f;
			Vertex[] vertices = new Vertex[]
			{
				new Vertex(-halfsize, -halfsize, -halfsize),
				new Vertex(-halfsize, -halfsize, halfsize),
				new Vertex(halfsize, -halfsize, -halfsize),
				new Vertex(halfsize, -halfsize, halfsize),
				new Vertex(-halfsize, halfsize, -halfsize),
				new Vertex(-halfsize, halfsize, halfsize),
				new Vertex(halfsize, halfsize, -halfsize),
				new Vertex(halfsize, halfsize, halfsize),
			};


		uint[] indices = new uint[]
			{
				0,1,3, //down
				0,3,2,

				4,5,6, //up
				4,6,7,

				1,5,0, //left
				0,5,4,

				2,6,3, //right
				3,6,7,

				4,0,6, //front
				0,6,2


			};

			indicesCount = indices.Length;

			GL.BindVertexArray(vao);

			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Marshal.SizeOf<Vertex>()), vertices, BufferUsageHint.StaticDraw);

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
			GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsageHint.StaticDraw);

			vertices[0].SetGLAttributes();

			GL.BindVertexArray(0);
		}

		public override void Render()
		{
			GL.BindVertexArray(vao);
			GL.DrawElements(BeginMode.Triangles, indicesCount, DrawElementsType.UnsignedInt, 0);
		}
	}
}
