using System;

namespace Superman.Trainer
{
    public enum TrainerItem
    {
        Superman,
        Flight,
        SuperSpeed,
        Powers,
        Combat,
        Suit,
        Animation,
        Physics,
        Settings
    }

    public sealed class SupermanTrainer
    {
        private bool _open;
        private int _selected;

        private readonly TrainerItem[] _items =
        {
            TrainerItem.Superman,
            TrainerItem.Flight,
            TrainerItem.SuperSpeed,
            TrainerItem.Powers,
            TrainerItem.Combat,
            TrainerItem.Suit,
            TrainerItem.Animation,
            TrainerItem.Physics,
            TrainerItem.Settings
        };

        public bool IsOpen => _open;

        public TrainerItem Selected => _items[_selected];

        public int SelectedIndex => _selected;

        public int ItemCount => _items.Length;

        public void Toggle()
        {
            _open = !_open;
        }

        public void Open()
        {
            _open = true;
        }

        public void Close()
        {
            _open = false;
        }

        public void MoveUp()
        {
            _selected--;

            if (_selected < 0)
                _selected = _items.Length - 1;
        }

        public void MoveDown()
        {
            _selected++;

            if (_selected >= _items.Length)
                _selected = 0;
        }

        public string Activate()
        {
            switch (Selected)
            {
                case TrainerItem.Superman:
                    return "Superman";

                case TrainerItem.Flight:
                    return "Flight";

                case TrainerItem.SuperSpeed:
                    return "Super Speed";

                case TrainerItem.Powers:
                    return "Powers";

                case TrainerItem.Combat:
                    return "Combat";

                case TrainerItem.Suit:
                    return "Suit";

                case TrainerItem.Animation:
                    return "Animation";

                case TrainerItem.Physics:
                    return "Physics";

                case TrainerItem.Settings:
                    return "Settings";

                default:
                    return "Unknown";
            }
        }

        public string GetSelectedName()
        {
            switch (Selected)
            {
                case TrainerItem.Superman:
                    return "SUPERMAN";

                case TrainerItem.Flight:
                    return "FLIGHT";

                case TrainerItem.SuperSpeed:
                    return "SUPER SPEED";

                case TrainerItem.Powers:
                    return "POWERS";

                case TrainerItem.Combat:
                    return "COMBAT";

                case TrainerItem.Suit:
                    return "SUIT";

                case TrainerItem.Animation:
                    return "ANIMATION";

                case TrainerItem.Physics:
                    return "PHYSICS";

                case TrainerItem.Settings:
                    return "SETTINGS";

                default:
                    return "UNKNOWN";
            }
        }

        public string[] GetMenuItems()
        {
            return new[]
            {
                "SUPERMAN",
                "FLIGHT",
                "SUPER SPEED",
                "POWERS",
                "COMBAT",
                "SUIT",
                "ANIMATION",
                "PHYSICS",
                "SETTINGS"
            };
        }
    }
}
