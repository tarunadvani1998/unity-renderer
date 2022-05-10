using System;
using UnityEngine;

namespace DCLPlugins.Transform
{
    public class Model
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public int parent;
    }

    public static class TransformDeserializer
    {
        public static byte[] tmpBuf = new byte[44];
        public static unsafe Model Deserialize(object data)
        {
            Model model = new Model();

            byte[] bytes = (byte[])data;

            if (bytes.Length < 40)
            {
                throw new IndexOutOfRangeException($"trying to deserialize Transform with a byte array of lenght {bytes.Length}");
            }

            if (BitConverter.IsLittleEndian)
            {
                for (int i = 0; i < 40; i++)
                {
                    tmpBuf[i] = bytes[39 - i];
                }

                fixed (byte* numPtr = &tmpBuf[0])
                {
                    float* arr = (float*)numPtr;
                    model.position.x = arr[9];
                    model.position.y = arr[8];
                    model.position.z = arr[7];
                    model.rotation.x = arr[6];
                    model.rotation.y = arr[5];
                    model.rotation.z = arr[4];
                    model.rotation.w = arr[3];
                    model.scale.x = arr[2];
                    model.scale.y = arr[1];
                    model.scale.z = arr[0];
                }
            }
            else
            {

                fixed (byte* numPtr = &bytes[0])
                {
                    float* arr = (float*)numPtr;
                    model.position.x = arr[0];
                    model.position.y = arr[1];
                    model.position.z = arr[2];
                    model.rotation.x = arr[3];
                    model.rotation.y = arr[4];
                    model.rotation.z = arr[5];
                    model.rotation.w = arr[6];
                    model.scale.x = arr[7];
                    model.scale.y = arr[8];
                    model.scale.z = arr[9];
                }
            }

            // fixed (byte* numPtr = &bytes[offset])
            // {
            //     model.parent = ByteUtils.PointerToInt32(numPtr);
            // }

            return model;
        }
    }
}