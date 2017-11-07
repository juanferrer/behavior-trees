using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public class Inverter : Decorator
    {
        public Inverter()
        {

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
