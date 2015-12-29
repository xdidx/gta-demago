using GTA;
using GTA.Native;
using System.Windows.Forms;

namespace DemagoScript.GUI.popup
{
    class NotificationPopup : Popup
    {
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