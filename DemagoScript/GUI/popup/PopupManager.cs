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

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            for (int i = this.popups.Count - 1; i >= 0; i--)
            {
                this.popups[i].OnKeyDown(sender, e);
            }
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            for (int i = this.popups.Count - 1; i >= 0; i--)
            {
                this.popups[i].OnKeyUp(sender, e);
            }
        }
    }
}
