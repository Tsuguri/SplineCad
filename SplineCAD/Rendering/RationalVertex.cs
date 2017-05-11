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
	public struct RationalVertex: IVertex
	{
		public Vector4 Position;

		public RationalVertex(Vector4 pos)
		{
			Position = pos;
		}

		public RationalVertex(Vector3 pos, float w)
		{
			Position=new Vector4(pos,w);
		}

		public RationalVertex(float x, float y, float z, float w)
		{
			Position = new Vector4(x, y, z,w);
		}

		public void SetGlAttributes()
		{
			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, SizeInBytes, PositionOffsetInBytes);
		}

		public static readonly int SizeInBytes = Marshal.SizeOf<RationalVertex>();
		public static readonly int PositionOffsetInBytes = (int)Marshal.OffsetOf<RationalVertex>(nameof(Position));
	}
}
