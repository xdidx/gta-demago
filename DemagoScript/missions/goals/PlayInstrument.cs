using GTA;
using GTA.Math;
using GTA.Native;
using System;
using IrrKlang;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemagoScript
{

    enum InstrumentHash
    {
        Guitar
    }

    class PlayInstrument : Goal
    {
        private DateTime startTime;
        private Prop instrumentProp;
        private InstrumentHash instrumentHash;
        private string musicToPlay = "";
        private Music musiques;
        private bool wasPaused = false;

        public float SecondsToPlay { get; set; }

        public PlayInstrument( InstrumentHash instrumentHash, float secondsToPlay, String musicToPlay, Music musics )
        {
            this.instrumentHash = instrumentHash;
            this.musicToPlay = musicToPlay;
            SecondsToPlay = secondsToPlay / 1000;
            musiques = musics;
        }

        public override bool initialize()
        {
            if ( !base.initialize() ) {
                return false;
            }

            musiques.stopAll();
            musiques.playMusic( musicToPlay );

            startTime = DateTime.Now;

            startAnimation();

            return true;
        }

        public override bool update()
        {
            if (!base.update() || isAccomplished())
                return false;

            if(Game.IsKeyPressed(System.Windows.Forms.Keys.Back))
            {
                CameraShotsList.Instance.reset();
                musiques.pauseMusic(musicToPlay);
                Game.Player.Character.Task.ClearAllImmediately();
                accomplish();
                return false;
            }

            musiques.playMusic(musicToPlay);
            
            SecondsToPlay -= Game.LastFrameTime;
            if (SecondsToPlay <= 0)
            {
                Game.Player.Character.Task.ClearAllImmediately();
                accomplish();
                return false;
            }
            else if (SecondsToPlay >= 0)
                setGoalText("Attend que les spectateurs aient assez apprécié la musique de Joe. (Back pour passer)");
            return true;
        }

        private void startAnimation()
        {
            Ped player = Game.Player.Character;

            player.Task.ClearAllImmediately();

            if ( instrumentProp == null ) {
                if ( instrumentHash == InstrumentHash.Guitar ) {
                    instrumentProp = World.CreateProp( "prop_acc_guitar_01", player.Position + player.ForwardVector * 4.0f, true, true );
                    if ( instrumentProp != null ) {
                        instrumentProp.HasCollision = true;
                        instrumentProp.HasGravity = true;
                        instrumentProp.AttachTo( player, player.GetBoneIndex( Bone.SKEL_Pelvis ), new Vector3( -0.18f, 0.28f, -0.1f ), new Vector3( 195f, -24f, 0f ) );
                    }

                    player.Task.PlayAnimation( "amb@world_human_musician@guitar@male@base", "base", 8f, -1, true, -1f );
                }
            }
        }

        public override void clear( bool removePhysicalElements = false )
        {
            if ( instrumentProp != null && instrumentProp.Exists() ) {
                instrumentProp.Delete();
                instrumentProp = null;
            }
        }

        public override void setPause(bool isPaused)
        {
            if (isAccomplished())
                return;

            if (isPaused && musiques.isPlaying(musicToPlay))
            {
                musiques.pauseMusic(musicToPlay);
                this.wasPaused = true;
            }
            else if (!isPaused && this.wasPaused)
            {
                musiques.playMusic(musicToPlay);
            }
        }
    }
}
