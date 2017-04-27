using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplineCAD.Rendering;

namespace SplineCAD.Objects
{
	public abstract class Surface : Model
	{
		private bool polygonVisible = true;

		public bool PolygonVisible
		{
			get => polygonVisible;
			set
			{
				polygonVisible = value;
				OnPropertyChanged();
			}
		}

		private bool fillSurface = true;

		public bool FillSurface
		{
			get => fillSurface;
			set
			{
				fillSurface = value;
				OnPropertyChanged();
			}
		}
		private int patchDivX = 10;

		public int PatchDivX
		{
			get => patchDivX;
			set
			{
				if (value == patchDivX || value < 1)
					return;
				patchDivX = value;
				OnPropertyChanged();
			}
		}

		private int patchDivY = 10;

		public int PatchDivY
		{
			get => patchDivY;
			set
			{
				if (value == patchDivY || value < 1)
					return;
				patchDivY = value;
				OnPropertyChanged();
			}
		}
	}
}
