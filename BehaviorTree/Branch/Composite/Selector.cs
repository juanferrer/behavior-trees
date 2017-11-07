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
            foreach (var n in children)
            {
                if (n.IsClosed && n.Result == Status.SUCCESS)
                {
                    this.Result = Status.SUCCESS;
                    return this.Result;
                }
                else if (!n.IsClosed)
                {
                    this.Result = n.Tick();
                    if (this.Result != Status.FAILURE) return this.Result;
                }
            }
            return Status.FAILURE;
        }
    }
}
