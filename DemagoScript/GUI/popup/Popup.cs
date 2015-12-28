using DemagoScript.GUI;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DemagoScript
{
    public delegate void PopupRefuseEvent();
    public delegate void PopupAcceptEvent();

    class Popup
    {
        /// <summary>
        /// Called when user accept popup question.
        /// </summary>
        public event PopupAcceptEvent OnPopupAccept;

        /// <summary>
        /// Called when user refuse popup question.
        /// </summary>
        public event PopupRefuseEvent OnPopupRefuse;


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

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (this.visible)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.hide();
                    OnPopupAccept?.Invoke();
                }

                if (e.KeyCode == Keys.Escape)
                {
                    this.hide();
                    OnPopupRefuse?.Invoke();
                }
            }
        }
    }
}
