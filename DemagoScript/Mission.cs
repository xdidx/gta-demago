using DemagoScript.GUI;
using DemagoScript.GUI.popup;
using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
using System;
using System.Collections.Generic;

namespace DemagoScript
{
    abstract class Mission
    {
        public delegate void MissionStartEvent( Mission sender );
        public delegate void MissionFailEvent(Mission sender, string reason);
        public delegate void MissionStopEvent(Mission sender, string reason);
        public delegate void MissionOverEvent(Mission sender, string reason);
        public delegate void MissionAccomplishedEvent( Mission sender, TimeSpan elaspedTime );

        protected List<Goal> goals = new List<Goal>();
        private List<Checkpoint> checkpoints = new List<Checkpoint>();

        private Vector3 playerPositionBeforeMission = Vector3.Zero;
        private int lastCheckpointIndex = 0;
        private bool active = false;
        private bool over = false;
        private bool initialized = false;
        private bool failed = false;
        private DateTime startMissionTime;

        /// <summary>
        /// Called when user start the mission.
        /// </summary>
        public event MissionStartEvent OnMissionStart;

        /// <summary>
        /// Called when all goals are accomplished
        /// </summary>
        public event MissionAccomplishedEvent OnMissionAccomplished;

        /// <summary>
        /// Called when a important event stop the mission
        /// </summary>
        public event MissionFailEvent OnMissionFail;

        /// <summary>
        /// Called when mission is voluntarily stopped 
        /// </summary>
        public event MissionStopEvent OnMissionStop;

        /// <summary>
        /// Called when mission is over, by fail, stop or whatever
        /// </summary>
        public event MissionOverEvent OnMissionOver;

        /**
         * Commence la mission
         */
        public void start()
        {
            OnMissionStart?.Invoke( this );

            if (playerPositionBeforeMission == Vector3.Zero)
            {
                playerPositionBeforeMission = Game.Player.Character.Position;
            }

            this.reset();
            clear(true);
            this.initialize();

            this.active = true;
            this.startMissionTime = DateTime.Now;
            Game.Player.WantedLevel = 0;
        }

        /**
         * Reset les etapes à réaliser dans la mission
         */
        public void reset()
        {
            this.initialized = false;

            foreach ( Goal goal in this.goals ) {
                goal.reset();
            }
        }

        // DO NOT TOUCH
        private void initialize()
        {
            if ( this.initialized ) {
                return;
            }

            this.doInitialization();

            initialized = true;
        }

        // TOUCH HERE INSTEAD
        protected virtual void doInitialization()
        {
            this.clear( true );
            this.over = false;
            this.failed = false;
        }

        /**
         * Stop la mission en cours
         */
        public void stop( string reason = "" )
        {
            if ( this.active )
            {
                if ( reason != "" ) {
                    GTA.UI.Notify( "Mission arrêtée : " + reason );
                }

                this.failed = false;
                OnMissionStop?.Invoke(this, reason);
                ends(reason);
            }
        }

        public void fail(string reason = "")
        {
            if (this.active)
            {
                if (reason != "")
                {
                    GTA.UI.Notify("Mission échouée : " + reason);
                }

                this.failed = true;
                OnMissionFail?.Invoke(this, reason);
                ends(reason);
            }
        }

        private void ends(string reason = "")
        {
            OnMissionOver?.Invoke(this, reason);

            this.clear(false);
            this.reset();

            this.active = false;
        }

        public virtual void accomplish()
        {
            TimeSpan elapsedTime = DateTime.Now - startMissionTime;
            OnMissionAccomplished?.Invoke( this, elapsedTime );

            this.active = false;
            this.over = true;
            this.clear( false );

            Game.Player.WantedLevel = 0;
            Game.Player.Character.Armor = 100;
            Game.Player.Character.Health = Game.Player.Character.MaxHealth;
        }

        public bool isInProgress()
        {
            return ( this.active && !this.over && !this.failed );
        }

        public void addGoal(Goal goalToAdd)
        {
            goalToAdd.OnGoalFail += (sender, reason) => {
                fail(reason);
            };
            goalToAdd.OnGoalAccomplished += (sender, elaspedTime) => {
                foreach (Checkpoint checkpoint in checkpoints)
                {
                    if (goalToAdd == checkpoint.getGoalToLaunch())
                    {
                        lastCheckpointIndex = checkpoints.IndexOf(checkpoint);
                        break;
                    }
                }
            }; 
            goals.Add(goalToAdd);
        }

        public void addCheckpoint(Checkpoint checkpoint)
        {
            checkpoints.Add(checkpoint);
        }

        public virtual void update()
        {
            this.isPlayerDeadOrArrested();

            if ( !isInProgress() ) {
                this.stop();
                return;
            }
            
            bool waitingGoals = false;
            foreach (Goal goal in goals)
            {
                if (goal.update())
                {
                    waitingGoals = true;
                    break;
                }
                if (goal.isFailed())
                {
                    waitingGoals = true;
                    break;
                }
            }

            if ( !waitingGoals )
            {
                this.accomplish();
            }
        }

        /**
         * Indique si le joueur est mort ou s'il a été arrêté
         */
        private void isPlayerDeadOrArrested()
        {
            if (Game.Player.IsDead)
            {
                this.fail("Vous êtes mort");
            }

            if (Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED, Game.Player, true))
            {
                this.fail("Vous vous êtes fait arrêté");
            }
        }

        public void loadLastCheckpoint()
        {
            Tools.log("load last checkpoint "+ lastCheckpointIndex);

            if (lastCheckpointIndex > 0)
            {
                if (lastCheckpointIndex >= checkpoints.Count)
                {
                    lastCheckpointIndex = checkpoints.Count - 1;
                }

                Checkpoint checkpointToStart = checkpoints[lastCheckpointIndex];

                clear(true, true);

                foreach (Goal goal in goals)
                {
                    if (goal == checkpointToStart.getGoalToLaunch())
                    {
                        break;
                    }
                    else
                    {
                        goal.accomplish();
                    }
                }

                checkpointToStart.start();

                this.active = true;
                this.initialized = true;
            }
            else
            {
                start();
            }
        }

        abstract public string getName();

        public virtual void clear( bool removePhysicalElements = false, bool keepGoalsList = false)
        {
            foreach ( Goal goal in goals )
                goal.clear( removePhysicalElements );

            if (keepGoalsList)
            {
                if (removePhysicalElements)
                    goals.Clear();
            }
        }

        public virtual UIMenuItem addStartItem( ref UIMenu menu )
        {
            var startItem = new UIMenuItem( "Démarrer la mission" );
            menu.AddItem( startItem );

            menu.OnItemSelect += ( sender, item, index ) => {
                if ( item == startItem ) {
                    sender.Visible = false;
                }
            };

            return startItem;
        }

        public virtual void fillMenu(ref UIMenu menu) { }
        public virtual void setPause(bool isPaused)
        {
            foreach (Goal goal in goals)
            {
                goal.setPause(isPaused);
            }
        }
    }
}
