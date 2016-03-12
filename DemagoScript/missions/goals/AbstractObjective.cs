using GTA;
using GTA.Native;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using DemagoScript.GUI.elements;
using DemagoScript.GUI;
using GTA.Math;

namespace DemagoScript
{
    abstract class AbstractObjective
    {
        protected string name = "";

        private bool active = false;
        private bool inProgress = false;
        private float elapsedTime = 0;

        public string ObjectiveText { get; set; }
        public string AdviceText { get; set; }

        public Checkpoint Checkpoint { get; set; }

        public delegate void AccomplishedEvent(AbstractObjective sender, float elaspedTime);
        public delegate void FailedEvent(AbstractObjective sender, string reason);
        public delegate void StartedEvent(AbstractObjective sender);
        public delegate void EndedEvent(AbstractObjective sender);
        public delegate void UpdatedEvent(AbstractObjective sender, float elaspedTime);

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

        /// <summary>
        /// Called when the objective is updated.
        /// </summary>
        public event UpdatedEvent OnUpdated;

        /// <summary>
        /// Populate all elements that will have to be cleaned at the end
        /// </summary>
        public virtual void populateDestructibleElements()
        {
            this.checkRequiredElements();

            if (Checkpoint != null)
            {
                Checkpoint.initialize();
            }
        }

        /// <summary>
        /// Check all requiered elements for all the objective duration
        /// </summary>
        public virtual void checkRequiredElements() { }

        /// <summary>
        /// Remove all elements that have been created during the objective
        /// </summary>
        public abstract void depopulateDestructibleElements(bool removePhysicalElements = false);

        /// <summary>
        /// Return the objective's name
        /// </summary>
        public string getName() { return this.name; }

        /// <summary>
        /// Return elapsed time until start of current objective in millisecond
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
        /// Return if objective isn't in progress but isn't stop yet
        /// </summary>
        public bool isWaiting()
        {
            return !inProgress && elapsedTime > 0;
        }

        /// <summary>
        /// Pause the objective
        /// </summary>
        public virtual void pause()
        {
            this.inProgress = false;
        }

        /// <summary>
        /// Play the objective
        /// </summary>
        public virtual void play()
        {
            this.depopulateDestructibleElements();
            this.populateDestructibleElements();
            this.inProgress = true;
        }

        /// <summary>
        /// Initialize and start objective
        /// </summary>
        public virtual void start()
        {
            Tools.log("Start " + getName());

            if (this.inProgress)
            {
                return;
            }
            elapsedTime = 0;

            this.play();

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

            OnUpdated?.Invoke(this, elapsedTime);

            if ( !Function.Call<bool>( Hash.IS_HUD_HIDDEN ) ) {
                GUIManager.Instance.missionUI.setObjective(this.ObjectiveText);
                GUIManager.Instance.missionUI.setAdvice(this.AdviceText);
            }

            elapsedTime += Game.LastFrameTime * 1000;
            return true;
        }

        /// <summary>
        /// Stop the objective
        /// </summary>
        public virtual void stop( bool removePhysicalElements = false )
        {
            Tools.log("Stop " + getName());

            this.elapsedTime = 0;

            this.depopulateDestructibleElements(removePhysicalElements);

            this.inProgress = false;

            this.ObjectiveText = "";
            this.AdviceText = "";
            
            OnEnded?.Invoke(this);
        }

        /// <summary>
        /// Fail the objective
        /// </summary>
        /// <param name="reason">Reason of fail</param>
        public virtual void fail(string reason)
        {
            Tools.log( "Fail " + getName() );

            this.stop();
            OnFailed?.Invoke(this, reason);
        }

        /// <summary>
        /// Accomplish the objective
        /// </summary>
        public virtual void accomplish()
        {
            Tools.log( "Accomplish " + getName() );

            var finalElapsedTime = elapsedTime;
            this.stop();
            OnAccomplished?.Invoke(this, finalElapsedTime);
        }
    }
}
