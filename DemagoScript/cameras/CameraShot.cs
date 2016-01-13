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
        private Vector3 start_position   = Vector3.Zero;
        private Vector3 end_position     = Vector3.Zero;
        private float   duration        = 0;
        private float   percentage      = 0;

        private Camera camera;

        public CameraShot(float duration, Vector3 start_position) : this(duration, start_position, Vector3.Zero) { }

        public CameraShot(float duration, Vector3 start_position, Vector3 end_position)
        {
            this.start_position = start_position;
            this.end_position   = end_position; // if == Vector3.Zero then its a fixed camera shot
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
        public void update( float progressTime )
        {
            // Plan fixe, pas la peine d'aller plus loin
            if ( this.duration <= 0 || this.end_position == Vector3.Zero ) {
                return;
            }
            
            // Pourcentage d'avancement
            this.percentage = progressTime / this.duration;

            // Si le plan est actif depuis moins de temps que son temps max alors
            if ( progressTime <= this.duration && this.end_position != Vector3.Zero ) {
                // On modifie la position de la camera
                this.camera.Position = this.start_position + ( ( this.end_position - this.start_position ) * this.percentage );
            }
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
            Function.Call( Hash.RENDER_SCRIPT_CAMS, 1, 0, this.camera.Handle, 0, 0 );
            World.RenderingCamera = this.camera;
        }
        
        public void lookAt( Entity target )
        {
            this.camera.PointAt( target );
        }

        public void lookAt( Vector3 target )
        {
            this.camera.Direction = target;
        }
    }
}
