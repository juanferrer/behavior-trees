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

        public Repeater(int times = -1)
        {
            n = times;
        }

        /// <summary>
        /// Repeat n times and return
        /// </summary>
        /// <returns></returns>
        public override Status Tick()
        {
            Status status;
            if (n == -1)
            {             
                while (true)
                {
                    status = child.Tick();

                    if (status == Status.ERROR) return status;
                }
            }
            else
            {
                for (int i = 0; i < n; ++i)
                {
                    status = child.Tick();
                    if (status == Status.ERROR) return status;
                }
            }
            return Status.SUCCESS;
        }
    }
}
