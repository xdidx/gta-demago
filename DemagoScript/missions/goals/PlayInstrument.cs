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
        private Prop instrumentProp;
        private InstrumentHash instrumentHash;
        private string musicToPlay = "";
        private float secondToPlay = 0;

        public PlayInstrument(InstrumentHash instrumentHash, string musicToPlay)
        {
            this.name = "Play instrument";

            this.instrumentHash = instrumentHash;
            this.musicToPlay = musicToPlay;
        }

        public override void populateDestructibleElements()
        {
            base.populateDestructibleElements();
            
            Function.Call( Hash.DISPLAY_RADAR, false );

            AudioManager.Instance.startSound(musicToPlay);
            this.secondToPlay = AudioManager.Instance.getLength(musicToPlay);

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
                }
            }
            #endregion
        }

        public override void depopulateDestructibleElements(bool removePhysicalElements = false)
        {
            Function.Call( Hash.DISPLAY_RADAR, true );

            Game.Player.Character.Task.ClearAllImmediately();
            if (instrumentProp != null && instrumentProp.Exists())
            {
                instrumentProp.Detach();
                instrumentProp.MarkAsNoLongerNeeded();
                if (removePhysicalElements)
                {
                    instrumentProp.Delete();
                    instrumentProp = null;
                }
            }
        }

        public override bool update()
        {
            if (!base.update()) {
                return false;
            }

            if (Game.IsKeyPressed(System.Windows.Forms.Keys.Back)) {
                accomplish();
                return false;
            }
            
            secondToPlay -= Game.LastFrameTime;
            if (secondToPlay <= 0) {
                accomplish();
                return false;
            } else {
                ObjectiveText = "Attend que les spectateurs aient assez apprécié la musique de Joe. (Back pour passer)";
            }

            return true;
        }

        protected override void accomplish()
        {
            CameraShotsList.Instance.reset();
            AudioManager.Instance.stopAll();
            Game.Player.Character.Task.ClearAllImmediately();
            base.accomplish();
        }
    }
}
