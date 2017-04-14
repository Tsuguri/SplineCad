using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SplineCAD.Rendering
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Vertex
	{
		public Vector3 Position;

		public Vertex(Vector3 pos)
		{
			Position = pos;
		}

		public Vertex(float x, float y, float z)
		{
			Position=new Vector3(x,y,z);
		}

		public void SetGLAttributes()
		{
			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, SizeInBytes, PositionOffsetInBytes);
		}

		public static readonly int SizeInBytes = Marshal.SizeOf<Vertex>();
		public static readonly int PositionOffsetInBytes = (int)Marshal.OffsetOf<Vertex>(nameof(Position));
	}
}
