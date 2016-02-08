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

        public override void removeDestructibleElements(bool removePhysicalElements = false)
        {
            Tools.trace( getName() + " removePhysicalElements = " + removePhysicalElements, System.Reflection.MethodBase.GetCurrentMethod().Name, "Mission" );

            foreach ( AbstractObjective objective in objectives)
                objective.removeDestructibleElements(removePhysicalElements);

            if (removePhysicalElements)
                objectives.Clear();
        }

        public override void start()
        {
            base.start();

            Game.Player.WantedLevel = 0;

            Tools.log("start first objective");
            objectives[currentObjectiveIndex].start();
        }

        public override void stop()
        {
            base.stop();
            currentObjectiveIndex = 0;
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
                Tools.log("On failed goal event" + sender.getName());
                fail(reason);
            };
            objective.OnAccomplished += (sender, elapsedTime) => {
                Tools.log("On accomplished goal event" + sender.getName());
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


            this.checkIfPlayerIsDeadOrArrested();

            if (currentObjectiveIndex < objectives.Count)
            {
                AbstractObjective objective = objectives[currentObjectiveIndex];
                objective.update();
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
            AbstractObjective objective = objectives[currentObjectiveIndex];
            Tools.log("-----Next objective named " + objective.getName() + "!!!!");

            objective.start();
        }

        private void checkIfPlayerIsDeadOrArrested()
        {
            if (Game.Player.IsDead)
            {
                this.fail("Vous êtes mort");
            }

            if (Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED, Game.Player, true))
            {
                this.fail("Vous vous êtes fait arrêter");
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
