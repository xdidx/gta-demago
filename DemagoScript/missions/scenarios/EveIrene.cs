using DemagoScript.GUI;
using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemagoScript
{
    class EveIrene : Mission
    {
        #region Positions
        public static Vector3 eveHomePosition { get; } = new Vector3(1550f, 6323f, 46f);
        public static Vector3 carPositionAtHome { get; } = new Vector3(1552f, 6325f, 46f);
        public static Vector3 firstRestaurantPosition { get; } = new Vector3(1584f, 6420f, 46f); 
        #endregion

        public static Vehicle eveCar = null;
        private Random random = new Random();

        #region Regeneration variables
        private bool bikeRegen = false;
        private int playerLifeUpCounter = 0;
        #endregion
        
        #region Intro variables
        private bool playerDown = false;
        private bool playerWalked = false;
        private bool playerMoved = false;
        #endregion

        public EveIrene()
        {
            this.name = "Eve Irène";
            this.isActivated = true;
        }

        public List<Ped> getPedsListByName(string entitiesListName)
        {
            return new List<Ped>();
        }

        public Entity getEntityByName(string entityName)
        {
            return null;
        }

        public void createEntities()
        {
        }

        public override void checkRequiredElements()
        {
            base.checkRequiredElements();
            
            ModelManager.Instance.setDemagoModel(DemagoModel.Eve);
            CameraShotsList.Instance.reset();
            AudioManager.Instance.FilesSubFolder = @"eve\eve";
        }

        protected override void populateDestructibleElements()
        {
            base.populateDestructibleElements();

            this.createEntities();
            this.createAndAddObjectives();
        }

        private void createAndAddObjectives()
        {
            #region Objectives 
            GoToPosition goToFirstRestaurantObjective = new GoToPosition(eveHomePosition);
            goToFirstRestaurantObjective.Checkpoint = new Checkpoint();
            goToFirstRestaurantObjective.Checkpoint.addEntity(EveIrene.eveCar, carPositionAtHome, 0);
            goToFirstRestaurantObjective.Checkpoint.PlayerPosition = eveHomePosition;
            goToFirstRestaurantObjective.Checkpoint.setClockHour(9);
            goToFirstRestaurantObjective.Checkpoint.Armor = 100;
            goToFirstRestaurantObjective.Checkpoint.Weather = Weather.ExtraSunny;
            goToFirstRestaurantObjective.Checkpoint.WantedLevel = 0;
            goToFirstRestaurantObjective.Checkpoint.Heading = 0;

            VehicleHash[] trailersHashes = new VehicleHash[] { VehicleHash.Trailers, VehicleHash.Trailers2, VehicleHash.Trailers3 };
            LeaveVehicles leaveTrailersObjective = new LeaveVehicles(firstRestaurantPosition, trailersHashes, 2);
            leaveTrailersObjective.Checkpoint = new Checkpoint();
            leaveTrailersObjective.Checkpoint.Activable = true;
            leaveTrailersObjective.Checkpoint.PlayerPosition = firstRestaurantPosition;

            GoToPositionInVehicle goToHome = new GoToPositionInVehicle(eveHomePosition);
            goToHome.Checkpoint.Armor = 100;
            goToHome.Checkpoint.WantedLevel = 2;
            goToHome.Checkpoint.Weather = Weather.Clouds;

            addObjective(goToFirstRestaurantObjective);
            addObjective(leaveTrailersObjective);
            addObjective(goToHome);

            #endregion

        }

        protected override void depopulateDestructibleElements(bool removePhysicalElements = false)
        {
            base.depopulateDestructibleElements(removePhysicalElements);

            if (EveIrene.eveCar != null && EveIrene.eveCar.Exists())
            {
                EveIrene.eveCar.MarkAsNoLongerNeeded();

                if (removePhysicalElements)
                {
                    EveIrene.eveCar.Delete();
                    EveIrene.eveCar = null;
                }
            }
        }
        
        public override bool update()
        {
            if (!base.update())
            {
                return false;
            }

            Ped player = Game.Player.Character;

            Ped[] nearbyPeds = World.GetNearbyPeds(Game.Player.Character.Position, 30);
            foreach (Ped ped in nearbyPeds)
            {
                if (!ped.IsHuman && ped.IsDead && ped.IsOnlyDamagedByPlayer)
                {
                    this.fail("Tu as tué un animal...");
                }
            }

            return true;
        }
    }
}