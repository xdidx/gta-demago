using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemagoScript
{
    abstract class Checkpoint
    {
        private int wantedLevel = -1;
        private int health = -1;
        private int armor = -1;
        protected int clockHour = -1;
        protected float clockTransitionTime = -1;
        protected Dictionary<Entity, Vector3> entitiesCollector = new Dictionary<Entity, Vector3>();

        public Vector3 PlayerPosition { get; set; } = Vector3.Zero;
        public Weather Weather { get; set; } = Weather.Smog;
        public int Heading { get; set; } = -1;

        /// <summary>
        /// Health, minimum 0, maximum 5
        /// </summary>
        public int WantedLevel
        {
            get { return wantedLevel; }
            set { wantedLevel = Math.Min(Math.Max(value, 0), 5); }
        }

        /// <summary>
        /// Health, minimum 0
        /// </summary>
        public int Health
        {
            get { return health; }
            set { health = Math.Max(value, 0); }
        }

        /// <summary>
        /// Armor, minimum 0
        /// </summary>
        public int Armor
        {
            get { return armor; }
            set { armor = Math.Max(value, 0); }
        }

        /// <summary>
        /// Set hour on clock [transition in seconds] between 0 and 23
        /// </summary>
        public void setClockHour(int hour, float transitionTime = 0)
        {
            clockHour = Math.Min(Math.Max(hour, 0), 23);
            clockTransitionTime = Math.Max(transitionTime, 0);
        }

        public void addEntity(Entity entity, Vector3 position)
        {
            entitiesCollector.Add(entity, position);
        }
    }
}
