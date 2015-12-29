﻿using GTA;
using GTA.Native;
using System.Windows.Forms;

namespace DemagoScript.GUI.popup
{
    class NotificationPopup : Popup
    {
        /// <summary>
        /// Called when user close popup.
        /// </summary>
        public event PopupCloseEvent OnPopupClose;

        public override void draw()
        {
            base.draw();
            this.hideUselessElements();
        }

        private void hideUselessElements()
        {
            if ( this.isVisible() ) {
                // Hide DemagoMenu
                GUIManager.Instance.menu.hide();

                // Hide HUD and RADAR
                Function.Call( Hash.HIDE_HUD_AND_RADAR_THIS_FRAME );
            }

            if (!Game.IsPaused)
                Function.Call(Hash.SET_GAME_PAUSED, this.isVisible());
        }

        public override void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (this.isVisible())
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)
                {
                    this.hide();
                    OnPopupClose?.Invoke();
                }
            }
        }
    }
}