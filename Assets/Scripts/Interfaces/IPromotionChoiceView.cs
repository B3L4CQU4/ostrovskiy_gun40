using System;
using TacticalPrototype;

namespace TacticalPrototype.Interfaces
{
    public interface IPromotionChoiceView
    {
        void Show(Action<UnitType> onChoice);
        void Hide();
    }
}
