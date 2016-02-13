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
    abstract class AbstractObjective : Checkpoint
    {
        protected string name = "";

        private bool active = false;
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
        public virtual void populateDestructibleElements()
        {
            if (WantedLevel != -1)
                Game.Player.WantedLevel = WantedLevel;

            if (Health != -1)
                Game.Player.Character.Health = Health;

            if (Armor!= -1)
                Game.Player.Character.Armor = Armor;

            if (clockHour != -1)
                Tools.setClockTime(clockHour, Math.Max(clockTransitionTime, 0));

            if (PlayerPosition != Vector3.Zero)
                Tools.TeleportPlayer(PlayerPosition);

            if (Heading != -1)
                Game.Player.Character.Heading = Heading;

            if (Weather != Weather.Smog)
                World.Weather = Weather;

            foreach (KeyValuePair<Entity, Vector3> pair in entitiesCollector)
            {
                if (pair.Key != null && pair.Key.Exists())
                { 
                    pair.Key.Position = pair.Value;
                }
            }
        }

        /// <summary>
        /// Remove all elements that have been created during the objective
        /// </summary>
        public virtual void depopulateDestructibleElements(bool removePhysicalElements = false)
        {
            if (this.inProgress)
            {
                this.inProgress = false;
                this.active = false;
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
        protected virtual void accomplish()
        {
            var finalElapsedTime = elapsedTime;
            this.stop();
            OnAccomplished?.Invoke(this, finalElapsedTime);
        }
    }
}
