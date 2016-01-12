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
                lastInstance = this;
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
                lastInstance = null;
                Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 0, 1, 1);
            }
        }

        public void update()
        {
            CameraShot currentCameraShot = cameraShots[cameraShotIndex];
            if (timeOnCurrentCamera > currentCameraShot.getDuration())
            {
                cameraShotIndex++;
                timeOnCurrentCamera = 0;
            }
            
            if (timeOnShotsList < maxDuration && cameraShotIndex < cameraShots.Count)
            {
                if (timeOnShotsList == 0 || currentCameraShot != cameraShots[cameraShotIndex])
                {
                    currentCameraShot = cameraShots[cameraShotIndex];
                    Tools.log("cameraChange, camera " + cameraShotIndex + " et " + timeOnShotsList + " < " + maxDuration);
                    Function.Call(Hash.RENDER_SCRIPT_CAMS, 1, 0, currentCameraShot.getCamera().Handle, 0, 0);
                }

                currentCameraShot.update(timeOnCurrentCamera);
            }
            else
            {
                stop();
            }

            timeOnShotsList += Game.LastFrameTime * 1000;
            timeOnCurrentCamera += Game.LastFrameTime * 1000;
        }
        
    }
}
