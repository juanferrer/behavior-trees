using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public class RepeatUntilFail : Decorator
    {
        public RepeatUntilFail(string name)
        {
            this.Name = name;
        }

        public override Node Copy()
        {
            RepeatUntilFail newNode = new RepeatUntilFail(this.Name);
            newNode.AddChild(this.child.Copy());
            return newNode;
        }

        /// <summary>
        /// Repeat until FAILURE
        /// </summary>
        /// <returns></returns>
        protected override Status tick()
        {
            do
            {
                this.Result = child.Tick();
                if (this.Result == Status.ERROR) return this.Result;
            } while (this.Result != Status.FAILURE);

            this.Result = Status.SUCCESS;
            return this.Result;
        }
    }
}
