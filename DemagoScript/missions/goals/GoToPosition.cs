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
    class GoToPosition : AbstractObjective
    {
        private Vector3 destination;
        private Blip destinationBlip = null;
        private int finishCheckpoint = -1;

        public GoToPosition( Vector3 position )
        {
            this.name = "Go to position";
            this.destination = position;
        }

        /// <summary>
        /// Populate objective elements
        /// </summary>
        public override void populateDestructibleElements()
        {
            destinationBlip = World.CreateBlip(destination);
            destinationBlip.Sprite = BlipSprite.Crosshair;
            destinationBlip.Color = BlipColor.Green;
            destinationBlip.IsFlashing = true;
            destinationBlip.ShowRoute = true;
            destinationBlip.Position = destination;

            this.finishCheckpoint = Function.Call<int>(Hash.CREATE_CHECKPOINT, 24, this.destination.X, this.destination.Y, 0.0f, this.destination.X, this.destination.Y, 0.0f, 2f, 254, 207, 12, 200, 40);
            Function.Call(Hash._SET_CHECKPOINT_ICON_RGBA, this.finishCheckpoint, 0, 0, 256, 60);
            Function.Call(Hash.SET_CHECKPOINT_CYLINDER_HEIGHT, this.finishCheckpoint, Tools.GetGroundedPosition( this.destination ).Z + 30.0f, Tools.GetGroundedPosition( this.destination ).Z + 30.0f, 30.0f);
        }

        public override void removeDestructibleElements(bool removePhysicalElements = false)
        {
            Tools.trace( "removePhysicalElements=" + removePhysicalElements, System.Reflection.MethodBase.GetCurrentMethod().Name, "GoToPosition" );

            if ( this.destinationBlip != null) {
                this.destinationBlip.Remove();
                this.destinationBlip = null;
            }
            
            if ( this.finishCheckpoint != -1) {
                Function.Call( Hash.DELETE_CHECKPOINT, this.finishCheckpoint );
                this.finishCheckpoint = -1;
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <returns></returns>
        public override bool update()
        {
            if ( !base.update() ) {
                return false;
            }

            Ped player = Game.Player.Character;

            if ( this.destination.DistanceTo( Game.Player.Character.Position ) < 1.4 ) {
                this.accomplish();
            } else {
                this.ObjectiveText = "Rejoins l'endroit indiqué par le GPS";
            }

            return true;
        }

    }
}
