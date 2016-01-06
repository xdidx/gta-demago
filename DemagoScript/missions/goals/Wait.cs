using System;

namespace DemagoScript
{
    class Wait : Goal
    {
        private int secondsToWait;
        private int currentDuration;
        private DateTime startTime;
        private int remainingSeconds;

        public override bool initialize()
        {
            if (!base.initialize())
            {
                return false;
            }

            return true;
        }

        public Wait(int secondsToWait)
        {
            this.secondsToWait = secondsToWait;
            this.startTime = DateTime.Now;
        }

        public override bool update()
        {
            if (!base.update())
            {
                return false;
            }

            currentDuration = (int)(DateTime.Now - startTime).TotalSeconds;
            remainingSeconds = (secondsToWait - currentDuration);
            if (remainingSeconds <= 0)
            {
                accomplish();
            }
            else
            {
                setGoalText("Il reste " + remainingSeconds + " secondes à tenir");
            }

            return true;
        }

        public override void clear(bool removePhysicalElements = false)
        {
            
        }
    }
}
