using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public class RandomSelector : Selector
    {
        public RandomSelector(string name) : base(name)
        { }

        protected override void Open()
        {
            base.Open();

            children.Shuffle();
        }
    }
}
