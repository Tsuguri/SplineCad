using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using SplineCAD.Data;

namespace SplineCAD.Objects
{
	public delegate void ChangedHandler<T>(IPoint<T> point);
	public interface IPoint<T>
	{
		T Position { get; set; }
		event ChangedHandler<T> OnChanged;
	}

}
