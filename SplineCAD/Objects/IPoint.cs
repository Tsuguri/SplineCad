using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using SplineCAD.Data;

namespace SplineCAD.Objects
{
	public interface IPoint
	{
		Vector3 Position { get; set; }
	}

}
