using UnityEngine;
using System.Collections;

public class PentagramPlayer : MonoBehaviour {

    Camera cam;
    public PentagramPipe carriedPipe;
    public PentagramPipe lastSelectedPipe;

    void Start () {
        cam = Camera.main;
    }
	
	void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            
            if (carriedPipe != null)
            {
                carriedPipe = carriedPipe.OnClick(); 
                //carriedPipe = null; // could have onclick() return type PentagramPipe if we need to set null or this more flexibly
            }
            else {
                Debug.Log("do raycast");
                RaycastHit hit;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 5.0f, (1 << 9))) //layer mask: only piece
                {
                    Debug.Log("something hit");
                    Debug.Log("hit " + hit.collider.gameObject.name);
                    PentagramPipe pipe = hit.collider.GetComponentInParent<PentagramPipe>();
                    if (pipe != null)
                    {
                        Debug.Log("pipe hit");
                        //if (carriedPipe != null && pipe != carriedPipe)
                        //{
                        //    Debug.Log("another pipe hit");
                        //    carriedPipe.AnotherSelected();
                        //    carriedPipe = null;
                        //}
                        lastSelectedPipe = carriedPipe = pipe;
                        pipe.OnClick();
                    }
                }
            }

            //if (carryObject == null)
            //{
            //    TryPickup();
            //}
            //else if (!carryObject.carried)
            //{
            //    carryObject = null;
            //    TryPickup();
            //}
            //else
            //{
            //    carryObject.GetComponent<PickupableItem>().Drop();
            //    carryObject = null;
            //}
        }
    }
}
