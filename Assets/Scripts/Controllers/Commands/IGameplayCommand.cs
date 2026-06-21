using TacticalPrototype.Units;

namespace TacticalPrototype.Controllers.Commands
{
    public interface IGameplayCommand
    {
        void Interact(Cell cell);
        void Cancel();
        void Confirm();
    }
}
