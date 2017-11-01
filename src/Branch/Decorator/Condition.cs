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
        protected override Status tick()
        {
            try
            {
                if (condition())
                {
                    this.Result = child.Tick();
                }
                else
                {
                    this.Result = Status.FAILURE;
                }
                return this.Result;
            }
            catch
            {
                this.Result = Status.ERROR;
                return this.Result;
            }
        }
    }
}
