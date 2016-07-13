using System;
using Autofac;
using Autofac.Integration.Mvc;

namespace Varys.API.Infrastructure
{
    public class EngineContext
    {
        public static ILifetimeScope Current
        {
            get
            {
                return AutofacDependencyResolver.Current.RequestLifetimeScope;
            }
        }

        public static T Resolve<T>(string key = "") where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                return Current.Resolve<T>();
            }
            return Current.ResolveKeyed<T>(key);
        }

        public static object Resolve(Type type)
        {
            return Current.Resolve(type);
        }
    }
}