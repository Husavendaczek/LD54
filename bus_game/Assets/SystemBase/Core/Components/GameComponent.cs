using System;
using System.Collections.Generic;
using System.Linq;
using SystemBase.Core.GameSystems;
using SystemBase.Utils;
using UniRx;
using UnityEngine;

namespace SystemBase.Core.Components
{
    public class GameComponent : MonoBehaviour, IGameComponent
    {
        private readonly List<IDisposable> _componentDisposables = new();
        public virtual IGameSystem System { get; set; }

        private void RegisterToGame()
        {
            var game = IoC.Resolve<Game>();
            game.RegisterComponent(this);
            if (GetType().GetCustomAttributes(typeof(SingletonComponentAttribute), true).Any())
            {
                game.RegisterSingletonComponent(this);
            }
        }

        public IObservable<TComponent> WaitOn<TComponent>(ReactiveProperty<TComponent> componentToWaitOnTo)
            where TComponent : GameComponent
        {
            return componentToWaitOnTo.WhereNotNull().Select(waitedComponent => waitedComponent);
        }

        public IDisposable WaitOn<TComponent>(ReactiveProperty<TComponent> componentToWaitOnTo, Action<TComponent> onNext)
            where TComponent : GameComponent
        {
            return componentToWaitOnTo
                .WhereNotNull()
                .Select(waitedComponent => waitedComponent)
                .Subscribe(onNext)
                .AddTo(this);
        }

        public T AddDisposable<T>(T disposable) where T : IDisposable
        {
            _componentDisposables.Add(disposable);
            return disposable;
        }

        protected void Start()
        {
            RegisterToGame();

            OverwriteStart();
        }

        protected virtual void OverwriteStart()
        {
        }

        protected void OnDestroy()
        {
            _componentDisposables.ForEach(disposable => disposable.Dispose());
        }
    }

    public class SemanticGameComponent<TGameComponent> : GameComponent where TGameComponent : IGameComponent
    {
        public TGameComponent dependency;

        public TGameComponent Dependency
        {
            get => dependency != null ? dependency : GetComponent<TGameComponent>();
            set => dependency = value;
        }
    }
}
