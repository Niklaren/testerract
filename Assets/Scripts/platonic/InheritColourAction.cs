using UnityEngine;
using System.Collections;
using System;

public class InheritColourAction : IAction {

    [SerializeField]
    UseableItem item;
    [SerializeField]
    ElementSolid element;

    //InheritColourAction(UseableItem p_item, ElementSolid p_element)
    //{
    //    item = p_item;
    //    element = p_element;
    //}

    public override void Perform()
    {
        element.GetComponentInChildren<MeshRenderer>().material = item.GetComponent<MeshRenderer>().material;
    }
}
