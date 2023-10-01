using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SystemBase.Core.Components;
using SystemBase.Utils;
using UnityEngine;

namespace SystemBase.Core.GameSystems
{
    public class GameSystemCollection
    {
        private Dictionary<Type, List<IGameSystem>> _componentToSystemMap;
        private Dictionary<Type, IGameSystem> _systems;

        public void Initialize()
        {
            var systemInstances = InstantiateSystems();
            systemInstances = GameSystemSorter.Sort(systemInstances);
            PrintDependencyList(systemInstances);
            _componentToSystemMap = ComponentToSystemsMapper.CreateMap(systemInstances);
            _systems = systemInstances.ToDictionary(system => system.GetType(), system => system);

            foreach (var system in _systems.Values)
            {
                ResolveSystemIoCDependencies(system);
                system.Init();
            }
        }

        private static void ResolveSystemIoCDependencies(IGameSystem system)
        {
            var systemType = system.GetType();
            var properties = systemType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in properties)
            {
                if (!fieldInfo.GetCustomAttributes(typeof(IoCResolveAttribute), false).Any()) continue;

                var fieldType = fieldInfo.FieldType;
                var ioCDependency = IoC.Resolve(fieldType);
                fieldInfo.SetValue(system, ioCDependency);
            }
        }

        public TSystem GetSystem<TSystem>() where TSystem : IGameSystem, new()
        {
            if (_systems.TryGetValue(typeof(TSystem), out var system)) return (TSystem)system;

            throw new ArgumentException("System: " + typeof(TSystem) + " not registered!");
        }

        public void RegisterComponent(GameComponent component)
        {
            if (_componentToSystemMap.TryGetValue(component.GetType(), out var systemList))
                systemList.ForEach(system => system.RegisterComponent(component));
        }

        private static void PrintDependencyList(IEnumerable<IGameSystem> systems)
        {
            var listToPrint = systems
                .Aggregate("", (current, system) => current + (system.GetType() + "\n"));
            Debug.Log($"System List:\n{listToPrint}");
        }

        private static List<IGameSystem> InstantiateSystems()
        {
            return CollectAllSystems()
                .Select(systemType => Activator.CreateInstance(systemType) as IGameSystem)
                .ToList();
        }

        private static IEnumerable<Type> CollectAllSystems()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(ass => ass.GetTypes(), (ass, type) => new { ass, type })
                .Where(assemblyType => Attribute.IsDefined(assemblyType.type, typeof(GameSystemAttribute)))
                .Select(assemblyType => assemblyType.type);
        }
    }
}