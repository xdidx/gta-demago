using DemagoScript.GUI;
using DemagoScript.GUI.elements;
using GTA;
using GTA.Native;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DemagoScript
{
    public delegate void PopupRefuseEvent();
    public delegate void PopupAcceptEvent();

    class Popup : UIStack
    {
        /// <summary>
        /// Called when user accept popup question.
        /// </summary>
        public event PopupAcceptEvent OnPopupAccept;

        /// <summary>
        /// Called when user refuse popup question.
        /// </summary>
        public event PopupRefuseEvent OnPopupRefuse;
        
        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (this.isVisible())
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
