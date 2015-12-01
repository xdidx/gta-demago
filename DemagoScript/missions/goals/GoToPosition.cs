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
    class GoToPosition : Goal
    {
        private Vector3 destination;
        private Blip destinationBlip = null;
        private int finishCheckpoint = -1;

        public GoToPosition(Vector3 position)
        {
            destination = position;
        }

        public override bool initialize()
        {
            if (!base.initialize())
            {
                return false;
            }
           
            if (destinationBlip != null)
            {
                destinationBlip.Remove();
            }
            createDestinationBlip();

            finishCheckpoint = Function.Call<int>(Hash.CREATE_CHECKPOINT, 24, destination.X, destination.Y, Tools.GetGroundedPosition(destination).Z, destination.X, destination.Y, Tools.GetGroundedPosition(destination).Z, 2f, 254, 207, 12, 100, 40);
            Function.Call(Hash._SET_CHECKPOINT_ICON_RGBA, finishCheckpoint, 0, 0, 256, 60);

            return true;
        }

        public void createDestinationBlip()
        {
            destinationBlip = World.CreateBlip(destination);
            destinationBlip.Sprite = BlipSprite.Crosshair;
            destinationBlip.Color = BlipColor.Green;
            destinationBlip.IsFlashing = true;
            destinationBlip.ShowRoute = true;
            destinationBlip.Position = destination;
        }

        public override bool update()
        {
            if (!base.update())
            {
                return false;
            }

            Ped player = Game.Player.Character;

            if (destination.DistanceTo(Game.Player.Character.Position) < 1.4)
            {
                destinationBlip.Remove();
                accomplish();
                return false;
            }
            else
            {
                if (destinationBlip == null)
                {
                    createDestinationBlip();
                }
                setGoalText("Rejoins l'endroit indiqué par le GPS");
            }

            return true;
        }

        public override void clear(bool removePhysicalElements = false)
        {
            if (destinationBlip != null && destinationBlip.Exists())
                destinationBlip.Remove();

            if (finishCheckpoint >= 0)
                Function.Call(Hash.DELETE_CHECKPOINT, finishCheckpoint);
        }
    }
}
