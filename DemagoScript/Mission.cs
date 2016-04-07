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
        protected bool loadingCheckpoint = false; 
        protected bool introEnded = false;
        protected bool isActivated = false;

        private int lastActivableCheckpointIndex = 0;

        protected List<AbstractObjective> getObjectives()
        {
            return objectives;
        }

        public bool getIsActivated()
        {
            return isActivated;
        }

        public virtual void loadLastCheckpoint()
        {
            Game.FadeScreenOut(500);
            Script.Wait(500);

            this.lastActivableCheckpointIndex = 0;

            if (this.currentObjectiveIndex >= objectives.Count)
            {
                stop(true);
                Game.FadeScreenIn(500);
                return;
            }

            for (int objectiveIndex = this.currentObjectiveIndex; objectiveIndex >= 0 && this.lastActivableCheckpointIndex == 0; objectiveIndex--)
            {
                var currentObjective = objectives[objectiveIndex];
                if (currentObjective != null && currentObjective.Checkpoint != null && currentObjective.Checkpoint.Activable)
                {
                    this.lastActivableCheckpointIndex = objectiveIndex;
                    break;
                }
            }

            if (this.lastActivableCheckpointIndex == 0)
            {
                this.stop(true);
                this.start();
            }
            else
            {
                this.reset();
                this.loadingCheckpoint = true;
                this.play();
            }
        }

        protected override void depopulateDestructibleElements(bool removePhysicalElements = false)
        {
            this.reset();
        }

        public override void play()
        {
            base.play();
            GUIManager.Instance.missionUI.show();
        }

        public override void start()
        {
            if ((PedHash)Game.Player.Character.Model.Hash == PedHash.Michael || (PedHash)Game.Player.Character.Model.Hash == PedHash.Franklin || (PedHash)Game.Player.Character.Model.Hash == PedHash.Trevor)
            {
                DemagoScript.savedPlayerModelHash = (PedHash)Game.Player.Character.Model.Hash;
            }

            Game.FadeScreenOut(500);
            Script.Wait(500);

            base.start();

            if (this.lastActivableCheckpointIndex == 0 && this.currentObjectiveIndex == 0 && objectives.Count > 0)
            {
                objectives[0].start();
            }

            Game.FadeScreenIn(500);
        }

        /// <summary>
        /// When the mission is stopped, everything has to be removed and the timer is set to 0
        /// </summary>
        /// <param name="removePhysicalElements"></param>
        public override void stop(bool removePhysicalElements = false)
        {
            base.stop(true);
        }

        /// <summary>
        /// When the mission is resetted, everything has to be removed and the timer ISN't reinitialized
        /// </summary>
        public void reset()
        {
            #region Depopulate objectives
            foreach (AbstractObjective objective in objectives)
            {
                objective.stop(true);
            }
            objectives.Clear();

            this.currentObjectiveIndex = 0;
            #endregion

            #region Player health
            Ped player = Game.Player.Character;
            Function.Call(Hash.SET_PED_MAX_HEALTH, player, 100);
            player.MaxHealth = 100;
            player.Health = 100;
            player.Armor = 100;
            #endregion
            
            World.Weather = Weather.Clear;
            Game.Player.WantedLevel = 0;

            CameraShotsList.Instance.reset();
            GUIManager.Instance.missionUI.hide();
            AudioManager.Instance.stopAll();
            ModelManager.Instance.resetPlayerModel();
        }

        public override void accomplish()
        {
            base.accomplish();
        }

        public void addObjective(AbstractObjective objective)
        {
            objective.OnFailed += (sender, reason) => {
                #region Show checkpoint popup
                var title = reason;
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
                #endregion
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

            if (this.currentObjectiveIndex < objectives.Count)
            {
                AbstractObjective objective = objectives[currentObjectiveIndex];

                if (introEnded)
                {
                    objective.update();
                }

                if (this.loadingCheckpoint)
                {
                    if (this.currentObjectiveIndex < this.lastActivableCheckpointIndex)
                    {
                        if (this.currentObjectiveIndex == 0)
                        {
                            objectives[0].start();
                        }
                        objective.accomplish();
                    }
                    else
                    {
                        this.lastActivableCheckpointIndex = 0;
                        this.loadingCheckpoint = false;
                        Game.FadeScreenIn(500);
                    }
                }
                
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

            GUIManager.Instance.missionUI.setAdvice("");
            GUIManager.Instance.missionUI.setObjective("");

            if (currentObjectiveIndex >= objectives.Count)
            {
                this.accomplish();
            }
            else
            {
                this.checkRequiredElements();
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
    }
}
