﻿using System;
using System.Windows.Forms;
using GTA;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using DemagoScript.GUI;
using DemagoScript.GUI.popup;
using DemagoScript.GUI.elements;
using GTA.Native;

namespace DemagoScript
{
    // TODO: Améliorer cette classe en mettant une variable contenant la mission en cours
    // Cela évitera d'avoir des boucles sur toutes les missions
    public class DemagoScript : Script
    {
        // TODO: j'ai pas de meilleur endroit où le mettre pour l'instant
        public static Model savedPlayerModel = null;

        private static float scriptTime = 0;   
        private static List<Mission> missions = null;
        private static Mission lastMission = null;

        private bool initialized = false;
        private bool isPaused = false;
        private bool isSitting = false;

        public DemagoScript()
        {
            var date = DateTime.Now;
            Tools.log("-------------Initialisation du mod GTA Démago------------");
            GTA.UI.Notify( "GTA Démago - " + date.Hour + ':' + date.Minute + ':' + date.Second );

            createMissions();

            Tick += OnTick;
            KeyDown += OnKeyDown;
        }
        
        public static void loadLastCheckpointOnCurrentMission()
        {
            if (lastMission != null && (lastMission.isInProgress() || lastMission.isWaiting()))
                lastMission.loadLastCheckpoint(); 
        }

        public static void stopCurrentMission()
        {
            if (lastMission != null && (lastMission.isInProgress() || lastMission.isWaiting()))
                lastMission.stop();
        }

        public static void failCurrentMission(string reason = "")
        {
            if (lastMission != null && (lastMission.isInProgress() || lastMission.isWaiting()))
                lastMission.fail(reason);
        }

        private void initialize()
        {
            if (initialized)
            {
                return;
            }
            
            GUIManager.Instance.initialize(missions);

            initialized = true;
        }

        public static float getScriptTime()
        {
            return scriptTime;
        }

        void OnTick(object sender, EventArgs e)
        {
            if (isPaused)
            {
                togglePause();
            }

            if (!initialized)
            {
                initialize();
            }

            scriptTime += (Game.LastFrameTime * 1000);

            Tools.update();
            Timer.updateAllTimers();
            CameraShotsList.Instance.update();
            AudioManager.Instance.update();

            #region Update sur la mission en cours
            foreach ( Mission mission in missions ) {
                if ( mission.isInProgress() ) {
                    mission.update();
                }
            }
            #endregion

            GUIManager.Instance.update();

        }    

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Decimal)
            {
                playerSitting();
            }

            if (e.KeyCode == Keys.Escape)
            {
                togglePause();
            }

            GUIManager.Instance.OnKeyDown(sender, e);
        }

        private void togglePause()
        {
            isPaused = !isPaused;

            //TODO : audio player pause
        }

        private void playerSitting()
        {
            if (isSitting)
            {
                Game.Player.Character.Task.PlayAnimation("amb@world_human_picnic@male@exit", "exit", 8f, -1, false, -1f);
                isSitting = false;
            }
            else
            {
                Game.Player.Character.Heading = Game.Player.Character.Heading + 180;
                TaskSequence sitDown = new TaskSequence();
                sitDown.AddTask.PlayAnimation("amb@world_human_picnic@male@enter", "enter", 8f, -1, false, -1f);
                sitDown.AddTask.PlayAnimation("amb@world_human_picnic@male@base", "base", 8f, -1, true, -1f);
                Game.Player.Character.Task.PerformSequence(sitDown);
                isSitting = true;
            }
        }

        private void createMissions()
        {
            missions = new List<Mission>();

            Type[] missionsClassesList = Assembly.GetExecutingAssembly().GetTypes().Where(type => type.IsSubclassOf(typeof(Mission))).ToArray();
            foreach (Type missionClass in missionsClassesList)
            {
                Mission newMission = (Mission)Activator.CreateInstance(missionClass);

                newMission.OnStarted += (sender) =>
                {
                    if (DemagoScript.lastMission != null)
                    {
                        DemagoScript.lastMission.stop();
                    }
                    DemagoScript.lastMission = newMission;
                    GTA.UI.Notify(sender.getName());
                };

                newMission.OnAccomplished += (sender, time) =>
                {
                    string missionTime = "Temps inconnu";
                    if (Tools.getTextFromMilliSeconds(time) != "")
                    {
                        missionTime = "En " + Tools.getTextFromMilliSeconds(time);
                    }

                    SuccessMissionPopup successPopup = new SuccessMissionPopup(sender.getName(), missionTime);
                    successPopup.show();
                    successPopup.OnPopupClose += () =>
                    {
                        GUIManager.Instance.popupManager.remove(successPopup);

                        NotificationPopup creditsPopup = new NotificationPopup();
                        creditsPopup.add(new UIRectElement(0.5, 0.5, 1, 1, UIColor.BLACK, 200));
                        creditsPopup.add(new UITextElement("GTA Démago", 0.5, 0.2, 1.5, true, Font.Pricedown, UIColor.GTA_YELLOW));
                        creditsPopup.add(new UITextElement("Merci d’avoir jouer à GTA Démago !", 0.5, 0.29, 0.7, true, Font.ChaletLondon, UIColor.WHITE));
                        creditsPopup.add(new UITextElement("De nouvelles missions seront bientôt disponibles alors rejoignez nous sur ", 0.5, 0.33, 0.7, true, Font.ChaletLondon, UIColor.WHITE));
                        creditsPopup.add(new UITextElement("Twitch : http://twitch.tv/realmyop2", 0.5, 0.39, 0.5, true, Font.ChaletLondon, UIColor.WHITE));
                        creditsPopup.add(new UITextElement("Facebook : http://facebook.com/realmyop", 0.5, 0.42, 0.5, true, Font.ChaletLondon, UIColor.WHITE));
                        creditsPopup.add(new UITextElement("Twitter : http://twitter.com/RealMyop", 0.5, 0.45, 0.5, true, Font.ChaletLondon, UIColor.WHITE));
                        creditsPopup.add(new UITextElement("Venez nombreux !", 0.5, 0.525, 1, true, Font.HouseScript, UIColor.WHITE));
                        creditsPopup.add(new UITextElement("Entrée pour fermer", 0.5, 0.9, 0.6, true, Font.HouseScript, UIColor.WHITE));
                        creditsPopup.OnPopupClose += () =>
                        {
                            GUIManager.Instance.popupManager.remove(creditsPopup);
                        };
                        
                        creditsPopup.show();
                    };
                };

                newMission.OnEnded += (sender) =>
                {
                    CameraShotsList.Instance.reset();
                    DemagoScript.lastMission = null;
                };

                missions.Add(newMission);
            }
        }

    }
}
