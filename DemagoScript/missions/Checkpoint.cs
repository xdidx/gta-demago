using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemagoScript
{
    class Checkpoint
    {
        public delegate void CheckpointStartEvent(Checkpoint sender);

        /// <summary>
        /// Called when user accept popup question.
        /// </summary>
        public event CheckpointStartEvent OnCheckpointStart;

        private Goal goalToLaunch = null;

        public Checkpoint(Goal goal)
        {
            goalToLaunch = goal;
        }

        public void start()
        {
            OnCheckpointStart?.Invoke(this);
        }
    }
}
