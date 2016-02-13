using DemagoScript.GUI;
using DemagoScript.GUI.popup;
using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
using System;
using System.Collections.Generic;

namespace DemagoScript
{
    abstract class Mission : AbstractObjective
    {
        private List<AbstractObjective> objectives = new List<AbstractObjective>();
        private int currentObjectiveIndex = 0;

        // TODO: faire mieux que ça
        private Model missionModel = null;

        public virtual void loadLastCheckpoint()
        {
            Tools.log( "loadLastCheckpoint" );
            Game.Player.WantedLevel = 0;
            
            var currentObjective = objectives[currentObjectiveIndex];
            if ( currentObjective != null ) {
                Tools.log( "Mise en place du model de la mission" );
                Function.Call( Hash.SET_PLAYER_MODEL, Game.Player.Handle, missionModel.Hash );
                Tools.log( "TeleportPlayer to "+currentObjective.getPlayerPosition() );
                Tools.TeleportPlayer( currentObjective.getPlayerPosition() );
                Tools.log( "StartObjective " + currentObjective.getName() );
                currentObjective.start();
            }
            
        }

        public override void populateDestructibleElements()
        {

        }

        public override void depopulateDestructibleElements(bool removePhysicalElements = false)
        {
            Tools.trace( getName() + " removePhysicalElements = " + removePhysicalElements, System.Reflection.MethodBase.GetCurrentMethod().Name, "Mission" );

            foreach (AbstractObjective objective in objectives)
            {
                objective.depopulateDestructibleElements(removePhysicalElements);
            }

            if (removePhysicalElements)
                objectives.Clear();
        }

        public override void start()
        {
            savePlayerModel();

            base.start();

            Game.Player.WantedLevel = 0;
            
            var currentObjective = objectives[currentObjectiveIndex];
            if ( currentObjective != null ) {
                Tools.trace( "Lancement du premier objectif", System.Reflection.MethodBase.GetCurrentMethod().Name, "Mission" );
                currentObjective.start();
            } else {
                Tools.trace( "Pas d'objectifs dans cette missions", System.Reflection.MethodBase.GetCurrentMethod().Name, "Mission" );
            }
        }

        /// <summary>
        /// When the mission is stopped, everything has to be removed
        /// </summary>
        /// <param name="removePhysicalElements"></param>
        public override void stop( bool removePhysicalElements = false )
        {
            Tools.trace( getName() + " removePhysicalElements = " + removePhysicalElements, System.Reflection.MethodBase.GetCurrentMethod().Name, "Mission" );

            base.stop(true);
            
            // Reset the player model
            resetPlayerModel();

            currentObjectiveIndex = 0;
            foreach(AbstractObjective objective in objectives)
            {
                objective.depopulateDestructibleElements(true);
            }
            objectives.Clear();
        }
        
        public override void accomplish()
        {
            base.accomplish();

            Game.Player.WantedLevel = 0;
            Game.Player.Character.Armor = 100;
            Game.Player.Character.MaxHealth = 100;
            Game.Player.Character.Health = Game.Player.Character.MaxHealth;
        }
        
        public void addObjective(AbstractObjective objective)
        {
            objective.OnFailed += (sender, reason) => {
                fail(reason);
            };
            objective.OnAccomplished += (sender, elapsedTime) => {
                this.next();
            };
            objectives.Add(objective);
        }

        public override bool update()
        {
            if (!base.update())
            {
                return false;
            }
            
            this.failIfPlayerIsDeadOrArrested();
            
            if (currentObjectiveIndex < objectives.Count)
            {
                AbstractObjective objective = objectives[currentObjectiveIndex];
                objective.update();

                GUIManager.Instance.missionUI.setMissionTime(Tools.getTextFromMilliSeconds(this.getElaspedTime()));
                GUIManager.Instance.missionUI.setObjectiveTime(Tools.getTextFromMilliSeconds(objective.getElaspedTime()));
            }
            else
            {
                this.accomplish();
            }
            
            return true;
        }

        private void next()
        {
            currentObjectiveIndex++;

            if (currentObjectiveIndex >= objectives.Count)
            {
                this.accomplish();
            }
            else
            {
                AbstractObjective objective = objectives[currentObjectiveIndex];
                objective.start();
            }
        }

