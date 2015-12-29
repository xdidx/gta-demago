using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemagoScript.GUI.elements
{
    class UIStack
    {
        private bool visible = true;
        private List<IUIElement> elements = new List<IUIElement>();

        public void add( IUIElement element )
        {
            this.elements.Add( element );
        }

        public virtual void draw()
        {
            if ( this.visible ) {
                foreach ( IUIElement element in this.elements ) {
                    element.draw();
                }
            }
        }

        public virtual void show()
        {
            this.visible = true;
        }

        public virtual void hide()
        {
            this.visible = false;
        }

        public bool isVisible()
        {
            return this.visible;
        }
    }
}
