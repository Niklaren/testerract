using UnityEngine;
using System.Collections;

public class ElementSolid : PickupableItem {

    enum Socket { None = -1, Earth = 0, Water, Fire, Air, Ether}
    [SerializeField]
    Socket socket;
    [SerializeField]
    Socket correct_socket;

    [SerializeField]
    Transform[] sockets;

    [SerializeField]        //you could very well subclass this behaviour into an Action class that knew what items it needed.
    UseableItem[] Items;    //but for a proto (with only 1 action) I guess it's fine here.

    bool complete;
    bool locked;
    PlatonicPuzzle puzz;

    public bool InCorrectSocket()
    {
        return (socket == correct_socket);
    }
    public bool IsComplete()
    {
        return complete;
    }

    protected override void Start()
    {
        Debug.Log("ES start");
        puzz = FindObjectOfType<PlatonicPuzzle>();
        base.Start();
        GetComponent<ParticleSystem>().Stop();
    }

    //void OnMouseDown()
    //{

    //}

    public override void Pickup ()
    {
        if (!locked)
        {
            base.Pickup();
            GetComponent<ParticleSystem>().Stop();
            socket = Socket.None;
        }
    }

    public override void Drop ()
    {
        base.Drop();
        Place();
    }

    public void Place ()
    {
        for(int i = 0; i < sockets.Length; i++)
        {
            if(Vector3.Distance(transform.position, sockets[i].position) < 0.4f)
            {
                PlaceinSocket(i);
                CheckComplete();
                puzz.CheckPuzzleStage();
                return;
            }
        }
    }

    private void PlaceinSocket(int i)
    {
        transform.parent = sockets[i];
        transform.position = sockets[i].position;
        transform.eulerAngles = Vector3.zero;
        socket = (Socket)i;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        rb.useGravity = false;
    }

    public void LockIn()
    {
        PlaceinSocket((int)correct_socket);
        locked = true;
    }

    public void OnComplete()
    {
        GetComponent<ParticleSystem>().Play();
        complete = true;
        puzz.CheckPuzzleStage();
    }

    public bool CheckComplete()
    {
        if (InCorrectSocket() && puzz.GetStage() > 0)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i].used == false)
                {
                    return false;
                }
            }
            // if all items required are used, this element is complete
            OnComplete();
            return true;
        }
        return false;
    }
}
