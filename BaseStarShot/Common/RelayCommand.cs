using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BaseStarShot
{
    /// <summary>
    /// An ICommand implementation.
    /// </summary>
    public class RelayCommand : ICommand
    {
        readonly Action callback;
        readonly Action<object> callback2;
        readonly Func<bool> canExecute;
        readonly Func<object, bool> canExecute2;
        public event EventHandler<bool> ExecuteStateChanged;
        public event EventHandler<object> OnResponseEvent = delegate { };

        private bool canRun;
        public bool CanRun
        {
            get { return canRun; }
            set
            {
                if (canRun == value) return;
                canRun = value;
                ChangeCanExecute();
                if (ExecuteStateChanged != null)
                    ExecuteStateChanged(this, canRun);
            }
        }

        public RelayCommand(Action handler, bool canRun = true)
        {
            callback = handler;
            this.canRun = canRun;

            ExecuteStateChanged = (object sender, bool e) =>
            {

            };
        }

        public void OnResponse(object response)
        {
            OnResponseEvent.Invoke(this, response);
        }

        public RelayCommand(Action<object> handler, bool canRun = true)
        {
            callback2 = handler;
            this.canRun = canRun;
        }

        public RelayCommand(Action handler, Func<bool> canExecute)
        {
            callback = handler;
            this.canExecute = canExecute;
            this.canRun = true;
        }

        public RelayCommand(Action<object> handler, Func<object, bool> canExecute)
        {
            callback2 = handler;
            this.canExecute2 = canExecute;
            this.canRun = true;
        }

        public bool CanExecute(object parameter)
        {
            if (canRun)
            {
                if (this.canExecute != null)
                    return this.canExecute();
                if (this.canExecute2 != null)
                    return this.canExecute2(parameter);
                return true;
            }
            return false;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        public void ChangeCanExecute()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            if (callback != null) callback();
            if (callback2 != null) callback2(parameter);
        }
    }

    /// <summary>
    /// An ICommand implementation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RelayCommand<T> : ICommand
    {
        readonly Action<T> callback;

        private bool canRun;
        public bool CanRun
        {
            get { return canRun; }
            set
            {
                if (canRun == value) return;
                canRun = value;
                ChangeCanExecute();
            }
        }

        private Func<T, bool> canExecute;

        public RelayCommand(Action<T> handler, bool canRun = true)
        {
            callback = handler;
            this.canRun = canRun;
        }

        public RelayCommand(Action<T> handler, Func<T, bool> canExecute)
        {
            callback = handler;
            this.canExecute = canExecute;
            this.canRun = true;
        }

        public bool CanExecute(object parameter)
        {
            if (canRun)
            {
                if (this.canExecute != null)
                {
                    var r = this.canExecute((T)parameter);
                    return r;
                }
                return true;
            }
            return false;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        public void ChangeCanExecute()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            if (callback != null) callback((T)parameter);
        }
    }
}
