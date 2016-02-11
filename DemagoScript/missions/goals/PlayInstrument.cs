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

    class PlayInstrument : AbstractObjective
    {
        private DateTime startTime;
        private Prop instrumentProp;
        private InstrumentHash instrumentHash;
        private string musicToPlay = "";
        private Music musiques;

        public float SecondsToPlay { get; set; }

        public PlayInstrument( InstrumentHash instrumentHash, float secondsToPlay, String musicToPlay, Music musics )
        {
            this.name = "Play instrument";

            this.instrumentHash = instrumentHash;
            this.musicToPlay = musicToPlay;
            SecondsToPlay = secondsToPlay / 1000;
            musiques = musics;
        }

        public override void populateDestructibleElements()
        {
            musiques.stopAll();
            musiques.playMusic(musicToPlay);

            startTime = DateTime.Now;

            #region Start animation
            Ped player = Game.Player.Character;

            player.Task.ClearAllImmediately();

            if (instrumentProp == null)
            {
                if (instrumentHash == InstrumentHash.Guitar)
                {
                    instrumentProp = World.CreateProp("prop_acc_guitar_01", player.Position + player.ForwardVector * 4.0f, true, true);
                    if (instrumentProp != null)
                    {
                        instrumentProp.HasCollision = true;
                        instrumentProp.HasGravity = true;
                        instrumentProp.AttachTo(player, player.GetBoneIndex(Bone.SKEL_Pelvis), new Vector3(-0.18f, 0.28f, -0.1f), new Vector3(195f, -24f, 0f));
                    }

                    player.Task.PlayAnimation("amb@world_human_musician@guitar@male@base", "base", 8f, -1, true, -1f);
                    Tools.log("play animation PlayInstrument");
                }
            }
            #endregion
        }

        public override void depopulateDestructibleElements(bool removePhysicalElements = false)
        {
            Game.Player.Character.Task.ClearAllImmediately();

            if (instrumentProp != null && instrumentProp.Exists())
            {
                instrumentProp.Delete();
                instrumentProp = null;
            }
        }

        public override bool update()
        {
            if (!base.update())
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
                accomplish();
                return false;
            }
            else if (SecondsToPlay >= 0)
                ObjectiveText = "Attend que les spectateurs aient assez apprécié la musique de Joe. (Back pour passer)";
            return true;
        }
        
    }
}
