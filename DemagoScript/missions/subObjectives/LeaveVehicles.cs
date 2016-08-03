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
        private VehicleHash[] vehiclesHashes;

        public LeaveVehicles(Vector3 position, VehicleHash[] vehiclesHashes, int requiredNumber = 1, int radius = 30)
        {
            this.name = "Leave vehicles";

            this.position = position;
            this.vehiclesHashes = vehiclesHashes;
            this.requiredNumber = requiredNumber;
            this.radius = radius;
        }

        public override bool update()
        {
            if (!base.update())
            {
                return false;
            }

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
    }
}
