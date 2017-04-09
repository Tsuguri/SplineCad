using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SplineCAD.Utilities
{
	public class CommandHandler : ICommand
	{
		private readonly Action action;

		public CommandHandler(Action action)
		{
			this.action = action;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			action?.Invoke();
		}

		public event EventHandler CanExecuteChanged;
	}

	class RelayCommand : ICommand
	{
		private readonly Action action;
		private readonly Func<bool> func;

		public RelayCommand(Action action, Func<bool> func)
		{
			this.action = action;
			this.func = func;
		}

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, new EventArgs());
		}

		#region ICommand Members

		public bool CanExecute(object parameter)
		{
			return func == null || func();
		}



		public event EventHandler CanExecuteChanged;

		public void Execute(object parameter)
		{
			action();
		}

		#endregion
	}
}
