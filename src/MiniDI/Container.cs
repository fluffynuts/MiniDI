namespace MiniDI
{
    public class Container
    {
        private readonly Dictionary<Type, Type> _transientResolutions = new();

        public void RegisterTransient<TService, TImplementation>()
        {
            _transientResolutions[typeof(TService)] = typeof(TImplementation);
        }
        
        public TService Resolve<TService>()
        {
            var serviceType = typeof(TService);
            return (TService)Resolve(serviceType);
        }
        
        public object Resolve(Type serviceType)
        {
            if (!serviceType.IsInterface)
            {
                // naive!
                return Activator.CreateInstance(serviceType);
            }
        
            if (!_transientResolutions.TryGetValue(serviceType, out var serviceImplementation))
            {
                throw new NotSupportedException($"Can't build service of type {serviceType}");
            }
        
            var constructors = serviceImplementation.GetConstructors();
            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                var parameterImplementations = parameters.Select(p =>
                {
                    var parameterType = p.ParameterType;
                    var supported = _transientResolutions.TryGetValue(parameterType, out var parameterImplementation);
                    return new
                    {
                        Implementation = parameterImplementation,
                        Supported = supported
                    };
                }).ToArray();
        
                if (parameterImplementations.Length == 0 || 
                    parameterImplementations.All(p => p.Supported))
                {
                    var constructorParameters = parameterImplementations
                        .Select(o => Resolve(o.Implementation))
                        .ToArray();
                    return constructor.Invoke(constructorParameters);
                }
            }
        
            throw new NotSupportedException($"No constructor for {serviceType} could be invoked");
        }
    }
}