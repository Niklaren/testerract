using UnityEngine;
using System.Collections;

public class UseOn : UseableItem {

    int LayerMask = 1 << 9; // use on pieces

    PickupableItem p;

    // Use this for initialization
    protected override void Start () {
        p = gameObject.AddComponent<PickupableItem>();
        base.Start();
	}
	
	// Update is called once per frame
	override protected void Update () {
        if (Input.GetKeyDown(KeyCode.E) && p.carried)
        {
            Debug.Log("try use");
            TryUse();
        }
    }

    override protected bool TryUse()
    {
        Debug.Log("UseOn: TryUse");
        RaycastHit hit;
        ElementSolid ES = null;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 5.0f, LayerMask))
        {
            ES = hit.collider.GetComponent<ElementSolid>();
        }

        if (ES != correctTarget)
        {
            Debug.Log("incorrect target");
            FailToUse();
            return false;
        }

        return base.TryUse();
    }

    override protected void Use()
    {
        Debug.Log("UseOn: use");
        p.Drop();
        Destroy(p);
        Remove();
        base.Use();
    }
}
