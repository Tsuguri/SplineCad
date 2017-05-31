using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplineCAD.Utilities
{
	public class FloatWrapper : BindableObject
	{
		private float val;
		public float Value
		{
			get => val;
			set
			{
				val = value;
				OnPropertyChanged();
			}
		}

		internal FloatWrapper(float value)
		{
			Value = value;
		}
	}
}
