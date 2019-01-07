using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public class Repeater : Decorator
    {
        private int n;
        private int attempts;

        public Repeater(string name, int times = 0)
        {
            this.Name = name;
            n = times;
        }

        public override object Clone()
        {
            Repeater newNode = new Repeater(this.Name, this.n);
            newNode.AddChild(this.child.Clone() as Node);
            return newNode;
        }

        /// <summary>
        /// Repeat n times and return
        /// </summary>
        /// <returns></returns>
        protected override Status tick()
        {
            this.Result = child.Tick();
            // If something went wrong, crash here
            if (this.Result == Status.ERROR) return this.Result;
            // If the child completed, count as an attempt
            if (this.Result != Status.RUNNING) ++attempts;

            if (attempts >= n)
            {
                // This was last tick, return SUCCESS
                this.Result = Status.SUCCESS;
            }
            else if (attempts < n || n == 0)
            {
                // Needs to keep going or it repeats forever, return RUNNING
                this.Result = Status.RUNNING;
            }

            return this.Result;
        }
    }
}
