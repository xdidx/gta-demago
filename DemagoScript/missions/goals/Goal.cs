using GTA;
using GTA.Native;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemagoScript
{
    abstract class Goal
    {
        protected bool initialized = false;
        private bool over = false;
        private bool failed = false;
        private DateTime startGoalTime;

        private UIResText goalUIText = null;
        private UIResText goalShadow = null;
        private string goalText = "";

        private UIResText adviceUIText = null;
        private UIResText adviceShadow = null;
        private string adviceText = "";

        public delegate void GoalAccomplishedEvent(Goal sender, TimeSpan elaspedTime);
        public delegate void GoalFailEvent(Goal sender, string reason); 
        public delegate void GoalStartEvent(Goal sender); 

        /// <summary>
        /// Called when user accomplish the goal.
        /// </summary>
        public event GoalAccomplishedEvent OnGoalAccomplished;

        /// <summary>
        /// Called when goal is started.
        /// </summary>
        public event GoalStartEvent OnGoalStart;

        /// <summary>
        /// Called when user fail a goal.
        /// </summary>
        public event GoalFailEvent OnGoalFail;

        public bool isOver()
        {
            return over;
        }

        public virtual bool update()
        {
            initialize();
            if (over)
            {
                return false;
            }

            if (!Function.Call<bool>(Hash.IS_HUD_HIDDEN))
            {
                if (goalText != "")
                {
                    goalShadow.Caption = goalUIText.Caption = goalText;
                    goalShadow.Draw();
                    goalUIText.Draw();
                }

                if (adviceText != "")
                {
                    adviceShadow.Caption = adviceUIText.Caption = adviceText;
                    adviceShadow.Draw();
                    adviceUIText.Draw();
                }
            }

            return true;
        }

        public virtual bool initialize()
        {
            if (initialized)
            {
                return false;
            }

            //goalUIText = new UIResText("", new Point(330, Game.ScreenResolution.Height - 100), 0.7f, Color.WhiteSmoke, GTA.Font.ChaletComprimeCologne, UIResText.Alignment.Left);
            //goalShadow = new UIResText("", new Point(332, Game.ScreenResolution.Height - 98), 0.7f, Color.Black, GTA.Font.ChaletComprimeCologne, UIResText.Alignment.Left);
            goalUIText = new UIResText("", new Point(Game.ScreenResolution.Width / 2, Game.ScreenResolution.Height / 5), 0.7f, Color.WhiteSmoke, GTA.Font.ChaletComprimeCologne, UIResText.Alignment.Centered);
            goalShadow = new UIResText("", new Point(Game.ScreenResolution.Width / 2 + 2, Game.ScreenResolution.Height / 5 + 2), 0.7f, Color.Black, GTA.Font.ChaletComprimeCologne, UIResText.Alignment.Centered);

            adviceUIText = new UIResText("", new Point(Game.ScreenResolution.Width / 2, Game.ScreenResolution.Height / 5 + 40), 0.6f, Color.Green, GTA.Font.ChaletComprimeCologne, UIResText.Alignment.Centered);
            adviceShadow = new UIResText("", new Point(Game.ScreenResolution.Width / 2 + 2, Game.ScreenResolution.Height / 5 + 42), 0.6f, Color.Black, GTA.Font.ChaletComprimeCologne, UIResText.Alignment.Centered);

            startGoalTime = DateTime.Now;

            initialized = true;
            over = false;
            failed = false;

            OnGoalStart?.Invoke(this);
            return true;
        }

        public virtual void reset()
        {
            initialized = false;
        }

        public abstract void clear(bool removePhysicalElements = false);

        public bool isFailed()
        {
            return failed;
        }

        public bool isAccomplished()
        {
            return over && !failed;
        }

        public void accomplish()
        {
            failed = false;
            over = true;

            clear(false);

            TimeSpan elapsedTime = DateTime.Now - startGoalTime;
            OnGoalAccomplished?.Invoke(this, elapsedTime);
        }

        protected void setGoalText(string goalText)
        {
            this.goalText = goalText;
        }

        public void setAdviceText(string adviceText)
        {
            adviceUIText.Color = Color.Green;
            adviceUIText.Scale = adviceShadow.Scale = 0.6f;
            this.adviceText = adviceText;
        }

        public void setWarningText(string warningText)
        {
            adviceUIText.Color = Color.Red;
            adviceUIText.Scale = adviceShadow.Scale = 0.8f;
            this.adviceText = warningText;
        }

        public void clearAdviceText()
        {
            this.adviceText = "";
        }

        public void fail(string reason)
        {
            failed = true;
            over = true;
            OnGoalFail?.Invoke(this, reason);
        }

    }
}
