using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Puzzle : MonoBehaviour {

    List<Piece> pieces = new List<Piece>();

    List<Edge> edges = new List<Edge>();

    List<Cut> all_cuts = new List<Cut>();
    public List<Cut> Get_all_cuts() {   if (all_cuts.Count == 0) RecordAllCuts();
                                        return all_cuts; }

    [SerializeField]
    GameObject piece_object;

    [SerializeField]
    int desired_pieces;

    [SerializeField]
    float random_seed;

	// Use this for initialization
	void Start ()
    {
        CreateCube();

        edges.Add(edges[00].cut_edge(0.2f));
        edges.Add(edges[02].cut_edge(0.3f));
        edges.Add(edges[08].cut_edge(0.4f));
        edges.Add(edges[10].cut_edge(0.5f));

        edges.Add(edges[04].cut_edge(0.5f));
        edges.Add(edges[05].cut_edge(0.6f));
        edges.Add(edges[06].cut_edge(0.7f));
        edges.Add(edges[07].cut_edge(0.8f));

        edges.Add(edges[01].cut_edge(0.1f));

        Debug.Log(edges.Count);

        RecalculateConnectedEdges();
        RecalculatePieces();
        RecordAllCuts();

        for (int i = 0; i < pieces.Count; i++)
        {
            pieces[i].CreatePieceObject();
        }
    }

    void Update()
    {
        Debug.Log(CheckComplete());
    }
	
    public void CreateCube()
    {
        edges.Clear();

        edges.Add(new Edge(new Vector3(0, 0, 0), new Vector3(1, 0, 0)));
        edges.Add(new Edge(new Vector3(1, 0, 0), new Vector3(1, 1, 0)));
        edges.Add(new Edge(new Vector3(1, 1, 0), new Vector3(0, 1, 0)));
        edges.Add(new Edge(new Vector3(0, 1, 0), new Vector3(0, 0, 0)));

        edges.Add(new Edge(new Vector3(0, 0, 0), new Vector3(0, 0, 1)));
        edges.Add(new Edge(new Vector3(1, 0, 0), new Vector3(1, 0, 1)));
        edges.Add(new Edge(new Vector3(1, 1, 0), new Vector3(1, 1, 1)));
        edges.Add(new Edge(new Vector3(0, 1, 0), new Vector3(0, 1, 1)));

        edges.Add(new Edge(new Vector3(0, 0, 1), new Vector3(1, 0, 1)));
        edges.Add(new Edge(new Vector3(1, 0, 1), new Vector3(1, 1, 1)));
        edges.Add(new Edge(new Vector3(1, 1, 1), new Vector3(0, 1, 1)));
        edges.Add(new Edge(new Vector3(0, 1, 1), new Vector3(0, 0, 1)));

        RecalculateConnectedEdges();
    }

    private void CreateRandomPuzzle()
    {
        while (pieces.Count < desired_pieces)
        {
            // cut random edge at a random distance along

            // recalc pieces

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

    private void RecalculatePieces()
    {
        pieces.Clear();
        List<Edge> unvisited = new List<Edge>(edges);

        while (unvisited.Count > 0)
        {
            //MakePiece(unvisited[0]);
            Piece p = Instantiate(piece_object).GetComponent<Piece>(); //new Piece();
            p.AddEdge(unvisited[0]);

            for (int i = 0; i < p.Get_edges().Count; i++)
            {
                unvisited.Remove(p.Get_edges()[i]);
                for (int j = 0; j < p.Get_edges()[i].Get_connected_edges().Count; j++)
                {
                    if (!p.Get_edges().Contains(p.Get_edges()[i].Get_connected_edges()[j])) {
                        p.AddEdge(p.Get_edges()[i].Get_connected_edges()[j]);
                    }
                }
                
            }

            string s = "made piece with:";
            for (int i = 0; i < p.Get_edges().Count; i++)
            {
                s += " ";
                s += p.Get_edges()[i].GetID().ToString();
            }
            Debug.Log(s);

            pieces.Add(p);
        }

        Debug.Log(pieces.Count);
    }

    private void RecordAllCuts()
    {
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
            if (!c.IfCutsTouch())
                return false;
        }

        return true;
    }
}
