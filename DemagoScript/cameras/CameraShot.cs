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
        private bool pointAtPlayer = false;
        private Camera camera = null;
        private Entity target = null;

        public CameraShot(float newDuration, Vector3 startPosition) : this(newDuration, startPosition, Vector3.Zero) { }

        public CameraShot(float newDuration, Vector3 newStartPosition, Vector3 newEndPosition)
        {
            startPosition = newStartPosition;
            endPosition = newEndPosition;
            duration = newDuration;
        }

        public void update(float progressTime)
        {
            if (camera == null)
            {
                camera = World.CreateCamera(startPosition, Vector3.Zero, GameplayCamera.FieldOfView);
                camera.Position = startPosition;
                if (pointAtPlayer)
                {
                    camera.PointAt(Game.Player.Character);
                }
                else if (target != null && target.Exists())
                {
                    camera.PointAt(target);
                }
                Function.Call(Hash.RENDER_SCRIPT_CAMS, 1, 0, camera.Handle, 0, 0);
                World.RenderingCamera = camera;
            }

            if (duration > 0 && progressTime < duration && endPosition != Vector3.Zero)
            {
                float progressPercent = progressTime / duration;
                Vector3 newPosition = startPosition + ((endPosition - startPosition) * progressPercent);
                camera.Position = newPosition;
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

        public void setPointAtPlayer(bool newPointAtPlayer)
        {
            pointAtPlayer = newPointAtPlayer;
        }

        public void setTarget(Entity newTarget)
        {
            target = newTarget;
        }
    }
}
