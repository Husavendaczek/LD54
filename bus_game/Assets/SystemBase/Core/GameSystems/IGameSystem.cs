using System;
using SystemBase.Core.Components;

namespace SystemBase.Core.GameSystems
{
    public interface IGameSystem
    {
        Type[] ComponentsToRegister { get; }

        void Init();

        void RegisterComponent(GameComponent component);
    }
}
