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
	public struct PositionVertex : IVertex
	{
		public Vector3 Position;

		public PositionVertex(Vector3 pos)
		{
			Position = pos;
		}

		public PositionVertex(float x, float y, float z)
		{
			Position=new Vector3(x,y,z);
		}

		public void SetGlAttributes()
		{
			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, SizeInBytes, PositionOffsetInBytes);
		}

		public static readonly int SizeInBytes = Marshal.SizeOf<PositionVertex>();
		public static readonly int PositionOffsetInBytes = (int)Marshal.OffsetOf<PositionVertex>(nameof(Position));
	}
}
