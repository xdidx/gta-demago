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

        protected override void populateDestructibleElements()
        {
            base.populateDestructibleElements();

            AudioManager.Instance.startSound(musicToPlay);
            this.secondToPlay = AudioManager.Instance.getLength(musicToPlay);

            #region Start animation
            if (instrumentHash == InstrumentHash.Guitar)
            {
                Ped player = Game.Player.Character;

                while (instrumentProp == null)
                {
                    instrumentProp = World.CreateProp("prop_acc_guitar_01", player.Position + player.ForwardVector * 4.0f, true, true);
                }

                instrumentProp.HasCollision = true;
                instrumentProp.HasGravity = true;
                instrumentProp.AttachTo(player, player.GetBoneIndex(Bone.SKEL_Pelvis), new Vector3(-0.18f, 0.28f, -0.1f), new Vector3(195f, -24f, 0f));
                player.Task.ClearAllImmediately();
                player.Task.PlayAnimation("amb@world_human_musician@guitar@male@base", "base", 8f, -1, true, -1f);
            }
            #endregion
        }

        protected override void depopulateDestructibleElements(bool removePhysicalElements = false)
        {
            Game.Player.Character.Task.ClearAllImmediately();
            if (instrumentProp != null)
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

            if (Game.IsControlPressed(0, GTA.Control.PhoneCancel) && !Game.IsControlPressed(0, GTA.Control.FrontendPause) && !Game.IsControlPressed(0, GTA.Control.FrontendPauseAlternate))
            {
                this.accomplish();
            }

            secondToPlay -= Game.LastFrameTime * 1000;
            if (secondToPlay <= 0) {
                this.accomplish();
                return false;
            } else {
                ObjectiveText = "Attend que les spectateurs aient assez apprécié la musique de Joe. (Bouton B pour passer)";
            }

            return true;
        }

        public override void accomplish()
        {
            CameraShotsList.Instance.reset();
            AudioManager.Instance.stopAll();
            Game.Player.Character.Task.ClearAllImmediately();
            base.accomplish();
        }
    }
}
