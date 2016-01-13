using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public static Vector3 thirdSongPublicPosition1 { get; } = new Vector3(643.53f, 564.02f, 129.05f);
        public static Vector3 thirdSongPublicPosition2 { get; } = new Vector3(59.13f, -22.26f, 0.0f);
        public static Vector3 thirdSongPublicPosition3 { get; } = new Vector3(-16.09f, -20.29f, 1.0f);

        private static string[] sonsEtape1 = new string[] { "flics1", "dialogue1", "dialogue2", "dialogue3" };
        private static string[] sonsEtape2 = new string[] { "flics2", "flics3", "flics4", "dialogue4", "dialogue5", "dialogue6" };
        private static string[] sonsEtape3 = new string[] { "flics5", "dialogue8", "dialogue9" };

        public static Vehicle bike = null;
        private List<Ped> spectatorsPeds = new List<Ped>();
        private List<Ped> spectatorsPeds2 = new List<Ped>();
        private List<Ped> copsPeds = new List<Ped>();
        private List<Ped> spectatorsPeds3 = new List<Ped>();

        private bool bikeRegen = false;
        private bool playerDown = true;
        private bool playerWalked = false;
        private bool playerMoved = false;
        private bool introEnded = false;
        private int nbMusiqueEtape = -1, cameraChangeTimer = 0, etapeMission = 0, playerLifeUpCounter = 0;
        private Ped introPed, nadineMorano;
        private float startTime = 0;
        private Random random = new Random();

        private List<string[]> musiques = new List<string[]>();
        private Music musicPlaylist = null;
        private string currentPlay = "";
        private string interruptPlay = "";
        private string currentInterruptPlay = "";

        public override string getName()
        {
            return "Joe l'anticonformiste";
        }

        private void loadMusic()
        {
            //TODO : Optimiser tout ca, mettre des dictionnary voire faire un truc automatique
            musiques.Clear();
            musiques.Add( new string[] { "balle1", "joeBalle1.wav" } );
            musiques.Add( new string[] { "balle2", "joeBalle2.wav" } );
            musiques.Add( new string[] { "balle3", "joeBalle3.wav" } );
            musiques.Add( new string[] { "balle4", "joeBalle4.wav" } );
            musiques.Add( new string[] { "balle5", "joeBalle5.wav" } );
            musiques.Add( new string[] { "balle6", "joeBalle6.wav" } );
            musiques.Add( new string[] { "balle7", "joeBalle7.wav" } );
            musiques.Add( new string[] { "balle8", "joeBalle8.wav" } );
            musiques.Add( new string[] { "balle9", "joeBalle9.wav" } );
            musiques.Add( new string[] { "balle10", "joeBalle10.wav" } );
            musiques.Add( new string[] { "insulte1", "joeInsulte1.wav" } );
            musiques.Add( new string[] { "insulte2", "joeInsulte2.wav" } );
            musiques.Add( new string[] { "insulte3", "joeInsulte3.wav" } );
            musiques.Add( new string[] { "insulte4", "joeInsulte4.wav" } );
            musiques.Add( new string[] { "insulte5", "joeInsulte5.wav" } );
            musiques.Add( new string[] { "insulte6", "joeInsulte6.wav" } );
            musiques.Add( new string[] { "insulte7", "joeInsulte7.wav" } );
            musiques.Add( new string[] { "voiture1", "joeVoiture1.wav" } );
            musiques.Add( new string[] { "voiture2", "joeVoiture2.wav" } );
            musiques.Add( new string[] { "voiture3", "joeVoiture3.wav" } );
            musiques.Add( new string[] { "voiture4", "joeVoiture4.wav" } );
            musiques.Add( new string[] { "voiture5", "joeVoiture5.wav" } );
            musiques.Add( new string[] { "voiture6", "joeVoiture6.wav" } );
            musiques.Add( new string[] { "voiture7", "joeVoiture7.wav" } );
            musiques.Add( new string[] { "voiture8", "joeVoiture8.wav" } );
            musiques.Add( new string[] { "dialogue0", "joeDialogue0.wav" } );
            musiques.Add( new string[] { "dialogue1", "joeDialogue1.wav" } );
            musiques.Add( new string[] { "dialogue2", "joeDialogue2.wav" } );
            musiques.Add( new string[] { "dialogue3", "joeDialogue3.wav" } );
            musiques.Add( new string[] { "dialogue4", "joeDialogue4.wav" } );
            musiques.Add( new string[] { "dialogue5", "joeDialogue5.wav" } );
            musiques.Add( new string[] { "dialogue6", "joeDialogue6.wav" } );
            musiques.Add( new string[] { "dialogue7", "joeDialogue7.wav" } );
            musiques.Add( new string[] { "dialogue8", "joeDialogue8.wav" } );
            musiques.Add( new string[] { "dialogue9", "joeDialogue9.wav" } );
            musiques.Add( new string[] { "dialogue10", "joeDialogue10.wav" } );
            musiques.Add( new string[] { "musique1", "joeAnticonformiste.wav" } );
            musiques.Add( new string[] { "musique2", "joeLesFlics.wav" } );
            musiques.Add( new string[] { "musique3", "joeDegueulasse.wav" } );
            musiques.Add( new string[] { "flics1", "joeFlics1.wav" } );
            musiques.Add( new string[] { "flics2", "joeFlics2.wav" } );
            musiques.Add( new string[] { "flics3", "joeFlics3.wav" } );
            musiques.Add( new string[] { "flics4", "joeFlics4.wav" } );
            musiques.Add( new string[] { "flics5", "joeFlics5.wav" } );
            musiques.Add( new string[] { "nadine", "joeNadine.wav" } );
            musiques.Add( new string[] { "amphiHoo1", "joeDegueulasseHoo.wav" } );
            musiques.Add( new string[] { "amphiHoo2", "joeDegueulasseHoo2.wav" } );
        }

        private void createBike(Vector3 position)
        {
            if (Joe.bike == null || !Joe.bike.Exists())
            {
                Joe.bike = World.CreateVehicle(VehicleHash.TriBike, position);
            }
            else
            {
                Joe.bike.Position = position;
            }
        }

        protected override void doInitialization()
        {
            base.doInitialization();

            loadMusic();

            musicPlaylist = new Music(musiques);
            musicPlaylist.setVolume(0.9f);

            #region Goals 
            
            GoToPosition goToFirstSongGoal = new GoToPosition(firstSongPosition);
            goToFirstSongGoal.OnGoalStart += (sender) =>
            {
                createBike(bikePositionAtHome);

                bikeRegen = false;
                playerDown = true;
                playerWalked = false;
                playerMoved = false;
                introEnded = false;
                cameraChangeTimer = 0;
                etapeMission = 0;
                nbMusiqueEtape = -1;
                
                World.Weather = Weather.ExtraSunny;
                Tools.setClockTime(10);
                Tools.setDemagoModel(DemagoModel.Joe);

                Ped player = Game.Player.Character;
                player.MaxHealth = 300;
                player.Armor = 100;
                player.Health = player.MaxHealth;
                Function.Call(Hash.SET_PED_MAX_HEALTH, player, player.MaxHealth);

                Tools.TeleportPlayer(joeStart, false);
                introPed = Function.Call<Ped>(Hash.CLONE_PED, Game.Player.Character, Function.Call<int>(Hash.GET_ENTITY_HEADING, Function.Call<int>(Hash.PLAYER_PED_ID)), false, true);
                Tools.TeleportPlayer(joeHomePosition);

                player.IsVisible = false;
                player.Heading += 35;
                player.Task.StandStill(-1);

                introPed.Task.PlayAnimation("amb@world_human_picnic@male@base", "base", 8f, -1, true, -1f);

                //Camera gestion
                Function.Call(Hash.DISPLAY_HUD, false);
                Function.Call(Hash.DISPLAY_RADAR, false);

                Vector3 largeShotPosition = new Vector3(2213.186f, 2510.148f, 82.73711f);
                Vector3 firstShotPosition = new Vector3(2361.558f, 2527.512f, 46.66772f);
                Vector3 secondShotPosition = new Vector3(2351.906f, 2530.494f, 48f);

                List<CameraShot> cameraShots = new List<CameraShot>();
                float time_split = musicPlaylist.length( "dialogue0" ) / 3;

                CameraShot cameraShot = new CameraShot( time_split, largeShotPosition );
                cameraShot.lookAt( introPed );
                cameraShots.Add(cameraShot);

                cameraShot = new CameraShot( time_split, firstShotPosition, secondShotPosition );
                cameraShot.lookAt( introPed );
                cameraShots.Add(cameraShot);

                cameraShot = new CameraShot( time_split, secondShotPosition, bikePositionAtHome );
                cameraShot.lookAt( introPed );
                cameraShots.Add(cameraShot);
                
                CameraShotsList.Instance.initialize( cameraShots, musicPlaylist.length( "dialogue0" ) );
                
                Joe.bike.EnginePowerMultiplier = 100;
                Joe.bike.IsInvincible = true;
                Joe.bike.CanTiresBurst = false;

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
                        Tools.log("GoTO sequence on spectator ped ");
                    }
                }

                musicPlay("dialogue0");
            };

            Goal firstSongGoals = new PlayInstrument(InstrumentHash.Guitar, musicPlaylist.length("musique1"), "musique1", musicPlaylist);
            firstSongGoals.OnGoalStart += (sender) =>
            {
                Tools.setClockTime(11, musicPlaylist.length("musique1"));

                if (currentPlay != "")
                    musicPlaylist.pauseMusic(currentPlay);

                currentPlay = "musique1";

                Vector3 firstCameraPosition = firstSongPosition;
                firstCameraPosition.X += 8;
                firstCameraPosition.Z += 2;
                Vector3 secondCameraPosition = firstCameraPosition;
                secondCameraPosition.X += 8;
                secondCameraPosition.Z += 2;
                Vector3 thirdCameraPosition = secondCameraPosition;
                thirdCameraPosition.X += 8;
                thirdCameraPosition.Z += 2;
                Vector3 fourthCameraPosition = thirdCameraPosition;
                fourthCameraPosition.X -= 8;
                fourthCameraPosition.Y -= 8;

                List<CameraShot> cameraShots = new List<CameraShot>();

                CameraShot cameraShot = new CameraShot(musicPlaylist.length("musique1") / 3, firstCameraPosition, secondCameraPosition);
                cameraShot.lookAt( Game.Player.Character );
                cameraShots.Add(cameraShot);

                cameraShot = new CameraShot(musicPlaylist.length("musique1") / 3, secondCameraPosition, thirdCameraPosition);
                cameraShot.lookAt( Game.Player.Character );
                cameraShots.Add(cameraShot);

                cameraShot = new CameraShot(musicPlaylist.length("musique1") / 3, thirdCameraPosition, fourthCameraPosition);
                cameraShot.lookAt( Game.Player.Character );
                cameraShots.Add(cameraShot);
                
                CameraShotsList.Instance.initialize( cameraShots, musicPlaylist.length( "musique1" ) );

                Ped player = Game.Player.Character;

                foreach (Ped spectator in spectatorsPeds)
                {
                    if (spectator.Position.DistanceTo(firstSongPosition) > 10)
                    {
                        spectator.Position = firstSongPosition.Around(7).Around(2);
                    }

                    TaskSequence angrySpectator = new TaskSequence();
                    angrySpectator.AddTask.ClearAllImmediately();
                    angrySpectator.AddTask.TurnTo(player, 1000);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_what_hard", 8f, -1, false, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_nod_yes_soft", 0.01f, musicPlaylist.length("musique1") - 6000, true, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_you_soft", 8f, -1, false, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_nod_no_hard", 8f, -1, true, -1f);

                    spectator.Task.PerformSequence(angrySpectator);
                }
            };
            
            GoToPositionInVehicle goToPoliceWithBikeGoal = new GoToPositionInVehicle(roadFaceToPoliceStationPosition);
            goToPoliceWithBikeGoal.OnGoalStart += (sender) =>
            {
                createBike(bikePositionAtHome);
                goToPoliceWithBikeGoal.setVehicle(Joe.bike);

                currentPlay = "";
                Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 0, 1, 1);

                foreach (Ped spectator in spectatorsPeds)
                {
                    if (spectator != null && spectator.Exists())
                    {
                        spectator.Task.ClearAllImmediately();
                        if (random.Next(0, 1) == 0)
                        {
                            spectator.Task.FightAgainst(Game.Player.Character);
                        }
                        else
                        {
                            spectator.Task.UseMobilePhone();
                        }

                    }
                }

                Ped player = Game.Player.Character;
                player.Health = 300;
                player.Armor = 100;
                GTA.UI.ShowSubtitle("Spectateurs : C'est nul ! Casse toi ! On a appelé les flics !", 3000);

                Tools.setClockTime(14, 50000);
                Game.Player.WantedLevel = 2;
                World.Weather = Weather.Clouds;

                etapeMission = 1;
            };

            GoToPosition goToSecondSongGoal = new GoToPosition(secondSongPosition);
            goToSecondSongGoal.OnGoalStart += (sender) =>
            {
                List<PedHash> spectatorsHashesSecondSong = new List<PedHash>();
                spectatorsHashesSecondSong.Add(PedHash.Cop01SFY);
                spectatorsHashesSecondSong.Add(PedHash.Cop01SMY);
                spectatorsHashesSecondSong.Add(PedHash.Cop01SFY);
                spectatorsHashesSecondSong.Add(PedHash.Cop01SMY);

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
            };

            Goal secondSongGoals = new PlayInstrument(InstrumentHash.Guitar, musicPlaylist.length("musique2"), "musique2", musicPlaylist);
            secondSongGoals.OnGoalStart += (sender) =>
            {
                if (currentPlay != "")
                    musicPlaylist.pauseMusic(currentPlay);

                Tools.setClockTime(16, musicPlaylist.length("musique2"));
                currentPlay = "musique2";

                World.Weather = Weather.Clouds;

                foreach (Ped spectator in spectatorsPeds)
                {
                    if (spectator != null && spectator.Exists())
                    {
                        spectator.Delete();
                    }
                }
                
                Ped player = Game.Player.Character;

                foreach (Ped spectator in World.GetNearbyPeds(player, 15))
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

                foreach (Ped spectator in spectatorsPeds2)
                {
                    TaskSequence angrySpectator = new TaskSequence();
                    angrySpectator.AddTask.ClearAllImmediately();
                    angrySpectator.AddTask.TurnTo(player, 1000);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_what_hard", 8f, -1, false, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_nod_yes_soft", 0.01f, musicPlaylist.length("musique2") - 6000, true, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_you_soft", 8f, -1, false, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_nod_no_hard", 8f, -1, true, -1f);

                    spectator.Task.PerformSequence(angrySpectator);
                }
                
                Vector3 firstCameraPosition = secondSongPosition;
                firstCameraPosition.X += 4;
                firstCameraPosition.Z += 2;
                Vector3 secondCameraPosition = firstCameraPosition;
                secondCameraPosition.X -= 4;
                secondCameraPosition.Y += 4;
                Vector3 thirdCameraPosition = secondCameraPosition;
                thirdCameraPosition.X -= 4;
                thirdCameraPosition.Y -= 4;
                Vector3 fourthCameraPosition = thirdCameraPosition;
                fourthCameraPosition.X += 4;
                fourthCameraPosition.Y += 4;

                List<CameraShot> cameraShots = new List<CameraShot>();

                CameraShot cameraShot = new CameraShot(musicPlaylist.length("musique2") / 3, firstCameraPosition, secondCameraPosition);
                cameraShot.lookAt( Game.Player.Character );
                cameraShots.Add(cameraShot);

                cameraShot = new CameraShot(musicPlaylist.length("musique2") / 3, secondCameraPosition, thirdCameraPosition);
                cameraShot.lookAt( Game.Player.Character );
                cameraShots.Add(cameraShot);

                cameraShot = new CameraShot(musicPlaylist.length("musique2") / 3, thirdCameraPosition, fourthCameraPosition);
                cameraShot.lookAt( Game.Player.Character );
                cameraShots.Add(cameraShot);
                
                CameraShotsList.Instance.initialize( cameraShots, musicPlaylist.length( "musique2" ) );

            };
            
            GoToPositionInVehicle goToTheaterWithBikeGoal = new GoToPositionInVehicle(thirdSongBikePosition);
            goToTheaterWithBikeGoal.OnGoalStart += (sender) =>
            {
                createBike(roadFaceToPoliceStationPosition);
                goToTheaterWithBikeGoal.setVehicle(Joe.bike);

                Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 0, 1, 1);

                Ped player = Game.Player.Character;
                if (player.Position.DistanceTo(secondSongPosition) > 50)
                    Tools.TeleportPlayer(secondSongPosition, false);
                
                currentPlay = "";

                foreach (Ped ped in copsPeds)
                {
                    if (ped != null && ped.Exists())
                    {
                        ped.Weapons.Give(WeaponHash.Pistol, 1, true, true);
                        Function.Call(Hash.SET_PED_AS_COP, ped, true);
                        ped.MarkAsNoLongerNeeded();
                    }
                }

                foreach (Ped spectator in spectatorsPeds2)
                {
                    if (spectator != null && spectator.Exists())
                    {
                        spectator.Task.ClearAllImmediately();
                        if (random.Next(0, 1) == 0)
                        {
                            spectator.Task.FleeFrom(Game.Player.Character);
                        }
                        else
                        {
                            spectator.Task.UseMobilePhone();
                        }
                    }
                }

                List<PedHash> spectatorsHashesThirdSong = new List<PedHash>();
                spectatorsHashesThirdSong.Add(PedHash.Beach01AFM);
                spectatorsHashesThirdSong.Add(PedHash.MovAlien01);
                spectatorsHashesThirdSong.Add(PedHash.Jesus01);
                spectatorsHashesThirdSong.Add(PedHash.Zombie01);

                for (int num = 0; num < 120; num++)
                {
                    Ped ped = World.CreatePed(spectatorsHashesThirdSong.ElementAt<PedHash>(random.Next(spectatorsHashesThirdSong.Count)), thirdSongPublicPosition1 + (float)random.NextDouble() * thirdSongPublicPosition2 + (float)random.NextDouble() * thirdSongPublicPosition3);

                    if (ped != null && ped.Exists())
                    {
                        if (ped.Model == PedHash.MovAlien01)
                        {
                            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped.Handle, 0, 0, 0, 2);
                            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped.Handle, 3, 0, 0, 2);
                            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped.Handle, 4, 1, 0, 2);
                        }

                        spectatorsPeds3.Add(ped);
                    }
                }

                while (nadineMorano == null || !nadineMorano.Exists())
                {
                    nadineMorano = World.CreatePed(PedHash.Business02AFM, thirdSongPosition);
                    nadineMorano.Task.TurnTo(spectatorsPeds3[0]);
                }

                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, nadineMorano.Handle, 2, 1, 2, 2);

                player.Health = 300;
                player.Armor = 100;
                
                World.Weather = Weather.Clouds;
                Tools.setClockTime(18, 40000);
                etapeMission = 2;

                Script.Wait(2000);
                Game.Player.WantedLevel = 1;
            };
            goToTheaterWithBikeGoal.OnFirstTimeOnVehicle += (sender, vehicle) => {
                Ped player = Game.Player.Character;
                player.Health = 300;
                player.Armor = 100;
                Game.Player.WantedLevel = 3;
            };

            GoToPosition goToThirdSongPosition = new GoToPosition(thirdSongPosition);

            Timer chansonHoo2;
            Goal thirdSongGoals = new PlayInstrument(InstrumentHash.Guitar, musicPlaylist.length("musique3"), "musique3", musicPlaylist);
            thirdSongGoals.OnGoalStart += (sender) =>
            {
                Ped player = Game.Player.Character;

                if (currentPlay != "")
                    musicPlaylist.pauseMusic(currentPlay);

                nadineMorano.Task.FleeFrom(player);
                musicPlaylist.playMusic("nadine");
                musicPlaylist.playMusic("amphiHoo1");
                
                currentPlay = "musique3";
                chansonHoo2 = new Timer(musicPlaylist.length("musique3") - 19000);
                chansonHoo2.OnTimerStop += (timerSender) =>
                {
                    musicPlaylist.playMusic("amphiHoo2");
                };

                foreach (Ped spectator in spectatorsPeds2)
                {
                    if (spectator != null && spectator.Exists())
                    {
                        spectator.Delete();
                    }
                }

                foreach (Ped ped in copsPeds)
                {
                    if (ped != null && ped.Exists())
                    {
                        ped.Delete();
                    }
                }

                player.Heading = 180;
                Game.Player.WantedLevel = 0;

                Function.Call(Hash.TASK_TURN_PED_TO_FACE_COORD, player.Handle, 640f, 448f, 100f, -1);
                Tools.setClockTime(20, musicPlaylist.length("musique3"));

                foreach (Ped spectator in spectatorsPeds3)
                {
                    TaskSequence angrySpectator = new TaskSequence();
                    angrySpectator.AddTask.ClearAllImmediately();
                    angrySpectator.AddTask.TurnTo(player, 1000);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_what_hard", 8f, -1, false, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_nod_yes_soft", 0.01f, musicPlaylist.length("musique3") - 6000, true, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_you_soft", 8f, -1, false, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_nod_no_hard", 8f, -1, true, -1f);

                    spectator.Task.PerformSequence(angrySpectator);
                }
                
                Vector3 firstCameraPosition = secondSongPosition;
                firstCameraPosition.X += 8;
                firstCameraPosition.Z += 2;
                Vector3 secondCameraPosition = firstCameraPosition;
                secondCameraPosition.X -= 8;
                secondCameraPosition.Y += 8;
                Vector3 thirdCameraPosition = secondCameraPosition;
                thirdCameraPosition.X -= 8;
                thirdCameraPosition.Y -= 8;
                Vector3 fourthCameraPosition = thirdCameraPosition;
                fourthCameraPosition.X += 8;
                fourthCameraPosition.Y += 8;

                List<CameraShot> cameraShots = new List<CameraShot>();

                CameraShot cameraShot = new CameraShot(musicPlaylist.length("musique3") / 3, firstCameraPosition, secondCameraPosition);
                cameraShot.lookAt( Game.Player.Character );
                cameraShots.Add(cameraShot);

                cameraShot = new CameraShot(musicPlaylist.length("musique3") / 3, secondCameraPosition, thirdCameraPosition);
                cameraShot.lookAt( Game.Player.Character );
                cameraShots.Add(cameraShot);

                cameraShot = new CameraShot(musicPlaylist.length("musique3") / 3, thirdCameraPosition, fourthCameraPosition);
                cameraShot.lookAt( Game.Player.Character );
                cameraShots.Add(cameraShot);
                
                CameraShotsList.Instance.initialize( cameraShots, musicPlaylist.length( "musique2" ) );

                World.Weather = Weather.Clouds;
            };

            GoToPositionInVehicle goToHome = new GoToPositionInVehicle(joeHomePosition);
            goToHome.OnGoalStart += (sender) =>
            {
                createBike(thirdSongBikePosition);
                goToHome.setVehicle(Joe.bike);

                Ped player = Game.Player.Character;
                if (player.Position.DistanceTo(thirdSongPosition) > 50)
                    Tools.TeleportPlayer(thirdSongPosition, false);
                
                currentPlay = "";

                chansonHoo2 = null;
                Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 0, 1, 1);

                player.Health = 300;
                player.Armor = 100;
                bikeRegen = true;
                Game.Player.WantedLevel = 4;
                Tools.setClockTime(21, 10000);
                World.Weather = Weather.ThunderStorm;

                int number = 0;

                foreach (Ped ped in spectatorsPeds3)
                {
                    if (ped != null && ped.Exists() && ped != player)
                    {
                        ped.Task.ClearAllImmediately();
                        if (number < 10)
                        {
                            ped.Task.FightAgainst(Game.Player.Character);
                        }
                        else
                        {
                            if (number < 55)
                            {
                                ped.Task.ReactAndFlee(Game.Player.Character);
                            }
                            else
                            {
                                ped.Task.FleeFrom(Game.Player.Character);
                            }
                        }

                        number++;
                    }

                }

                GTA.UI.ShowSubtitle("Spectateurs : C'est nul ! Casse toi ! On a encore appelé les flics ! Tu vas avoir des problèmes !", 3000);
                etapeMission = 3;
            };
            goToHome.OnFirstTimeOnVehicle += (sender, vehicle) => {
                goToHome.setAdviceText("Evite les routes pour éviter les voitures de police");
                Tools.setClockTime(22, 10000);
            };
            goToHome.OnGoalAccomplished += (sender, elapsedTime) => {
                musicPlay("dialogue10");
            };

            addGoal(goToFirstSongGoal);
            addGoal(firstSongGoals);
            addGoal(goToPoliceWithBikeGoal);
            addGoal(goToSecondSongGoal);
            addGoal(secondSongGoals);
            addGoal(goToTheaterWithBikeGoal);
            addGoal(goToThirdSongPosition);
            addGoal(thirdSongGoals);
            addGoal(goToHome);

            #endregion

            #region Checkpoints
            
            Checkpoint firstCheckpoint = new Checkpoint(goToFirstSongGoal);
            Checkpoint secondCheckpoint = new Checkpoint(goToTheaterWithBikeGoal);
            Checkpoint thirdCheckpoint = new Checkpoint(goToHome);

            addCheckpoint(firstCheckpoint);
            addCheckpoint(secondCheckpoint);
            addCheckpoint(thirdCheckpoint);

            #endregion

            startTime = DemagoScript.getScriptTime();
        }

        private void musicPlay(string musique)
        {
            currentPlay = musique;
            musicPlaylist.playMusic(currentPlay);
        }

        public override void setPause(bool isPaused)
        {
            base.setPause(isPaused);
            if (musicPlaylist != null)
            {
                if (isPaused)
                {
                    musicPlaylist.pauseMusic(currentPlay);
                    Script.Wait(1000);
                }
                else
                {
                    musicPlaylist.playMusic(currentPlay);
                }
            }
        }

        public override void clear(bool removePhysicalElements = false, bool keepGoalsList = false)
        {
            base.clear(removePhysicalElements, keepGoalsList);

            if (introPed != null && introPed.Exists())
                introPed.Delete();

            if (nadineMorano != null && nadineMorano.Exists())
            {
                if (removePhysicalElements)
                    nadineMorano.Delete();

                nadineMorano.MarkAsNoLongerNeeded();
            }
            
            if (musicPlaylist != null)
            {
                musicPlaylist.pauseMusic(currentPlay);
                if (!keepGoalsList)
                {
                    musicPlaylist.dispose();
                }
            }

            World.Weather = Weather.Clear;

            if (bike != null && Joe.bike.Exists())
            {
                if (removePhysicalElements)
                    Joe.bike.Delete();

                Joe.bike.MarkAsNoLongerNeeded();
            }

            foreach (Ped spectator in spectatorsPeds)
                if (spectator != null && spectator.Exists())
                {
                    spectator.MarkAsNoLongerNeeded();
                    if (removePhysicalElements)
                        spectator.Delete();
                }

            spectatorsPeds.Clear();

            foreach (Ped spectator in copsPeds)
                if (spectator != null && spectator.Exists())
                {
                    spectator.MarkAsNoLongerNeeded();
                    if (removePhysicalElements)
                        spectator.Delete();
                }

            copsPeds.Clear();

            foreach (Ped spectator in spectatorsPeds2)
                if (spectator != null && spectator.Exists())
                {
                    spectator.MarkAsNoLongerNeeded();
                    if (removePhysicalElements)
                        spectator.Delete();
                }

            spectatorsPeds2.Clear();

            foreach (Ped spectator in spectatorsPeds3)
                if (spectator != null && spectator.Exists())
                {
                    spectator.MarkAsNoLongerNeeded();
                    if (removePhysicalElements)
                        spectator.Delete();
                }

            spectatorsPeds2.Clear();
        }

        public override void update()
        {
            base.update();

            updateIntro();

            updatePlaylist();
            
            playAmbiance();

            updateSubtitles();

            Ped player = Game.Player.Character;
            if (player.IsInVehicle() && player.CurrentVehicle == bike)
                playerLifeUpCounter++;
            else
                playerLifeUpCounter = 0;

            if (playerLifeUpCounter >= GTA.Game.FPS / 5 && bikeRegen && Joe.bike.Speed > 0) {
                playerLifeUpCounter = 0;
                if (player.Health < player.MaxHealth)
                    player.Health++;
                if (player.Armor < 100)
                    player.Armor++;
            }

            Game.Player.Character.Weapons.RemoveAll();
        }

        private void updatePlaylist()
        {
            if (currentPlay != "")
            {
                if (interruptPlay != "" && currentInterruptPlay == "")
                {
                    currentInterruptPlay = interruptPlay;
                    interruptPlay = "";

                    if (!currentPlay.StartsWith("flic"))
                        musicPlaylist.pauseMusic(currentPlay);

                    musicPlaylist.playMusic(currentInterruptPlay);
                }

                if (currentInterruptPlay != "" && musicPlaylist.isFinished(currentInterruptPlay))
                {
                    musicPlaylist.playMusic(currentPlay);
                    musicPlaylist.restart(currentInterruptPlay);
                    musicPlaylist.pauseMusic(currentInterruptPlay);
                    Function.Call(Hash.CLEAR_ENTITY_LAST_DAMAGE_ENTITY, Game.Player.Character);
                    Function.Call(Hash.CLEAR_ENTITY_LAST_WEAPON_DAMAGE, Game.Player.Character);
                    currentInterruptPlay = "";
                }

                if (musicPlaylist.isFinished(currentPlay))
                    currentPlay = "";
            }
            else
            {
                if (interruptPlay != "" && currentInterruptPlay == "")
                {
                    currentInterruptPlay = interruptPlay;
                    interruptPlay = "";
                    musicPlaylist.playMusic(currentInterruptPlay);
                }

                if (currentInterruptPlay != "" && musicPlaylist.isFinished(currentInterruptPlay))
                {
                    musicPlaylist.restart(currentInterruptPlay);
                    musicPlaylist.pauseMusic(currentInterruptPlay);
                    Function.Call(Hash.CLEAR_ENTITY_LAST_DAMAGE_ENTITY, Game.Player.Character);
                    Function.Call(Hash.CLEAR_ENTITY_LAST_WEAPON_DAMAGE, Game.Player.Character);
                    currentInterruptPlay = "";
                }
            }
        }

        public void playAmbiance()
        {
            if (currentPlay == "" || musicPlaylist.isFinished(currentPlay))
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

            if (interruptPlay == "")
            {
                if (Function.Call<Boolean>(Hash.HAS_PED_BEEN_DAMAGED_BY_WEAPON, Game.Player.Character, 0, 2))
                {
                    int next = random.Next(10) + 1;
                    interruptPlay = "balle" + next;
                }
                else
                {
                    if (Function.Call<Boolean>(Hash.HAS_ENTITY_BEEN_DAMAGED_BY_ANY_VEHICLE, Game.Player.Character))
                    {
                        int next = random.Next(8) + 1;
                        interruptPlay = "voiture" + next;
                    }
                    else
                    {
                        if (Function.Call<Boolean>(Hash.HAS_ENTITY_BEEN_DAMAGED_BY_ANY_OBJECT, Game.Player.Character))
                        {
                            int next = random.Next(7) + 1;
                            interruptPlay = "insulte" + next;
                        }
                    }
                }
            }
        }

        private void updateSubtitles()
        {
            string subtitle = "";
            foreach (string[] musiqueNames in musiques)
            {
                if (musiqueNames[0] == currentPlay && musicPlaylist.isPlaying(currentPlay))
                {
                    string songFile = musiqueNames[1];
                    subtitle = Subtitles.getSubtitle(songFile, (int)musicPlaylist.getPlayingPosition(currentPlay));
                }
            }

            if (subtitle != "")
            {
                GTA.UI.ShowSubtitle(subtitle);
            }
        }

        private void updateIntro()
        {
            TimeSpan missionElapsedTime = DateTime.Now - startMissionTime;
            if (!introEnded && startTime >= missionElapsedTime.TotalSeconds)
            {
                Ped player = Game.Player.Character;

                if (player.IsVisible)
                {
                    player.IsVisible = false;
                }

                float elapsedTime = DemagoScript.getScriptTime() - startTime;
                float musicTime = musicPlaylist.length("dialogue0");
                float musicTimeSplit = musicTime / 3;

                if (Game.IsKeyPressed(System.Windows.Forms.Keys.Back))
                {
                    CameraShotsList.Instance.reset();
                    elapsedTime = musicTime + 1;
                    playerDown = false;
                    playerMoved = true;
                    playerWalked = true;
                }

                if (elapsedTime > musicTimeSplit + 2000 && playerDown)
                {
                    introPed.Task.PlayAnimation("amb@world_human_picnic@male@exit", "exit", 8f, 3000, false, -1f);
                    playerDown = false;
                }
                if (elapsedTime > musicTimeSplit + 5000 && !playerWalked)
                {
                    introPed.Task.ClearAllImmediately();
                    introPed.Task.GoTo(joeHomePosition, true);
                    playerWalked = true;
                }
                if (elapsedTime > musicTimeSplit*2 && !playerMoved)
                {
                    introPed.Task.ClearAllImmediately();
                    introPed.Task.PlayAnimation("gestures@m@standing@casual", "gesture_why", 8f, -1, false, -1f);
                    playerMoved = true;
                }
                if (elapsedTime > musicTime && !introEnded)
                {
                    musicPlaylist.pauseMusic("dialogue0");
                    currentPlay = "";
                    player.Task.ClearAllImmediately();
                    introPed.IsVisible = false;
                    introPed.Delete();
                    player.IsVisible = true;
                    Function.Call(Hash.DISPLAY_HUD, true);
                    Function.Call(Hash.DISPLAY_RADAR, true);
                    Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 0, 1, 1);
                    introEnded = true;
                }
                else
                    cameraChangeTimer++;
            }
        }
    }
}
