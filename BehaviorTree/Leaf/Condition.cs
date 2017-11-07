using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public class Condition : Leaf
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
        protected override Status tick()
        {
            try
            {
                return condition() ? Status.SUCCESS : Status.FAILURE;
            }
            catch
            {
                return Status.ERROR;
            }
        }
    }
}
