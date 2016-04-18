using System.Collections.Generic;
using System.Windows.Forms;

namespace DemagoScript.GUI.popup
{
    class PopupManager
    {
        private List<Popup> popups = null;

        public PopupManager()
        {
            this.popups = new List<Popup>();
        }

        public void add(Popup popup)
        {
            this.popups.Add(popup);
        }

        public void remove(Popup popup)
        {
            if (this.popups.Contains(popup))
            {
                this.popups.Remove(popup);
            }
        }

        public void update()
        {
            foreach ( Popup popup in this.popups ) {
                if (popup != null)
                {
                    popup.draw();
                }
            }
        }
    }
}
