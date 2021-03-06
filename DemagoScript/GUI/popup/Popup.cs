﻿using DemagoScript.GUI;
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
        public Popup()
        {
            GUIManager.Instance.popupManager.add(this);
        }

        ~Popup()
        {
            GUIManager.Instance.popupManager.remove(this);
        }
    }
}
