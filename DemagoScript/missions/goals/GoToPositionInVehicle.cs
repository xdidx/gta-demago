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
    class GoToPositionInVehicle : AbstractObjective
    {
        public delegate void FirstTimeOnVehicleEvent( AbstractObjective sender, Vehicle vehicle );
        public event FirstTimeOnVehicleEvent OnFirstTimeOnVehicle;

        private Vector3 destination;
        private Blip destinationBlip = null;
        private Vehicle vehicle = null;
        private VehicleHash vehicleHash;
        private Vector3 vehiclePosition;
        private Vector3 vehicleRotation;
        private bool teleportPlayerInVehicle;
        private bool alreadyMountedOnBike = false;
        private int destinationCheckpoint = -1;

        public GoToPositionInVehicle( Vector3 position, VehicleHash newVehicleHash, Vector3 newVehiclePosition, Vector3 newVehicleRotation, bool newTeleportPlayerInVehicle )
        {
            this.name = "Go to position in vehicle";

            destination = position;
            vehicleHash = newVehicleHash;
            vehiclePosition = newVehiclePosition;
            vehicleRotation = newVehicleRotation;
            teleportPlayerInVehicle = newTeleportPlayerInVehicle;
        }

        public GoToPositionInVehicle( Vector3 position )
        {
            this.name = "Go to position in vehicle";

            destination = position;
            vehiclePosition = Vector3.Zero;
            vehicleRotation = Vector3.Zero;
            teleportPlayerInVehicle = false;
        }

        public override void populateDestructibleElements()
        {
            alreadyMountedOnBike = false;

            if (!vehicleHasBeenGivenInConstruct())
            {
                vehicle = null;
                while (vehicle == null)
                {
                    vehicle = World.CreateVehicle(vehicleHash, vehiclePosition);
                }

                vehicle.Rotation = vehicleRotation;
                if (teleportPlayerInVehicle)
                    Game.Player.Character.SetIntoVehicle(vehicle, VehicleSeat.Driver);
            }
            else if (vehicle == null || !vehicle.Exists())
            {
                Tools.log("Le véhicule obligatoire n'existe plus", vehicle);
                Tools.log("Le véhicule static : ", Joe.bike);
                fail("Le véhicule obligatoire n'existe plus");
            }

            createDestinationBlip();

            destination = Tools.GetGroundedPosition(destination);
        }

        public override void removeDestructibleElements(bool removePhysicalElements = false)
        {
            if (vehicle != null && vehicle.Exists())
            {
                if (removePhysicalElements && vehicleHasBeenGivenInConstruct())
                    vehicle.Delete();

                if (vehicle.CurrentBlip != null)
                    vehicle.CurrentBlip.Remove();
            }

            if (destinationCheckpoint >= 0)
                Function.Call(Hash.DELETE_CHECKPOINT, destinationCheckpoint);

            if ( destinationBlip != null && destinationBlip.Exists() ) {
                this.removeDestinationBlip();
            }
        }

        /// <summary>
        /// Return true if the vehicle has been given in the constructor
        /// </summary>
        /// <returns>boolean</returns>
        public bool vehicleHasBeenGivenInConstruct()
        {
            /// If no position is set, the vehicle was given in constructor. If that's, we have to check if vehicle is available
            return ( vehiclePosition == Vector3.Zero );
        }

        /// <summary>
        /// Set vehicle
        /// </summary>
        /// <param name="newVehicle"></param>
        public void setVehicle(Vehicle newVehicle)
        {
            this.vehicle = newVehicle;
        }
        
        /// <summary>
        /// Create destinationblip
        /// </summary>
        public void createDestinationBlip()
        {
            destinationBlip = World.CreateBlip( destination );
            destinationBlip.Sprite = BlipSprite.Crosshair;
            destinationBlip.Color = BlipColor.Green;
            destinationBlip.IsFlashing = true;
            destinationBlip.ShowRoute = true;
            destinationBlip.Position = destination;
        }

        /// <summary>
        /// Remove destinationblip and set the value to null
        /// </summary>
        private void removeDestinationBlip()
        {
            if ( this.destinationBlip != null ) {
                this.destinationBlip.Remove();
            }
            this.destinationBlip = null;
        }

        public void createdDestinationCheckpoint()
        {
            destinationCheckpoint = Function.Call<int>(Hash.CREATE_CHECKPOINT, 24, destination.X, destination.Y, 0.0f, destination.X, destination.Y, 0.0f, 10f, 254, 207, 12, 200, 40);
            Function.Call(Hash._SET_CHECKPOINT_ICON_RGBA, destinationCheckpoint, 0, 0, 256, 60);
            Function.Call(Hash.SET_CHECKPOINT_CYLINDER_HEIGHT, destinationCheckpoint, destination.Z + 56.0f, destination.Z + 196.0f, 1000.0f);
        }

        public override bool update()
        {
            if ( !base.update() )
                return false;

            Ped player = Game.Player.Character;
            if (!vehicle.Exists() || !vehicle.IsDriveable || vehicle.IsDead || vehicle.IsOnFire ) {
                fail( "Le véhicule a été détruit" );
                return false;
            }

            if ( isArrived() ) {
                accomplish();
                return false;
            }

            if ( player.IsInVehicle() && player.CurrentVehicle == vehicle ) {
                if ( vehicle.CurrentBlip != null && vehicle.CurrentBlip.Exists() )
                    vehicle.CurrentBlip.Remove();

                if ( destinationBlip == null )
                    createDestinationBlip();

                if ( destinationCheckpoint < 0 )
                    createdDestinationCheckpoint();

                if ( !isArrived() )
                    ObjectiveText = "Rejoins l'endroit indiqué par le GPS";

                if ( !alreadyMountedOnBike )
                    OnFirstTimeOnVehicle?.Invoke( this, vehicle );
                alreadyMountedOnBike = true;
            } else {
                if ( player.IsInVehicle() )
                    player.Task.LeaveVehicle();

                if ( destinationBlip != null && destinationBlip.Exists() ) {
                    this.removeDestinationBlip();
                }

                if ( !vehicle.CurrentBlip.Exists() ) {
                    vehicle.AddBlip();
                    vehicle.CurrentBlip.Sprite = BlipSprite.PersonalVehicleCar;
                    vehicle.CurrentBlip.Color = BlipColor.Red;
                    vehicle.CurrentBlip.IsFlashing = true;
                    vehicle.CurrentBlip.ShowRoute = true;
                }

                ObjectiveText = "Rejoins ton véhicule pour continuer la mission";
            }
            return true;
        }

        public bool isArrived()
        {
            return destination.DistanceTo( Game.Player.Character.Position ) < 8 && Game.Player.Character.IsInVehicle();
        }

    }
}
