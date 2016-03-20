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
        private string missionTime = "";
        private string objectiveTime = "";

        private UITextElement objectiveTextElement = null;
        private UITextElement adviceTextElement = null;
        private UITextElement missionTimeTextElement = null;
        private UITextElement objectiveTimeTextElement = null;

        public MissionUI()
        {
            
        }

        public void setObjective(string text)
        {
            if ( String.IsNullOrWhiteSpace( text ) ) {
                return;
            }
            
            this.objective = text;

            if (this.objectiveTextElement == null)
            {
                this.objectiveTextElement = new UITextElement(this.objective, 0.5, 0.2, 0.7f, true, GTA.Font.ChaletComprimeCologne, UIColor.WHITE);
                this.add(this.objectiveTextElement);
            }

            this.objectiveTextElement.setText(this.objective);
            this.show();
        }

        public void setAdvice(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                return;
            }

            this.advice = text;

            if (this.adviceTextElement == null)
            {
                this.adviceTextElement = new UITextElement(this.advice, 0.5, 0.25, 0.7f, true, GTA.Font.ChaletComprimeCologne, UIColor.GREEN);
                this.add(this.adviceTextElement);
            }

            this.adviceTextElement.setText(this.advice);
            this.show();
        }

        public void setMissionTime(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                return;
            }

            this.missionTime = text;

            if (this.missionTimeTextElement == null)
            {
                this.missionTimeTextElement = new UITextElement(this.advice, 0.1, 0.1, 0.5f, true, GTA.Font.ChaletComprimeCologne, UIColor.WHITE);
                this.add(this.missionTimeTextElement);
            }

            this.missionTimeTextElement.setText(this.missionTime);
        }

        public void setObjectiveTime(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                return;
            }

            this.objectiveTime = text;

            if (this.objectiveTimeTextElement == null)
            {
                this.objectiveTimeTextElement = new UITextElement(this.advice, 0.1, 0.13, 0.5f, true, GTA.Font.ChaletComprimeCologne, UIColor.WHITE);
                this.add(this.objectiveTimeTextElement);
            }

            this.objectiveTimeTextElement.setText(this.objectiveTime);
        }
    }
}
