using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource_Material : Resource
{
    public new Material Get() => (Material)value; 
    
    public override void Destroy()
    {
        Material material = Get();
        if (material != null)
        {
            // Utils.SafeDestroy(material);
        }
    }
}
