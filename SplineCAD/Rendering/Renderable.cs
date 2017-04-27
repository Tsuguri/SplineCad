using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplineCAD.Utilities;

namespace SplineCAD.Rendering
{
	public abstract class Renderable : BindableObject
	{
		public abstract void Render();
	}
}
