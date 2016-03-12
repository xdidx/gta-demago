using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemagoScript
{
    class Checkpoint
    {
        private int wantedLevel = -1;
        private int health = -1;
        private int armor = -1;
        protected int clockHour = -1;
        protected float clockTransitionTime = -1;
        protected Dictionary<Entity, Vector3> entitiesCollectorPositions = new Dictionary<Entity, Vector3>();
        protected Dictionary<Entity, int> entitiesCollectorHeadings = new Dictionary<Entity, int>();

        public Vector3 PlayerPosition { get; set; } = Vector3.Zero;
        public Weather Weather { get; set; } = Weather.Smog;
        public int Heading { get; set; } = -1;

        public bool Activable { get; set; } = false;

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

        public void addEntity(Entity entity, Vector3 position, int heading = -1)
        {
            entitiesCollectorPositions.Add(entity, position);
            entitiesCollectorHeadings.Add(entity, heading);
        }

        public Vector3 getEntityPosition(Entity entity)
        {
            return this.entitiesCollectorPositions[entity];
        }

        public int getEntityHeading(Entity entity)
        {
            return this.entitiesCollectorHeadings[entity];
        }

        public void initialize()
        {
            if (WantedLevel != -1)
                Game.Player.WantedLevel = WantedLevel;

            if (Health != -1)
                Game.Player.Character.Health = Health;

            if (Armor != -1)
                Game.Player.Character.Armor = Armor;

            if (clockHour != -1)
                Tools.setClockTime(clockHour, Math.Max(clockTransitionTime, 0));

            if (PlayerPosition != Vector3.Zero && PlayerPosition.DistanceTo(Game.Player.Character.Position) > 30)
            {
                teleportPlayerToCheckpoint();
            }

            if (Heading != -1)
                Game.Player.Character.Heading = Heading;

            if (Weather != Weather.Smog)
                World.Weather = Weather;

            foreach (KeyValuePair<Entity, Vector3> pair in entitiesCollectorPositions)
            {
                Entity entity = pair.Key;
                if (entity != null && entity.Exists() && entity.Position.DistanceTo(pair.Value) > 30)
                {
                    entity.Position = pair.Value;
                    if (entitiesCollectorHeadings[entity] != -1)
                    {
                        entity.Heading = entitiesCollectorHeadings[entity];
                    }
                }
            }
        }

        public void teleportPlayerToCheckpoint()
        {
            Timer safeThing = new Timer( 5000 );
            safeThing.OnTimerUpdate += ( elapsedMilliseconds, elapsedPourcent ) =>
            {
                if ( PlayerPosition.DistanceTo( Game.Player.Character.Position ) > 100 ) {
                    Tools.TeleportPlayer( PlayerPosition );
                }
            };
        }

        public void load()
        {
            if (WantedLevel != -1)
                Game.Player.WantedLevel = WantedLevel;

            if (Health != -1)
                Game.Player.Character.Health = Health;

            if (Armor != -1)
                Game.Player.Character.Armor = Armor;

            if (clockHour != -1)
                Tools.setClockTime(clockHour, Math.Max(clockTransitionTime, 0));

            if (Heading != -1)
                Game.Player.Character.Heading = Heading;

            if (Weather != Weather.Smog)
                World.Weather = Weather;

            teleportPlayerToCheckpoint();
        }
    }
}
