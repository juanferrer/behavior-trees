using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public class Condition : Decorator
    {
        private Func<bool> condition;

        public Condition(Func<bool> f)
        {
            condition = f;
        }

        /// <summary>
        /// Return SUCCESS if the condition if met. Otherwise, FAILURE.
        /// </summary>
        /// <returns></returns>
        public override Status Tick()
        {
            try
            {
                if (condition())
                {
                    var status = child.Tick();
                    return status;
                }
                else return Status.FAILURE;
            }
            catch
            {
                return Status.ERROR;
            }
        }
    }
}
