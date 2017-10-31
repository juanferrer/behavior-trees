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
        public override Status Tick()
        {
            foreach (var n in children)
            {
                var status = n.Tick();
                if (status != Status.FAILURE) return status;
            }
            return Status.FAILURE;
        }
    }
}
