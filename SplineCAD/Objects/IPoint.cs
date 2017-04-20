using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using SplineCAD.Data;

namespace SplineCAD.Objects
{
	public delegate void ChangedHandler(IPoint point);
	public interface IPoint
	{
		Vector3 Position { get; set; }
		event ChangedHandler OnChanged;
	}

}
