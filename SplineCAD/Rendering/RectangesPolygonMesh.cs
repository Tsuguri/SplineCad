﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SplineCAD.Objects;

namespace SplineCAD.Rendering
{
	class Vector3RectangesPolygonMesh
	{
		private IMesh mesh;
		public Vector3RectangesPolygonMesh(IPoint<Vector3>[,] points)
		{
			uint xDiv = (uint)points.GetLength(0);
			uint yDiv = (uint)points.GetLength(1);

			var vertices = new List<IPoint<Vector3>>((int)(xDiv * yDiv));
			var indices = new uint[((xDiv - 1) * (yDiv - 1) * 2 + xDiv + yDiv - 2) * 2];
			for (int i = 0; i < xDiv; i++)
			for (int j = 0; j < yDiv; j++)
			{
				vertices.Add(points[i, j]);
			}
			int ind = 0;
			for (uint i = 0; i < xDiv - 1; i++)
			for (uint j = 0; j < yDiv - 1; j++)
			{
				indices[ind++] = i * yDiv + j;
				indices[ind++] = i * yDiv + j + 1;
				indices[ind++] = i * yDiv + j;
				indices[ind++] = (i + 1) * yDiv + j;

			}

			for (uint i = 0; i < xDiv - 1; i++)
			{
				indices[ind++] = (i + 1) * yDiv - 1;
				indices[ind++] = (i + 2) * yDiv - 1;
			}
			for (uint j = 0; j < yDiv - 1; j++)
			{
				indices[ind++] = (xDiv - 1) * yDiv + j;
				indices[ind++] = (xDiv - 1) * yDiv + j + 1;
			}

			mesh = new Vector3SelfActualizingMesh(vertices, indices, BeginMode.Lines);
		}

		public void Render()
		{
			mesh.Render();
		}

		public void Dispose()
		{
			mesh.Dispose();
		}
	}

	class Vector4RectangesPolygonMesh
	{
		private IMesh mesh;
		public Vector4RectangesPolygonMesh(IPoint<Vector4>[,] points)
		{
			uint xDiv = (uint)points.GetLength(0);
			uint yDiv = (uint)points.GetLength(1);

			var vertices = new List<IPoint<Vector4>>((int)(xDiv * yDiv));
			var indices = new uint[((xDiv - 1) * (yDiv - 1) * 2 + xDiv + yDiv - 2) * 2];
			for (int i = 0; i < xDiv; i++)
			for (int j = 0; j < yDiv; j++)
			{
				vertices.Add(points[i, j]);
			}
			int ind = 0;
			for (uint i = 0; i < xDiv - 1; i++)
			for (uint j = 0; j < yDiv - 1; j++)
			{
				indices[ind++] = i * yDiv + j;
				indices[ind++] = i * yDiv + j + 1;
				indices[ind++] = i * yDiv + j;
				indices[ind++] = (i + 1) * yDiv + j;

			}

			for (uint i = 0; i < xDiv - 1; i++)
			{
				indices[ind++] = (i + 1) * yDiv - 1;
				indices[ind++] = (i + 2) * yDiv - 1;
			}
			for (uint j = 0; j < yDiv - 1; j++)
			{
				indices[ind++] = (xDiv - 1) * yDiv + j;
				indices[ind++] = (xDiv - 1) * yDiv + j + 1;
			}

			mesh = new Vector4SelfActualizingMesh(vertices, indices, BeginMode.Lines);
		}

		public void Render()
		{
			mesh.Render();
		}

		public void Dispose()
		{
			mesh.Dispose();
		}
	}
}
