using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseStarShot.Services;

namespace BaseStarShot
{
    public abstract class BaseViewModel : CommonViewModel
    {
        /// <summary>
        /// Called once view model is pushed on the stack.
        /// </summary>
        public virtual void OnPush(bool modal)
        {

        }

        /// <summary>
        /// Called once view model is popped from the stack.
        /// </summary>
        public virtual void OnPop(bool modal)
        {

        }

        /// <summary>
        /// Called once view model is removed from the stack.
        /// </summary>
        public virtual void OnRemove(bool modal)
        {

        }

        /// <summary>
        /// Called once view model is revisited by becoming on top of the stack again.
        /// </summary>
        /// <param name="modal">True if the view model is displayed modally.</param>
        /// <param name="fromModal">True if the popped view model is displayed modally.</param>
        public virtual void OnRevisit(bool modal, bool fromModal)
        {

        }
    }
}
