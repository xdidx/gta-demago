using GTA;
using GTA.Math;
using GTA.Native;
using IrrKlang;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemagoScript
{
    class Joe : Mission
    {
        public static Vector3 joeHomePosition { get; } = new Vector3(2350f, 2529f, 46f);
        public static Vector3 firstSongPosition { get; } = new Vector3(2338f, 2547f, 48f);
        public static Vector3 bikePositionAtHome { get; } = new Vector3(2353f, 2526f, 48f);
        public static Vector3 roadFaceToPoliceStationPosition { get; } = new Vector3(414f, -978f, 29f);
        public static Vector3 secondSongPosition { get; } = new Vector3(445f, -985f, 30f);
        public static Vector3 thirdSongBikePosition { get; } = new Vector3(681f, 565f, 128f);
        public static Vector3 thirdSongPosition { get; } = new Vector3(686f, 578f, 131f);
        public static Vector3 joeStart { get; } = new Vector3(2353.457f, 2522.366f, 47.68944f);

        private Vehicle bike;
        private List<Ped> spectatorsPeds = new List<Ped>();
        private List<Ped> spectatorsPeds2 = new List<Ped>();
        private List<Ped> copsPeds = new List<Ped>();
        private int playerLifeUpCounter = 0;
        private bool bikeRegen;
        private bool playerDown;
        private bool playerWalked;
        private bool introEnded;
        private int cameraChangeTimer;
        private Ped introPed;
        private float startTime = 0;

        ISoundEngine engine = new ISoundEngine();
        ISound sound;

        public override string getName()
        {
            return "Joe l'anticonformiste";
        }

        public override bool initialize()
        {
            if (!base.initialize())
            {
                return false;
            }

            bikeRegen = false;
            playerDown = true;
            playerWalked = false;
            introEnded = false;
            cameraChangeTimer = 0;

            Function.Call(Hash.DISPLAY_HUD, false);
            Function.Call(Hash.DISPLAY_RADAR, false);

            World.Weather = Weather.ExtraSunny;
            Tools.setClockTime(10);
            Tools.setDemagoModel(DemagoModel.Joe);

            Ped player = Game.Player.Character;
            player.MaxHealth = 300;
            Function.Call(Hash.SET_PED_MAX_HEALTH, player, 300);
            player.Health = 300;
            player.Armor = 100;

            Tools.TeleportPlayer(joeStart);
            introPed = Function.Call<Ped>(Hash.CLONE_PED, Game.Player.Character, Function.Call<int>(Hash.GET_ENTITY_HEADING, Function.Call<int>(Hash.PLAYER_PED_ID)), false, true);
            Tools.TeleportPlayer(joeHomePosition);
            player.Heading += 35;
            player.Task.StandStill(-1);
            player.IsVisible = false;

            introPed.Task.PlayAnimation("amb@world_human_picnic@male@base", "base", 8f, -1, true, -1f);

            List<Vector3> positions = new List<Vector3>();
            positions.Add(new Vector3(2361.558f, 2527.512f, 46.66772f));
            positions.Add(new Vector3(2351.906f, 2530.494f, 48f));

            Tools.traveling(positions, 37000, introPed, true);
            
            bike = World.CreateVehicle(VehicleHash.TriBike, bikePositionAtHome);
            bike.EnginePowerMultiplier = 100;
            bike.IsInvincible = true;
            bike.CanTiresBurst = false;

            List<PedHash> spectatorsHashesFirstSong = new List<PedHash>();
            spectatorsHashesFirstSong.Add(PedHash.Ashley);
            spectatorsHashesFirstSong.Add(PedHash.Car3Guy2);
            spectatorsHashesFirstSong.Add(PedHash.Car3Guy1);
            spectatorsHashesFirstSong.Add(PedHash.Bankman);
            spectatorsHashesFirstSong.Add(PedHash.Barry);
            spectatorsHashesFirstSong.Add(PedHash.Beach01AFM);
            spectatorsHashesFirstSong.Add(PedHash.Beach01AFY);
            spectatorsHashesFirstSong.Add(PedHash.Beach01AMM);
            spectatorsHashesFirstSong.Add(PedHash.Beach02AMM);

            List<PedHash> spectatorsHashesSecondSong = new List<PedHash>();
            spectatorsHashesSecondSong.Add(PedHash.Cop01SFY);
            spectatorsHashesSecondSong.Add(PedHash.Cop01SMY);
            spectatorsHashesSecondSong.Add(PedHash.Cop01SFY);
            spectatorsHashesSecondSong.Add(PedHash.Cop01SMY);
            
            foreach (PedHash hash in spectatorsHashesFirstSong)
            {
                Ped ped = World.CreatePed(hash, firstSongPosition.Around(50), 0);

                TaskSequence incomingSpectator = new TaskSequence();
                incomingSpectator.AddTask.GoTo(firstSongPosition.Around(7).Around(2));
                incomingSpectator.AddTask.TurnTo(player);
                incomingSpectator.AddTask.LookAt(player);
                incomingSpectator.AddTask.PlayAnimation("facials@gen_male@variations@angry", "mood_angry_1", 8f, -1, true, -1f);

                if (ped != null && ped.Exists())
                {
                    ped.Task.PerformSequence(incomingSpectator);
                    spectatorsPeds.Add(ped);
                }
            }

            addGoal(new GoToPosition(firstSongPosition));

            Goal firstSongGoals = new PlayInstrument(InstrumentHash.Guitar, 25, "joeAnticonformiste");
            addGoal(firstSongGoals);
            firstSongGoals.OnGoalStart += (sender) =>
            {
                Tools.setClockTime(11, 10000);
            };

            firstSongGoals.OnGoalAccomplished += (sender, elaspedTime) =>
            {
                foreach (Ped spectator in spectatorsPeds)
                {
                    spectator.Task.ClearAllImmediately();
                    spectator.Task.UseMobilePhone();
                }

                player.Health = 300;
                player.Armor = 100;
                GTA.UI.ShowSubtitle("Spectateurs : C'est nul ! Casse toi ! On a appelé les flics !", 3000);
                
                Tools.setClockTime(12, 10000);
                Game.Player.WantedLevel = 2;
                World.Weather = Weather.Clouds;
            };

            foreach (PedHash hash in spectatorsHashesSecondSong)
            {
                Ped ped = World.CreatePed(hash, new Vector3(451.7534f, -978.9359f, 30.68965f));
                if (ped != null && ped.Exists())
                    copsPeds.Add(ped);

                ped = World.CreatePed(hash, secondSongPosition.Around(2));
                if (ped != null && ped.Exists())
                {
                    ped.Task.WanderAround(secondSongPosition, 5);
                    copsPeds.Add(ped);
                }
            }
            
            Ped pedWaiting = World.CreatePed(PedHash.Hooker01SFY, new Vector3(441.7531f, -987.5613f, 30.68965f));
            if (pedWaiting != null && pedWaiting.Exists())
            {
                pedWaiting.Task.UseMobilePhone();
                spectatorsPeds2.Add(pedWaiting);
            }
            pedWaiting = World.CreatePed(PedHash.Hooker02SFY, new Vector3(441.7531f, -985.9871f, 30.68965f));
            if (pedWaiting != null && pedWaiting.Exists())
            {
                pedWaiting.Task.UseMobilePhone();
                spectatorsPeds2.Add(pedWaiting);
            }
            pedWaiting = World.CreatePed(PedHash.Hooker03SFY, new Vector3(440.7078f, -987.5613f, 30.68965f));
            if (pedWaiting != null && pedWaiting.Exists())
            {
                pedWaiting.Task.UseMobilePhone();
                spectatorsPeds2.Add(pedWaiting);
            }
            pedWaiting = World.CreatePed(PedHash.Lost01GMY, new Vector3(440.7078f, -985.9871f, 30.68965f));
            if (pedWaiting != null && pedWaiting.Exists())
            {
                pedWaiting.Task.UseMobilePhone();
                spectatorsPeds2.Add(pedWaiting);
            }

            GoToPositionInVehicle goToPoliceWithBikeGoal = new GoToPositionInVehicle(roadFaceToPoliceStationPosition, bike);
            addGoal(goToPoliceWithBikeGoal);
            goToPoliceWithBikeGoal.OnFirstTimeOnVehicle += (sender, vehicle) =>
            {
                Tools.setClockTime(14, 10000);
            };


            addGoal(new GoToPosition(secondSongPosition));

            Goal secondSongGoals = new PlayInstrument(InstrumentHash.Guitar, 20, "joeHippie");
            addGoal(secondSongGoals);
            secondSongGoals.OnGoalStart += (sender) =>
            {
                Tools.setClockTime(16, 10000);

                foreach (Ped spectator in World.GetNearbyPeds(player, 12))
                {
                    spectator.Task.ClearAllImmediately();
                    Function.Call(Hash.TASK_TURN_PED_TO_FACE_ENTITY, spectator.Handle, player.Handle);
                    spectator.Task.LookAt(Game.Player.Character);
                }

                TaskSequence policeSurrounding = new TaskSequence();
                policeSurrounding.AddTask.TurnTo(player);
                policeSurrounding.AddTask.StandStill(10000);
                policeSurrounding.AddTask.GoTo(player.Position.Around(2).Around(1));
                policeSurrounding.AddTask.TurnTo(player);
                policeSurrounding.AddTask.LookAt(player);

                player.Heading = 90;

                foreach (Ped spectator in copsPeds)
                    spectator.Task.PerformSequence(policeSurrounding);
            };
            secondSongGoals.OnGoalAccomplished += (sender, elaspedTime) =>
            {
                foreach( Ped ped in copsPeds)
                {
                    ped.Weapons.Give(WeaponHash.Pistol, 1, true, true);
                    Function.Call(Hash.SET_PED_AS_COP, ped, true);
                    ped.MarkAsNoLongerNeeded();
                }

                player.Health = 300;
                player.Armor = 100;

                Game.Player.WantedLevel = 1;
                GTA.UI.ShowSubtitle("Policier : Si tu ne sors pas, c'est nous qui allons te faire sortir !", 4000);
                World.Weather = Weather.Raining;
                Tools.setClockTime(17, 10000);
            };

            GoToPositionInVehicle goToTheaterWithBikeGoal = new GoToPositionInVehicle(thirdSongBikePosition, bike);
            addGoal(goToTheaterWithBikeGoal);
            goToTheaterWithBikeGoal.OnFirstTimeOnVehicle += (sender, vehicle) =>
            {
                Tools.setClockTime(18, 10000);
                player.Health = 300;
                player.Armor = 100;
                Game.Player.WantedLevel = 3;
            };

            goToTheaterWithBikeGoal.OnGoalAccomplished += (sender, elapsedTime) =>
            {
                Tools.setClockTime(19, 10000);
            };
            
            addGoal(new GoToPosition(thirdSongPosition));

            Goal thirdSongGoals = new PlayInstrument(InstrumentHash.Guitar, 55, "joeDegueulasse");
            addGoal(thirdSongGoals);

            thirdSongGoals.OnGoalStart += (sender) =>
            {
                player.Heading = 180;
                Game.Player.WantedLevel = 0;
                Function.Call(Hash.TASK_TURN_PED_TO_FACE_COORD, player.Handle, 640f, 448f, 100f, -1);
                Tools.setClockTime(20, 10000);
            };

            thirdSongGoals.OnGoalAccomplished += (sender, elaspedTime) =>
            {
                player.Health = 300;
                player.Armor = 100;
                bikeRegen = true;
                Game.Player.WantedLevel = 4;
                Tools.setClockTime(21, 10000);
                World.Weather = Weather.ThunderStorm;

                GTA.UI.ShowSubtitle("Spectateurs : C'est nul ! Casse toi ! On a encore appelé les flics ! Tu vas avoir des problèmes !", 3000);
            };
            GoToPositionInVehicle goToHome = new GoToPositionInVehicle(joeHomePosition, bike);
            addGoal(goToHome);
            goToHome.OnFirstTimeOnVehicle += (sender, vehicle) =>
            {
                goToHome.setAdviceText("Evite les routes pour éviter les voitures de police");
                Tools.setClockTime(22, 10000);
            };
            goToHome.OnGoalAccomplished += (sender, elapsedTime) =>
            {
                Tools.setClockTime(23, 10000);
            };

            startTime = DemagoScript.getScriptTime();

            try {
                sound = engine.Play2D(@"C:\Program Files\Rockstar Games\Grand Theft Auto V\Music\intro.wav");
            }
            catch (Exception ex)
            {
                Tools.log(ex.Message);
            }

            return true;
        }

        public override void clear(bool removePhysicalElements = false)
        {
            base.clear(removePhysicalElements);
            World.Weather = Weather.Clear;

            if (removePhysicalElements)
            {
                if (bike != null)
                    bike.Delete();

                foreach (Ped spectator in spectatorsPeds)
                    if (spectator != null && spectator.Exists() && spectator.IsAlive)
                        spectator.Delete();

                spectatorsPeds.Clear();

                foreach (Ped spectator in copsPeds)
                    if (spectator != null && spectator.Exists() && spectator.IsAlive)
                        spectator.Delete();

                copsPeds.Clear();

                foreach (Ped spectator in spectatorsPeds2)
                    if (spectator != null && spectator.Exists() && spectator.IsAlive)
                        spectator.Delete();

                spectatorsPeds2.Clear();
            }
        }

        public override bool update()
        {
            if (Game.IsPaused)
                sound.Paused = true;
            else
                sound.Paused = false;

            if (!base.update())
                return false;

                Ped player = Game.Player.Character;

            if (playerDown || !playerWalked || !introEnded)
            {
                float elapsedTime = DemagoScript.getScriptTime() - startTime;

                if (elapsedTime > 12000 && playerDown)
                {
                    introPed.Task.PlayAnimation("amb@world_human_picnic@male@exit", "exit", 8f, 3000, false, -1f);
                    playerDown = false;
                }
                if (elapsedTime > 15000 && !playerWalked)
                {
                    introPed.Task.ClearAllImmediately();
                    introPed.Task.GoTo(joeHomePosition, true);
                    playerWalked = true;
                }
                if (elapsedTime > 40000 && !introEnded)
                {
                    player.Task.ClearAllImmediately();
                    introPed.IsVisible = false;
                    introPed.Delete();
                    player.IsVisible = true;
                    Function.Call(Hash.DISPLAY_HUD, true);
                    Function.Call(Hash.DISPLAY_RADAR, true);
                    introEnded = true;
                }
                else
                    cameraChangeTimer++;
            }
            
            if (player.IsInVehicle() && player.CurrentVehicle == bike)
                playerLifeUpCounter++;
            else
                playerLifeUpCounter = 0;

            if (playerLifeUpCounter >= GTA.Game.FPS / 5 && bikeRegen && bike.Speed > 0)
            {
                playerLifeUpCounter = 0;
                if (player.Health < player.MaxHealth)
                    player.Health++;
                if (player.Armor < 100)
                    player.Armor++;
            }

            Game.Player.Character.Weapons.RemoveAll();

            return true;
        }
    }
}
