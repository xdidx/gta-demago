using System;

namespace DemagoScript
{
    class Wait : AbstractObjective
    {
        private int millisecondsToWait;
        private int remainingSeconds;

        public Wait(int millisecondsToWait)
        {
            this.name = "Wait";

            this.millisecondsToWait = millisecondsToWait;
        }

        public override bool update()
        {
            if (!base.update())
            {
                return false;
            }

            remainingSeconds = (int)(millisecondsToWait - this.getElaspedTime());
            if (remainingSeconds <= 0)
            {
                accomplish();
            }
            else
            {
                ObjectiveText = "Il reste " + remainingSeconds + " secondes à tenir";
            }

            return true;
        }

        public override void populateDestructibleElements()
        {
            base.populateDestructibleElements();
        }

        public override void depopulateDestructibleElements(bool removePhysicalElements = false)
        {
        }
    }
}
