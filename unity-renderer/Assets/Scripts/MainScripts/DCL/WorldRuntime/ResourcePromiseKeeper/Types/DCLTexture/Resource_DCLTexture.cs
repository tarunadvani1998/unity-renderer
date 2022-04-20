using System.Collections;
using System.Collections.Generic;
using DCL;
using UnityEngine;

public class Resource_DCLTexture : Resource
{
    public override void Destroy()
    {
        DCLTexture texture = (DCLTexture)Get();
        // texture.Dispose();
    }
}
