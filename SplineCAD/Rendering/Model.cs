using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplineCAD.Utilities;

namespace SplineCAD.Rendering
{
	public abstract class Model : BindableObject
	{
		private string name= "name";

		public string Name
		{
			get => name;
			set
			{
				name = value;
				OnPropertyChanged();
			}
		}

		private bool isSelected;

		public bool IsSelected
		{
			get => isSelected;
			set
			{
				isSelected = value;
				OnPropertyChanged();
			}
		}

		public abstract void CleanUp();

		public abstract void Render();
	}
}
