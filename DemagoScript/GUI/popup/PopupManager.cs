using System.Collections.Generic;

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
                if ( popup.isVisible() ) {
                    popup.draw();
                }
            }   
        }
    }
}
