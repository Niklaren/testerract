using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UseableItem : MonoBehaviour {

    int useOrder;
    public ElementSolid correctTarget;

    [SerializeField]
    List<UseableItem> othersRequired = new List<UseableItem>();
    public bool used;

    [SerializeField]
    IAction action;

    [SerializeField]
    AudioClip success;
    [SerializeField]
    AudioClip failure;

    AudioSource audioSource;

    // Use this for initialization
    virtual protected void Start () {
        action = GetComponent<IAction>();
        audioSource = gameObject.AddComponent<AudioSource>();
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

        // posibly require puzzle to not be in stage 0?

        if (used)
        {
            Debug.Log(" already used");
            FailToUse();
            return false;
        }

        if(!correctTarget.InCorrectSocket())
        {
            Debug.Log("target not in socket");
            FailToUse();
            return false;
        }

        for (int i = 0; i < othersRequired.Count; i++)
        {
            if (!othersRequired[i].used)
            {
                Debug.Log("prerequisite not met");
                FailToUse();
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
        audioSource.clip = success;
        audioSource.Play();
        action.Perform();
        correctTarget.CheckComplete();
    }

    protected void FailToUse()
    {
        audioSource.clip = failure;
        audioSource.Play();
    }

    public void Remove()
    {
        transform.position = new Vector3(-999, -999, -999);
        Destroy(GetComponent<Rigidbody>());
 
    }
}
