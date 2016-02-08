﻿using GTA;
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
            destination = position;
        }

        public override void populateDestructibleElements()
        {
            if (destinationBlip != null)
            {
                destinationBlip.Remove();
            }
            createDestinationBlip();

            finishCheckpoint = Function.Call<int>(Hash.CREATE_CHECKPOINT, 24, destination.X, destination.Y, 0.0f, destination.X, destination.Y, 0.0f, 2f, 254, 207, 12, 200, 40);
            Function.Call(Hash._SET_CHECKPOINT_ICON_RGBA, finishCheckpoint, 0, 0, 256, 60);
            Function.Call(Hash.SET_CHECKPOINT_CYLINDER_HEIGHT, finishCheckpoint, Tools.GetGroundedPosition(destination).Z + 30.0f, Tools.GetGroundedPosition(destination).Z + 30.0f, 30.0f);

        }

        public override void removeDestructibleElements(bool removePhysicalElements = false)
        {
            Tools.trace( "removePhysicalElements=" + removePhysicalElements, System.Reflection.MethodBase.GetCurrentMethod().Name, "GoToPosition" );

            if ( destinationBlip != null && destinationBlip.GetType() == typeof(Blip) && destinationBlip.Exists()) {
                destinationBlip.Remove();
                destinationBlip = null;
            }
            
            if (finishCheckpoint != -1) {
                Function.Call( Hash.DELETE_CHECKPOINT, finishCheckpoint );
                finishCheckpoint = -1;
            }
        }

        public void createDestinationBlip()
        {
            destinationBlip = World.CreateBlip( destination );
            destinationBlip.Sprite = BlipSprite.Crosshair;
            destinationBlip.Color = BlipColor.Green;
            destinationBlip.IsFlashing = true;
            destinationBlip.ShowRoute = true;
            destinationBlip.Position = destination;
        }

        public override bool update()
        {
            if ( !base.update() ) {
                return false;
            }

            Ped player = Game.Player.Character;

            if ( destination.DistanceTo( Game.Player.Character.Position ) < 1.4 ) {
                destinationBlip.Remove();
                accomplish();
            } else {
                if ( destinationBlip == null ) {
                    createDestinationBlip();
                }
                this.ObjectiveText = "Rejoins l'endroit indiqué par le GPS";
            }

            return true;
        }

    }
}
