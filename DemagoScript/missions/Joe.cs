using DemagoScript.GUI;
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
        #region Positions
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
        #endregion

        public static Vehicle bike = null;
        private Ped introPed = null, nadineMorano = null;
        private List<Ped> spectatorsPeds = new List<Ped>(), copsPeds = new List<Ped>(), spectatorsPeds3 = new List<Ped>();
        
        private Random random = new Random();

        #region Regeneration variables
        private bool bikeRegen = false;
        private int playerLifeUpCounter = 0;
        #endregion
        
        #region Intro variables
        private bool playerDown = false;
        private bool playerWalked = false;
        private bool playerMoved = false;
        private bool introEnded = false;
        #endregion

        public Joe()
        {
            this.name = "Joe l'anticonformiste";
        }

        public override void checkRequiredElements()
        {
            base.checkRequiredElements();
            
            // Si le velo existe
            if (Joe.bike != null && Joe.bike.Exists()) {
                // Et qu'il est loin du joueur
                if ( Joe.bike.Position.DistanceTo( bikePositionAtHome ) > 5 ) {
                    // On le replace
                    Joe.bike.Position = bikePositionAtHome;
                }
            }

            // Si le velo n'existe pas
            while (Joe.bike == null || !Joe.bike.Exists()) {
                // On le créer à sa position initiale
                Joe.bike = World.CreateVehicle(VehicleHash.TriBike, bikePositionAtHome);
            }

            Joe.bike.EnginePowerMultiplier = 100;
            Joe.bike.IsInvincible = true;
            Joe.bike.CanTiresBurst = false;

            Tools.setDemagoModel(DemagoModel.Joe);

            Ped player = Game.Player.Character;
            player.MaxHealth = 300;
            Function.Call(Hash.SET_PED_MAX_HEALTH, player, player.MaxHealth);

            CameraShotsList.Instance.reset();

            AudioManager.Instance.FilesSubFolder = @"joe\joe";
        }

        public override void populateDestructibleElements()
        {
            Game.FadeScreenOut( 500 );

            base.populateDestructibleElements();
            
            checkRequiredElements();

            #region Objectives 
            GoToPosition goToFirstSongObjective = new GoToPosition(firstSongPosition);
            goToFirstSongObjective.Checkpoint = new Checkpoint();
            goToFirstSongObjective.Checkpoint.addEntity(Joe.bike, bikePositionAtHome);
            goToFirstSongObjective.Checkpoint.PlayerPosition = joeStart;
            goToFirstSongObjective.Checkpoint.setClockHour(10);
            goToFirstSongObjective.Checkpoint.Health = 300;
            goToFirstSongObjective.Checkpoint.Armor = 100;
            goToFirstSongObjective.Checkpoint.Weather = Weather.ExtraSunny;
            goToFirstSongObjective.Checkpoint.WantedLevel = 0;
            goToFirstSongObjective.OnStarted += (sender) =>
            {   
                #region Intro cinematic
                bikeRegen = false;
                playerDown = true;
                playerWalked = false;
                playerMoved = false;
                introEnded = false;

                Tools.TeleportPlayer(joeStart, false);

                Ped player = Game.Player.Character;
                introPed = Function.Call<Ped>(Hash.CLONE_PED, player, Function.Call<int>(Hash.GET_ENTITY_HEADING, Function.Call<int>(Hash.PLAYER_PED_ID)), false, true);

                Tools.TeleportPlayer(joeHomePosition);

                player.IsVisible = false;
                player.Heading += 35;
                player.Task.StandStill(-1);

                introPed.Task.PlayAnimation("amb@world_human_picnic@male@base", "base", 8f, -1, true, -1f);
                
                Vector3 largeShotPosition = new Vector3(2213.186f, 2510.148f, 82.73711f);
                Vector3 firstShotPosition = new Vector3(2361.558f, 2527.512f, 46.66772f);
                Vector3 secondShotPosition = new Vector3(2351.906f, 2530.494f, 48f);

                List<CameraShot> cameraShots = new List<CameraShot>();
                float time_split = AudioManager.Instance.getLength("dialogue0") / 3;

                CameraShot cameraShot = new CameraShot(time_split, largeShotPosition);
                cameraShot.lookAt(introPed);
                cameraShots.Add(cameraShot);

                cameraShot = new CameraTraveling(time_split, firstShotPosition, secondShotPosition);
                cameraShot.lookAt(introPed);
                cameraShots.Add(cameraShot);

                cameraShot = new CameraTraveling(time_split, secondShotPosition, bikePositionAtHome);
                cameraShot.lookAt(introPed);
                cameraShots.Add(cameraShot);

                CameraShotsList.Instance.initialize(cameraShots, AudioManager.Instance.getLength("dialogue0"));
                
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
                    }
                }
                
                Function.Call( Hash.DISPLAY_RADAR, false );
                Game.FadeScreenIn( 500 );
                AudioManager.Instance.startSound("dialogue0");
                #endregion
            };

            AbstractObjective firstSongObjectives = new PlayInstrument(InstrumentHash.Guitar, "anticonformiste");
            firstSongObjectives.Checkpoint = new Checkpoint();
            firstSongObjectives.Checkpoint.Activable = true;
            firstSongObjectives.Checkpoint.addEntity(Joe.bike, bikePositionAtHome);
            firstSongObjectives.Checkpoint.PlayerPosition = firstSongPosition;
            firstSongObjectives.Checkpoint.setClockHour(11);
            firstSongObjectives.Checkpoint.WantedLevel = 0;
            firstSongObjectives.OnStarted += (sender) =>
            {
                #region Cinematic
                Vector3 firstCameraPosition = firstSongPosition;
                firstCameraPosition.Z += 2;
                firstCameraPosition.X += 4;
                firstCameraPosition.Y += 4;
                Vector3 secondCameraPosition = firstCameraPosition;
                secondCameraPosition.X -= 8;

                Vector3 thirdCameraPosition = Vector3.Zero;
                thirdCameraPosition.X = 2321;
                thirdCameraPosition.Y = 2555.7f;
                thirdCameraPosition.Z = firstSongPosition.Z;

                Vector3 fourthCameraPosition = Vector3.Zero;
                fourthCameraPosition.X = 2338;
                fourthCameraPosition.Y = 2548.5f;
                fourthCameraPosition.Z = firstSongPosition.Z;

                Vector3 fifthCameraPosition = firstSongPosition;
                fifthCameraPosition.X += 1;
                fifthCameraPosition.Z -= 1;
                Vector3 sixthCameraPosition = fifthCameraPosition;
                sixthCameraPosition.Z += 2;

                List<CameraShot> cameraShots = new List<CameraShot>();

                CameraShot cameraShot = new CameraTraveling(AudioManager.Instance.getLength("anticonformiste") / 4, firstCameraPosition, secondCameraPosition);
                cameraShot.lookAt(Game.Player.Character);
                cameraShots.Add(cameraShot);

                cameraShot = new CameraShot(AudioManager.Instance.getLength("anticonformiste") / 4, thirdCameraPosition);
                cameraShot.lookAt(Game.Player.Character);
                cameraShots.Add(cameraShot);

                cameraShot = new CameraShot(AudioManager.Instance.getLength("anticonformiste") / 4, fourthCameraPosition);
                cameraShot.lookAt(Game.Player.Character);
                cameraShots.Add(cameraShot);

                cameraShot = new CameraTraveling(AudioManager.Instance.getLength("anticonformiste") / 4, fifthCameraPosition, sixthCameraPosition);
                cameraShot.lookAt(Game.Player.Character);
                cameraShots.Add(cameraShot);

                CameraShotsList.Instance.initialize(cameraShots);

                Ped player = Game.Player.Character;
                foreach (Ped spectator in spectatorsPeds)
                {
                    TaskSequence angrySpectator = new TaskSequence();
                    angrySpectator.AddTask.ClearAllImmediately();
                    angrySpectator.AddTask.TurnTo(player, 1000);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_what_hard", 8f, -1, false, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_nod_yes_soft", 0.01f, AudioManager.Instance.getLength("anticonformiste") - 6000, true, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_you_soft", 8f, -1, false, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_nod_no_hard", 8f, -1, true, -1f);

                    spectator.Task.PerformSequence(angrySpectator);
                }
                #endregion
            };
            firstSongObjectives.OnAccomplished += (sender, elapsedTime) => {
                GTA.UI.ShowSubtitle("Spectateurs : C'est nul ! Casse toi ! On a appelé les flics !", 3000);

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
            }; 

            GoToPositionInVehicle goToPoliceWithBikeObjective = new GoToPositionInVehicle(roadFaceToPoliceStationPosition);
            goToPoliceWithBikeObjective.setVehicle(Joe.bike);
            goToPoliceWithBikeObjective.Checkpoint = new Checkpoint();
            goToPoliceWithBikeObjective.Checkpoint.addEntity(Joe.bike, bikePositionAtHome);
            goToPoliceWithBikeObjective.Checkpoint.PlayerPosition = firstSongPosition;
            goToPoliceWithBikeObjective.Checkpoint.setClockHour(14);
            goToPoliceWithBikeObjective.Checkpoint.Health = 300;
            goToPoliceWithBikeObjective.Checkpoint.Armor = 100;
            goToPoliceWithBikeObjective.Checkpoint.Weather = Weather.Clearing;
            goToPoliceWithBikeObjective.Checkpoint.WantedLevel = 2;
            goToPoliceWithBikeObjective.OnStarted += (sender) =>
            {
                AudioManager.Instance.startPlaylist(new string[] { "flics1", "dialogue1", "dialogue2", "dialogue3" });
            };

            GoToPosition goToSecondSongObjective = new GoToPosition(secondSongPosition);
            goToSecondSongObjective.Checkpoint = new Checkpoint();
            goToSecondSongObjective.Checkpoint.addEntity(Joe.bike, roadFaceToPoliceStationPosition);
            goToSecondSongObjective.Checkpoint.PlayerPosition = roadFaceToPoliceStationPosition;
            goToSecondSongObjective.OnStarted += (sender) =>
            {
                foreach (Ped spectator in spectatorsPeds)
                {
                    if (spectator != null && spectator.Exists())
                    {
                        spectator.MarkAsNoLongerNeeded();
                    }
                }

                List<PedHash> spectatorsHashesSecondSong = new List<PedHash> { PedHash.Cop01SFY, PedHash.Cop01SMY, PedHash.Cop01SFY, PedHash.Cop01SMY };
                foreach (PedHash hash in spectatorsHashesSecondSong)
                {
                    Ped ped = World.CreatePed(hash, secondSongPosition.Around(2));
                    if (ped != null && ped.Exists())
                    {
                        ped.Task.WanderAround(secondSongPosition, 5);
                        copsPeds.Add(ped);
                    }
                }
            };

            AbstractObjective secondSongObjectives = new PlayInstrument(InstrumentHash.Guitar, "lesFlics");
            secondSongObjectives.Checkpoint = new Checkpoint();
            secondSongObjectives.Checkpoint.Activable = true;
            secondSongObjectives.Checkpoint.addEntity(Joe.bike, roadFaceToPoliceStationPosition);
            secondSongObjectives.Checkpoint.PlayerPosition = secondSongPosition;
            secondSongObjectives.Checkpoint.setClockHour(16);
            secondSongObjectives.Checkpoint.Weather = Weather.Clouds;
            secondSongObjectives.Checkpoint.WantedLevel = 0;
            secondSongObjectives.OnStarted += (sender) =>
            {
                #region Cinematic
                Ped player = Game.Player.Character;
                player.Heading = 90;

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
                foreach (Ped spectator in copsPeds)
                {
                    if (spectator != null && spectator.Exists())
                    {
                        spectator.Task.PerformSequence(policeSurrounding);
                    }
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

                CameraShot cameraShot = new CameraTraveling(AudioManager.Instance.getLength("lesFlics") / 3, firstCameraPosition, secondCameraPosition);
                cameraShot.lookAt(Game.Player.Character);
                cameraShots.Add(cameraShot);

                cameraShot = new CameraTraveling(AudioManager.Instance.getLength("lesFlics") / 3, secondCameraPosition, thirdCameraPosition);
                cameraShot.lookAt(Game.Player.Character);
                cameraShots.Add(cameraShot);

                cameraShot = new CameraTraveling(AudioManager.Instance.getLength("lesFlics") / 3, thirdCameraPosition, fourthCameraPosition);
                cameraShot.lookAt(Game.Player.Character);
                cameraShots.Add(cameraShot);

                CameraShotsList.Instance.initialize(cameraShots, AudioManager.Instance.getLength("lesFlics"));

                #endregion
            };

            GoToPositionInVehicle goToTheaterWithBikeObjective = new GoToPositionInVehicle(thirdSongBikePosition);
            goToTheaterWithBikeObjective.setVehicle(Joe.bike);
            goToTheaterWithBikeObjective.Checkpoint = new Checkpoint();
            goToTheaterWithBikeObjective.Checkpoint.addEntity(Joe.bike, roadFaceToPoliceStationPosition);
            goToTheaterWithBikeObjective.Checkpoint.PlayerPosition = secondSongPosition;
            goToTheaterWithBikeObjective.Checkpoint.Health = 300;
            goToTheaterWithBikeObjective.Checkpoint.Armor = 100;
            goToTheaterWithBikeObjective.Checkpoint.WantedLevel = 1;
            goToTheaterWithBikeObjective.Checkpoint.Weather = Weather.Clouds;
            goToTheaterWithBikeObjective.Checkpoint.setClockHour(18, 40000);
            goToTheaterWithBikeObjective.OnStarted += (sender) =>
            {
                AudioManager.Instance.startPlaylist(new string[] { "flics2", "flics3", "flics4", "dialogue4", "dialogue5", "dialogue6" });

                foreach (Ped ped in copsPeds)
                {
                    if (ped != null && ped.Exists())
                    {
                        ped.Weapons.Give(WeaponHash.Pistol, 1, true, true);
                        Function.Call(Hash.SET_PED_AS_COP, ped, true);
                        ped.MarkAsNoLongerNeeded();
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
                }
                nadineMorano.Task.TurnTo( spectatorsPeds3[0] );
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, nadineMorano.Handle, 2, 1, 2, 2);
            };
            goToTheaterWithBikeObjective.OnFirstTimeOnVehicle += (sender, vehicle) => {
                Game.Player.WantedLevel = 3;
            };
            goToTheaterWithBikeObjective.OnAccomplished += (sender, elapsedTime) => {
                foreach (Ped ped in copsPeds)
                {
                    if (ped != null && ped.Exists())
                    {
                        ped.MarkAsNoLongerNeeded();
                    }
                }
            };

            GoToPosition goToThirdSongPosition = new GoToPosition(thirdSongPosition);
            goToThirdSongPosition.Checkpoint = new Checkpoint();
            goToThirdSongPosition.Checkpoint.addEntity(Joe.bike, thirdSongBikePosition);
            goToThirdSongPosition.Checkpoint.PlayerPosition = thirdSongBikePosition;

            Timer chansonHoo2 = null;
            AbstractObjective thirdSongObjectives = new PlayInstrument(InstrumentHash.Guitar, "degueulasse");
            thirdSongObjectives.Checkpoint = new Checkpoint();
            thirdSongObjectives.Checkpoint.Activable = true;
            thirdSongObjectives.Checkpoint.addEntity(Joe.bike, thirdSongBikePosition);
            thirdSongObjectives.Checkpoint.PlayerPosition = thirdSongPosition;
            thirdSongObjectives.Checkpoint.Weather = Weather.Clouds;
            thirdSongObjectives.Checkpoint.WantedLevel = 0;
            thirdSongObjectives.Checkpoint.setClockHour(20);
            thirdSongObjectives.OnStarted += (sender) =>
            {
                Ped player = Game.Player.Character;
                Function.Call(Hash.TASK_TURN_PED_TO_FACE_COORD, player.Handle, 640f, 448f, 100f, -1);

                #region Cinematic
                if (nadineMorano != null)
                {
                    nadineMorano.Task.FleeFrom(player);
                    AudioManager.Instance.startIndependantSound("nadine");
                }
                AudioManager.Instance.startIndependantSound("degueulasseHoo");

                chansonHoo2 = new Timer(AudioManager.Instance.getLength("degueulasse") - 19000);
                chansonHoo2.OnTimerStop += (timerSender) =>
                {
                    AudioManager.Instance.startIndependantSound("degueulasseHoo2");
                };

                foreach (Ped spectator in spectatorsPeds3)
                {
                    TaskSequence angrySpectator = new TaskSequence();
                    angrySpectator.AddTask.ClearAllImmediately();
                    angrySpectator.AddTask.TurnTo(player, 1000);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_what_hard", 8f, -1, false, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_nod_yes_soft", 0.01f, AudioManager.Instance.getLength("degueulasse") - 6000, true, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_you_soft", 8f, -1, false, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_nod_no_hard", 8f, -1, true, -1f);

                    spectator.Task.PerformSequence(angrySpectator);
                }

                Vector3 firstCameraPosition = thirdSongPosition;
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

                CameraShot cameraShot = new CameraTraveling(AudioManager.Instance.getLength("degueulasse") / 3, firstCameraPosition, secondCameraPosition);
                cameraShot.lookAt(Game.Player.Character);
                cameraShots.Add(cameraShot);

                cameraShot = new CameraTraveling(AudioManager.Instance.getLength("degueulasse") / 3, secondCameraPosition, thirdCameraPosition);
                cameraShot.lookAt(Game.Player.Character);
                cameraShots.Add(cameraShot);

                cameraShot = new CameraTraveling(AudioManager.Instance.getLength("degueulasse") / 3, thirdCameraPosition, fourthCameraPosition);
                cameraShot.lookAt(Game.Player.Character);
                cameraShots.Add(cameraShot);

                CameraShotsList.Instance.initialize(cameraShots, AudioManager.Instance.getLength( "degueulasse" ) );

                // ugly fix
                TaskSequence joeThirdSongSequence = new TaskSequence();
                joeThirdSongSequence.AddTask.ClearAllImmediately();
                joeThirdSongSequence.AddTask.TurnTo( spectatorsPeds3[0], 500 );
                joeThirdSongSequence.AddTask.PlayAnimation( "amb@world_human_musician@guitar@male@base", "base", 8f, -1, true, -1f );
                Game.Player.Character.Task.PerformSequence(joeThirdSongSequence);
                Game.Player.Character.Task.PlayAnimation( "amb@world_human_musician@guitar@male@base", "base", 8f, -1, true, -1f );
                #endregion
            };
            thirdSongObjectives.OnAccomplished += (sender, elapsedTime) =>
            {
                if (chansonHoo2 != null)
                    chansonHoo2.stop();

                int number = 0;
                foreach (Ped ped in spectatorsPeds3)
                {
                    if (ped != null && ped.Exists() && ped != Game.Player.Character)
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
            };

            GoToPositionInVehicle goToHome = new GoToPositionInVehicle(joeHomePosition);
            goToHome.setVehicle(Joe.bike);
            goToHome.Checkpoint = new Checkpoint();
            goToHome.Checkpoint.addEntity(Joe.bike, thirdSongBikePosition);
            goToHome.Checkpoint.PlayerPosition = thirdSongPosition;
            goToHome.Checkpoint.Health = 300;
            goToHome.Checkpoint.Armor = 100;
            goToHome.Checkpoint.WantedLevel = 4;
            goToHome.Checkpoint.Weather = Weather.ThunderStorm;
            goToHome.Checkpoint.setClockHour(23, 60000);
            goToHome.OnStarted += (sender) =>
            {
                bikeRegen = true;

                AudioManager.Instance.startPlaylist(new string[] { "flics5", "dialogue8", "dialogue9" });

                GTA.UI.ShowSubtitle("Spectateurs : C'est nul ! Casse toi ! On a encore appelé les flics ! Tu vas avoir des problèmes !", 3000);
            };
            goToHome.OnFirstTimeOnVehicle += (sender, vehicle) => {
                goToHome.AdviceText = "Evite les routes pour éviter les voitures de police";
            };
            goToHome.OnAccomplished += (sender, elapsedTime) => {
                AudioManager.Instance.startSound("dialogue10");
            };

            addObjective(goToFirstSongObjective);
            addObjective(firstSongObjectives);
            addObjective(goToPoliceWithBikeObjective);
            addObjective(goToSecondSongObjective);
            addObjective(secondSongObjectives);
            addObjective(goToTheaterWithBikeObjective);
            addObjective(goToThirdSongPosition);
            addObjective(thirdSongObjectives);
            addObjective(goToHome);

            #endregion

        }

        public override void depopulateDestructibleElements(bool removePhysicalElements = false)
        {
            base.depopulateDestructibleElements(removePhysicalElements);

            if (introPed != null && introPed.Exists())
            {
                introPed.Delete();
                introPed = null;
            }

            if (nadineMorano != null && nadineMorano.Exists())
            {
                nadineMorano.MarkAsNoLongerNeeded();

                if (removePhysicalElements)
                {
                    nadineMorano.Delete();
                    nadineMorano = null;
                }
            }

            AudioManager.Instance.stopAll();

            if (Joe.bike != null && Joe.bike.Exists())
            {
                Joe.bike.MarkAsNoLongerNeeded();

                if (removePhysicalElements)
                {
                    Joe.bike.Delete();
                    Joe.bike = null;
                }
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

            foreach (Ped spectator in spectatorsPeds3)
                if (spectator != null && spectator.Exists())
                {
                    spectator.MarkAsNoLongerNeeded();
                    if (removePhysicalElements)
                        spectator.Delete();
                }

            spectatorsPeds3.Clear();

            Ped player = Game.Player.Character;
            player.MaxHealth = 100;
            Function.Call(Hash.SET_PED_MAX_HEALTH, player, player.MaxHealth);

            World.Weather = Weather.Clear;

            player.Health = 100;
            player.Armor = 100;
        }
        
        public override bool update()
        {
            if (!base.update())
            {
                return false;
            }

            Ped player = Game.Player.Character;

            #region Intro gestion
            if (!introEnded)
            {
                if (player.IsVisible)
                {
                    player.IsVisible = false;
                }

                float elapsedMilliseconds = this.getElaspedTime();

                float musicTime = AudioManager.Instance.getLength("dialogue0");
                float musicTimeSplit = musicTime / 3;

                if (Game.IsKeyPressed(System.Windows.Forms.Keys.Back))
                {
                    CameraShotsList.Instance.reset();
                    AudioManager.Instance.clearSubtitles();
                    elapsedMilliseconds = musicTime + 1;
                    playerDown = false;
                    playerMoved = true;
                    playerWalked = true;
                    Function.Call( Hash.DISPLAY_RADAR, true );
                }

                if (elapsedMilliseconds > musicTimeSplit + 2000 && playerDown)
                {
                    introPed.Task.PlayAnimation("amb@world_human_picnic@male@exit", "exit", 8f, 3000, false, -1f);
                    playerDown = false;
                }
                if (elapsedMilliseconds > musicTimeSplit + 5000 && !playerWalked)
                {
                    introPed.Task.ClearAllImmediately();
                    introPed.Task.GoTo(joeHomePosition, true);
                    playerWalked = true;
                }
                if (elapsedMilliseconds > musicTimeSplit * 2 && !playerMoved)
                {
                    introPed.Task.ClearAllImmediately();
                    introPed.Task.PlayAnimation("gestures@m@standing@casual", "gesture_why", 8f, -1, false, -1f);
                    playerMoved = true;
                }
                if (elapsedMilliseconds > musicTime && !introEnded)
                {
                    AudioManager.Instance.stopAll();

                    foreach (Ped spectator in spectatorsPeds)
                        if (spectator.Position.DistanceTo(firstSongPosition) > 10)
                            spectator.Position = firstSongPosition.Around(7).Around(2);

                    player.Task.ClearAllImmediately();
                    introPed.IsVisible = false;
                    introPed.Delete();
                    introPed = null;
                    player.IsVisible = true;
                    
                    Function.Call( Hash.DISPLAY_RADAR, true );
                    Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 0, 1, 1);

                    introEnded = true;
                }
            }
            #endregion

            #region Regeneration
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
            #endregion

            Game.Player.Character.Weapons.RemoveAll();

            return true;
        }
    }
}
