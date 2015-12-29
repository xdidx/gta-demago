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
        public static Vector3 thirdSongPublicPosition1 { get; } = new Vector3(643.53f, 564.02f, 129.05f);
        public static Vector3 thirdSongPublicPosition2 { get; } = new Vector3(59.13f, -22.26f, 0.0f);
        public static Vector3 thirdSongPublicPosition3 { get; } = new Vector3(-16.09f, -20.29f, 1.0f);

        private static string[] sonsEtape1 = new string[] { "flics1", "dialogue1", "dialogue2", "dialogue3" };
        private static string[] sonsEtape2 = new string[] { "flics2", "flics3", "dialogue4", "dialogue5", "dialogue6" };
        private static string[] sonsEtape3 = new string[] { "flics4", "dialogue7", "dialogue8", "dialogue9" };

        private Vehicle bike = null;
        private List<Ped> spectatorsPeds = new List<Ped>();
        private List<Ped> spectatorsPeds2 = new List<Ped>();
        private List<Ped> copsPeds = new List<Ped>();
        private List<Ped> spectatorsPeds3 = new List<Ped>();
        private int playerLifeUpCounter = 0;
        private bool bikeRegen;
        private bool playerDown;
        private bool playerWalked;
        private bool introEnded;
        private int nbMusiqueEtape;
        private int cameraChangeTimer, etapeMission;
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
            musiques.Add( new string[] { "musique2", "joeHippie.wav" } );
            musiques.Add( new string[] { "musique3", "joeDegueulasse.wav" } );
            musiques.Add( new string[] { "flics1", "joeFlics1.wav" } );
            musiques.Add( new string[] { "flics2", "joeFlics2.wav" } );
            musiques.Add( new string[] { "flics3", "joeFlics3.wav" } );
            musiques.Add( new string[] { "flics4", "joeFlics4.wav" } );
        }

        protected override void doInitialization()
        {
            
            base.doInitialization();
            
            loadMusic();

            
            musicPlaylist = new Music(musiques);
            musicPlaylist.setVolume(0.9f);

            bikeRegen = false;
            playerDown = true;
            playerWalked = false;
            introEnded = false;
            cameraChangeTimer = 0;
            etapeMission = 0;
            nbMusiqueEtape = -1;

            Function.Call( Hash.DISPLAY_HUD, false );
            Function.Call( Hash.DISPLAY_RADAR, false );

            
            World.Weather = Weather.ExtraSunny;
            Tools.setClockTime( 10 );
            Tools.setDemagoModel( DemagoModel.Joe );

            Ped player = Game.Player.Character;
            player.MaxHealth = 300;
            player.Armor = 100;
            player.Health = player.MaxHealth;
            Function.Call( Hash.SET_PED_MAX_HEALTH, player, player.MaxHealth );

            Tools.TeleportPlayer(joeStart, false);
            introPed = Function.Call<Ped>(Hash.CLONE_PED, Game.Player.Character, Function.Call<int>(Hash.GET_ENTITY_HEADING, Function.Call<int>(Hash.PLAYER_PED_ID)), false, true);
            Tools.TeleportPlayer(joeHomePosition);

            
            player.Heading += 35;
            player.Task.StandStill( -1 );
            player.IsVisible = false;

            introPed.Task.PlayAnimation( "amb@world_human_picnic@male@base", "base", 8f, -1, true, -1f );

            List<Vector3> positions = new List<Vector3>();
            positions.Add(new Vector3(2361.558f, 2527.512f, 46.66772f));
            positions.Add(new Vector3(2351.906f, 2530.494f, 48f));
            positions.Add(bikePositionAtHome);
            Tools.traveling(positions, 37000, introPed);
            
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

            List<PedHash> spectatorsHashesThirdSong = new List<PedHash>();
            spectatorsHashesThirdSong.Add(PedHash.Beach01AFM);
            spectatorsHashesThirdSong.Add(PedHash.MovAlien01);
            spectatorsHashesThirdSong.Add(PedHash.Jesus01);
            spectatorsHashesThirdSong.Add(PedHash.Zombie01);
            
            for(int num = 0; num < 120; num++)
            {
                Ped ped = World.CreatePed(spectatorsHashesThirdSong.ElementAt<PedHash>(random.Next(spectatorsHashesThirdSong.Count)), thirdSongPublicPosition1 + (float) random.NextDouble() * thirdSongPublicPosition2 + (float)random.NextDouble() * thirdSongPublicPosition3);

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

            foreach (PedHash hash in spectatorsHashesFirstSong)
            {
                Ped ped = World.CreatePed(hash, firstSongPosition.Around(50), 0);
                if (ped != null && ped.Exists())
                {
                    TaskSequence incomingSpectator = new TaskSequence();
                    incomingSpectator.AddTask.GoTo( firstSongPosition.Around( 7 ).Around( 2 ) );
                    incomingSpectator.AddTask.TurnTo( player );
                    incomingSpectator.AddTask.LookAt( player );
                    incomingSpectator.AddTask.PlayAnimation( "facials@gen_male@variations@angry", "mood_angry_1", 8f, -1, true, -1f );

                    ped.Task.PerformSequence( incomingSpectator );
                    spectatorsPeds.Add( ped );
                }
            }

            addGoal(new GoToPosition(firstSongPosition));

            Goal firstSongGoals = new PlayInstrument(InstrumentHash.Guitar, musicPlaylist.length("musique1"), "musique1", musicPlaylist);
            addGoal(firstSongGoals);
            firstSongGoals.OnGoalStart += (sender) =>
            {
                Tools.setClockTime(11, musicPlaylist.length("musique1"));

                List<Vector3> travelingPositions = new List<Vector3>();
                Vector3 cameraPosition = firstSongPosition;
                cameraPosition.X += 8;
                cameraPosition.Z += 2;
                travelingPositions.Add(cameraPosition);
                cameraPosition.X -= 8;
                cameraPosition.Y += 8;
                travelingPositions.Add(cameraPosition);
                cameraPosition.X -= 8;
                cameraPosition.Y -= 8;
                travelingPositions.Add(cameraPosition);
                Tools.traveling(travelingPositions, musicPlaylist.length("musique1"), Game.Player.Character, true);

                foreach (Ped spectator in spectatorsPeds)
                {
                    TaskSequence angrySpectator = new TaskSequence();
                    angrySpectator.AddTask.ClearAllImmediately();
                    angrySpectator.AddTask.TurnTo(player, 1000);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_what_hard", 8f, -1, false, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_nod_yes_soft", 2f, musicPlaylist.length("musique1") - 6000, true, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_you_soft", 8f, -1, false, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_nod_no_hard", 8f, -1, true, -1f);

                    spectator.Task.PerformSequence(angrySpectator);
                }
            };

            
            firstSongGoals.OnGoalAccomplished += (sender, elaspedTime) =>
            {
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

                player.Health = 300;
                player.Armor = 100;
                GTA.UI.ShowSubtitle( "Spectateurs : C'est nul ! Casse toi ! On a appelé les flics !", 3000 );

                Tools.setClockTime( 12, 10000 );
                Game.Player.WantedLevel = 2;
                World.Weather = Weather.Clouds;

                etapeMission = 1;
            };

            
            foreach ( PedHash hash in spectatorsHashesSecondSong ) {
                Ped ped = World.CreatePed( hash, new Vector3( 451.7534f, -978.9359f, 30.68965f ) );
                if ( ped != null && ped.Exists() )
                    copsPeds.Add( ped );

                ped = World.CreatePed( hash, secondSongPosition.Around( 2 ) );
                if ( ped != null && ped.Exists() ) {
                    ped.Task.WanderAround( secondSongPosition, 5 );
                    copsPeds.Add( ped );
                }
            }

            Ped pedWaiting = World.CreatePed( PedHash.Hooker01SFY, new Vector3( 441.7531f, -987.5613f, 30.68965f ) );
            if ( pedWaiting != null && pedWaiting.Exists() ) {
                pedWaiting.Task.UseMobilePhone();
                spectatorsPeds2.Add( pedWaiting );
            }
            pedWaiting = World.CreatePed( PedHash.Hooker02SFY, new Vector3( 441.7531f, -985.9871f, 30.68965f ) );
            if ( pedWaiting != null && pedWaiting.Exists() ) {
                pedWaiting.Task.UseMobilePhone();
                spectatorsPeds2.Add( pedWaiting );
            }
            pedWaiting = World.CreatePed( PedHash.Hooker03SFY, new Vector3( 440.7078f, -987.5613f, 30.68965f ) );
            if ( pedWaiting != null && pedWaiting.Exists() ) {
                pedWaiting.Task.UseMobilePhone();
                spectatorsPeds2.Add( pedWaiting );
            }
            pedWaiting = World.CreatePed( PedHash.Lost01GMY, new Vector3( 440.7078f, -985.9871f, 30.68965f ) );
            if ( pedWaiting != null && pedWaiting.Exists() ) {
                pedWaiting.Task.UseMobilePhone();
                spectatorsPeds2.Add( pedWaiting );
            }

            
            GoToPositionInVehicle goToPoliceWithBikeGoal = new GoToPositionInVehicle( roadFaceToPoliceStationPosition, bike );
            addGoal( goToPoliceWithBikeGoal );
            goToPoliceWithBikeGoal.OnFirstTimeOnVehicle += ( sender, vehicle ) => {
                Tools.setClockTime( 14, 10000 );
            };

            addGoal( new GoToPosition( secondSongPosition ) );

            Goal secondSongGoals = new PlayInstrument(InstrumentHash.Guitar, musicPlaylist.length("musique2"), "musique2", musicPlaylist);
            addGoal(secondSongGoals);
            secondSongGoals.OnGoalStart += (sender) =>
            {
                foreach (Ped spectator in spectatorsPeds)
                {
                    if (spectator != null && spectator.Exists())
                    {
                        spectator.Delete();
                    }
                }

                if (currentPlay != "")
                {
                    musicPlaylist.pauseMusic(currentPlay);
                    currentPlay = "";
                }

                Tools.setClockTime(16, musicPlaylist.length("musique2"));

                foreach ( Ped spectator in World.GetNearbyPeds( player, 12 ) ) {
                    if ( spectator != null && spectator.Exists() ) {
                        spectator.Task.ClearAllImmediately();
                        Function.Call( Hash.TASK_TURN_PED_TO_FACE_ENTITY, spectator.Handle, player.Handle );
                        spectator.Task.LookAt( Game.Player.Character );
                    }
                }

                TaskSequence policeSurrounding = new TaskSequence();
                policeSurrounding.AddTask.TurnTo( player );
                policeSurrounding.AddTask.StandStill( 10000 );
                policeSurrounding.AddTask.GoTo( player.Position.Around( 2 ).Around( 1 ) );
                policeSurrounding.AddTask.TurnTo( player );
                policeSurrounding.AddTask.LookAt( player );

                player.Heading = 90;

                foreach ( Ped spectator in copsPeds ) {
                    if ( spectator != null && spectator.Exists() ) {
                        spectator.Task.PerformSequence( policeSurrounding );
                    }
                }

                foreach (Ped spectator in spectatorsPeds2)
                {
                    TaskSequence angrySpectator = new TaskSequence();
                    angrySpectator.AddTask.ClearAllImmediately();
                    angrySpectator.AddTask.TurnTo(player, 1000);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_what_hard", 8f, -1, false, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_nod_yes_soft", 2f, musicPlaylist.length("musique2") - 6000, true, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_you_soft", 8f, -1, false, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_nod_no_hard", 8f, -1, true, -1f);

                    spectator.Task.PerformSequence(angrySpectator);
                }

                List<Vector3> travelingPositions = new List<Vector3>();
                Vector3 cameraPosition = secondSongPosition;
                cameraPosition.X += 4;
                cameraPosition.Z += 2;
                travelingPositions.Add(cameraPosition);
                cameraPosition.X -= 4;
                cameraPosition.Y += 4;
                travelingPositions.Add(cameraPosition);
                cameraPosition.X -= 4;
                cameraPosition.Y -= 4;
                travelingPositions.Add(cameraPosition);
                Tools.traveling(travelingPositions, musicPlaylist.length("musique2"), Game.Player.Character, true);
            };

            
            secondSongGoals.OnGoalAccomplished += (sender, elaspedTime) =>
            {
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
                            spectator.Task.FightAgainst(Game.Player.Character);
                        }
                        else
                        {
                            spectator.Task.UseMobilePhone();
                        }
                    }
                }

                player.Health = 300;
                player.Armor = 100;

                GTA.UI.ShowSubtitle("Policier : Si tu ne sors pas, c'est nous qui allons te faire sortir !", 4000);
                World.Weather = Weather.Raining;
                Tools.setClockTime(17, 10000);
                etapeMission = 2;

                Script.Wait(2000);
                Game.Player.WantedLevel = 1;
            };
            

            GoToPositionInVehicle goToTheaterWithBikeGoal = new GoToPositionInVehicle( thirdSongBikePosition, bike );
            addGoal( goToTheaterWithBikeGoal );
            goToTheaterWithBikeGoal.OnFirstTimeOnVehicle += ( sender, vehicle ) => {
                Tools.setClockTime( 18, 10000 );
                player.Health = 300;
                player.Armor = 100;
                Game.Player.WantedLevel = 3;
            };

            goToTheaterWithBikeGoal.OnGoalAccomplished += ( sender, elapsedTime ) => {
                Tools.setClockTime( 19, 10000 );
            };

            GoToPosition goToThirdSongPosition = new GoToPosition(thirdSongPosition);
            addGoal(goToThirdSongPosition);

            goToThirdSongPosition.OnGoalAccomplished += (sender, elapsedTime) =>
            {
                nadineMorano.Task.FleeFrom(player);
            };

            

            Goal thirdSongGoals = new PlayInstrument(InstrumentHash.Guitar, musicPlaylist.length("musique3"), "musique3", musicPlaylist);
            addGoal(thirdSongGoals);

            thirdSongGoals.OnGoalStart += (sender) =>
            {
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

                if (currentPlay != "")
                {
                    musicPlaylist.pauseMusic(currentPlay);
                    currentPlay = "";
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
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_nod_yes_soft", 2f, musicPlaylist.length("musique3") - 6000, true, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_you_soft", 8f, -1, false, -1f);
                    angrySpectator.AddTask.PlayAnimation("gestures@m@standing@casual", "gesture_nod_no_hard", 8f, -1, true, -1f);

                    spectator.Task.PerformSequence(angrySpectator);
                }

                List<Vector3> travelingPositions = new List<Vector3>();
                Vector3 cameraPosition = thirdSongPosition;
                cameraPosition.X += 8;
                cameraPosition.Z += 2;
                travelingPositions.Add(cameraPosition);
                cameraPosition.X -= 8;
                cameraPosition.Y += 8;
                travelingPositions.Add(cameraPosition);
                cameraPosition.X -= 8;
                cameraPosition.Y -= 8;
                travelingPositions.Add(cameraPosition);
                Tools.traveling(travelingPositions, musicPlaylist.length("musique3"), Game.Player.Character, true);
            };

            
            thirdSongGoals.OnGoalAccomplished += ( sender, elaspedTime ) => {
                player.Health = 300;
                player.Armor = 100;
                bikeRegen = true;
                Game.Player.WantedLevel = 4;
                Tools.setClockTime( 21, 10000 );
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
            
            GoToPositionInVehicle goToHome = new GoToPositionInVehicle( joeHomePosition, bike );
            addGoal( goToHome );
            goToHome.OnFirstTimeOnVehicle += ( sender, vehicle ) => {
                goToHome.setAdviceText( "Evite les routes pour éviter les voitures de police" );
                Tools.setClockTime( 22, 10000 );
            };
            goToHome.OnGoalAccomplished += ( sender, elapsedTime ) => {
                Tools.setClockTime( 23, 10000 );
                musicPlay( "dialogue10" );
            };

            startTime = DemagoScript.getScriptTime();

            musicPlay( "dialogue0" );
        }

        private void musicPlay( string musique )
        {
            currentPlay = musique;
            musicPlaylist.playMusic( currentPlay );
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

        public override void clear(bool removePhysicalElements = false)
        {
            base.clear(removePhysicalElements);

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
                musicPlaylist.dispose();
            }

            World.Weather = Weather.Clear;
            if (bike != null && bike.Exists())
            {
                if (removePhysicalElements)
                    bike.Delete();

                bike.MarkAsNoLongerNeeded();
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

        public void playAmbiance()
        {
            if (currentPlay == "" || musicPlaylist.isFinished(currentPlay))
            {
                switch (etapeMission)
                {
                    case 1:
                        nbMusiqueEtape++;
                        if ( nbMusiqueEtape < sonsEtape1.Length ) {
                            currentPlay = sonsEtape1[nbMusiqueEtape];
                            musicPlay( currentPlay );
                        } else {
                            nbMusiqueEtape = -1;
                            etapeMission = 0;
                        }

                        break;
                    case 2:
                        nbMusiqueEtape++;
                        if ( nbMusiqueEtape < sonsEtape2.Length ) {
                            currentPlay = sonsEtape2[nbMusiqueEtape];
                            musicPlay( currentPlay );
                        } else {
                            nbMusiqueEtape = -1;
                            etapeMission = 0;
                        }
                        break;
                    case 3:
                        nbMusiqueEtape++;
                        if ( nbMusiqueEtape < sonsEtape3.Length ) {
                            currentPlay = sonsEtape3[nbMusiqueEtape];
                            musicPlay( currentPlay );
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

        public override void update()
        {
            base.update();

            if (currentPlay != "")
            {
                if (interruptPlay != "" && currentInterruptPlay == "")
                {
                    currentInterruptPlay = interruptPlay;
                    interruptPlay = "";

                    if ( !currentPlay.StartsWith( "flic" ) )
                        musicPlaylist.pauseMusic( currentPlay );

                    musicPlaylist.playMusic( currentInterruptPlay );
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

            playAmbiance();

            Ped player = Game.Player.Character;

            if ( playerDown || !playerWalked || !introEnded ) {
                float elapsedTime = DemagoScript.getScriptTime() - startTime;

                if ( elapsedTime > 12000 && playerDown ) {
                    introPed.Task.PlayAnimation( "amb@world_human_picnic@male@exit", "exit", 8f, 3000, false, -1f );
                    playerDown = false;
                }
                if ( elapsedTime > 15000 && !playerWalked ) {
                    introPed.Task.ClearAllImmediately();
                    introPed.Task.GoTo( joeHomePosition, true );
                    playerWalked = true;
                }
                if ( elapsedTime > 40000 && !introEnded ) {
                    player.Task.ClearAllImmediately();
                    introPed.IsVisible = false;
                    introPed.Delete();
                    player.IsVisible = true;
                    Function.Call( Hash.DISPLAY_HUD, true );
                    Function.Call( Hash.DISPLAY_RADAR, true );
                    introEnded = true;
                } else
                    cameraChangeTimer++;
            }

            if ( player.IsInVehicle() && player.CurrentVehicle == bike )
                playerLifeUpCounter++;
            else
                playerLifeUpCounter = 0;

            if ( playerLifeUpCounter >= GTA.Game.FPS / 5 && bikeRegen && bike.Speed > 0 ) {
                playerLifeUpCounter = 0;
                if ( player.Health < player.MaxHealth )
                    player.Health++;
                if ( player.Armor < 100 )
                    player.Armor++;
            }

            Game.Player.Character.Weapons.RemoveAll();
        }
    }
}
