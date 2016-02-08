using DemagoScript.GUI.elements;
using GTA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemagoScript.GUI
{
    class MissionUI : UIStack
    {
        private string objective = "";
        private string advice = "";

        private UITextElement objectiveTextElement = null;
        private UITextElement adviceTextElement = null;

        public MissionUI()
        {
            
        }

        public void setObjective(string text)
        {
            this.objective = text;

            if (this.objectiveTextElement == null)
            {
                this.objectiveTextElement = new UITextElement(this.objective, Game.ScreenResolution.Width / 2, Game.ScreenResolution.Height / 5, 0.7f, true, GTA.Font.ChaletComprimeCologne, UIColor.WHITE);
                this.add(this.objectiveTextElement);
            }

            this.objectiveTextElement.setText(this.objective);
        }

        public void setAdvice(string text)
        {
            this.advice = text;

            if (this.adviceTextElement == null)
            {
                this.adviceTextElement = new UITextElement(this.advice, Game.ScreenResolution.Width / 2, Game.ScreenResolution.Height / 5, 0.7f, true, GTA.Font.ChaletComprimeCologne, UIColor.GREEN);
                this.add(this.adviceTextElement);
            }

            this.adviceTextElement.setText(this.advice);
        }
    }
}
