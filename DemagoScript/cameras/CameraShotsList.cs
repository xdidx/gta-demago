using DemagoScript.GUI;
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
        private List<CameraShot> sequence       = new List<CameraShot>();
        private float sequence_elapsed_time     = 0;
        private float sequence_total_duration   = 0;
        private int   current_index             = 0;
        private float current_elapsed_time      = 0;
        private CameraShot current_element      = null;

        private static CameraShotsList instance = null;

        private CameraShotsList() { }

        public static CameraShotsList Instance
        {
            get {
                if ( CameraShotsList.instance == null ) {
                    CameraShotsList.instance = new CameraShotsList();
                }
                return CameraShotsList.instance;
            }
        }
        
        // Init
        public void initialize( List<CameraShot> sequence, float sequence_duration = 0 )
        {
            this.reset();

            if ( sequence == null || sequence.Count <= 0 ) {
                Tools.log( "CameraShotsList: sequence list cannot be empty or null." );
                return;
            }

            Function.Call(Hash.DISPLAY_RADAR, false);
            Function.Call(Hash.DISPLAY_HUD, false);
            GUIManager.Instance.missionUI.hide();

            this.sequence = sequence;
            this.sequence_total_duration = sequence_duration;
            this.current_element = this.sequence[this.current_index];
        }

        // Update
        public void update()
        {
            // Si on a pas d'élément courant alors on sort
            if ( this.current_element == null ) { // CAD qu'on a pas encore initialisé
                return;
            }

            // Si l'element en cours dure plus longtemps que prévu
            if ( this.current_elapsed_time > this.current_element.getDuration() ) {

                // On passe au suivant
                this.next();

            }

            // Si la sequence est en cours et que l'index n'est pas trop grand
            if ((this.sequence_total_duration == 0 || this.sequence_elapsed_time < this.sequence_total_duration) && this.current_index < this.sequence.Count )
            { 
                // On recupere le current element
                this.current_element = this.sequence[this.current_index];

                // Changement de CameraShot
                if ( this.current_elapsed_time == 0 ) {
                    this.current_element.activateCamera();
                }

                // Mise à jour du CameraShot
                this.current_element.update( this.current_elapsed_time );

            } else { // Sinon (si la sequence n'est plus en cours)

                // On stop la sequence
                this.reset();

            }

            // Mise à jours des temps
            float time = Game.LastFrameTime * 1000;
            this.current_elapsed_time += time; // element
            this.sequence_elapsed_time += time; // sequence
        }

        // Element suivant dans la sequence
        public void next()
        {
            this.current_index++;
            this.current_elapsed_time = 0;
        }

        // On reset la sequence
        public void reset()
        {
            Function.Call(Hash.DISPLAY_RADAR, true);
            Function.Call(Hash.DISPLAY_HUD, true);
            GUIManager.Instance.missionUI.show();
            
            this.sequence = null;
            this.sequence_elapsed_time = 0;
            this.sequence_total_duration = 0;
            this.current_index = 0;
            this.current_elapsed_time = 0;
            this.current_element = null;
            World.RenderingCamera = null;
        }
    }
}
