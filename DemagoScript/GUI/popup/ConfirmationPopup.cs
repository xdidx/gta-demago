using DemagoScript.GUI.elements;
using GTA.Math;
using GTA;

namespace DemagoScript.GUI.popup
{
    class ConfirmationPopup : NotificationPopup
    {
        private UIRectElement background = null;
        private UITextElement title = null;
        private UITextElement content = null;

        private UITextElement infos = null;
        private const string INFOS = "entrer pour valider, echap pour annuler";

        private string text_title = "";
        private string text_content = "";

        public ConfirmationPopup( string text_title, string text_content )
        {
            this.text_title = text_title;
            this.text_content = text_content;
            
            // background
            this.background = new UIRectElement( 0, 0, 2, 2, UIColor.BLACK, 230 );
            this.add( this.background );
            
            // title
            this.title = new UITextElement( this.text_title, 0.5, 0.4, 2, true, Font.Pricedown, UIColor.GTA_YELLOW );
            this.add( this.title );
            
            // top separator
            this.add( new UIRectElement( 0.5, 0.525, 0.9, 0.002, UIColor.WHITE, 255 ) );

            // content
            this.content = new UITextElement( this.text_content, 0.5, 0.530, 0.475, true, Font.ChaletLondon, UIColor.WHITE );
            this.add( this.content );
            
            // bottom separator
            this.add( new UIRectElement( 0.5, 0.58, 0.9, 0.002, UIColor.WHITE, 255 ) );

            // helper
            this.infos = new UITextElement( INFOS, 0.5, 0.9, 0.6, true, Font.HouseScript, UIColor.WHITE );
            this.add( this.infos );
        }
    }
}
