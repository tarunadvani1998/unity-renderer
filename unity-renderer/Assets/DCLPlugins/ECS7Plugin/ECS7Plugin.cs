using DCL;
using DCLPlugins.Transform;

namespace DCLPlugins
{
    public class ECS7Plugin : IPlugin
    {
        public ECS7Plugin()
        {
            var factory = DataStore.i.ecs7.componentsFactory;
            var transform = new TransformPlugin(factory);
        }

        public void Dispose()
        {
            // throw new System.NotImplementedException();
        }
    }
}