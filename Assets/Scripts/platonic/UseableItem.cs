using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UseableItem : MonoBehaviour {

    int useOrder;
    public ElementSolid useOn;

    [SerializeField]
    List<UseableItem> othersRequired = new List<UseableItem>();
    public bool used;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	virtual protected void Update () {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 5.0f))
            {
                UseableItem item = hit.collider.GetComponent<UseableItem>();
                if (item == this)
                {
                    Debug.Log("try use");
                    TryUse();
                }
            }
        }

    }

    virtual protected bool TryUse()
    {
        Debug.Log("UseableItem: TryUse");
        if (used)
        {
            Debug.Log(" already used");
            // FailToUse()
            return false;
        }

        if(!useOn.InCorrectSocket())
        {
            Debug.Log("target not in socket");
            // FailToUse()
            return false;
        }

        for (int i = 0; i < othersRequired.Count; i++)
        {
            if (!othersRequired[i].used)
            {
                Debug.Log("prerequisite not met");
                // Fail To Use()
                return false;
            }
        }
        Use();
        return true;
    }

    virtual protected void Use()
    {
        Debug.Log("Useable: use");
        used = true;
        useOn.CheckComplete();
    }

    void FailToUse()
    {

    }

    public void Remove()
    {
        transform.position = new Vector3(-999, -999, -999);
        Destroy(GetComponent<Rigidbody>());
 
    }
}
