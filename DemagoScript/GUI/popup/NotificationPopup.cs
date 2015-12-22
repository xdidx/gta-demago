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
            Function.Call( Hash.HIDE_HUD_AND_RADAR_THIS_FRAME );

            // TODO: Hide DemagoMenu for a frame
            // ???

            // TODO: Pause the game
            // ???
        }
    }
}
