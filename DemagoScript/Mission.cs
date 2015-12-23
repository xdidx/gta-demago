using GTA;
using GTA.Native;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public virtual bool initialize()
        {
            if (initialized)
                return false;

            clear(true);

            initialized = true;
            over = false;
            failed = false;

            return true;
        }

        public void start()
        {
            OnMissionStart?.Invoke(this);

            reset();

            initialized = false;
            initialize();

            active = true;
            startMissionTime = DateTime.Now;
            Game.Player.WantedLevel = 0;
        }

        public void reset()
        {
            foreach (Goal goal in goals)
                goal.clear(false);
        }

        public void stop(string reason = "")
        {
            if (active)
            {
                if (reason != "")
                    GTA.UI.Notify("Mission arrêtée : " + reason);

                active = false;
                reset();
            }
        }

        public virtual void accomplish()
        {
            TimeSpan elapsedTime = DateTime.Now - startMissionTime;
            OnMissionAccomplished?.Invoke(this, elapsedTime);

            active = false;
            over = true;

            clear(false);

            Game.Player.WantedLevel = 0;
            Game.Player.Character.Armor = 100;
            Game.Player.Character.Health = Game.Player.Character.MaxHealth;
        }

        public bool isInProgress()
        {
            return active && !over && !failed;
        }

        public void fail(string reason = "")
        {
            if (active)
                OnMissionFail?.Invoke(this, reason);

            clear(false);
            reset();
            initialized = false;
            failed = true;
            active = false;
        }

        public void addGoal(Goal goal)
        {
            goal.OnGoalFail += (sender, reason) =>
            {
                fail(reason);
            };
            goals.Add(goal);
        }

        public virtual bool update()
        {
            if (!isInProgress())
            {
                active = false;
                return false;
            }

            initialize();
            
            if (Game.Player.IsDead)
            {
                fail("Vous êtes mort");
            }

            if (Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED, Game.Player, true))
            {
                fail("Vous vous êtes fait arrêté");
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
            
            if (!waitingGoals)
                accomplish();

            return true;
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
        public virtual void setPause(bool isPaused)
        {
            foreach (Goal goal in goals)
            {
                goal.setPause(isPaused);
            }
        }
    }
}
