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

        private static string[] sonsEtape1 = new string[] { "flics1", "dialogue1", "dialogue2", "dialogue3" };
        private static string[] sonsEtape2 = new string[] { "flics2", "flics3", "dialogue4", "dialogue5", "dialogue6" };
        private static string[] sonsEtape3 = new string[] { "flics4", "dialogue7", "dialogue8", "dialogue9" };

        private Vehicle bike;
        private List<Ped> spectatorsPeds = new List<Ped>();
        private List<Ped> spectatorsPeds2 = new List<Ped>();
        private List<Ped> copsPeds = new List<Ped>();
        private int playerLifeUpCounter = 0;
        private bool bikeRegen;
        private bool playerDown;
        private bool playerWalked;
        private bool introEnded;
        private int  nbMusiqueEtape;
        private int cameraChangeTimer, etapeMission;
        private Ped introPed;
        private float startTime = 0;
        private Random r = new Random();

        private List<string[]> musiques = new List<string[]>();
        private Music musicPlaylist = null;
        private string currentPlay = null;
        private string interruptPlay = null;
        private string currentInterruptPlay = null;

        public override string getName()
        {
            return "Joe l'anticonformiste";
        }

        private void loadMusic()
        {
            musiques.Clear();
            musiques.Add(new string[] { "balle1", "joeBalle1.wav" });
            musiques.Add(new string[] { "balle2", "joeBalle2.wav" });
            musiques.Add(new string[] { "balle3", "joeBalle3.wav" });
            musiques.Add(new string[] { "balle4", "joeBalle4.wav" });
            musiques.Add(new string[] { "balle5", "joeBalle5.wav" });
            musiques.Add(new string[] { "balle6", "joeBalle6.wav" });
            musiques.Add(new string[] { "balle7", "joeBalle7.wav" });
            musiques.Add(new string[] { "balle8", "joeBalle8.wav" });
            musiques.Add(new string[] { "balle9", "joeBalle9.wav" });
            musiques.Add(new string[] { "balle10", "joeBalle10.wav" });
            musiques.Add(new string[] { "insulte1", "joeInsulte1.wav" });
            musiques.Add(new string[] { "insulte2", "joeInsulte2.wav" });
            musiques.Add(new string[] { "insulte3", "joeInsulte3.wav" });
            musiques.Add(new string[] { "insulte4", "joeInsulte4.wav" });
            musiques.Add(new string[] { "insulte5", "joeInsulte5.wav" });
            musiques.Add(new string[] { "insulte6", "joeInsulte6.wav" });
            musiques.Add(new string[] { "insulte7", "joeInsulte7.wav" });
            musiques.Add(new string[] { "voiture1", "joeVoiture1.wav" });
            musiques.Add(new string[] { "voiture2", "joeVoiture2.wav" });
            musiques.Add(new string[] { "voiture3", "joeVoiture3.wav" });
            musiques.Add(new string[] { "voiture4", "joeVoiture4.wav" });
            musiques.Add(new string[] { "voiture5", "joeVoiture5.wav" });
            musiques.Add(new string[] { "voiture6", "joeVoiture6.wav" });
            musiques.Add(new string[] { "voiture7", "joeVoiture7.wav" });
            musiques.Add(new string[] { "voiture8", "joeVoiture8.wav" });
            musiques.Add(new string[] { "dialogue0", "joeDialogue0.wav" });
            musiques.Add(new string[] { "dialogue1", "joeDialogue1.wav" });
            musiques.Add(new string[] { "dialogue2", "joeDialogue2.wav" });
            musiques.Add(new string[] { "dialogue3", "joeDialogue3.wav" });
            musiques.Add(new string[] { "dialogue4", "joeDialogue4.wav" });
            musiques.Add(new string[] { "dialogue5", "joeDialogue5.wav" });
            musiques.Add(new string[] { "dialogue6", "joeDialogue6.wav" });
            musiques.Add(new string[] { "dialogue7", "joeDialogue7.wav" });
            musiques.Add(new string[] { "dialogue8", "joeDialogue8.wav" });
            musiques.Add(new string[] { "dialogue9", "joeDialogue9.wav" });
            musiques.Add(new string[] { "dialogue10", "joeDialogue10.wav" });
            musiques.Add(new string[] { "musique1", "joeAnticonformiste.wav" });
            musiques.Add(new string[] { "musique2", "joeHippie.wav" });
            musiques.Add(new string[] { "musique3", "joeDegueulasse.wav" });
            musiques.Add(new string[] { "flics1", "joeFlics1.wav" });
            musiques.Add(new string[] { "flics2", "joeFlics2.wav" });
            musiques.Add(new string[] { "flics3", "joeFlics3.wav" });
            musiques.Add(new string[] { "flics4", "joeFlics4.wav" });
        }

        protected override void doInitialization()
        {
            base.doInitialization();

            loadMusic();
            musicPlaylist = new Music(musiques);
            musicPlaylist.setVolume(0.4f);

            bikeRegen = false;
            playerDown = true;
            playerWalked = false;
            introEnded = false;
            cameraChangeTimer = 0;
            etapeMission = 0;
            nbMusiqueEtape = -1;

            Function.Call(Hash.DISPLAY_HUD, false);
            Function.Call(Hash.DISPLAY_RADAR, false);

            World.Weather = Weather.ExtraSunny;
            Tools.setClockTime(10);
            Tools.setDemagoModel(DemagoModel.Joe);

            Ped player = Game.Player.Character;
            player.MaxHealth = 300;
            player.Armor = 100;
            player.Health = player.MaxHealth;
            Function.Call(Hash.SET_PED_MAX_HEALTH, player, player.MaxHealth);
            
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

            bike = null;
            while ( bike == null ) {
                bike = World.CreateVehicle( VehicleHash.TriBike, bikePositionAtHome );
            }
            if ( bike.Exists() ) {
                bike.EnginePowerMultiplier = 100;
                bike.IsInvincible = true;
                bike.CanTiresBurst = false;
            }
            
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
                if (ped != null && ped.Exists())
                {
                    TaskSequence incomingSpectator = new TaskSequence();
                    incomingSpectator.AddTask.GoTo(firstSongPosition.Around(7).Around(2));
                    incomingSpectator.AddTask.TurnTo(player);
                    incomingSpectator.AddTask.LookAt(player);
                    incomingSpectator.AddTask.PlayAnimation("facials@gen_male@variations@angry", "mood_angry_1", 8f, -1, true, -1f);

                    ped.Task.PerformSequence(incomingSpectator);
                    spectatorsPeds.Add(ped);
                }
            }

            Goal gotoFirstSongPosition = new GoToPosition( firstSongPosition );
            addGoal(gotoFirstSongPosition);

            Goal firstSongGoals = new PlayInstrument(InstrumentHash.Guitar, 25, "musique1", musicPlaylist);
            addGoal(firstSongGoals);
            firstSongGoals.OnGoalStart += (sender) =>
            {
                Tools.setClockTime(11, 10000);

                foreach (Ped spectator in spectatorsPeds)
                {
                    spectator.Task.ClearAllImmediately();
                    spectator.Task.PlayAnimation("gestures@m@standing@casual", "gesture_displeased", 8f, 20000, false, -1f);
                }
            };

            firstSongGoals.OnGoalAccomplished += (sender, elaspedTime) =>
            {
                foreach (Ped spectator in spectatorsPeds)
                {
                    if (spectator != null && spectator.Exists())
                    {
                        spectator.Task.ClearAllImmediately();
                        spectator.Task.UseMobilePhone();
                    }
                }

                player.Health = 300;
                player.Armor = 100;
                GTA.UI.ShowSubtitle("Spectateurs : C'est nul ! Casse toi ! On a appelé les flics !", 3000);
                
                Tools.setClockTime(12, 10000);
                Game.Player.WantedLevel = 2;
                World.Weather = Weather.Clouds;
                etapeMission = 1;
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

            Goal secondSongGoals = new PlayInstrument(InstrumentHash.Guitar, 20, "musique2", musicPlaylist);
            addGoal(secondSongGoals);
            secondSongGoals.OnGoalStart += (sender) =>
            {
                if (currentPlay != null)
                {
                    musicPlaylist.pauseMusic(currentPlay);
                    currentPlay = null;
                }

                Tools.setClockTime(16, 10000);

                foreach (Ped spectator in World.GetNearbyPeds(player, 12))
                {
                    if (spectator != null && spectator.Exists())
                    {
                        spectator.Task.ClearAllImmediately();
                        Function.Call(Hash.TASK_TURN_PED_TO_FACE_ENTITY, spectator.Handle, player.Handle);
                        spectator.Task.LookAt(Game.Player.Character);
                    }
                }

                TaskSequence policeSurrounding = new TaskSequence();
                policeSurrounding.AddTask.TurnTo(player);
                policeSurrounding.AddTask.StandStill(10000);
                policeSurrounding.AddTask.GoTo(player.Position.Around(2).Around(1));
                policeSurrounding.AddTask.TurnTo(player);
                policeSurrounding.AddTask.LookAt(player);

                player.Heading = 90;

                foreach (Ped spectator in copsPeds)
                {
                    if (spectator != null && spectator.Exists())
                    {
                        spectator.Task.PerformSequence(policeSurrounding);
                    }
                }
            };
            
            secondSongGoals.OnGoalAccomplished += (sender, elaspedTime) =>
            {
                foreach( Ped ped in copsPeds)
                {
                    if (ped != null && ped.Exists())
                    {
                        ped.Weapons.Give(WeaponHash.Pistol, 1, true, true);
                        Function.Call(Hash.SET_PED_AS_COP, ped, true);
                        ped.MarkAsNoLongerNeeded();
                    }
                }

                player.Health = 300;
                player.Armor = 100;

                Game.Player.WantedLevel = 1;
                GTA.UI.ShowSubtitle("Policier : Si tu ne sors pas, c'est nous qui allons te faire sortir !", 4000);
                World.Weather = Weather.Raining;
                Tools.setClockTime(17, 10000);
                etapeMission = 2;
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

            Goal thirdSongGoals = new PlayInstrument(InstrumentHash.Guitar, 55, "musique3", musicPlaylist);
            addGoal(thirdSongGoals);

            thirdSongGoals.OnGoalStart += (sender) =>
            {
                if (currentPlay != null)
                {
                    musicPlaylist.pauseMusic(currentPlay);
                    currentPlay = null;
                }

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
                etapeMission = 3;
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
                musicPlay("dialogue10");
            };
            
            startTime = DemagoScript.getScriptTime();

            musicPlay("dialogue0");
        }

        private void musicPlay(string musique)
        {
            currentPlay = musique;
            musicPlaylist.playMusic(currentPlay);
        }

        public override void clear(bool removePhysicalElements = false)
        {
            base.clear(removePhysicalElements);

            if (musicPlaylist != null)
                musicPlaylist.dispose();

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

        public void playAmbiance()
        {
            if (!musicPlaylist.isPlaying(currentPlay))
            {
                switch (etapeMission)
                {
                    case 1:
                        nbMusiqueEtape++;
                        if (nbMusiqueEtape < sonsEtape1.Length)
                        {
                            currentPlay = sonsEtape1[nbMusiqueEtape];
                            musicPlay(currentPlay);
                        }
                        else
                        {
                            nbMusiqueEtape = -1;
                            etapeMission = 0;
                        }

                        break;
                    case 2:
                        nbMusiqueEtape++;
                        if (nbMusiqueEtape < sonsEtape2.Length)
                        {
                            currentPlay = sonsEtape2[nbMusiqueEtape];
                            musicPlay(currentPlay);
                        }
                        else
                        {
                            nbMusiqueEtape = -1;
                            etapeMission = 0;
                        }
                        break;
                    case 3:
                        nbMusiqueEtape++;
                        if (nbMusiqueEtape < sonsEtape3.Length)
                        {
                            currentPlay = sonsEtape3[nbMusiqueEtape];
                            musicPlay(currentPlay);
                        }
                        break;
                    default:
                        break;
                }
            }
            if (interruptPlay == null)
            {
                if (Function.Call<Boolean>(Hash.HAS_PED_BEEN_DAMAGED_BY_WEAPON, Game.Player.Character, 0, 2))
                {
                    int next = r.Next(10) + 1;
                    interruptPlay = "balle" + next;
                }
                else
                {
                    if (Function.Call<Boolean>(Hash.HAS_ENTITY_BEEN_DAMAGED_BY_ANY_VEHICLE, Game.Player.Character))
                    {
                        int next = r.Next(8) + 1;
                        interruptPlay = "voiture" + next;
                    }
                    else
                    {
                        if (Function.Call<Boolean>(Hash.HAS_ENTITY_BEEN_DAMAGED_BY_ANY_OBJECT, Game.Player.Character))
                        {
                            int next = r.Next(7) + 1;
                            interruptPlay = "insulte" + next;
                        }
                    }
                }
            }
        }

        public override void update()
        {
            base.update();

            if ( currentPlay != null)
            {                
                if (interruptPlay != null && currentInterruptPlay == null)
                {
                    currentInterruptPlay = interruptPlay;
                    interruptPlay = null;

                    if (!currentPlay.StartsWith("flic"))
                        musicPlaylist.pauseMusic(currentPlay);

                    musicPlaylist.playMusic(currentInterruptPlay);
                }

                if (!musicPlaylist.isPlaying(currentInterruptPlay) && currentInterruptPlay != null)
                {
                    musicPlaylist.playMusic(currentPlay);
                    musicPlaylist.restart(currentInterruptPlay);
                    musicPlaylist.pauseMusic(currentInterruptPlay);
                    Function.Call(Hash.CLEAR_ENTITY_LAST_DAMAGE_ENTITY, Game.Player.Character);
                    Function.Call(Hash.CLEAR_ENTITY_LAST_WEAPON_DAMAGE, Game.Player.Character);
                    currentInterruptPlay = null;
                }

                if (!musicPlaylist.isPlaying(currentPlay))
                    currentPlay = null;
            }
            else
            {
                if (interruptPlay != null && currentInterruptPlay == null)
                {
                    currentInterruptPlay = interruptPlay;
                    interruptPlay = null;
                    musicPlaylist.playMusic(currentInterruptPlay);
                }

                if (!musicPlaylist.isPlaying(currentInterruptPlay) && currentInterruptPlay != null)
                {
                    musicPlaylist.restart(currentInterruptPlay);
                    musicPlaylist.pauseMusic(currentInterruptPlay);
                    Function.Call(Hash.CLEAR_ENTITY_LAST_DAMAGE_ENTITY, Game.Player.Character);
                    Function.Call(Hash.CLEAR_ENTITY_LAST_WEAPON_DAMAGE, Game.Player.Character);
                    currentInterruptPlay = null;
                }
            }

            playAmbiance();

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
        }
    }
}
