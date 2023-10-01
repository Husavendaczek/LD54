using System;
using System.Collections.Generic;
using SystemBase.Utils;
using UniRx;

namespace SystemBase.Core.Components
{
    public interface ISharedComponentCollection
    {
        void RegisterComponent(GameComponent component);
        T Get<T>() where T : GameComponent, new();
        bool TryGet<T>(out T component) where T : GameComponent, new();
        IDisposable Subscribe<T>(Action<T> action) where T : GameComponent, new();
    }

    public class SharedComponentCollection : ISharedComponentCollection
    {
        private readonly Dictionary<Type, ReactiveProperty<GameComponent>> _components = new();

        public void RegisterComponent(GameComponent component)
        {
            var type = component.GetType();
            if (!_components.ContainsKey(type)) _components.Add(type, new ReactiveProperty<GameComponent>());

            _components[type].Value = component;
        }

        public T Get<T>() where T : GameComponent, new()
        {
            var type = typeof(T);
            if (!_components.ContainsKey(type)) throw new Exception($"Component of type {type} not found");

            return (T)_components[type].Value;
        }

        public bool TryGet<T>(out T component) where T : GameComponent, new()
        {
            var type = typeof(T);
            if (!_components.ContainsKey(type) || _components[type].Value == null)
            {
                component = null;
                return false;
            }

            component = (T)_components[type].Value;
            return true;
        }

        public IDisposable Subscribe<T>(Action<T> action) where T : GameComponent, new()
        {
            var type = typeof(T);
            if (_components.TryGetValue(type, out var c))
                return c.WhereNotNull().Select(component => (T)component).Subscribe(action);
            {
                var reactiveProperty = new ReactiveProperty<GameComponent>();
                _components.Add(type, reactiveProperty);
                return reactiveProperty.WhereNotNull().Select(component => (T)component).Subscribe(action);
            }
        }
    }
}