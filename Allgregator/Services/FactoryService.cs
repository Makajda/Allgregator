using Prism.Ioc;

namespace Allgregator.Services {
    public class FactoryService {
        private readonly IContainerProvider container;
        public FactoryService(IContainerProvider container) {
            this.container = container;
        }

        public T Resolve<T>() {
            return container.Resolve<T>();
        }

        public T Resolve<T1, T>(object parameter) {
            return container.Resolve<T>((typeof(T1), parameter));
        }
    }
}
