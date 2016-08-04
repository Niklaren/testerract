using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Puzzle : MonoBehaviour {

    protected List<Piece> pieces = new List<Piece>();
    public Piece selected;
    public Piece core;

    protected List<Edge> edges = new List<Edge>();

    List<Cut> all_cuts = new List<Cut>();
    public List<Cut> Get_all_cuts() {   if (all_cuts.Count == 0) RecordAllCuts();
                                        return all_cuts; }

    [SerializeField]
    GameObject piece_object;

    [SerializeField]
    int desired_pieces;

    [SerializeField]
    int random_seed;

    public bool complete;

	// Use this for initialization
	void Start ()
    {

        //CreateCube();
        CreateTess();

        if (random_seed == 0)
            CreatePredefinedCuts();
        else
            CreateRandomCuts();

        Debug.Log(edges.Count);

        RecalculatePieces();

        for (int i = 0; i < pieces.Count; i++)
        {
            pieces[i].CreatePieceObject().gameObject.transform.SetParent(transform);
        }

        core = pieces[0];
        core.SetCore();

        RecordAllCuts();

        DispersePieces();

        ConfigurePiecePhysics();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (selected != null)
            {
                selected.DeSelectPiece();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (selected != null)
        {
            //Debug.Log(CheckComplete());
            //if (!Input.GetMouseButton(0))
            {
                //if (Input.GetKeyDown(KeyCode.Alpha1))
                //{
                //    Vector3 AB = Camera.main.transform.position - selected.transform.position;
                //    if (Mathf.Abs(AB.x) > Mathf.Abs(AB.z))
                //    {
                //        selected.RotateTween(new Vector3(0, 0, 1 * AB.x), 90);
                //    }
                //    else
                //    {
                //        selected.RotateTween(new Vector3(-1 * AB.z, 0, 0), 90);
                //    }
                //    selected.RoundTo90();
                //    selected.SnapToCut();
                //}
                //else if (Input.GetKeyDown(KeyCode.Q))
                //{
                //    Vector3 AB = Camera.main.transform.position - selected.transform.position;
                //    if (Mathf.Abs(AB.x) > Mathf.Abs(AB.z))
                //    {
                //        selected.RotateTween(new Vector3(0, 0, -1 * AB.x), 90);
                //    }
                //    else
                //    {
                //        selected.RotateTween(new Vector3(1 * AB.z, 0, 0), 90);
                //    }
                //    selected.RoundTo90();
                //    selected.SnapToCut();
                //}
                //if (Input.GetKeyDown(KeyCode.Alpha2))
                //{
                //    selected.RotateTween(new Vector3(0, 1, 0), 90);
                //    selected.RoundTo90();
                //    selected.SnapToCut();
                //}
                //else if (Input.GetKeyDown(KeyCode.Alpha3))
                //{
                //    selected.RotateTween(new Vector3(0, -1, 0), 90);
                //    selected.RoundTo90();
                //    selected.SnapToCut();
                //}

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    selected.RotateTween(new Vector3(0, 1, 0), 90);
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    selected.RotateTween(new Vector3(0, -1, 0), 90);
                }

                else if (Input.GetKeyDown(KeyCode.R))
                {
                    selected.ResetRotation();
                }
                else if (Input.GetKeyDown(KeyCode.T))
                {
                    //selected.RoundTo90();
                    selected.RotateTween(new Vector3(0, 0, 0), 0);
                }

                if (Input.GetMouseButton(1))
                {
                    float deltaX = Input.GetAxis("Mouse X");
                    float deltaY = Input.GetAxis("Mouse Y");
                    if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
                    {
                        //Vector3 r = new Vector3(0, -deltaX, 0);
                        Vector3 axis = new Vector3(0, 1, 0);
                        //Debug.Log("xyz rot " + deltaXYZ.ToString("F4"));
                        //float a = Vector3.Angle(Vector3.zero, deltaXYZ);
                        selected.RotatePiece(axis, -deltaX);
                    }
                    else if (Mathf.Abs(deltaY) > Mathf.Abs(deltaX))
                    {
                        Vector3 AB = Camera.main.transform.position - selected.transform.position;

                        Vector3 cross = Vector3.Cross(Vector3.up, AB);
                        //float a = Vector3.Angle(Vector3.up, AB);w

                        selected.RotatePiece(cross, -deltaY);
                    }

                    //selected.SnapToCut();

                }

                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    Vector3 AB = Camera.main.transform.position - selected.transform.position;
                    if (Mathf.Abs(AB.x) > Mathf.Abs(AB.z))
                    {
                        selected.RotateTween(new Vector3(0, 0, 1 * AB.x), 90);
                    }
                    else
                    {
                        selected.RotateTween(new Vector3(-1 * AB.z, 0, 0), 90);
                    }
                }

                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    Vector3 AB = Camera.main.transform.position - selected.transform.position;
                    if (Mathf.Abs(AB.x) > Mathf.Abs(AB.z))
                    {
                        selected.RotateTween(new Vector3(0, 0, -1 * AB.x), 90);
                    }
                    else
                    {
                        selected.RotateTween(new Vector3(1 * AB.z, 0, 0), 90);
                    }
                }
            }
        }
    }
	
    public void CreateCube()
    {
        edges.Clear();

        edges.Add(new Edge(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f)));
        edges.Add(new Edge(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, 0.5f)));
        edges.Add(new Edge(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, 0.5f)));
        edges.Add(new Edge(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, -0.5f)));

        edges.Add(new Edge(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f)));
        edges.Add(new Edge(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f)));
        edges.Add(new Edge(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f)));
        edges.Add(new Edge(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f)));

        edges.Add(new Edge(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f)));
        edges.Add(new Edge(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f)));
        edges.Add(new Edge(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f)));
        edges.Add(new Edge(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, -0.5f)));

        RecalculateConnectedEdges();
    }

    public void CreateTess()
    {
        edges.Clear();

        // outer quad 0-11
        edges.Add(new Edge(new Vector3(-1.5f, -1.5f, -1.5f), new Vector3(1.5f, -1.5f, -1.5f)));
        edges.Add(new Edge(new Vector3(1.5f, -1.5f, -1.5f), new Vector3(1.5f, -1.5f, 1.5f)));
        edges.Add(new Edge(new Vector3(1.5f, -1.5f, 1.5f), new Vector3(-1.5f, -1.5f, 1.5f)));
        edges.Add(new Edge(new Vector3(-1.5f, -1.5f, 1.5f), new Vector3(-1.5f, -1.5f, -1.5f)));

        edges.Add(new Edge(new Vector3(-1.5f, -1.5f, -1.5f), new Vector3(-1.5f, 1.5f, -1.5f)));
        edges.Add(new Edge(new Vector3(1.5f, -1.5f, -1.5f), new Vector3(1.5f, 1.5f, -1.5f)));
        edges.Add(new Edge(new Vector3(1.5f, -1.5f, 1.5f), new Vector3(1.5f, 1.5f, 1.5f)));
        edges.Add(new Edge(new Vector3(-1.5f, -1.5f, 1.5f), new Vector3(-1.5f, 1.5f, 1.5f)));

        edges.Add(new Edge(new Vector3(-1.5f, 1.5f, -1.5f), new Vector3(1.5f, 1.5f, -1.5f)));
        edges.Add(new Edge(new Vector3(1.5f, 1.5f, -1.5f), new Vector3(1.5f, 1.5f, 1.5f)));
        edges.Add(new Edge(new Vector3(1.5f, 1.5f, 1.5f), new Vector3(-1.5f, 1.5f, 1.5f)));
        edges.Add(new Edge(new Vector3(-1.5f, 1.5f, 1.5f), new Vector3(-1.5f, 1.5f, -1.5f)));

        // inner quad 12-23
        edges.Add(new Edge(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f)));
        edges.Add(new Edge(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, 0.5f)));
        edges.Add(new Edge(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, 0.5f)));
        edges.Add(new Edge(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, -0.5f)));

        edges.Add(new Edge(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f)));
        edges.Add(new Edge(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f)));
        edges.Add(new Edge(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f)));
        edges.Add(new Edge(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f)));

        edges.Add(new Edge(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f)));
        edges.Add(new Edge(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f)));
        edges.Add(new Edge(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f)));
        edges.Add(new Edge(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, -0.5f)));

        // connectors 24-31
        edges.Add(new Edge(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1.5f, -1.5f, -1.5f)));
        edges.Add(new Edge(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1.5f, -1.5f, -1.5f)));
        edges.Add(new Edge(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(1.5f, 1.5f, -1.5f)));
        edges.Add(new Edge(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-1.5f, 1.5f, -1.5f)));

        edges.Add(new Edge(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-1.5f, -1.5f, 1.5f)));
        edges.Add(new Edge(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1.5f, -1.5f, 1.5f)));
        edges.Add(new Edge(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1.5f, 1.5f, 1.5f)));
        edges.Add(new Edge(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-1.5f, 1.5f, 1.5f)));


        RecalculateConnectedEdges();
    }

    private void CreatePredefinedCuts()
    {
        /*
        edges.Add(edges[00].cut_edge(0.5f));
        edges.Add(edges[02].cut_edge(0.5f));
        edges.Add(edges[08].cut_edge(0.5f));
        edges.Add(edges[10].cut_edge(0.5f));
        */
        /*
        edges.Add(edges[00].cut_edge(0.2f));
        edges.Add(edges[02].cut_edge(0.3f));
        edges.Add(edges[08].cut_edge(0.4f));
        edges.Add(edges[10].cut_edge(0.5f));

        edges.Add(edges[04].cut_edge(0.5f));
        edges.Add(edges[05].cut_edge(0.6f));
        edges.Add(edges[06].cut_edge(0.7f));
        edges.Add(edges[07].cut_edge(0.8f));

        edges.Add(edges[01].cut_edge(0.1f));

        edges.Add(edges[12].cut_edge(0.3f));
        edges.Add(edges[13].cut_edge(0.2f));
        edges.Add(edges[14].cut_edge(0.1f));
        edges.Add(edges[15].cut_edge(0.1f));

        edges.Add(edges[16].cut_edge(0.5f));
        edges.Add(edges[17].cut_edge(0.5f));
        edges.Add(edges[18].cut_edge(0.5f));
        edges.Add(edges[19].cut_edge(0.5f));

        edges.Add(edges[20].cut_edge(0.5f));
        edges.Add(edges[22].cut_edge(0.5f));
        edges.Add(edges[24].cut_edge(0.5f));
        edges.Add(edges[28].cut_edge(0.5f));
        edges.Add(edges[30].cut_edge(0.5f));

        edges.Add(edges[32].cut_edge(0.5f));
        */
        edges.Add(edges[04].cut_edge(0.95f));
        edges.Add(edges[05].cut_edge(0.05f));
        edges.Add(edges[06].cut_edge(0.05f));
        edges.Add(edges[07].cut_edge(0.95f));
        edges.Add(edges[08].cut_edge(0.95f));
        edges.Add(edges[09].cut_edge(0.95f));
        edges.Add(edges[11].cut_edge(0.05f));

        edges.Add(edges[12].cut_edge(0.95f));
        edges.Add(edges[14].cut_edge(0.05f));
        edges.Add(edges[15].cut_edge(0.95f));
        edges.Add(edges[16].cut_edge(0.05f));
        edges.Add(edges[17].cut_edge(0.05f));
        edges.Add(edges[18].cut_edge(0.95f));
        edges.Add(edges[19].cut_edge(0.95f));
        edges.Add(edges[20].cut_edge(0.95f));
        edges.Add(edges[22].cut_edge(0.05f));
        edges.Add(edges[23].cut_edge(0.05f));

        edges.Add(edges[24].cut_edge(0.95f));
        edges.Add(edges[26].cut_edge(0.95f));
        edges.Add(edges[27].cut_edge(0.95f));
        edges.Add(edges[28].cut_edge(0.05f));
        //edges.Add(edges[29].cut_edge(0.95f));
        edges.Add(edges[30].cut_edge(0.05f));
        edges.Add(edges[31].cut_edge(0.95f));

    }

    private void CreateRandomCuts()
    {
        ShuffleBag s = new ShuffleBag(edges.Count);
        for (int i = 4; i < edges.Count; i++) // don't cut bottom 4 pieces :)
            s.Add(i, 1); //2 for dbl cuts? // that's no good! dont do that!!

        if (random_seed!=0)
            Random.seed = random_seed;

        while (pieces.Count < desired_pieces)
        {
            // cut random edge at a random distance along

            int edgeToCut = s.Next();

            //float distToCut = Random.Range(0.1f, 0.9f);

            float distToCut = Random.Range(0, 1);
            if (distToCut == 0)
                distToCut = 0.05f;
            else
                distToCut = 0.95f;

            edges.Add(edges[edgeToCut].cut_edge(distToCut));
            // s.Add(edges.Count,1);
            // recalc pieces
            RecalculatePieces();
        }
    }

    private void DispersePieces()
    {
        for(int i = 1; i < pieces.Count; i++)
        {
            pieces[i].MovePieceDelta(new Vector3(Random.Range(-5f, 5f), Random.Range(0f, 1f), Random.Range(-5f, 5f)));
            pieces[i].RotatePiece(new Vector3(Random.Range(0, 3), 0, 0),90); 
            //pieces[i].RotatePiece(new Vector3(Random.Range(0, 3), Random.Range(0, 3), Random.Range(0, 3)),90);
            pieces[i].RotatePiece(new Vector3(0, Random.Range(0, 3), 0),90);
        }
    }

    private void RecalculateConnectedEdges()
    {
        for (int i = 0; i < edges.Count; i++)
        {
            for (int j = i + 1; j < edges.Count; j++)
            {
                if (edges[i].check_connected_edge(edges[j]))
                    Debug.Log("connect " + i + " and " + j);
            }
        }
    }

    protected void AddCutsOnUnconnected()
    {
        for (int i = 0; i < edges.Count; i++)
        {
            edges[i].AddCutOnUnconnected();
        }
    }

    protected void RecalculatePieces()
    {
        RecalculateConnectedEdges();

        //pieces.Clear();
        List<Edge> unvisited = new List<Edge>(edges);

        int piece_index = 0;

        while (unvisited.Count > 0)
        {
            //MakePiece(unvisited[0]);
            Piece p;
            if (piece_index >= pieces.Count) {
                p = Instantiate(piece_object).GetComponent<Piece>(); //p = new Piece(); 
            } else {
                p = pieces[piece_index];
                p.Reset();
            }

            p.AddEdge(unvisited[0]);

            for (int i = 0; i < p.Get_edges().Count; i++)
            {
                unvisited.Remove(p.Get_edges()[i]);
                for (int j = 0; j < p.Get_edges()[i].Get_connected_edges().Count; j++)
                {
                    if (!p.Get_edges().Contains(p.Get_edges()[i].Get_connected_edges()[j])) {   // if piece doesn't contain connected edge
                        p.AddEdge(p.Get_edges()[i].Get_connected_edges()[j]);                   // add edge to piece
                    }
                }
                
            }

            p.SetPuzzle(this);

            string s = "made piece with:";
            for (int i = 0; i < p.Get_edges().Count; i++)
            {
                s += " ";
                s += p.Get_edges()[i].GetID().ToString();
            }
            Debug.Log(s);

            if(!pieces.Contains(p))
                pieces.Add(p);
            p.name = "Piece " + piece_index;
            piece_index++;
        }

        Debug.Log(pieces.Count + " pieces");
    }

    private void RecordAllCuts()
    {
        all_cuts.Clear();
        foreach (Edge e in edges)
        {
            Cut a = e.Get_cut_a();
            Cut b = e.Get_cut_b();

            if (a != null)
            {
                if (!all_cuts.Contains(a))
                    all_cuts.Add(a);
            }
            if (b != null)
            {
                if (!all_cuts.Contains(b))
                    all_cuts.Add(b);
            }
        }
    }

    private void MakePiece(Edge e)
    {

    }

    public bool CheckComplete()
    {
        foreach(Cut c in all_cuts)
        {
            if (!c.IfAnyCutsTouchThis(all_cuts))
            {
                //Debug.Log("incomplete");
                return complete;
            }
        }

        Debug.Log("complete");
        foreach (Piece p in pieces)
            p.SetCore();
        complete = true;
        return complete;
    }

    public void ConfigurePiecePhysics()
    {
        Debug.Log("puzz config physics");
        foreach(Piece p in pieces)
        {
            if(p.IsPieceConnected())
            {
                p.Freeze();
            }
            else
            {
                p.UnFreeze();
            }
        }
    }
}
