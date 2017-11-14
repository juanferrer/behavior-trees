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

        public Action(string name, Func<Status> f)
        {
            this.Name = name;
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
                this.Result = action();
            }
            catch
            {
                this.Result = Status.ERROR;
            }
            return this.Result;
        }
    }
}
