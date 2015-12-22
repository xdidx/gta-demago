using GTA;
using GTA.Native;

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

            // TODO: Hide DemagoMenu for a frame
            // ???

            // Pause the game
            // TODO: Faire mieux (SET_GAME_PAUSED est appelé a chaque frame...)
            Function.Call( Hash.SET_GAME_PAUSED, this.isVisible() );
        }
    }
}
