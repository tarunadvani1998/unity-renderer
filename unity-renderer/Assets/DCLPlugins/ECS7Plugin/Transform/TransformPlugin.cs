using System;
using DCL.ECSRuntime;
using UnityEngine;

namespace DCLPlugins.Transform
{
    public class TransformPlugin
    {
        public TransformPlugin(ECSComponentsFactory factory)
        {
            var handler = new Handler();
            factory.AddOrReplaceComponent(1, TransformDeserializer.Deserialize, () => handler);
            Debug.Log($"PATO: FUCKING LITTLE ENDIAN? {BitConverter.IsLittleEndian}");
        }
    }
}