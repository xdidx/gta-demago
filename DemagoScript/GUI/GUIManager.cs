using DemagoScript.GUI.elements;
using DemagoScript.GUI.popup;
using GTA;
using GTA.Math;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DemagoScript.GUI
{
    class GUIManager
    {
        public DemagoMenu menu = null;
        public PopupManager popupManager = null;
        public MissionUI missionUI = null;

        private static GUIManager instance;

        private GUIManager() { }

        public static GUIManager Instance
        {
            get
            {
                if (GUIManager.instance == null)
                {
                    GUIManager.instance = new GUIManager();
                }
                return GUIManager.instance;
            }
        }

        public void initialize(List<Mission> missions = null)
        {
            this.menu = new DemagoMenu(missions);
            this.popupManager = new PopupManager();
            this.missionUI = new MissionUI();
        }

        public void update()
        {
            this.menu.process();
            this.missionUI.draw();
            this.popupManager.update();
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            this.menu.OnKeyDown(sender, e);
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            this.menu.OnKeyUp(sender, e);
        }
    }
}
