using System;
using System.Drawing;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using GTA.Math;
using NativeUI;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace DemagoScript
{
    public class DemagoScript : Script
    {
        private List<Mission> missions = null;
        private DemagoMenu menu;
        private static float scriptTime = 0;
        private bool initialized = false;
        private bool isSitting = false;

        public DemagoScript()
        {
            Tools.log("-------------Initialisation du mod GTA Démago------------");

            Tick += OnTick;
            KeyDown += OnKeyDown;
        }

        private void initialize()
        {
            if (initialized)
            {
                return;
            }
            
            menu = new DemagoMenu(getMissions());
            initialized = true;
        }

        public static float getScriptTime()
        {
            return scriptTime;
        }

        void OnTick(object sender, EventArgs e)
        {
            initialize();

            scriptTime += (Game.LastFrameTime * 1000);
            
            Tools.update();
            Timer.updateAllTimers();

            menu.process();

            var missions = getMissions();
            foreach (Mission mission in missions)
            {
                if (mission.isInProgress())
                {
                    mission.update();
                }
            }
        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                menu.toggleDisplay();
            }
            if (e.KeyCode == Keys.Decimal)
            {
                playerSitting();
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

        private List<Mission> getMissions()
        {
            if (missions == null)
            {
                missions = new List<Mission>();

                Type[] missionsClassesList = Assembly.GetExecutingAssembly().GetTypes().Where(type => type.IsSubclassOf(typeof(Mission))).ToArray();
                foreach (Type missionClass in missionsClassesList)
                {
                    Mission newMission = (Mission)Activator.CreateInstance(missionClass);

                    newMission.OnMissionStart += (sender) =>
                    {
                        foreach (Mission mission in missions)
                        {
                            mission.stop("Une autre mission a été démarrée");
                        }
                        GTA.UI.Notify(sender.getName());
                    };

                    newMission.OnMissionAccomplished += (sender, time) =>
                    {
                        var successMessage = sender.getName() + " : Mission accomplie";
                        if (Tools.getTextFromTimespan(time) != "")
                        {
                            successMessage += " en " + Tools.getTextFromTimespan(time);
                        }
                        GTA.UI.Notify(successMessage);
                    };

                    newMission.OnMissionFail += (sender, reason) =>
                    {
                        GTA.UI.Notify("La mission a échouée : " + reason);
                    };

                    missions.Add(newMission);
                }
            }

            return missions;
        }
        
    }
}