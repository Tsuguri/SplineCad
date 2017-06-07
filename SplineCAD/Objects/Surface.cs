using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplineCAD.Rendering;
using OpenTK;
using System.Windows.Media;

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
				VisibilityChanged();
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
				VisibilityChanged();
			}
		}
		private int patchDivX = 25;

		public int PatchDivX
		{
			get => patchDivX;
			set
			{
				if (value == patchDivX || value < 1)
					return;
				patchDivX = value;
				OnPropertyChanged();
				PatchDivChanged();
			}
		}

		private int patchDivY = 25;

		public int PatchDivY
		{
			get => patchDivY;
			set
			{
				if (value == patchDivY || value < 1)
					return;
				patchDivY = value;
				OnPropertyChanged();
				PatchDivChanged();
			}
		}

        private Color surfaceclr = Colors.Turquoise;

        public Color SurfaceColor
        {
            get => surfaceclr;
            set
            {
                surfaceclr = value;
                OnPropertyChanged();
            }
        }

        protected virtual void PatchDivChanged()
		{
		}

		protected virtual void VisibilityChanged()
		{
		}
	}
}
