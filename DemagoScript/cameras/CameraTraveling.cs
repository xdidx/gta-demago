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
    class CameraTraveling : CameraShot
    {
        private Vector3 end_position = Vector3.Zero;

        public CameraTraveling(float duration, Vector3 start_position, Vector3 end_position, bool lookAtPlayer = false) : base(duration, start_position, lookAtPlayer)
        {
            this.end_position   = end_position;
        }

        /**
         * Boucle de rendu
         * @param progressTime (temps depuis lequel il est activé)
         */
        public override void update( float progressTime )
        {
            base.update(progressTime);

            if ( progressTime <= this.duration) {
                this.camera.Position = this.start_position + ( ( this.end_position - this.start_position ) * this.percentage );
            }
        }
    }
}
