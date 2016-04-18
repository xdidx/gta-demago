using GTA;
using GTA.Native;

namespace DemagoScript
{
    class Kingsman : Mission
    {
        private string[] musicFile = { "kingsman", "Kingsman.wav" };

        public Kingsman()
        {
            this.name = "Kingsman Mod";
        }
        
        protected override void populateDestructibleElements()
        {
            base.populateDestructibleElements();

            /*
             * TODO : Utiliser AudioManager
            musicPlaylist = new Music(musicFile);
            musicPlaylist.setVolume(0.9f);

            musicPlaylist.playMusic("kingsman");
            */

            Ped[] peds = World.GetAllPeds();
            for (int i = 0; i < peds.Length; i++)
            {
                if (peds[i] != Game.Player.Character && !peds[i].IsDead)
                {
                    peds[i].Task.ClearAllImmediately();
                    /*Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, peds[i], 46);
                    Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, peds[i], 3);
                    Function.Call(Hash.SET_PED_COMBAT_RANGE, peds[i], 2);
                    Function.Call(Hash.SET_PED_COMBAT_ABILITY, peds[i], 100);*/
                    peds[i].Task.FightAgainst(Tools.GetClosestPedAroundPed(Game.Player.Character));
                    //Tools.log("Fight entre tout le monde "+i);
                }
            }

            AbstractObjective waitObjective = new Wait(194000);
            addObjective(waitObjective);
        }

        protected override void depopulateDestructibleElements(bool removePhysicalElements = false)
        {
            base.depopulateDestructibleElements(removePhysicalElements);

            Ped[] peds = World.GetAllPeds();
            for (int i = 0; i < peds.Length; i++)
            {
                if (peds[i] != Game.Player.Character)
                {
                    peds[i].MarkAsNoLongerNeeded();
                }
            }
            /*
             * TODO : use audiomanager
            musicPlaylist.dispose();
            musicPlaylist = null;
            */
        }

        public override bool update()
        {
            if (!base.update())
            {
                return false;
            }

            Ped[] peds = World.GetAllPeds();
            for (int i = 0; i < peds.Length; i++)
            {
                if (peds[i].IsIdle && peds[i] != Game.Player.Character && !peds[i].IsDead)
                {
                    //Tools.log(peds[i].IsInMeleeCombat+" = Melee "+ peds[i].IsInCombat + " = Combat fight" + i);
                    peds[i].Task.FightAgainst(Tools.GetClosestPedAroundPed(peds[i]));
                }
            }

            return true;
        }
    }
}
