using System;
using SystemBase.Core.Components;
using SystemBase.Core.GameSystems;
using SystemBase.Utils;
using UniRx;
using UnityEngine;

namespace SystemBase.Core
{
    public class GameBase : MonoBehaviour, IGameSystem
    {
        public StringReactiveProperty debugMainFrameCallback = new();
        private readonly GameSystemCollection _systems = new();
        private ISharedComponentCollection _sharedComponents;

        public Type[] ComponentsToRegister => Type.EmptyTypes;

        public virtual void Init()
        {
            _sharedComponents = IoC.Resolve<ISharedComponentCollection>();
            _systems.Initialize();

            DontDestroyOnLoad(this);

            debugMainFrameCallback.ObserveOnMainThread()
                .Subscribe(OnDebugCallbackCalled);
        }

        public void RegisterComponent(GameComponent component)
        {
            _systems.RegisterComponent(component);
        }


        public T System<T>() where T : IGameSystem, new()
        {
            return _systems.GetSystem<T>();
        }

        protected virtual void OnDebugCallbackCalled(string s)
        {
            print(s);
        }

        public void RegisterSingletonComponent(GameComponent gameComponent)
        {
            _sharedComponents.RegisterComponent(gameComponent);
        }
    }
}