using System;
using System.Collections.Generic;

[Serializable]
public class DropdownComponentModel : BaseComponentModel
{
    public string title;
    public bool isMultiselect = true;
    public List<ToggleComponentModel> options;
    public string searchPlaceHolderText;
}
