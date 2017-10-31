using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public class Succeeder : Decorator
    {
        public Succeeder()
        {

        }

        /// <summary>
        /// Regardless of result, return SUCCESS
        /// </summary>
        /// <returns></returns>
        public override Status Tick()
        {
            var status = child.Tick();

            if (status != Status.ERROR) return Status.SUCCESS;
            else return status;
        }
    }
}
