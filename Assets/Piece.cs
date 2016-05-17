using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Piece : MonoBehaviour {

    [SerializeField]
    Material mat_default;
    [SerializeField]
    Material mat_core;
    [SerializeField]
    Material mat_selected;

    List<Edge> edges = new List<Edge>();
    public List<Edge> Get_edges() { return edges; }

    List<Cut> cuts = new List<Cut>();

    Puzzle puzzle;
    public void SetPuzzle(Puzzle p) { puzzle = p; }
    bool core = false;

    //GameObject PieceObject;
    public Vector3 GetPosition() { return gameObject.transform.position; }
    Vector3 NonRotatedPosition;
    Vector3 RotatedDelta;
    Vector3 LocalCentre;

    /*
    public void Update()
    {
        if(this == puzzle.selected)
        {

        }
    }
    */

    public void Reset()
    {
        edges.Clear();
        cuts.Clear();
    }

    public void AddEdge(Edge e)
    {
        if (!edges.Contains(e))
        {
            edges.Add(e);
            e.SetPiece(this);
        }
    }

    public void CreatePieceObject()
    {
        RecordCuts();
        JoinUnusedCuts();
        RecordCuts();
        RecordPositioning();

        for (int i = 0; i < edges.Count; i++)
        {
            GameObject EdgeObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

            EdgeObject.transform.position = edges[i].Get_Position();   //Get_point_a();  //
            EdgeObject.transform.up = edges[i].Get_point_b() - edges[i].Get_point_a();
            EdgeObject.transform.localScale = new Vector3(0.1f, Vector3.Distance(edges[i].Get_point_a(), edges[i].Get_point_b()) / 2, 0.1f);

            EdgeObject.transform.SetParent(gameObject.transform);

            EdgeObject.name = "cylinder" + edges[i].GetID();

            //parent cuts to edge
            if (edges[i].Get_cut_a() != null)
            {
                edges[i].Get_cut_a().gameObject.transform.SetParent(EdgeObject.transform);
            }
            if (edges[i].Get_cut_b() != null)
            {
                edges[i].Get_cut_b().gameObject.transform.SetParent(EdgeObject.transform);
            }
        } 
    }

    private void JoinUnusedCuts()
    {
        Debug.Log("try join unused");
        foreach (Cut c1 in cuts)
        {
            foreach (Cut c2 in cuts)
            {
                if(c1!= c2)
                {
                    if(c1.GetID() == c2.GetID())
                    {
                        Debug.Log("try joining now");
                        c1.GetEdge().join_cuts(c2.GetEdge());
                    }
                }
            }
        }
    }

    private void RecordCuts()
    {
        cuts.Clear();
        foreach (Edge e in edges)
        {
            if (e.Get_cut_a() != null)
            {
                if (!cuts.Contains(e.Get_cut_a()))
                    cuts.Add(e.Get_cut_a());
            }
            if (e.Get_cut_b() != null)
            {
                if (!cuts.Contains(e.Get_cut_b()))
                    cuts.Add(e.Get_cut_b());
            }
        }
    }

    private void RecordPositioning()
    {
        LocalCentre = Vector3.zero;
        foreach (Edge e in edges)
        {
            LocalCentre += e.Get_Position();
        }
        LocalCentre /= edges.Count;

        LocalCentre += transform.position;
    }

    public void SnapToCut()
    {
        List<Cut> all_cuts = puzzle.Get_all_cuts();

        for (int i = 0; i < cuts.Count; i++)
        {
            for (int j = 0; j < all_cuts.Count; j++)
            {
                if ((cuts[i] != all_cuts[j]) && (cuts[i].GetEdge() != all_cuts[j].GetEdge()))
                {
                    if (Vector3.Distance(cuts[i].Get_cut_pos(), all_cuts[j].Get_cut_pos()) < 0.1f)
                    {
                        Vector3 d = all_cuts[j].Get_cut_pos() - cuts[i].Get_cut_pos();
                        Debug.Log("this cut at " + cuts[i].Get_cut_pos().ToString("F4"));
                        Debug.Log("other cut at " + all_cuts[j].Get_cut_pos().ToString("F4"));
                        Debug.Log("move: " + d.ToString("F4"));
                        MovePieceDelta(d);
                        return;
                    }
                }
            }
        }
    }

    public void MovePieceTo(Vector3 v)
    {
        if (!core)
        {
            gameObject.transform.position = v;
            //gameObject.transform.position += RotatedDelta;
            //SnapToCut();
            NonRotatedPosition = v - RotatedDelta;
        }
    }

    public void MovePieceDelta(Vector3 v)
    {
        Debug.Log("move: " + v.ToString("F4"));
        gameObject.transform.position = (gameObject.transform.position + v);
        gameObject.transform.
        NonRotatedPosition += v;
        puzzle.CheckComplete();
    }

    public void RotatePiece(Vector3 r)
    {
        Vector3 p = NonRotatedPosition + LocalCentre;
        Debug.Log("Rotated Around " + p.ToString("F4") + "with loc pos " + LocalCentre.ToString("F4") + "and non-rotated pos " + NonRotatedPosition.ToString("F4"));
        gameObject.transform.RotateAround(p,r,r.magnitude);
        RotatedDelta = transform.position - NonRotatedPosition;
        Debug.Log("RotatedDelta " + RotatedDelta.ToString("F4"));
        SnapToCut();
    }

    public void SelectPiece()
    {
        if(puzzle.selected != null)
            puzzle.selected.DoDeSelection();
        DoSelection();
    }

    private void DoDeSelection()
    {
        puzzle.selected = null;
        MeshRenderer[] MRs = GetComponentsInChildren<MeshRenderer>();
        foreach(var mr in MRs)
        {
            mr.material = mat_default;
        }
    }
    private void DoSelection()
    {
        if (!core)
        {
            puzzle.selected = this;
            MeshRenderer[] MRs = GetComponentsInChildren<MeshRenderer>();
            foreach (var mr in MRs)
            {
                mr.material = mat_selected;
            }
        }
    }
    public void SetCore()
    {
        core = true;
        MeshRenderer[] MRs = GetComponentsInChildren<MeshRenderer>();
        foreach (var mr in MRs)
        {
            mr.material = mat_core;
        }
    }
}
