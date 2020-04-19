using DryIoc;

namespace Allgregator.Services {
    public class FactoryService {
        private readonly IContainer container;
        public FactoryService(
            IContainer container
            ) {
            this.container = container;
        }

        public T Resolve<T>(params object[] parameters) {
            return (T)container.Resolve(typeof(T), parameters);
        }
    }
}
