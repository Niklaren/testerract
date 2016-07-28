using UnityEngine;
using System.Collections;

public class PickupableItem : MonoBehaviour {

    protected Rigidbody rb;
    public bool carried;

    virtual protected void Start()
    {
        Debug.Log("PI start");
        rb = GetComponent<Rigidbody>();
        carried = false;
    }

    //void OnMouseDown()
    //{

    //}

    virtual public void Pickup()
    {
        carried = true;
        transform.parent = Camera.main.transform;
        transform.localPosition = new Vector3(0, 0, 0.7f);
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        rb.useGravity = false;
    }

    virtual public void Drop()
    {
        carried = false;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;
        transform.parent = null;
    }
}
