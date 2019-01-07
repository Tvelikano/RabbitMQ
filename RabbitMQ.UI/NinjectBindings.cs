using Ninject.Modules;
using RabbitMQ.Service;

namespace RabbitMQ.UI
{
    public class NinjectBindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IRabbitService>().To<BasicRabbitService>();
        }
    }

}
