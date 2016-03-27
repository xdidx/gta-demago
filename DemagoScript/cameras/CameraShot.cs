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
        protected Vector3 start_position   = Vector3.Zero;
        protected float   duration       = 0;
        protected float percentage = 0;
        public bool WithFadeTransition { get; set; } = false;

        protected Camera camera;
        
        public CameraShot(float duration, Vector3 start_position)
        {
            this.start_position = start_position;
            this.duration       = duration;

            this.camera = World.CreateCamera(
                this.start_position,            // position
                Vector3.Zero,                   // rotation
                GameplayCamera.FieldOfView      // fov
            );
        }

        /**
         * Boucle de rendu
         * @param progressTime (temps depuis lequel il est activé)
         */
        public virtual void update( float progressTime )
        {
            this.percentage = progressTime / this.duration;
        }

        public float getDuration()
        {
            return this.duration;
        }

        public Camera getCamera()
        {
            return this.camera;
        }

        public void activateCamera()
        {
            World.RenderingCamera = this.camera;
        }

        public void destroyCamera()
        {
            this.camera.Destroy();
            World.RenderingCamera = null;
        }
        
        public void lookAt( Entity target )
        {
            this.camera.PointAt( target );
        }
    }
}
