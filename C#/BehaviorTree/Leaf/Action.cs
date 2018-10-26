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

        public override object Clone()
        {
            Action newNode = new Action(this.Name, this.action);
            return newNode;
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
