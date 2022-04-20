using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCLMaterial
{
    public Material material;
    DataStore_ComponentTextures dataStore;
    string dcltextureId;
	
    public DCLMaterial(DataStore_ComponentTextures dataStore, string dcltextureId)
    {
        this.dataStore = dataStore;
        this.dataStore.textures.OnAdded =+ OnAdded;
        this.dcltextureId = dcltextureId;
    }
	
    public void OnAdded(string id)
    {
        if(id == dcltextureId)
            material.SetTexture("_MainTexture", dataStore.textures[id]);
    }
}
