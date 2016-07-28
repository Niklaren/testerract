using UnityEngine;
using System.Collections;

public class PlatonicPlayerScript : MonoBehaviour {

    Camera cam;

    PickupableItem carryObject;

    // Use this for initialization
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (carryObject == null)
            {
                TryPickup();   
            }
            else if(!carryObject.carried)
            {
                carryObject = null;
                TryPickup();
            }
            else
            {
                carryObject.GetComponent<PickupableItem>().Drop();
                carryObject = null;
            }
        }

        if (carryObject != null)
        {

        }
    }

    private void TryPickup()
    {
        Debug.Log("try pickup");
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 5.0f, ~(1 << 8))) //layer mask ~1<<8 = ignore player
        {
            Debug.Log("something hit");
            Debug.Log("hit " + hit.collider.gameObject.name);
            PickupableItem PI = hit.collider.GetComponent<PickupableItem>();
            if (PI != null)
            {
                Debug.Log("do pickup");
                carryObject = PI;
                PI.Pickup();
            }
        }
    }
}
