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
        public delegate void TimerUpdateEvent(float elapsedMilliseconds, float elapsedPourcent);
        public event TimerStopEvent OnTimerStop;
        public event TimerUpdateEvent OnTimerUpdate;

        private static List<Timer> timers = null;

        private float startTime;
        private float millisecondsToWait = 0;

        public Timer(float millisecondsToWait)
        {
            this.startTime = DemagoScript.getScriptTime();
            this.millisecondsToWait = millisecondsToWait;
            if (timers == null)
            {
                timers = new List<Timer>();
            }
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
            if (timers == null)
            {
                timers = new List<Timer>();
            }

            for (int i = timers.Count - 1; i >= 0; i--)
            {
                timers[i].update();
            }
        }

        public void stop()
        {
            timers.Remove(this);
            OnTimerStop?.Invoke(this);
        }

        public void update()
        {
            float elapsedMilliseconds = DemagoScript.getScriptTime() - startTime;
            if (elapsedMilliseconds > millisecondsToWait)
            {
                stop();
            }
            else
            {
                float elapsedPourcent = elapsedMilliseconds / millisecondsToWait;
                OnTimerUpdate?.Invoke(elapsedMilliseconds, elapsedPourcent);
            }
        }

    }
}
