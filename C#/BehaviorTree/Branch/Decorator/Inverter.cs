using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public class Inverter : Decorator
    {
        public Inverter(string name)
        {
            this.Name = name;
        }

        public override Node Copy()
        {
            Inverter newNode = new Inverter(this.Name);
            newNode.AddChild(this.child.Copy());
            return newNode;
        }

        /// <summary>
        /// Works as NOT logic operator
        /// </summary>
        /// <returns></returns>
        protected override Status tick()
        {
            this.Result = child.Tick();

            if (this.Result == Status.SUCCESS) this.Result = Status.FAILURE;
            else if (this.Result == Status.FAILURE) this.Result = Status.SUCCESS;

            return this.Result;
        }
    }
}
