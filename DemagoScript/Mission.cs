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
            Tools.log( "loadLastCheckpoint: Mission name: " + this.getName() );   
            var currentObjective = objectives[currentObjectiveIndex];
            if ( currentObjective != null && currentObjective.Checkpoint != null ) {
                Tools.log( "loadLastCheckpoint: currentObjective name: " + currentObjective.getName() + " action: teleportPlayerToCheckpoint");
                checkRequiredElements();
                currentObjective.Checkpoint.teleportPlayerToCheckpoint();
                this.play();
            } else {
                Tools.log( "loadLastCheckpoint: stop mission" );
                this.stop( true );
            }
        }

        public virtual void checkRequiredElements()
        {
            Tools.log( "checkRequiredElements" );
        }

        public override void populateDestructibleElements()
        {
        }

        public override void depopulateDestructibleElements(bool removePhysicalElements = false)
        {
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
                Tools.trace( "Start sur le currentObjective", System.Reflection.MethodBase.GetCurrentMethod().Name, "Mission" );
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
            base.stop(true);

            GUIManager.Instance.missionUI.hide();

            currentObjectiveIndex = 0;
            foreach(AbstractObjective objective in objectives)
            {
                objective.depopulateDestructibleElements(true);
            }
            objectives.Clear();

            // Reset the player model
            resetPlayerModel();

            Game.Player.WantedLevel = 0;
            Game.Player.Character.Armor = 100;
            Game.Player.Character.MaxHealth = 300;
            Function.Call( Hash.SET_PED_MAX_HEALTH, Game.Player.Character, Game.Player.Character.MaxHealth );
            Game.Player.Character.Health = 300;
        }
        
        protected override void accomplish()
        {
            base.accomplish();
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
            
            if (Game.Player.IsDead || Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED, Game.Player, true))
            {
                this.pause();
                this.resetPlayerModel();
            }
            
            if (currentObjectiveIndex < objectives.Count)
            {
                AbstractObjective objective = objectives[currentObjectiveIndex];
                objective.update();

                if (!Function.Call<bool>(Hash.IS_HUD_HIDDEN))
                {
                    GUIManager.Instance.missionUI.setMissionTime(Tools.getTextFromMilliSeconds(this.getElaspedTime()));
                    GUIManager.Instance.missionUI.setObjectiveTime(Tools.getTextFromMilliSeconds(objective.getElaspedTime()));
                }
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
                        Script.Wait( 100 );
                    }
                    
                    Function.Call(Hash.DISPLAY_HUD, true);
                    Function.Call(Hash.DISPLAY_RADAR, true);
                    
                    ConfirmationPopup checkpointPopup = new ConfirmationPopup( "Vous êtes mort", "Voulez-vous revenir au dernier checkpoint ?" );
                    checkpointPopup.OnPopupAccept += () => {
                        replacementPed.Delete();
                        DemagoScript.loadLastCheckpointOnCurrentMission();
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
                
                player.IsVisible = true;
            }
        }
    }
}
