using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public class Sequence : Composite
    {
        public Sequence(string name)
        {
            this.Name = name;
            children = new List<Node>();
        }

        /// <summary>
        /// Propagate tick to children. Return SUCCESS if no child fails
        /// </summary>
        /// <returns></returns>
        protected override Status tick()
        {
            if (this.Result == Status.RUNNING)
            {
                foreach (var n in children)
                {
                    if (!n.IsOpen && n.Result == Status.FAILURE)
                    {
                        this.Result = Status.FAILURE;
                        return this.Result;
                    }
                    else if (n.Result == Status.RUNNING)
                    {
                        this.Result = n.Tick();
                        if (this.Result != Status.SUCCESS) return this.Result;
                    }
                }
                this.Result = Status.SUCCESS;
            }
            return this.Result;
        }
    }
}
