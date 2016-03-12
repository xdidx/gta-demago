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
        protected List<AbstractObjective> objectives = new List<AbstractObjective>();
        protected int currentObjectiveIndex = 0;
        protected bool introEnded = false;

        public virtual void loadLastCheckpoint()
        {
            Tools.log("loadLastCheckpoint: Mission name: " + getName());
            var currentObjective = objectives[currentObjectiveIndex];
            if (currentObjective != null && currentObjective.Checkpoint != null) {
                Tools.log("loadLastCheckpoint: currentObjective name: " + currentObjective.getName() + " action: teleportPlayerToCheckpoint");
                checkRequiredElements();
                currentObjective.Checkpoint.load();
                play();
                AudioManager.Instance.playAll();
            } else {
                Tools.log("loadLastCheckpoint: stop mission");
                stop(true);
            }
        }

        public virtual void checkRequiredElements()
        {
            Tools.log("checkRequiredElements");
        }

        public override void populateDestructibleElements()
        {
            Tools.log("Mission::populateDesctructibleElements");
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
            if ((PedHash)Game.Player.Character.Model.Hash == PedHash.Michael || (PedHash)Game.Player.Character.Model.Hash == PedHash.Franklin || (PedHash)Game.Player.Character.Model.Hash == PedHash.Trevor)
            {
                DemagoScript.savedPlayerModelHash = (PedHash)Game.Player.Character.Model.Hash;
            }

            base.start();

            Game.Player.WantedLevel = 0;

            var currentObjective = objectives[currentObjectiveIndex];
            if (currentObjective != null) {
                Tools.trace("Start sur le currentObjective", System.Reflection.MethodBase.GetCurrentMethod().Name, "Mission");
                currentObjective.start();
            } else {
                Tools.trace("Pas d'objectifs dans cette missions", System.Reflection.MethodBase.GetCurrentMethod().Name, "Mission");
            }
        }

        /// <summary>
        /// When the mission is stopped, everything has to be removed
        /// </summary>
        /// <param name="removePhysicalElements"></param>
        public override void stop(bool removePhysicalElements = false)
        {
            base.stop(true);

            GUIManager.Instance.missionUI.hide();

            currentObjectiveIndex = 0;
            foreach (AbstractObjective objective in objectives)
            {
                objective.depopulateDestructibleElements(true);
            }
            objectives.Clear();

            Tools.log("call reset player model");
            // Reset the player model
            resetPlayerModel();
            
            World.Weather = Weather.Clear;

            Ped player = Game.Player.Character;
            Function.Call(Hash.SET_PED_MAX_HEALTH, player, 100);
            player.MaxHealth = 100;
            player.Health = 100;
            player.Armor = 100;

            Game.Player.WantedLevel = 0;
            
            CameraShotsList.Instance.reset();
            AudioManager.Instance.stopAll();
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

            if (Game.Player.IsDead || Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED, Game.Player, false))
            {
                this.pause();
                AudioManager.Instance.pauseAll();
                this.resetPlayerModel();
            }

            if (currentObjectiveIndex < objectives.Count)
            {
                AbstractObjective objective = objectives[currentObjectiveIndex];

                if (introEnded)
                {
                    objective.update();
                }
                else
                {
                    objective.ObjectiveText = "";
                }

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

        public virtual UIMenuItem addStartItem(ref UIMenu menu)
        {
            var startItem = new UIMenuItem("Démarrer la mission");
            menu.AddItem(startItem);

            menu.OnItemSelect += (sender, item, index) => {
                if (item == startItem) {
                    sender.Visible = false;
                }
            };

            return startItem;
        }

        public virtual void fillMenu(ref UIMenu menu) { }

        /// <summary>
        /// Reset old player model
        /// </summary>
        private void resetPlayerModel()
        {
            bool playerWasDead = false;
            bool playerWasArrested = false;
            Ped player = Game.Player.Character;
            if ((PedHash)player.Model.Hash != DemagoScript.savedPlayerModelHash)
            {
                Vehicle currentVehicle = null;
                if (player.IsInVehicle())
                {
                    currentVehicle = player.CurrentVehicle;
                }

                if (player.IsDead || Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED, Game.Player, false))
                {
                    #region Create remplacement ped
                    Ped replacementPed = Function.Call<Ped>(Hash.CLONE_PED, Game.Player.Character, Function.Call<int>(Hash.GET_ENTITY_HEADING, Function.Call<int>(Hash.PLAYER_PED_ID)), false, true);

                    if (player.IsDead)
                    {
                        playerWasDead = true;
                        replacementPed.Kill();
                    }
                    else
                    {
                        playerWasArrested = true;
                        replacementPed.Task.HandsUp(10000);
                    }

                    replacementPed.MarkAsNoLongerNeeded();
                    #endregion
                }

                #region Change player model with saved PedHash
                var characterModel = new Model(DemagoScript.savedPlayerModelHash);
                characterModel.Request(500);

                if (characterModel.IsInCdImage && characterModel.IsValid)
                {
                    while (!characterModel.IsLoaded) Script.Wait(100);

                    Function.Call(Hash.SET_PLAYER_MODEL, Game.Player, characterModel.Hash);
                    Function.Call(Hash.SET_PED_DEFAULT_COMPONENT_VARIATION, Game.Player.Character.Handle);
                }

                characterModel.MarkAsNoLongerNeeded();
                #endregion

                player = Game.Player.Character;
                if (currentVehicle != null)
                {
                    player.SetIntoVehicle(currentVehicle, VehicleSeat.Driver);
                }

                if (playerWasDead || playerWasArrested)
                {
                    #region Hide real player and wait for game recovery
                    player.IsVisible = false;
                    player.IsInvincible = true;
                    player.Task.StandStill(-1);

                    //Tant qu'on charge le jeu, on fait attendre le script GTA Démago
                    while (!Function.Call<bool>(Hash.IS_PLAYER_PLAYING, Game.Player))
                    {
                        Script.Wait(100);
                    }

                    player.IsVisible = true;
                    player.IsInvincible = false;
                    #endregion
                }

                #region Show death or arrested popups
                if (playerWasDead || playerWasArrested)
                {
                    Script.Wait(10000);

                    var title = (playerWasDead) ? "Vous êtes mort" : "Vous vous êtes fait arrêter";
                    var subtitle = "Voulez-vous revenir au dernier checkpoint ?";

                    ConfirmationPopup checkpointPopup = new ConfirmationPopup(title, subtitle);
                    checkpointPopup.OnPopupAccept += () =>
                    {
                        this.loadLastCheckpoint();
                    };
                    checkpointPopup.OnPopupRefuse += () =>
                    {
                        this.stop(true);
                    };
                    checkpointPopup.show();
                }
                #endregion
            }
        }
    }
}
