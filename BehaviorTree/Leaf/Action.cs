using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public class Action : Leaf
    {
        private Func<Status> action;

        public Action(Func<Status> f)
        {
            action = f;
        }

        /// <summary>
        /// Return result of action
        /// </summary>
        /// <returns></returns>
        protected override Status tick()
        {
            try
            {
                return action();
            }
            catch
            {
                return Status.ERROR;
            }
        }
    }
}
