using DemagoScript.GUI.elements;
using GTA.Math;
using GTA;
using System.Windows.Forms;
using GTA.Native;

namespace DemagoScript.GUI.popup
{
    class ConfirmationPopup : NotificationPopup
    {
        /// <summary>
        /// Called when user accept popup question.
        /// </summary>
        public event PopupAcceptEvent OnPopupAccept;

        /// <summary>
        /// Called when user refuse popup question.
        /// </summary>
        public event PopupRefuseEvent OnPopupRefuse;

        private UIRectElement background = null;
        private UITextElement title = null;
        private UITextElement content = null;

        private UITextElement infos = null;
        private const string INFOS = "\"Entrée\" ou \"A\" pour valider, \"Echap\" ou \"B\" pour annuler";

        public ConfirmationPopup(string text_title, string text_content) : base()
        {
            // background
            this.background = new UIRectElement(0, 0, 2, 2, UIColor.BLACK, 230);
            this.add(this.background);

            // title
            this.title = new UITextElement(text_title, 0.5, 0.4, 2, true, Font.Pricedown, UIColor.GTA_YELLOW);
            this.add(this.title);

            // top separator
            this.add(new UIRectElement(0.5, 0.525, 0.9, 0.002, UIColor.WHITE, 255));

            // content
            this.content = new UITextElement(text_content, 0.5, 0.530, 0.475, true, Font.ChaletLondon, UIColor.WHITE);
            this.add(this.content);

            // bottom separator
            this.add(new UIRectElement(0.5, 0.58, 0.9, 0.002, UIColor.WHITE, 255));

            // helper
            this.infos = new UITextElement(INFOS, 0.5, 0.9, 0.6, true, Font.HouseScript, UIColor.WHITE);
            this.add(this.infos);

            GUIManager.Instance.menu.OnControlPressed += (GTA.Control control) =>
            {
                if (this.isVisible())
                {
                    if (control == GTA.Control.PhoneSelect)
                    {
                        this.accept();
                    }

                    if (control == GTA.Control.PhoneCancel)
                    {
                        this.refuse();
                    }
                }
            };

            GUIManager.Instance.menu.OnKeysPressedEvent += (Keys key) =>
            {
                if (this.isVisible())
                {
                    if (key == Keys.Enter)
                    {
                        this.accept();
                    }

                    if (key == Keys.Escape)
                    {
                        this.refuse();
                    }
                }
            };
        }

        public void accept()
        {
            this.hide();
            Function.Call(Hash.SET_GAME_PAUSED, false);
            base.PopupClose();
            OnPopupAccept?.Invoke();
        }

        public void refuse()
        {
            this.hide();
            Function.Call(Hash.SET_GAME_PAUSED, false);
            base.PopupClose();
            OnPopupRefuse?.Invoke();
        }
    }
}
