using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace xpp_chatai_vs.Command
{
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// The <see cref="Action"/> to invoke when <see cref="Execute"/> is used.
        /// Willie Yao - 04/14/2025
        /// </summary>
        private readonly Action execute;

        /// <summary>
        /// The optional action to invoke when <see cref="CanExecute"/> is used.
        /// Willie Yao - 04/14/2025
        /// </summary>
        private readonly Func<bool> canExecute;

        /// <inheritdoc/>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class that can always execute.
        /// Willie Yao - 04/14/2025
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="execute"/> is <see langword="null"/>.</exception>
        public RelayCommand(Action execute)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));

            this.execute = execute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// Willie Yao - 04/14/2025
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="execute"/> or <paramref name="canExecute"/> are <see langword="null"/>.</exception>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            if (canExecute == null) throw new ArgumentNullException(nameof(canExecute));

            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <inheritdoc/>
        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanExecute(object parameter)
        {
            return this.canExecute?.Invoke() != false;
        }

        /// <inheritdoc/>
        public void Execute(object parameter)
        {
            this.execute();
        }
    }
}
