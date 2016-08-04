using UnityEngine;
using System.Collections;

public class PentagramPipe : MonoBehaviour {

    [SerializeField]
    private Piece p;
    private Camera cam;
    private PentagramPuzzle puzz;
    private Rigidbody rb;
    private PentagramPlayer player;

    [SerializeField]
    private Collider room;

    private float placedinwall = 12.4f;
    private float moveonwall = 12.25f;

    private Vector3 screenPoint;
    private Vector3 offset;

    enum PipeState { loose, carried, move, placed };
    PipeState pipestate;

	// Use this for initialization
	void Start () {
        p = GetComponent<Piece>();
        puzz = FindObjectOfType<PentagramPuzzle>();
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
        room = GameObject.Find("Floor").GetComponent<BoxCollider>();
        player = FindObjectOfType<PentagramPlayer>();
    }
	
	// Update is called once per frame
	void Update () {
	    //if(Input.GetMouseButtonDown(0))
     //   {
     //       //Debug.Log("pipe update mouse down");
     //       switch (pipestate)
     //       {
     //           case PipeState.loose:
     //               //PickupLoose();
     //               break;
     //           case PipeState.carried:
     //               //DropCarried();
     //               break;
     //           case PipeState.move:
     //
     //               break;
     //           case PipeState.placed:
     //
     //               break;
     //       }
     //   }

        if(pipestate == PipeState.move)
        {
            MoveOnWall();
        }

        //if (pipestate == PipeState.carried)
        //{
            KeepInRoom();
        //}
    }

    void OnMouseDown()
    {
        //Debug.Log("pipe mouse down");
        switch(pipestate)
        {
            case PipeState.loose:
                //PickupLoose();
                break;
            case PipeState.carried:
                //DropCarried();
                break;
            case PipeState.move:

                break;
            case PipeState.placed:

                break;

        }
    }

    public PentagramPipe OnClick()
    {
        Debug.Log("pipe onClick");
        switch (pipestate)
        {
            case PipeState.loose:
                PickupLoose();
                return this;
                break;
            case PipeState.carried:
                DropCarried();
                return null;
                break;
            case PipeState.move:
                DropCarried();
                return null;
                break;
            case PipeState.placed:
                pipestate = PipeState.move;
                //MoveOnWall();
                return this;
                break;

        }
        return this;
    }

    void PickupLoose()
    {
        Debug.Log("try pickup loose");
        if(puzz.selected == null)
        {
            if (Vector3.Distance(cam.transform.position, transform.position) < 4)
            {
                if (!p.selected)
                {
                    p.SelectPiece();
                }

                if (p.selected)
                {
                    p.transform.SetParent(cam.transform);
                }

                pipestate = PipeState.carried;
            }
        }
    }

    void DropCarried()
    {
        // drop on wall
        if (puzz.wall.bounds.Contains(transform.position))
        {
            TryPlace();
        }
        // else drop on floor
        else
        {
            Debug.Log("try drop carried");
            p.DoDeSelection();
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
            pipestate = PipeState.loose;
        }

    }

    void TryPlace()
    {
        Debug.Log("try place");
        //p.DeSelectPiece();
        //if(!p.IsPieceConnected())
        //{
        //    // place on wall
        //}
        Debug.Log("in wall");
        pipestate = PipeState.placed;

        p.DoDeSelection();

        transform.eulerAngles = Vector3.zero;
        transform.position = new Vector3(transform.position.x, transform.position.y, placedinwall);
        rb.constraints = RigidbodyConstraints.FreezeAll;

        p.SnapToCut();

        // if overlaps another pipe  // snap before or after?
        // pipestate = move
        // else
        puzz.CheckComplete();
    }

    void MoveOnWall()
    {
        Debug.Log("move on wall");

        ////screenPoint = cam.WorldToScreenPoint(gameObject.transform.position);
        //Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        //Debug.Log("mouse pos: " + mousePos);
        //offset = gameObject.transform.position - cam.ScreenToWorldPoint(mousePos);
        //Debug.Log("offset: " + offset);

        ////Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        //Vector3 Position = cam.ScreenToWorldPoint(mousePos) + offset;
        //Debug.Log("new position: " + Position);

        //Position = new Vector3(transform.position.x, transform.position.y, moveonwall); //or something like this?

        //p.MovePieceTo(Position);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // create a plane at 0,0,0 whose normal points to +Y:
        Plane hPlane = new Plane(new Vector3(0,0,-1), moveonwall);
        // Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
        float distance = 0;
        // if the ray hits the plane...
        if (hPlane.Raycast(ray, out distance))
        {
            //Debug.Log("hit. move to: " + ray.GetPoint(distance));
            // get the hit point:
            Vector3 moveTo = ray.GetPoint(distance);

            if (moveTo.x > puzz.wall.bounds.max.x)
            {
                moveTo.x = puzz.wall.bounds.max.x;
            }
            else if (moveTo.x < puzz.wall.bounds.min.x)
            {
                moveTo.x = puzz.wall.bounds.min.x;
            }

            if (moveTo.y > puzz.wall.bounds.max.y)
            {
                moveTo.y = puzz.wall.bounds.max.y;
            }
            else if (moveTo.y < puzz.wall.bounds.min.y)
            {
                moveTo.y = puzz.wall.bounds.min.y;
            }

            transform.position = moveTo;
        }
    }

    public void AnotherSelected()
    {
        switch (pipestate)
        {
            case PipeState.loose:
                Debug.Log("err what's happened here :S");
                break;
            case PipeState.carried:
                DropCarried();
                break;
            case PipeState.move:

                break;
            case PipeState.placed:

                break;

        }
    }

    void KeepInRoom()
    {
        //Debug.Log("try keep in room");

        Vector3 moveTo = transform.position;

        if(moveTo.x > room.bounds.max.x)
        {
            moveTo.x = room.bounds.max.x;
        }
            else if (moveTo.x < room.bounds.min.x)
        {
            moveTo.x = room.bounds.min.x;
        }

        if (moveTo.y > room.bounds.max.y)
        {
            moveTo.y = room.bounds.max.y;
        }
        else if (moveTo.y < room.bounds.min.y)
        {
            moveTo.y = room.bounds.min.y;
        }

        if (moveTo.z > room.bounds.max.z)
        {
            moveTo.z = room.bounds.max.z;
        }
        else if (moveTo.z < room.bounds.min.z)
        {
            moveTo.z = room.bounds.min.z;
        }

        transform.position = moveTo;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("onCollisionEnter with " + collision.gameObject.name);
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("onCollisionStay with " + collision.gameObject.name);

        //PentagramPipe temp = collision.gameObject.GetComponentInParent<PentagramPipe>();
        //if (temp != null)
        //{
        //    Debug.Log("onCollisionEnter with another pipe (child)");
        //}
        PentagramPipe temp = collision.gameObject.GetComponent<PentagramPipe>();
        if (temp != null)
        {
            Debug.Log("onCollisionEnter with another pipe (parent)");
            if (pipestate == PipeState.placed && this == player.lastSelectedPipe)
            {
                player.carriedPipe = player.lastSelectedPipe;
                pipestate = PipeState.move;
            }
        }
    }

    //private void OnCollisionExit(Collision collision)
    //{
    //    Debug.Log("onCollisionExit with " + collision.gameObject.name);

    //    PentagramPipe temp = collision.gameObject.GetComponent<PentagramPipe>();
    //    if (temp != null)
    //    {
    //        Debug.Log("onCollisionEnter with another pipe (parent)");
    //        if (pipestate == PipeState.move)
    //            TryPlace();
    //    }
    //}
}
