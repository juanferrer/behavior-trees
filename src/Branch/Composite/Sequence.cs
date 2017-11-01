using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public class Sequence : Composite
    {
        public Sequence()
        {
            children = new List<Node>();
        }

        /// <summary>
        /// Propagate tick to children. Return SUCCESS if no child fails
        /// </summary>
        /// <returns></returns>
        protected override Status tick()
        {
            foreach (var n in children)
            {
                if (n.IsClosed && n.Result == Status.FAILURE)
                {
                    this.Result = Status.FAILURE;
                    return this.Result;
                }
                else if (!n.IsClosed)
                {
                    this.Result = n.Tick();
                    if (this.Result != Status.SUCCESS) return this.Result;
                }
            }
            return Status.SUCCESS;
        }
    }
}
