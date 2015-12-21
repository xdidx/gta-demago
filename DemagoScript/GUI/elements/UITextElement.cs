﻿using GTA.Native;
using GTA.Math;

namespace DemagoScript.GUI.elements
{
    class UITextElement : IUIElement
    {
        private string text = "";
        private double x = 0;
        private double y = 0;
        private double scale = 1;
        private bool center = true;
        private int font = 1;
        private Vector3 color = new Vector3( 255, 255, 255 );

        // Exemple:
        // DemagoMenu.drawText( "Hello world", 0.14, 0.49, 2.9, true, 1, 255, 0, 0 );

        public UITextElement( string text, double x, double y, double scale, bool center, int font, Vector3 color )
        {
            this.text = text;
            this.x = x;
            this.y = y;
            this.scale = scale;
            this.center = center;
            this.font = font;
            this.color = color;
        }
        
        public void draw()
        {
            Function.Call( Hash.SET_TEXT_FONT, this.font );
            Function.Call( Hash.SET_TEXT_SCALE, this.scale, this.scale );
            Function.Call( Hash.SET_TEXT_COLOUR, (int)this.color.X, (int)this.color.Y, (int)this.color.Z, 255 );
            Function.Call( Hash.SET_TEXT_WRAP, 0.0, 1.0 );
            Function.Call( Hash.SET_TEXT_CENTRE, this.center );
            Function.Call( Hash.SET_TEXT_EDGE, 2, 255, 255, 255, 205 );
            Function.Call( Hash._SET_TEXT_ENTRY, "STRING" );
            Function.Call( Hash.SET_TEXT_DROPSHADOW, 2, 0, 0, 0, 205 );
            Function.Call( Hash._ADD_TEXT_COMPONENT_STRING, this.text );
            Function.Call( Hash._DRAW_TEXT, this.y, this.x );
        }
        
        public static void drawText( string text, double x, double y, double scale, bool center, int font, int r, int g, int b )
        {
            Function.Call( Hash.SET_TEXT_FONT, font );
            Function.Call( Hash.SET_TEXT_SCALE, scale, scale );
            Function.Call( Hash.SET_TEXT_COLOUR, r, g, b, 255 );
            Function.Call( Hash.SET_TEXT_WRAP, 0.0, 1.0 );
            Function.Call( Hash.SET_TEXT_CENTRE, center );
            Function.Call( Hash.SET_TEXT_EDGE, 2, 255, 255, 255, 205 );
            Function.Call( Hash._SET_TEXT_ENTRY, "STRING" );
            Function.Call( Hash.SET_TEXT_DROPSHADOW, 2, 0, 0, 0, 205 );
            Function.Call( Hash._ADD_TEXT_COMPONENT_STRING, text );
            Function.Call( Hash._DRAW_TEXT, y, x );
        }
    }
}
