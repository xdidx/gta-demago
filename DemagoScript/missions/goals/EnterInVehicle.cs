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
    class EnterInVehicle : Goal
    {
        private Vector3 position;
        private Vehicle vehicle;
        private VehicleHash vehicleHash;

        public EnterInVehicle(Vector3 position, VehicleHash vehicleHash)
        {
            this.position = position;
            this.vehicleHash = vehicleHash; 
        }

        protected override void doInitialization()
        {
            base.doInitialization();

            vehicle = World.CreateVehicle(vehicleHash, position);
            vehicle.AddBlip();
            vehicle.CurrentBlip.Sprite = BlipSprite.HelicopterAnimated;
            vehicle.CurrentBlip.Color = BlipColor.Green;
            vehicle.CurrentBlip.IsFlashing = true;
            vehicle.CurrentBlip.ShowRoute = true;
        }

        public override void update()
        {
            base.update();

            if (vehicle.IsDead || !vehicle.IsDriveable)
            {
                fail("Le véhicule est HS");
            }

            if (Game.Player.Character.CurrentVehicle == vehicle)
            {
                accomplish();
            }
            else if (vehicle.Position.DistanceTo(Game.Player.Character.Position) < 50)
            {
                if (vehicleHash == VehicleHash.Buzzard)
                {
                    setGoalText("Monte dans l'hélicoptère");
                }
                else
                {
                    setGoalText("Monte dans le véhicule");
                }
            }
            else
            {
                if (vehicleHash == VehicleHash.Buzzard)
                {
                    setGoalText("Rejoins l'hélicoptère pour t'enfuir");
                }
                else
                {
                    setGoalText("Rejoins le véhicule pour t'enfuir");
                }
            }
        }

        public override void clear(bool removePhysicalElements = false)
        {
            if (vehicle != null && vehicle.Exists())
            {
                if (vehicle.CurrentBlip != null)
                {
                    vehicle.CurrentBlip.Remove();
                }

                if (removePhysicalElements)
                {
                    vehicle.Delete();
                }
            }
        }
    }
}
