using DemagoScript.GUI;
using DemagoScript.GUI.elements;
using GTA;
using GTA.Native;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DemagoScript.GUI.popup
{
    public delegate void PopupRefuseEvent();
    public delegate void PopupAcceptEvent();
    public delegate void PopupCloseEvent();

    class Popup : UIStack
    {
        public virtual void OnKeyDown(object sender, KeyEventArgs e) { }
    }
}
