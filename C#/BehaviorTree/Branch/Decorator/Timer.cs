using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FluentBehaviorTree
{
    public class Timer : Decorator
    {
        private System.Timers.Timer timer;

        /// <summary>
        /// Reset its inner state
        /// </summary>
        protected override void Open()
        {
            base.Open();
            innerResult = Status.RUNNING;
            timerSet = false;
        }

        /// <summary>
        /// ElapsedEventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimeout(object sender, System.Timers.ElapsedEventArgs e)
        {
            innerResult = child.Tick();
            timer.Elapsed -= OnTimeout;
            timerSet = false;
        }

        private bool timerSet = false;

        private Status innerResult = Status.RUNNING;

        public Timer(string name, ulong ms)
        {
            this.Name = name;
            timer = new System.Timers.Timer(ms);
        }

        /// <summary>
        /// Wait until ms is reached and run child. Return running during that time
        /// </summary>
        /// <returns></returns>
        protected override Status tick()
        {
            if (timer.Enabled)
            {
                this.Result = Status.RUNNING;
            }
            else if (innerResult != Status.RUNNING)
            {
                this.Result = innerResult;
            }
            else
            {
                if (!timerSet)
                {
                    timer.AutoReset = false;
                    timer.Elapsed += OnTimeout;
                    timerSet = true;
                    timer.Enabled = true;
                }

                this.Result = Status.RUNNING;
            }
            return this.Result;
        }
    }
}
