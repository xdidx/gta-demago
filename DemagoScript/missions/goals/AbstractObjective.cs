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

        private bool inProgress = false;
        private float elapsedTime = 0;
        private Vector3 playerPosition = Vector3.Zero;

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
        
        /// <summary>
        /// Populate all elements that will have to be cleaned at the end
        /// </summary>
        public abstract void populateDestructibleElements();

        /// <summary>
        /// Remove all elements that have been created during the objective
        /// </summary>
        public virtual void depopulateDestructibleElements(bool removePhysicalElements = false)
        {
            if (this.inProgress)
            {
                this.inProgress = false;
                this.elapsedTime = 0;
                depopulateDestructibleElements(true);
            }
        }

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
        /// Pause the objective
        /// </summary>
        public virtual void pause()
        {
            this.inProgress = false;
        }

        /// <summary> Useless????
        /// Play the objective
        /// </summary>
        public virtual void play()
        {
            this.inProgress = true;
        }

        public Vector3 getPlayerPosition()
        {
            return this.playerPosition;
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

            playerPosition = Game.Player.Character.Position;
            Tools.log( "saveCurrentPlayerPosition: " + playerPosition );

            populateDestructibleElements();

            this.inProgress = true;

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
                GUIManager.Instance.missionUI.setObjective(this.ObjectiveText);
                GUIManager.Instance.missionUI.setAdvice(this.AdviceText);
            }

            //increment elapsed time timestamp with LastFrameTime
            elapsedTime += Game.LastFrameTime * 1000;
            return true;
        }

        /// <summary>
        /// Stop the objective
        /// </summary>
        public virtual void stop( bool removePhysicalElements = false )
        {
            Tools.trace( getName() + " removePhysicalElements = " + removePhysicalElements, System.Reflection.MethodBase.GetCurrentMethod().Name, "AbstractObjective" );

            this.inProgress = false;
            this.elapsedTime = 0;

            this.depopulateDestructibleElements(removePhysicalElements);

            this.ObjectiveText = "";
            this.AdviceText = "";
            GUIManager.Instance.missionUI.hide();
            
            OnEnded?.Invoke(this);
        }

        /// <summary>
        /// Fail the objective
        /// </summary>
        /// <param name="reason">Reason of fail</param>
        public virtual void fail(string reason)
        {
            Tools.trace( getName() + " reason = " + reason, System.Reflection.MethodBase.GetCurrentMethod().Name, "AbstractObjective" );

            this.stop();
            OnFailed?.Invoke(this, reason);
        }

        /// <summary>
        /// Accomplish the objective
        /// </summary>
        public virtual void accomplish()
        {
            var finalElapsedTime = elapsedTime;
            this.stop();
            OnAccomplished?.Invoke(this, finalElapsedTime);
        }
    }
}
