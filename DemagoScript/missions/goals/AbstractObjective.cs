using GTA;
using GTA.Native;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using DemagoScript.GUI.elements;

namespace DemagoScript
{
    abstract class AbstractObjective
    {
        protected string name = "";
        private bool inProgress = false;
        private float elapsedTime = 0;

        public string ObjectiveText { get; set; }
        public string AdviceText { get; set; }

        private List<Entity> entityCollector = new List<Entity>();

        public delegate void AccomplishedEvent(AbstractObjective sender, float elaspedTime);
        public delegate void FailedEvent(AbstractObjective sender, string reason);
        public delegate void StartedEvent(AbstractObjective sender);
        public delegate void EndedEvent(AbstractObjective sender);

        /// <summary>
        /// Called when user accomplish the objective.
        /// </summary>
        public event AccomplishedEvent OnAccomplished;

        /// <summary>
        /// Called when objective is started.
        /// </summary>
        public event StartedEvent OnStarted;

        /// <summary>
        /// Called when user fail an objective.
        /// </summary>
        public event FailedEvent OnFailed;

        /// <summary>
        /// Called when the objective is ended.
        /// </summary>
        public event EndedEvent OnEnded;

        ~AbstractObjective()
        {
            removeDestructibleElements(true);
        }

        /// <summary>
        /// Populate all elements that will have to be cleaned at the end
        /// </summary>
        public abstract void populateDestructibleElements();

        /// <summary>
        /// Remove all elements that have been created during the objective
        /// </summary>
        public abstract void removeDestructibleElements(bool removePhysicalElements = false);

        /// <summary>
        /// Return the objective's name
        /// </summary>
        public string getName() { return this.name; }

        /// <summary>
        /// Return elapsed time until start of current objective
        /// </summary>
        public float getElaspedTime()
        {
            return elapsedTime;
        }

        /// <summary>
        /// Return is in progress state
        /// </summary>
        public bool isInProgress()
        {
            return inProgress;
        }
        
        /// <summary>
        /// Initialize and start objective
        /// </summary>
        public virtual void start()
        {
            if (this.inProgress)
            {
                return;
            }

            elapsedTime = 0;

            populateDestructibleElements();

            this.inProgress = true;
            Tools.log("start " + getName());

            OnStarted?.Invoke(this);
        }

        /// <summary>
        /// Update objective datas
        /// </summary>
        public virtual bool update()
        {
            if ( !this.inProgress ) {
                return false;
            }

            if ( !Function.Call<bool>( Hash.IS_HUD_HIDDEN ) ) {
                if ( this.ObjectiveText != "" ) {
                    /*objectiveShadow.Caption = objectiveUIText.Caption = objectiveText;
                    objectiveShadow.Draw();
                    objectiveUIText.Draw();*/

                    /*
                    objectiveUIText = new UIResText( "", new Point( Game.ScreenResolution.Width / 2, Game.ScreenResolution.Height / 5 ), 0.7f, Color.WhiteSmoke, GTA.Font.ChaletComprimeCologne, UIResText.Alignment.Centered );
                    objectiveShadow = new UIResText( "", new Point( Game.ScreenResolution.Width / 2 + 2, Game.ScreenResolution.Height / 5 + 2 ), 0.7f, Color.Black, GTA.Font.ChaletComprimeCologne, UIResText.Alignment.Centered );

                    adviceUIText = new UIResText( "", new Point( Game.ScreenResolution.Width / 2, Game.ScreenResolution.Height / 5 + 40 ), 0.6f, Color.Green, GTA.Font.ChaletComprimeCologne, UIResText.Alignment.Centered );
                    adviceShadow = new UIResText( "", new Point( Game.ScreenResolution.Width / 2 + 2, Game.ScreenResolution.Height / 5 + 42 ), 0.6f, Color.Black, GTA.Font.ChaletComprimeCologne, UIResText.Alignment.Centered );
                    */

            Tools.log("drawText " + ObjectiveText);
                    // faire un GUIMananger.Instance.drawText(objectiveText)
                    UITextElement.drawText(this.ObjectiveText, Game.ScreenResolution.Width / 2, Game.ScreenResolution.Height / 5, 0.7f, true, GTA.Font.ChaletComprimeCologne, UIColor.WHITE);
                }

                if ( this.AdviceText != "" ) {
                    //faire un GUIManager.Instance.drawText(adviceText)
                    UITextElement.drawText(this.AdviceText, Game.ScreenResolution.Width / 2, Game.ScreenResolution.Height / 5 + 40, 0.7f, true, GTA.Font.ChaletComprimeCologne, UIColor.BLACK);
                }
            }

            //increment elapsed time timestamp with LastFrameTime
            elapsedTime += Game.LastFrameTime * 1000;
            return true;
        }

        /// <summary>
        /// Stop the objective
        /// </summary>
        /// <param name="reason">Reason of stop</param>
        public virtual void stop()
        {
            this.inProgress = false;
            this.elapsedTime = 0;
                        
            this.removeDestructibleElements();

            Tools.log("stop " + getName());

            OnEnded?.Invoke(this);
        }

        /// <summary>
        /// Fail the objective
        /// </summary>
        /// <param name="reason">Reason of fail</param>
        public virtual void fail(string reason)
        {
            this.stop();
            OnFailed?.Invoke(this, reason);
            Tools.log("fail "+ reason+ getName());
        }

        /// <summary>
        /// Accomplish the objective
        /// </summary>
        public virtual void accomplish()
        {
            this.stop();
            OnAccomplished?.Invoke(this, 0);

            Tools.log("accomplish "+getName());
        }
    }
}
