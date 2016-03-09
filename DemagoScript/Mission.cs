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

        // TODO: faire mieux que ça
        private Model missionModel = null;

        public virtual void loadLastCheckpoint()
        {
            Tools.log("loadLastCheckpoint: Mission name: " + getName());
            var currentObjective = objectives[currentObjectiveIndex];
            if (currentObjective != null && currentObjective.Checkpoint != null) {
                Tools.log("loadLastCheckpoint: currentObjective name: " + currentObjective.getName() + " action: teleportPlayerToCheckpoint");
                checkRequiredElements();
                currentObjective.Checkpoint.loadAndApplyLastCheckpointProperties();
                play();
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
            DemagoScript.savedPlayerModelHash = (PedHash)Game.Player.Character.Model.Hash;

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

            // Reset the player model
            resetPlayerModel();

            Game.Player.WantedLevel = 0;
            Game.Player.Character.Armor = 100;
            Game.Player.Character.MaxHealth = 300;
            Function.Call(Hash.SET_PED_MAX_HEALTH, Game.Player.Character, Game.Player.Character.MaxHealth);
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

            if (Game.Player.IsDead || Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED, Game.Player, false))
            {
                this.pause();
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
            Tools.log("changeplayerModel start");

            bool playerWasDead = false;
            bool playerWasArrested = false;
            Ped player = Game.Player.Character;
            if ((PedHash)player.Model.Hash != DemagoScript.savedPlayerModelHash)
            {
                this.missionModel = player.Model;

                if (player.IsDead)
                {
                    playerWasDead = true;
                    Ped replacementPed = Function.Call<Ped>(Hash.CLONE_PED, Game.Player.Character, Function.Call<int>(Hash.GET_ENTITY_HEADING, Function.Call<int>(Hash.PLAYER_PED_ID)), false, true);
                    replacementPed.Kill();
                    replacementPed.MarkAsNoLongerNeeded();
                }
                else if (Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED, Game.Player, false))
                {
                    playerWasArrested = true;
                    Ped replacementPed = Function.Call<Ped>(Hash.CLONE_PED, Game.Player.Character, Function.Call<int>(Hash.GET_ENTITY_HEADING, Function.Call<int>(Hash.PLAYER_PED_ID)), false, true);
                    replacementPed.Task.HandsUp(10000);
                    replacementPed.MarkAsNoLongerNeeded();
                }

                var characterModel = new Model(DemagoScript.savedPlayerModelHash);
                characterModel.Request(500);

                if (characterModel.IsInCdImage && characterModel.IsValid)
                {
                    while (!characterModel.IsLoaded) Script.Wait(100);

                    Function.Call(Hash.SET_PLAYER_MODEL, Game.Player, characterModel.Hash);
                    Function.Call(Hash.SET_PED_DEFAULT_COMPONENT_VARIATION, Game.Player.Character.Handle);
                }

                characterModel.MarkAsNoLongerNeeded();

                player = Game.Player.Character;
                player.Task.StandStill(-1);
                player.IsVisible = false;
                player.IsInvincible = true;

                //Tant qu'on charge le jeu, on fait attendre le script GTA Démago
                while (!Function.Call<bool>(Hash.IS_PLAYER_PLAYING, Game.Player))
                {
                    Script.Wait(100);
                }

                Script.Wait(3000);

                if (playerWasDead) {
                    Function.Call(Hash.DISPLAY_HUD, true);
                    Function.Call(Hash.DISPLAY_RADAR, true);

                    ConfirmationPopup checkpointPopup = new ConfirmationPopup("Vous êtes mort", "Voulez-vous revenir au dernier checkpoint ?");
                    checkpointPopup.OnPopupAccept += () => {
                        DemagoScript.loadLastCheckpointOnCurrentMission();
                    };
                    checkpointPopup.OnPopupRefuse += () => {
                        DemagoScript.stopCurrentMission();
                    };
                    checkpointPopup.show();

                } else if (playerWasArrested) {
                    ConfirmationPopup checkpointPopup = new ConfirmationPopup("Vous vous êtes fait arrêter", "Voulez-vous revenir au dernier checkpoint ?");

                    checkpointPopup.OnPopupAccept += () => {
                        DemagoScript.loadLastCheckpointOnCurrentMission();
                    };
                    checkpointPopup.OnPopupRefuse += () => {
                        DemagoScript.stopCurrentMission();
                    };
                    checkpointPopup.show();
                }
            }

            Tools.log("changeplayerModel end");

            Game.Player.Character.IsVisible = true;
            Game.Player.Character.IsInvincible = false;
        }
    }
}
