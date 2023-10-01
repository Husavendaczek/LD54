using SystemBase.Core.Components;
using SystemBase.Utils;

namespace SystemBase.Core
{
    public class IoCRegistrations : IIocRegistration
    {
        public void Register()
        {
            IoC.RegisterSingleton<ISharedComponentCollection, SharedComponentCollection>();
        }
    }
}