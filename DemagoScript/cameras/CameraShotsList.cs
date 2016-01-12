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
    class CameraShotsList
    {
        private List<CameraShot> cameraShots = new List<CameraShot>();
        private float maxDuration = 0;
        private int cameraShotIndex = 0;
        private float timeOnCurrentCamera = 0;
        private float timeOnShotsList = 0;
        private static CameraShotsList lastInstance = null;

        public CameraShotsList(List<CameraShot> newCameraShots, float newMaxDuration)
        {
            if (newCameraShots.Count > 1)
            {
                this.cameraShots = newCameraShots;
                this.maxDuration = newMaxDuration;
            }
        }

        public static void updateCameraShots()
        {
            if (lastInstance != null)
            {
                lastInstance.update();
            }
        }

        public static void stop()
        {
            if (lastInstance != null)
            {
                lastInstance.cameraShotIndex = 0;
                lastInstance.cameraShots.Clear();
            }
            Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 0, 1, 1);
        }

        public void update()
        {
            timeOnShotsList += Game.LastFrameTime;
            timeOnCurrentCamera += Game.LastFrameTime;

            bool cameraChange = false;
            CameraShot currentCameraShot = cameraShots[cameraShotIndex];
            if (timeOnCurrentCamera > currentCameraShot.getDuration())
            {
                cameraShotIndex++;
                cameraChange = true;
            }

            if (timeOnShotsList < maxDuration && cameraShotIndex < cameraShots.Count)
            {
                if (cameraChange)
                {
                    currentCameraShot = cameraShots[cameraShotIndex];
                    Function.Call(Hash.RENDER_SCRIPT_CAMS, 1, 0, currentCameraShot.getCamera().Handle, 0, 0);
                    World.RenderingCamera = currentCameraShot.getCamera();
                }

                currentCameraShot.update(timeOnCurrentCamera);
            }
            else
            {
                stop();
            }
        }
        
    }
}
