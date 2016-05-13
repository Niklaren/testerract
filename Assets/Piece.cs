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

    //GameObject PieceObject;
    public Vector3 GetPosition() { return gameObject.transform.position; }

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
        for (int i = 0; i < edges.Count; i++)
        {
            GameObject EdgeObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

            EdgeObject.transform.position = edges[i].Get_Position();   //Get_point_a();  //
            EdgeObject.transform.up = edges[i].Get_point_b() - edges[i].Get_point_a();
            EdgeObject.transform.localScale = new Vector3(0.1f, Vector3.Distance(edges[i].Get_point_a(), edges[i].Get_point_b()) / 2, 0.1f);

            EdgeObject.transform.SetParent(gameObject.transform);

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
        RecordCuts();
    }

    private void RecordCuts()
    {
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

    public void SnapToCut()
    {
        List<Cut> all_cuts = puzzle.Get_all_cuts();

        for (int i = 0; i < cuts.Count; i++)
        {
            for (int j = 0; j < all_cuts.Count; j++)
            {
                if (cuts[i] != all_cuts[j])
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
        gameObject.transform.position = v;
        //SnapToCut();
        
    }

    public void MovePieceDelta(Vector3 v)
    {
        Debug.Log("move: " + v.ToString("F4"));
        gameObject.transform.position = (gameObject.transform.position + v);
        puzzle.CheckComplete();
    }

    public void RotatePiece(Vector3 r)
    {
        Vector3 center = Vector3.zero;
        foreach(Edge e in edges)
        {
            center += e.Get_Position();
        }
        center /= edges.Count;

        center += transform.position;

        gameObject.transform.RotateAround(center,r,90);
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
        puzzle.selected = this;
        MeshRenderer[] MRs = GetComponentsInChildren<MeshRenderer>();
        foreach (var mr in MRs)
        {
            mr.material = mat_selected;
        }
    }
}
