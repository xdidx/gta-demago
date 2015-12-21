﻿using DemagoScript.GUI.popup;
using System.Collections.Generic;

namespace DemagoScript.GUI
{
    class GUIManager
    {
        private DemagoMenu menu = null;
        public PopupManager popupManager = null; 

        private static GUIManager instance;
        
        private GUIManager() { }

        public static GUIManager Instance
        {
            get {
                if ( GUIManager.instance == null ) {
                    GUIManager.instance = new GUIManager();
                }
                return GUIManager.instance; 
            }
        }

        public void initialize( List<Mission> missions = null )
        {
            this.menu = new DemagoMenu( missions );
            this.popupManager = new PopupManager();
        }

        public void update()
        {
            this.menu.process();
            this.popupManager.update();
        }

        public void toggleMenuDisplay()
        {
            this.menu.toggleDisplay();
        }
    }
}
