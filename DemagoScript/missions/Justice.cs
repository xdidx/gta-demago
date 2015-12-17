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
    class Justice : Mission
    {
        public static Vector3 buzzardPosition { get; } = new Vector3(-206f, -1997f, 27.0f);
        
        public override string getName()
        {
            return "La justice";
        }

        protected override void doInitialization()
        {
            base.doInitialization();

            Game.Player.Character.Weapons.RemoveAll();
            foreach (WeaponHash hash in Enum.GetValues(typeof(WeaponHash)))
            {
                Game.Player.Character.Weapons.Give(hash, -1, false, true);
            }

            Game.Player.Character.Armor = 100;
            Game.Player.Character.Health = 100;

            List<Vector3> startPositions = new List<Vector3>();
            startPositions.Add(new Vector3(127.1687f, -1929.726f, 21.38243f));
            startPositions.Add(new Vector3(118.2008f, -1920.967f, 21.32341f));
            startPositions.Add(new Vector3(100.6572f, -1911.966f, 21.40742f));
            startPositions.Add(new Vector3(86.02232f, -1959.530f, 21.12170f));
            startPositions.Add(new Vector3(76.29898f, -1948.164f, 21.17414f));

            addGoal(new SurviveInZone(PlacesPositions.GrooveStreet, startPositions, 120, 50));
            addGoal(new EnterInVehicle(buzzardPosition, VehicleHash.Buzzard));
        }
    }
}
