using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDCLTexture
{
    Texture2D texture  { get; }
    string id { get; }
}