        public virtual UIMenuItem addStartItem( ref UIMenu menu )
        {
            var startItem = new UIMenuItem( "Démarrer la mission" );
            menu.AddItem( startItem );

            menu.OnItemSelect += ( sender, item, index ) => {
                if ( item == startItem ) {
                    sender.Visible = false;
                }
            };

            return startItem;
        }

        public virtual void fillMenu(ref UIMenu menu) { }


        /// <summary>
        /// Fail if player is dead or arrested
        /// </summary>
        private void failIfPlayerIsDeadOrArrested()
        {
            if ( Game.Player.IsDead ) {
                Tools.log( "t'es mort, la mission est en pause" );
                //this.pause();
                this.resetPlayerModel();
                //this.fail( "Vous êtes mort" );
            }

            if ( Function.Call<bool>( Hash.IS_PLAYER_BEING_ARRESTED, Game.Player, true ) ) {
                this.fail( "Vous vous êtes fait arrêter" );
            }
        }
        
        /// <summary>
        /// Saved old player model
        /// </summary>
        private void savePlayerModel()
        {
            DemagoScript.savedPlayerModel = Game.Player.Character.Model;
        }

        /// <summary>
        /// Reset old player model
        /// </summary>
        private void resetPlayerModel()
        {
            Tools.log( "resetPlayerModel" );

            Ped player = Game.Player.Character;
            if ( player.Model != DemagoScript.savedPlayerModel ) {
                this.missionModel = player.Model;

                if ( DemagoScript.savedPlayerModel == null || !DemagoScript.savedPlayerModel.IsValid ) {
                    DemagoScript.savedPlayerModel = new Model( PedHash.Michael );
                }

                if ( player.IsDead ) {
                    Ped replacementPed = Function.Call<Ped>( Hash.CLONE_PED, Game.Player.Character, Function.Call<int>( Hash.GET_ENTITY_HEADING, Function.Call<int>( Hash.PLAYER_PED_ID ) ), false, true );
                    replacementPed.Kill();

                    Function.Call( Hash.SET_PLAYER_MODEL, Game.Player.Handle, DemagoScript.savedPlayerModel.Hash );

                    player = Game.Player.Character;
                    player.Task.StandStill( -1 );
                    player.IsVisible = false;
                    player.IsInvincible = true;

                    while ( Game.Player.IsDead ) {
                        if ( Game.Player.Character.IsDead ) {
                            resetPlayerModel();
                            return;
                        } else {
                            Script.Wait( 100 );
                        }
                    }

                    Tools.log( "T'es bien mort" );
                    ConfirmationPopup checkpointPopup = new ConfirmationPopup( "Vous êtes mort", "Voulez-vous revenir au dernier checkpoint ?" );
                    checkpointPopup.OnPopupAccept += () => {
                        Tools.log( "T'accepte la popup" );
                        DemagoScript.loadLastCheckpointOnCurrentMission();
                        // peut être simplifiable en : this.loadLastCheckpoint();
                    };
                    checkpointPopup.OnPopupRefuse += () => {
                        DemagoScript.stopCurrentMission();
                    };
                    checkpointPopup.OnPopupClose += () => {
                        Game.Player.Character.IsVisible = true;
                        Game.Player.Character.IsInvincible = false;
                    };
                    checkpointPopup.show();
                    
                } else if ( Function.Call<bool>( Hash.IS_PLAYER_BEING_ARRESTED, Game.Player, true ) ) {
                    Script.Wait( 3000 );
                    Function.Call( Hash.SET_PLAYER_MODEL, Game.Player.Handle, DemagoScript.savedPlayerModel.Hash );
                    Game.Player.Character.IsVisible = false;

                    ConfirmationPopup checkpointPopup = new ConfirmationPopup( "Vous vous êtes fait arrêter", "Voulez-vous revenir au dernier checkpoint ?" );
                    checkpointPopup.OnPopupAccept += () => {
                        DemagoScript.loadLastCheckpointOnCurrentMission();
                    };
                    checkpointPopup.OnPopupRefuse += () => {
                        DemagoScript.stopCurrentMission();
                    };
                    checkpointPopup.OnPopupClose += () => {
                        player.IsVisible = true;
                    };
                    checkpointPopup.show();

                } else {
                    Function.Call( Hash.SET_PLAYER_MODEL, Game.Player.Handle, DemagoScript.savedPlayerModel.Hash );
                }
            }
        }
    }
}
