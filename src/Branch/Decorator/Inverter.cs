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
        public override Status Tick()
        {
            var status = child.Tick();

            if (status == Status.SUCCESS) return Status.FAILURE;
            else if (status == Status.FAILURE) return Status.SUCCESS;
            else return status;
        }
    }
}
