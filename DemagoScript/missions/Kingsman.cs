using GTA;
using GTA.Native;

namespace DemagoScript
{
    class Kingsman : Mission
    {
        private Music musicPlaylist = null;
        private string[] musicFile = { "kingsman", "Kingsman.wav" };

        public override string getName()
        {
            return "Kingsman Mod";
        }

        protected override void doInitialization()
        {

            base.doInitialization();
            
            musicPlaylist = new Music(musicFile);
            musicPlaylist.setVolume(0.9f);

            musicPlaylist.playMusic("kingsman");

            //Tools.log("Lancement music Kingsman");

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

            Goal waitGoal = new Wait(194);
            addGoal(waitGoal);

        }

        public override void update()
        {
            base.update();

            Ped[] peds = World.GetAllPeds();
            for (int i = 0; i < peds.Length; i++)
            {
                if (peds[i] != Game.Player.Character && !peds[i].IsInCombat && !peds[i].IsInMeleeCombat && !peds[i].IsDead)
                {
                    //Tools.log(peds[i].IsInMeleeCombat+" = Melee "+ peds[i].IsInCombat + " = Combat fight" + i);
                    peds[i].Task.FightAgainst(Tools.GetClosestPedAroundPed(Game.Player.Character));
                }
            }
        }

    }
}
