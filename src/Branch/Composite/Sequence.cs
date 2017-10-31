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
        public override Status Tick()
        {
            foreach (var n in children)
            {
                var status = n.Tick();
                if (status != Status.SUCCESS) return status;
            }
            return Status.SUCCESS;
        }
    }
}
