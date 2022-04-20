using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Resource
{
    protected object value;

    public void Set(object value) => this.value = value;
    
    protected object Get() => value;
    
    public abstract void Destroy();
}
