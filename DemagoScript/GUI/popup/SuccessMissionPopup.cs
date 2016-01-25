using DemagoScript.GUI.elements;
using GTA;
using GTA.Native;
using System.Windows.Forms;

namespace DemagoScript.GUI.popup
{
    class SuccessMissionPopup : Popup
    {
        private UIRectElement background = null;
        private UITextElement title = null;
        private UITextElement content = null;

        private UITextElement infos = null;
        private const string INFOS = "\"Entrée\" pour fermer";

        /// <summary>
        /// Called when user close popup.
        /// </summary>
        public event PopupCloseEvent OnPopupClose;

        public SuccessMissionPopup(string missionName, string missionTime)
        {
            // background
            this.background = new UIRectElement(0.5, 0.5, 0.3, 0.2, UIColor.BLACK, 200);
            this.add(this.background);

            // title
            this.title = new UITextElement("Mission réussie", 0.5, 0.3, 1.5, true, Font.Pricedown, UIColor.GTA_YELLOW);
            this.add(this.title);

            // mission name
            this.content = new UITextElement(missionName, 0.5, 0.42, 1.2, true, Font.HouseScript, UIColor.WHITE);
            this.add(this.content);

            // top separator
            this.add(new UIRectElement(0.5, 0.515, 0.25, 0.002, UIColor.WHITE, 255));

            // mission time
            this.content = new UITextElement(missionTime, 0.5, 0.525, 0.475, true, Font.ChaletLondon, UIColor.WHITE);
            this.add(this.content);

            // bottom separator
            this.add(new UIRectElement(0.5, 0.57, 0.25, 0.002, UIColor.WHITE, 255));

            // helper
            this.infos = new UITextElement(INFOS, 0.5, 0.9, 0.6, true, Font.HouseScript, UIColor.WHITE);
            this.add(this.infos);
        }

        public override void draw()
        {
            base.draw();

            if (this.isVisible())
            {
                GUIManager.Instance.menu.hide();
            }
        }

        public override void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (this.isVisible())
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)
                {
                    this.hide();
                    Function.Call(Hash.SET_GAME_PAUSED, false);
                    OnPopupClose?.Invoke();
                }
            }
        }
    }
}