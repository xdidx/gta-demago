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
        private List<CameraShot> sequence = new List<CameraShot>();
        private CameraShot current_element = null;
        
        private float sequenceElapsedTime = 0; 
        private float sequence_total_duration = 0;
        private int current_index = -1;
        private float current_element_elapsed_time = 0;

        private Timer transitionTimer = null;
        private float fadeTransitionDuration = 300f;

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
            this.next();
        }

        // Update
        public void update()
        {
            if (this.isInProgress())
            {
                if (this.current_element_elapsed_time > this.current_element.getDuration())
                {
                    if (!this.next())
                    {
                        return;
                    }
                }

                this.current_element.update(this.current_element_elapsed_time);
             
                float millisecondsSinceLastFrame = Game.LastFrameTime * 1000;
                this.current_element_elapsed_time += millisecondsSinceLastFrame;
                this.sequenceElapsedTime += millisecondsSinceLastFrame;
            }
        }

        public bool isInProgress()
        {
            return this.current_element != null && (this.sequence_total_duration == 0 || this.sequenceElapsedTime < this.sequence_total_duration);
        }

        public bool next()
        {
            if (this.current_element != null)
            {
                this.current_element.destroyCamera();
            }

            if (current_index < -1)
            {
                current_index = -1;
            }

            this.current_index++;
            this.current_element_elapsed_time = 0;

            if (this.current_index < this.sequence.Count)
            {
                this.current_element = this.sequence[current_index];
                if (this.current_element.WithFadeTransition)
                {
                    Game.FadeScreenOut((int)fadeTransitionDuration);

                    transitionTimer = new Timer(fadeTransitionDuration);
                    transitionTimer.OnTimerStop += (sender) =>
                    {
                        Tools.log("transitionTimer.OnTimerStop ");
                        this.current_element.activateCamera();
                    };
                    transitionTimer.OnTimerInterrupt += (sender, elapsedMilliseconds) =>
                    {
                        Tools.log("transitionTimer.OnTimerInterrupt " + elapsedMilliseconds + "ms");
                        Game.FadeScreenIn((int)fadeTransitionDuration);
                    };
                }
                else
                {
                    this.current_element.activateCamera();
                }

                return true;
            }
            else
            {
                this.reset();
                return false;
            }
        }

        public void reset()
        {
            Function.Call(Hash.DISPLAY_RADAR, true);
            Function.Call(Hash.DISPLAY_HUD, true);

            GUIManager.Instance.missionUI.show();

            if (transitionTimer != null)
            {
                Tools.log("transitionTimer interrupt");

                transitionTimer.interrupt();
                transitionTimer = null;
            }

            foreach (CameraShot cameraShot in this.sequence)
            {
                cameraShot.destroyCamera();
            }
            this.sequence.Clear();

            this.current_element = null;

            this.sequenceElapsedTime = 0;
            this.sequence_total_duration = 0;
            this.current_index = -1;
            this.current_element_elapsed_time = 0;
        }
    }
}
