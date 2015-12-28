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
            // Hide HUD and RADAR
            if ( this.isVisible() ) {
                Function.Call( Hash.HIDE_HUD_AND_RADAR_THIS_FRAME );
            }

            if (!Game.IsPaused)
                Function.Call(Hash.SET_GAME_PAUSED, this.isVisible());
        }
    }
}
