using GTA;
using GTA.Native;
using System;
using System.Windows.Forms;

namespace DemagoScript.GUI.popup
{
    class NotificationPopup : Popup
    {
        /// <summary>
        /// Called when user close popup.
        /// </summary>
        public event PopupCloseEvent OnPopupClose;

        public NotificationPopup(bool includeControlsEvents = true) : base()
        {
            if (includeControlsEvents)
            {
                GUIManager.Instance.menu.OnControlPressed += (GTA.Control control) =>
                {
                    if (this.isVisible())
                    {
                        if (control == GTA.Control.PhoneSelect || control == GTA.Control.PhoneCancel)
                        {
                            this.PopupClose();
                        }
                    }
                };

                GUIManager.Instance.menu.OnKeysPressedEvent += (Keys key) =>
                {
                    if (this.isVisible())
                    {
                        if (key == Keys.Enter || key == Keys.Escape)
                        {
                            this.PopupClose();
                        }
                    }
                };
            }
        }

        protected virtual void PopupClose()
        {
            this.hide();
            Function.Call(Hash.SET_GAME_PAUSED, false);
            OnPopupClose?.Invoke();
        }

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
    }
}