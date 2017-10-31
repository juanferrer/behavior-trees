using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public class RepeatUntilFail : Decorator
    {
        public RepeatUntilFail()
        {

        }

        /// <summary>
        /// Repeat until FAILURE
        /// </summary>
        /// <returns></returns>
        public override Status Tick()
        {
            Status status;
            do
            {
                status = child.Tick();
                if (status == Status.ERROR) return status;
            } while (status != Status.FAILURE);

            return Status.SUCCESS;
        }
    }
}
