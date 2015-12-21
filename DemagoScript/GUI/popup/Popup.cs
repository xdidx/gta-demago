using DemagoScript.GUI;
using System.Collections.Generic;

namespace DemagoScript
{
    class Popup
    {
        private bool visible = true;
        private List<IUIElement> elements = new List<IUIElement>();
        
        public void add( IUIElement element )
        {
            this.elements.Add( element );
        }
        
        public void draw()
        {
            foreach ( IUIElement element in this.elements ) {
                element.draw();
            }
        }

        public void show()
        {
            this.visible = true;
        }

        public void hide()
        {
            this.visible = false;
        }

        public bool isVisible()
        {
            return this.visible;
        }
    }
}
