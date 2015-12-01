using DemagoScript;
using GTA.Math;
using System;
using System.Drawing;

namespace NativeUI
{
    public class UIMenuEditableNumericItem : UIMenuItem
    {
        private readonly Sprite _rightArrow;
        private readonly Sprite _leftArrow;
        private readonly UIResText _valueText;
        private int _followingKeyEventNumber;
        private DateTime _lastIncreaseTime;
        private DateTime _lastDecreaseTime;

        /// <summary>
        /// Numeric value item with arrow to edit value with keyboard.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="check"></param>
        /// <param name="description">Description for this item.</param>
        public UIMenuEditableNumericItem(string text, float defaultValue, float minimum, float maximum, float step)
            : this(text, defaultValue, minimum, maximum, step, "")
        {
        }

        public UIMenuEditableNumericItem(string text, float defaultValue, float minimum, float maximum, float step, string description)
            : base(text, description)
        {
            Value = defaultValue;
            Minimum = minimum;
            Maximum = maximum;
            Step = step;
            _followingKeyEventNumber = 0;

            _valueText = new UIResText(defaultValue.ToString(), new Point(0, 0), 0.37f, Color.WhiteSmoke, GTA.Font.ChaletLondon, UIResText.Alignment.Centered);
            _rightArrow = new Sprite("commonmenu", "arrowright", new Point(0, 0), new Size(30, 30));
            _leftArrow = new Sprite("commonmenu", "arrowleft", new Point(0, 0), new Size(30, 30));
        }

        /// <summary>
        /// Change or get current numeric item's value.
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// Change or get maximum value.
        /// </summary>
        public float Maximum { get; set; }

        /// <summary>
        /// Change or get maximum value.
        /// </summary>
        public float Minimum { get; set; }

        /// <summary>
        /// Change or get step to add or substract to value
        /// </summary>
        public float Step { get; set; }

        /// <summary>
        /// Change item's position.
        /// </summary>
        /// <param name="y">New Y value.</param>
        public override void Position(int y)
        {
            base.Position(y);
            _valueText.Position = new Point(360 + Offset.X + Parent.WidthOffset, y + 150 + Offset.Y);
            _leftArrow.Position = Point.Add(_valueText.Position, new Size(-40 - _leftArrow.Size.Width, -20));
            _rightArrow.Position = Point.Add(_valueText.Position, new Size(40, -20));
        }

        /// <summary>
        /// Draw item.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            _valueText.Position = new Point(370 + Offset.X + Parent.WidthOffset, _valueText.Position.Y);
            _leftArrow.Position = Point.Subtract(_valueText.Position, new Size(60, 0));
            _rightArrow.Position = Point.Add(_valueText.Position, new Size(30, 0));

            _valueText.Caption = Value.ToString();
            _valueText.Draw();

            if (Value != Minimum)
            {
                _leftArrow.Draw();
            }

            if (Value != Maximum)
            {
                _rightArrow.Draw();
            }
        }

        public void Increase()
        {
            if (Value < Maximum)
            {
                Value += GetRealStep("increase");
            }

            if (Value > Maximum)
            {
                Value = Maximum;
            }
        }

        public void Decrease()
        {
            if (Value > Minimum)
            {
                Value -= GetRealStep("decrease");
            }
            
            if (Value < Minimum)
            {
                Value = Minimum;
            }
        }

        private float GetRealStep(string inscreaseOrDecrease = "")
        {
            TimeSpan timeElapsed = new TimeSpan();
            if (inscreaseOrDecrease == "increase")
            {
                timeElapsed = DateTime.Now - _lastIncreaseTime;
                _lastIncreaseTime = DateTime.Now;
            }
            else if (inscreaseOrDecrease == "decrease")
            {
                timeElapsed = DateTime.Now - _lastDecreaseTime;
                _lastDecreaseTime = DateTime.Now;
            }
            else
            {
                return Step;
            }

            if (timeElapsed.TotalMilliseconds < 1000)
            {
                _followingKeyEventNumber++;
            }
            else
            {
                _followingKeyEventNumber = 0;
            }

            if (_followingKeyEventNumber > 20)
            {
                return Step * 500;
            }
            if (_followingKeyEventNumber > 15)
            {
                return Step * 100;
            }
            else if (_followingKeyEventNumber > 10)
            {
                return Step * 10;
            }
            else if (_followingKeyEventNumber > 6)
            {
                return Step * 2;
            }

            return Step;
        }

        public override void SetRightBadge(BadgeStyle badge)
        {
            throw new Exception("UIMenuCheckboxItem cannot have a right badge.");
        }

        public override void SetRightLabel(string text)
        {
            throw new Exception("UIMenuListItem cannot have a right label.");
        }
    }
}