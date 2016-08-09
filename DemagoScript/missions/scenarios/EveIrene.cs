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
        public static Vector3 eveHomePosition { get; } = new Vector3(1523f, 6331f, 24f);
        public static Vector3 carPositionAtHome { get; } = new Vector3(1537f, 6337f, 24f);
        public static Vector3 firstRestaurantPosition { get; } = new Vector3(1583f, 6448f, 25f); 
        #endregion

        public static Vehicle eveCar = null;
        private Random random = new Random();

        public EveIrene()
        {
            this.name = "Eve Irène";
            this.isActivated = true;
            this.introEnded = true;
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

            EveIrene.eveCar = World.CreateVehicle(VehicleHash.Kalahari, carPositionAtHome);

            ModelManager.Instance.setDemagoModel(DemagoModel.Eve);
            CameraShotsList.Instance.reset();
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
            GoToPositionInVehicle goToFirstRestaurantObjective = new GoToPositionInVehicle(firstRestaurantPosition);
            goToFirstRestaurantObjective.setVehicle(EveIrene.eveCar);
            goToFirstRestaurantObjective.Checkpoint = new Checkpoint();
            goToFirstRestaurantObjective.Checkpoint.addEntity(EveIrene.eveCar, carPositionAtHome, 90);
            goToFirstRestaurantObjective.Checkpoint.PlayerPosition = eveHomePosition;
            goToFirstRestaurantObjective.Checkpoint.setClockHour(9);
            goToFirstRestaurantObjective.Checkpoint.Armor = 100;
            goToFirstRestaurantObjective.Checkpoint.Weather = Weather.ExtraSunny;
            goToFirstRestaurantObjective.Checkpoint.WantedLevel = 0;
            goToFirstRestaurantObjective.Checkpoint.Heading = 75;

            VehicleHash[] trailersHashes = new VehicleHash[] { VehicleHash.Trailers, VehicleHash.Trailers2, VehicleHash.Trailers3 };
            LeaveVehicles leaveTrailersObjective = new LeaveVehicles(firstRestaurantPosition, trailersHashes, 2);
            leaveTrailersObjective.Checkpoint = new Checkpoint();
            leaveTrailersObjective.Checkpoint.Activable = true;
            leaveTrailersObjective.Checkpoint.PlayerPosition = firstRestaurantPosition;

            GoToPositionInVehicle goToHome = new GoToPositionInVehicle(carPositionAtHome);
            goToHome.setVehicle(EveIrene.eveCar);
            goToHome.Checkpoint = new Checkpoint();
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