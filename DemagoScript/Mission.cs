using GTA;
using GTA.Native;
using NativeUI;
using System;
using System.Collections.Generic;

namespace DemagoScript
{
    abstract class Mission
    {
        public delegate void MissionStartEvent(Mission sender);
        public delegate void MissionFailEvent(Mission sender, string reason);
        public delegate void MissionAccomplishedEvent(Mission sender, TimeSpan elaspedTime);

        private List<Goal> goals = new List<Goal>();

        private bool active = false;

        private bool over = false;
        private bool initialized = false;
        private bool failed = false;
        private DateTime startingTime;

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

        /**
         * Commence la mission
         */
        public void start()
        {
            OnMissionStart?.Invoke( this );

            this.reset();
            this.initialize();
            this.startingTime = DateTime.Now;
            
            // TODO: Mettre dans une fonction type doStarting()
            this.active = true;
            Game.Player.WantedLevel = 0;
        }

        /**
         * Reset les etapes à réaliser dans la mission
         */
        public void reset()
        {
            foreach ( Goal goal in this.goals ) {
                goal.reset();
            }
            this.initialized = false;
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
        public void stop(string reason = "")
        {
            if ( this.active ) {
                if ( reason != "" ) {
                    GTA.UI.Notify( "Mission arrêtée : " + reason );
                }
                
                this.active = false;
                this.reset();
            }
        }

        public virtual void accomplish()
        {
            TimeSpan elapsedTime = DateTime.Now - startingTime;
            OnMissionAccomplished?.Invoke(this, elapsedTime);

            this.active = false;
            this.over = true;
            this.clear(false);

            Game.Player.WantedLevel = 0;
            Game.Player.Character.Armor = 100;
            Game.Player.Character.Health = Game.Player.Character.MaxHealth;
        }

        public bool isInProgress()
        {
            return (this.active && !this.over && !this.failed);
        }

        public void fail(string reason = "")
        {
            if ( this.active ) {
                OnMissionFail?.Invoke( this, reason );
            }
            
            this.clear( false );
            this.reset();
            this.initialized = false;
            this.failed = true;
            this.active = false;
        }

        public void addGoal(Goal goal)
        {
            goal.OnGoalFail += (sender, reason) =>
            {
                fail(reason);
            };
            goals.Add( goal );
        }

        public virtual void update()
        {
            if ( !this.isInProgress() ) {
                this.stop();
                return;
            }

            this.isPlayerDeadOrArrested();

            bool waitingGoals = false;

            foreach ( Goal goal in goals ) {
                goal.update();
                if ( !goal.isOver() || goal.isFailed() ) {
                    waitingGoals = true;
                    break;
                }
            }
            
            if ( !waitingGoals ) {
                this.accomplish();
            }
        }

        /**
         * Indique si le joueur est mort ou s'il a été arrêté
         */
        private void isPlayerDeadOrArrested()
        {
            if ( Game.Player.IsDead ) {
                this.fail( "Vous êtes mort" );
            }

            if ( Function.Call<bool>( Hash.IS_PLAYER_BEING_ARRESTED, Game.Player, true ) ) {
                this.fail( "Vous vous êtes fait arrêté" );
            }
        }

        abstract public string getName();

        public virtual void clear(bool removePhysicalElements = false)
        {
            foreach (Goal goal in goals)
                goal.clear(removePhysicalElements);

            if (removePhysicalElements)
                goals.Clear();
        }

        public virtual UIMenuItem addStartItem(ref UIMenu menu)
        {
            var startItem = new UIMenuItem("Démarrer la mission");
            menu.AddItem(startItem);

            menu.OnItemSelect += (sender, item, index) =>
            {
                if (item == startItem)
                {
                    sender.Visible = false;
                }
            };

            return startItem;
        }

        public virtual void fillMenu(ref UIMenu menu) { }
    }
}
