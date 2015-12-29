using GTA.Math;
using GTA.Native;

namespace DemagoScript.GUI.elements
{
    class UIRectElement : IUIElement
    {
        private double x = 0;
        private double y = 0;
        private double width = 1;
        private double height = 1;
        private Vector3 color = UIColor.WHITE;
        private int alpha = 255;

        public UIRectElement( double x, double y, double width, double height, Vector3 color, int alpha )
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.color = color;
            this.alpha = alpha;
        }

        public void draw()
        {
            Function.Call( 
                Hash.DRAW_RECT,
                (double)this.x,
                (double)this.y,
                (double)this.width,
                (double)this.height,
                (int)this.color.X,
                (int)this.color.Y,
                (int)this.color.Z,
                this.alpha
           );
        }
    }
}
