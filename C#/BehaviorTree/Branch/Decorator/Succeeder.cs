using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public class Succeeder : Decorator
    {
        public Succeeder(string name)
        {
            this.Name = name;
        }

        public override object Clone()
        {
            Succeeder newNode = new Succeeder(this.Name);
            newNode.AddChild((this.child).Clone() as Node);
            return newNode;
        }

        /// <summary>
        /// Regardless of result, return SUCCESS
        /// </summary>
        /// <returns></returns>
        protected override Status tick()
        {
            this.Result = child.Tick();

            if (this.Result == Status.FAILURE)
            {
                this.Result = Status.SUCCESS;
            }
            return this.Result;
        }
    }
}
