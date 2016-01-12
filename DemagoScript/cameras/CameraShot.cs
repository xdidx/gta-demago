using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemagoScript
{
    class CameraShot
    {
        private Vector3 startPosition = Vector3.Zero;
        private Vector3 endPosition = Vector3.Zero;
        private float duration = 0;

        private Camera camera;

        public CameraShot(float newDuration, Vector3 startPosition) : this(newDuration, startPosition, Vector3.Zero) { }

        public CameraShot(float newDuration, Vector3 newStartPosition, Vector3 newEndPosition)
        {
            startPosition = newStartPosition;
            endPosition = newEndPosition;
            duration = newDuration;

            camera = World.CreateCamera(startPosition, Vector3.Zero, GameplayCamera.FieldOfView);
        }

        public void update(float progressTime)
        {
            if (duration > 0 && progressTime < duration && endPosition != Vector3.Zero)
            {
                float progressPercent = progressTime / duration;
                Vector3 newPosition = startPosition + ((endPosition - startPosition) * progressPercent);

                Tools.log("Update position" + Math.Round(progressPercent * 100) + "%", newPosition);

                camera.Position = startPosition + ((endPosition - startPosition) * progressPercent);

            }
        }

        public float getDuration()
        {
            return duration;
        }

        public Camera getCamera()
        {
            return camera;
        }

        public void pointAtPlayer(bool pointAtPlayer)
        {
            if (pointAtPlayer)
            {
                camera.PointAt(Game.Player.Character);
            }
            else
            {
                camera.StopPointing();
            }
        }
    }
}
