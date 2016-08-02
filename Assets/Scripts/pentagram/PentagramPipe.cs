using UnityEngine;
using System.Collections;

public class PentagramPipe : MonoBehaviour {

    [SerializeField]
    private Piece p;
    private Camera cam;
    private PentagramPuzzle puzz;

    private float placedinwall = 12.3f;
    private float moveonwall = 12.1f;

    private Vector3 screenPoint;
    private Vector3 offset;

    enum PipeState { loose, carried, move, placed };
    PipeState pipestate;

	// Use this for initialization
	void Start () {
        p = GetComponent<Piece>();
        puzz = FindObjectOfType<PentagramPuzzle>();
        cam = Camera.main;
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

     //               break;
     //           case PipeState.placed:

     //               break;

     //       }
     //   }

        if(pipestate == PipeState.move)
        {
            MoveOnWall();
        }
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
                return this;
                break;
            case PipeState.placed:
                return null;
                break;

        }
        return this;
    }

    void PickupLoose()
    {
        Debug.Log("try pickup loose");
        if(puzz.selected == null)
        {
            if (Vector3.Distance(cam.transform.position, p.transform.position) < 4)
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
        if (puzz.wall.bounds.Contains(p.transform.position))
        {
            TryPlace();
        }
        // else drop on floor
        else
        {
            Debug.Log("try drop carried");
            p.DoDeSelection();
            puzz.ConfigurePiecePhysics();
            pipestate = PipeState.loose;
        }

    }

    void TryPlace()
    {
        //p.DeSelectPiece();
        //if(!p.IsPieceConnected())
        //{
        //    // place on wall
        //}
        Debug.Log("in wall");
        pipestate = PipeState.placed;
        p.DoDeSelection();

        gameObject.transform.eulerAngles = Vector3.zero;
        gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, placedinwall);
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    void MoveOnWall()
    {
        screenPoint = cam.WorldToScreenPoint(gameObject.transform.position);
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        offset = gameObject.transform.position - cam.ScreenToWorldPoint(mousePos);

        //Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 Position = cam.ScreenToWorldPoint(mousePos) + offset;

        Position = new Vector3(Position.x, Position.y, moveonwall); //or something like this?

        p.MovePieceTo(Position);
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
}
