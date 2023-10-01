using SystemBase.Core.GameSystems;

namespace SystemBase.Core.Components
{
    public interface IGameComponent
    {
        IGameSystem System { get; set; }
    }
}
