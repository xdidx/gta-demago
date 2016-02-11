using DemagoScript.GUI;
using DemagoScript.GUI.popup;
using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
using System;
using System.Collections.Generic;

namespace DemagoScript
{
    abstract class Mission : AbstractObjective
    {
        private List<AbstractObjective> objectives = new List<AbstractObjective>();
        private int currentObjectiveIndex = 0;
        

        public override void populateDestructibleElements()
        {

        }

        public override void depopulateDestructibleElements(bool removePhysicalElements = false)
        {
            Tools.trace( getName() + " removePhysicalElements = " + removePhysicalElements, System.Reflection.MethodBase.GetCurrentMethod().Name, "Mission" );

            foreach (AbstractObjective objective in objectives)
            {
                objective.depopulateDestructibleElements(removePhysicalElements);
            }

            if (removePhysicalElements)
                objectives.Clear();
        }

        public override void start()
        {
            base.start();

            Game.Player.WantedLevel = 0;
            
            var currentObjective = objectives[currentObjectiveIndex];
            if ( currentObjective != null ) {
                Tools.trace( "Lancement du premier objectif", System.Reflection.MethodBase.GetCurrentMethod().Name, "Mission" );
                objectives[currentObjectiveIndex].start();
            } else {
                Tools.trace( "Pas d'objectifs dans cette missions", System.Reflection.MethodBase.GetCurrentMethod().Name, "Mission" );
            }
        }

        /// <summary>
        /// When the mission is stopped, everything has to be removed
        /// </summary>
        /// <param name="removePhysicalElements"></param>
        public override void stop( bool removePhysicalElements = false )
        {
            Tools.trace( getName() + " removePhysicalElements = " + removePhysicalElements, System.Reflection.MethodBase.GetCurrentMethod().Name, "Mission" );
            base.stop(true);
            currentObjectiveIndex = 0;
            foreach(AbstractObjective objective in objectives)
            {
                objective.depopulateDestructibleElements(true);
            }
            objectives.Clear();
        }
        
        public override void accomplish()
        {
            base.accomplish();

            Game.Player.WantedLevel = 0;
            Game.Player.Character.Armor = 100;
            Game.Player.Character.MaxHealth = 100;
            Game.Player.Character.Health = Game.Player.Character.MaxHealth;
        }
        
        public void addObjective(AbstractObjective objective)
        {
            objective.OnFailed += (sender, reason) => {
                fail(reason);
            };
            objective.OnAccomplished += (sender, elapsedTime) => {
                this.next();
            };
            objectives.Add(objective);
        }

        public override bool update()
        {
            if (!base.update())
            {
                return false;
            }

            if (currentObjectiveIndex < objectives.Count)
            {
                AbstractObjective objective = objectives[currentObjectiveIndex];
                objective.update();

                GUIManager.Instance.missionUI.setMissionTime(Tools.getTextFromMilliSeconds(this.getElaspedTime()));
                GUIManager.Instance.missionUI.setObjectiveTime(Tools.getTextFromMilliSeconds(objective.getElaspedTime()));
            }
            else
            {
                this.accomplish();
            }
            
            return true;
        }

        private void next()
        {
            currentObjectiveIndex++;

            if (currentObjectiveIndex >= objectives.Count)
            {
                this.accomplish();
            }
            else
            {
                AbstractObjective objective = objectives[currentObjectiveIndex];
                objective.start();
            }
        }

        public virtual UIMenuItem addStartItem( ref UIMenu menu )
        {
            var startItem = new UIMenuItem( "Démarrer la mission" );
            menu.AddItem( startItem );

            menu.OnItemSelect += ( sender, item, index ) => {
                if ( item == startItem ) {
                    sender.Visible = false;
                }
            };

            return startItem;
        }

        public virtual void fillMenu(ref UIMenu menu) { }
    }
}
