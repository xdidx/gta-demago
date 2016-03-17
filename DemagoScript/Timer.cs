using GTA;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemagoScript
{
    class Timer
    {
        public delegate void TimerStopEvent(Timer sender);
        public delegate void TimerInterruptEvent(Timer sender, float elapsedMilliseconds);
        public delegate void TimerUpdateEvent(float elapsedMilliseconds, float elapsedPourcent);
        public event TimerStopEvent OnTimerStop;
        public event TimerUpdateEvent OnTimerUpdate; 
        public event TimerInterruptEvent OnTimerInterrupt; 

        private static List<Timer> timers = new List<Timer>();

        private float startTime = 0;
        private float millisecondsToWait = 0;
        private bool isInProgress = true;

        public Timer(float millisecondsToWait)
        {
            this.startTime = DemagoScript.getScriptTime();
            this.millisecondsToWait = millisecondsToWait;
            timers.Add(this);
        }

        public float getElapsedMilliSeconds()
        {
            return DemagoScript.getScriptTime() - startTime;             
        }

        public float getElapsedSeconds()
        {
            return getElapsedMilliSeconds() / 1000;
        }

        public static void updateAllTimers()
        {
            for (int i = timers.Count - 1; i >= 0; i--)
            {
                if (timers[i].isInProgress)
                {
                    timers[i].update();
                }
            }
        }

        private void stop()
        {
            if (isInProgress)
            {
                this.interrupt();
                OnTimerStop?.Invoke(this);
            }
        }

        public void interrupt()
        {
            if (isInProgress)
            {
                timers.Remove(this);
                isInProgress = false;
                OnTimerInterrupt?.Invoke(this, this.getElapsedMilliSeconds());
            }
        }

        public void update()
        {
            if (isInProgress)
            {
                float elapsedMilliseconds = DemagoScript.getScriptTime() - startTime;
                if (elapsedMilliseconds <= millisecondsToWait)
                {
                    float elapsedPourcent = elapsedMilliseconds / millisecondsToWait;
                    OnTimerUpdate?.Invoke(elapsedMilliseconds, elapsedPourcent);
                }
                else
                {
                    this.stop();
                }
            }
        }

    }
}
