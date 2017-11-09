using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public class Selector : Composite
    {
        public Selector()
        {
            children = new List<Node>();
        }

        /// <summary>
        /// Propagate tick to children. Return FAILURE if no child succeeds
        /// </summary>
        /// <returns></returns>
        protected override Status tick()
        {
            if (this.Result == Status.RUNNING)
            {
                foreach (var n in children)
                {
                    if (!n.IsOpen && n.Result == Status.SUCCESS)
                    {
                        this.Result = Status.SUCCESS;
                        return this.Result;
                    }
                    else if (n.Result == Status.RUNNING)
                    {
                        this.Result = n.Tick();
                        if (this.Result != Status.FAILURE) return this.Result;
                    }
                }
                this.Result = Status.FAILURE;
            }
            return this.Result;
        }
    }
}
