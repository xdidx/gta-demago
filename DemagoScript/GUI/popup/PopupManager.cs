﻿using System.Collections.Generic;
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

        public void add( Popup popup )
        {
            this.popups.Add( popup );
        }

        public void update()
        {
            foreach ( Popup popup in this.popups ) {
                popup.draw();
            }
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            foreach (Popup popup in this.popups)
            {
                popup.OnKeyDown(sender, e);
            }
        }
    }
}
