using System;
using System.Windows.Forms;
using GTA;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using DemagoScript.GUI;
using DemagoScript.GUI.popup;
using DemagoScript.GUI.elements;

namespace DemagoScript
{
    public class DemagoScript : Script
    {
        private static List<Mission> missions = null;

        private static float scriptTime = 0;
        private bool initialized = false;
        private bool isPaused = false;
        private bool isSitting = false;

        public DemagoScript()
        {
            Tools.log("-------------Initialisation du mod GTA Démago------------");

            createMissions();

            Tick += OnTick;
            KeyDown += OnKeyDown;
        }

        public static void stopCurrentMission()
        {
            if (missions != null)
            {
                foreach (Mission mission in missions)
                {
                    if (mission.isInProgress())
                    {
                        mission.stop("Mission terminée à votre demande");
                    }
                }
            }
        }

        private void initialize()
        {
            if ( initialized ) {
                return;
            }

            createMissions();

            GUIManager.Instance.initialize( missions );

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

            if ( !initialized ) {
                initialize();
            }

            scriptTime += (Game.LastFrameTime * 1000);

            Tools.update();
            Timer.updateAllTimers();

            foreach (Mission mission in missions)
            {
                if (mission.isInProgress())
                {
                    mission.update();
                }
            }

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

            GUIManager.Instance.OnKeyDown( sender, e );
        }

        private void togglePause()
        {
            isPaused = !isPaused;

            foreach (Mission mission in missions)
            {
                mission.setPause(isPaused);
            }
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

            Type[] missionsClassesList = Assembly.GetExecutingAssembly().GetTypes().Where( type => type.IsSubclassOf( typeof( Mission ) ) ).ToArray();
            foreach ( Type missionClass in missionsClassesList ) {
                Mission newMission = (Mission)Activator.CreateInstance( missionClass );

                newMission.OnMissionStart += ( sender ) =>
                {
                    foreach ( Mission mission in missions )
                    {
                        if (mission.isInProgress())
                        {
                            mission.stop("Une autre mission a été démarrée");
                        }
                    }
                    GTA.UI.Notify( sender.getName() );
                };

                newMission.OnMissionAccomplished += ( sender, time ) =>
                {
                    string missionTime = "Une erreur est survenue, merci de nous dire comment :)";
                    if (Tools.getTextFromTimespan(time) != "")
                    {
                        missionTime = "En " + Tools.getTextFromTimespan(time);
                    }

                    SuccessMissionPopup successPopup = new SuccessMissionPopup(sender.getName(), missionTime);
                    GUIManager.Instance.popupManager.add(successPopup);
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

                        GUIManager.Instance.popupManager.add(creditsPopup);
                        creditsPopup.show();
                    };
                };

                newMission.OnMissionOver += (sender, reason) =>
                {
                    Tools.stopTraveling();
                };

                missions.Add( newMission );
            }
        }

    }
}
