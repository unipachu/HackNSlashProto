using UnityEngine;
public class ShowOnlyAttribute : PropertyAttribute
{
    //Nothing needed in here. However this Class shouldn't be in the Editor folder because script using this attribute
    //will get null reference when a build is made. I think this property should work even when ShowOnlyDrawer
    //is in Editor folder, but make sure of this BE
}

