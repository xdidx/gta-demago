﻿using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemagoScript
{
    class SurviveInZone : AbstractObjective
    {
        private bool alreadyEnter = false;
        private bool playerTargeted = false;
        private float ballasSpawnInterval = 2;
        private int currentDuration;
        private bool ennemyAlreadyCreated = false;
        private DateTime startTime;
        private Blip startPositionBlip = null;
        private List<Ped> ballasPeds = new List<Ped>();

        private Vector3 startPosition;
        private List<Vector3> startPositions;
        private int secondsToSurvive;
        private float maximumDistance;
        private int ennemyRelationshipGroup;

        public SurviveInZone( Vector3 startPosition, List<Vector3> startPositions, int secondsToSurvive, float maximumDistance )
        {
            this.name = "Survive in zone";

            this.startPosition = startPosition;
            this.startPositions = startPositions;
            this.secondsToSurvive = secondsToSurvive;
            this.maximumDistance = maximumDistance;
            ennemyRelationshipGroup = World.AddRelationshipGroup( "SurviveInZoneObjectiveEnnemies" );
        }

        protected override void populateDestructibleElements()
        {
            base.populateDestructibleElements();

            int ballasGroup = World.AddRelationshipGroup("Ballas");

            startPositionBlip = World.CreateBlip(startPosition);
            startPositionBlip.Sprite = BlipSprite.Deathmatch;
            startPositionBlip.Color = BlipColor.Red;
            startPositionBlip.IsFlashing = true;
            startPositionBlip.ShowRoute = true;
        }

        protected override void depopulateDestructibleElements(bool removePhysicalElements = false)
        {
            if (startPositionBlip != null && startPositionBlip.Exists())
            {
                startPositionBlip.Remove();
                startPositionBlip = null;
            }

            if (removePhysicalElements)
            {
                foreach (Ped ballasPed in ballasPeds)
                {
                    if (ballasPed != null && ballasPed.Exists())
                    {
                        ballasPed.Delete();
                    }
                }
                ballasPeds.Clear();
            }
        }

        public override bool update()
        {
            if ( !base.update() ) {
                return false;
            }

            currentDuration = (int)( DateTime.Now - startTime ).TotalSeconds;

            #region Ballas spawn management
            if ( !ennemyAlreadyCreated && currentDuration % ballasSpawnInterval == 0 ) {
                ennemyAlreadyCreated = true;
                int aliveBallasCount = 0;

                foreach ( Ped currentBallas in ballasPeds ) {
                    if ( currentBallas != null && currentBallas.Exists() && currentBallas.IsAlive ) {
                        aliveBallasCount++;
                        if ( alreadyEnter && !playerTargeted ) {
                            playerTargeted = true;
                            currentBallas.Task.ClearAll();
                            currentBallas.Task.FightAgainst( Game.Player.Character );
                        }
                    }
                }

                Random random = new Random();
                for ( int count = aliveBallasCount; count < 5; count++ ) {
                    Vector3 grooveStreetRandomPosition;
                    if ( startPositions != null && startPositions.Count > 0 ) {
                        int randomIndex = random.Next( 0, startPositions.Count - 1 );
                        grooveStreetRandomPosition = startPositions[randomIndex];
                    } else {
                        grooveStreetRandomPosition = startPosition.Around( maximumDistance );
                    }
                    Ped ballas = World.CreatePed( PedHash.BallaOrig01GMY, grooveStreetRandomPosition );

                    if ( ballas != null && ballas.Exists() ) {
                        int countLoop = 0;
                        while ( ballas.IsOnScreen ) {
                            countLoop++;
                            ballas.Position = World.GetNextPositionOnStreet( startPosition );
                            if ( countLoop == 100 ) {
                                break;
                            }
                        }

                        aliveBallasCount++;
                        if ( alreadyEnter ) {
                            ballas.Task.FightAgainst( Game.Player.Character );
                        }

                        ballas.RelationshipGroup = ennemyRelationshipGroup;
                        ballas.AlwaysKeepTask = true;
                        if ( count % 3 == 0 ) {
                            ballas.Weapons.Give( WeaponHash.Pistol, 100, true, true );
                        }
                        if ( count % 3 == 1 ) {
                            ballas.Weapons.Give( WeaponHash.Knife, -1, true, true );
                        } else {
                            ballas.Weapons.Give( WeaponHash.Bat, -1, true, true );
                        }
                    }

                    ballasPeds.Add( ballas );
                }
            } else if ( ennemyAlreadyCreated && currentDuration % ballasSpawnInterval != 0 ) {
                ennemyAlreadyCreated = false;
            }
            #endregion

            #region Timer and zone management
            if ( alreadyEnter ) {
                if ( startPositionBlip != null && startPositionBlip.Exists() ) {
                    startPositionBlip.Remove();
                    startPositionBlip = null;
                }

                if ( Game.Player.Character.Position.DistanceTo( startPosition ) < maximumDistance ) {
                    int remainingSeconds = secondsToSurvive - currentDuration;
                    if ( remainingSeconds > 0 ) {
                        Game.Player.WantedLevel = 0;
                        //Game.MaxWantedLevel = 0;
                        ObjectiveText = "Il reste " + remainingSeconds + " secondes à tenir";
                    } else {
                        accomplish();
                        return false;
                    }

                    if ( Game.Player.Character.Position.DistanceTo( startPosition ) > ( maximumDistance * 0.9 ) ) {
                        AdviceText = "Attention, il faut rester dans la zone de Groove Street !";
                        //TODO : IT'S A WARNING, NOT A SIMPLE ADVICE
                        // GTA.Game.PlaySound afin de pouvoir avoir une alerte sonore légère ?
                    } else {
                        AdviceText = "";
                    }
                } else {
                    fail( "Vous êtes sorti de la zone" );
                    return false;
                }
            } else {
                if ( Game.Player.Character.Position.DistanceTo( PlacesPositions.GrooveStreet ) < ( maximumDistance * 0.9 ) ) {
                    alreadyEnter = true;
                    startTime = DateTime.Now;
                } else {
                    ObjectiveText = "Va à Groove street et tue tous les Ballas qui ont envahi le quartier";
                }
            }
            #endregion

            return true;
        }
        
    }
}
