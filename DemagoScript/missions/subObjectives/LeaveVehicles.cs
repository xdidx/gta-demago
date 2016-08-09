using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemagoScript
{
    class LeaveVehicles : AbstractObjective
    {
        private Vector3 position;
        private int radius;
        private int requiredNumber;
        private int blippedVehiclesCount;
        private VehicleHash[] vehiclesHashes = { };
        private Vehicle[] markedVehicles = { };

        public LeaveVehicles(Vector3 position, VehicleHash[] vehiclesHashes, int requiredNumber = 1, int radius = 30)
        {
            this.name = "Leave vehicles";

            this.position = position;
            this.vehiclesHashes = vehiclesHashes;
            this.requiredNumber = requiredNumber;
            this.radius = radius;
        }


        protected override void populateDestructibleElements()
        {
            addBlipOnAvailableVehicles();
        }

        private void addBlipOnAvailableVehicles()
        {
            if (vehiclesHashes.Length == 0)
                return;

            Vehicle[] vehicles = World.GetAllVehicles();
            foreach (Vehicle vehicle in vehicles)
            {
                float distance = vehicle.Position.DistanceTo(Game.Player.Character.Position);
                if (distance > 200 && distance < 1000 && vehicle.IsSeatFree(VehicleSeat.Driver))
                {
                    Vector3 position = vehicle.Position;
                    Vector3 rotation = vehicle.Rotation;
                    vehicle.Delete();

                    Vehicle truck = World.CreateVehicle(VehicleHash.Phantom, position);
                    truck.Rotation = rotation;

                    truck.AddBlip();
                    truck.CurrentBlip.Sprite = BlipSprite.VehicleDeathmatch;
                    truck.CurrentBlip.Color = BlipColor.Blue;
                    truck.CurrentBlip.IsFlashing = true;
                    truck.CurrentBlip.ShowRoute = false;
                    this.markedVehicles[this.markedVehicles.Length] = truck;

                    Vehicle trailer = World.CreateVehicle(VehicleHash.Trailers, position);
                    rotation.Y = 90;
                    trailer.Rotation = rotation;

                    truck.CreatePedOnSeat(VehicleSeat.Driver, PedHash.DeadHooker);
                    Ped driver = truck.GetPedOnSeat(VehicleSeat.Driver);
                    driver.DrivingStyle = DrivingStyle.Normal;

                    blippedVehiclesCount++;
                    Tools.log("random blippedVehiclesCount : " + blippedVehiclesCount);
                }

                if (blippedVehiclesCount >= 3)
                {
                    break;
                }
            }
        }

        public override bool update()
        {
            if (!base.update())
            {
                return false;
            }

            ObjectiveText = "Dépose 3 remorques de camion devant le restaurant";

            int requiredNumberIndex = this.requiredNumber;
            Vehicle[] vehicles = World.GetNearbyVehicles(position, this.radius);
            foreach (Vehicle vehicle in vehicles)
            {
                if (Array.Exists(vehiclesHashes, hash => (int)hash == vehicle.GetHashCode()))
                {
                    vehicle.IsPersistent = true;
                    requiredNumberIndex--;
                    if (requiredNumberIndex == 0)
                    {
                        this.accomplish();
                        break;
                    }
                }
            }

            return true;
        }

        protected override void depopulateDestructibleElements(bool removePhysicalElements = false)
        {
        }
    }
}
