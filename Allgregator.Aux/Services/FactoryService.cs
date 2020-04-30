using Prism.Ioc;

namespace Allgregator.Aux.Services {
    public class FactoryService {
        private readonly IContainerProvider container;
        public FactoryService(IContainerProvider container) {
            this.container = container;
        }

        public T Resolve<T>() => container.Resolve<T>();
        public T Resolve<T>(string name) => container.Resolve<T>(name);
        public T Resolve<T1, T>(object parameter) => container.Resolve<T>((typeof(T1), parameter));
    }
}
