using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Piece : MonoBehaviour {

    List<Edge> edges = new List<Edge>();
    public List<Edge> Get_edges() { return edges; }

    List<Cut> cuts = new List<Cut>();

    Puzzle puz;

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
            EdgeObject.transform.SetParent(gameObject.transform);

            EdgeObject.transform.position = edges[i].Get_Position();   //Get_point_a();  //
            EdgeObject.transform.up = edges[i].Get_point_b() - edges[i].Get_point_a();
            EdgeObject.transform.localScale = new Vector3(0.1f, Vector3.Distance(edges[i].Get_point_a(), edges[i].Get_point_b()) / 2, 0.1f);
        }
        RecordCuts();
    }

    private void RecordCuts()
    {
        foreach (Edge e in edges)
        {
            Cut a = e.Get_cut_a();
            Cut b = e.Get_cut_b();

            if (a != null)
            {
                if (!cuts.Contains(a))
                    cuts.Add(a);
            }
            if (b != null)
            {
                if (!cuts.Contains(b))
                    cuts.Add(b);
            }
        }
    }

    private void SnapToCut()
    {
        List<Cut> all_cuts = puz.Get_all_cuts();
        for (int i = 0; i < cuts.Count; i++)
        {
            for(int j = 0; j < all_cuts.Count; j++)
            {
                if (cuts[i] != cuts[j])
                    ;
            }
        }
    }

    public void MovePieceTo(Vector3 v)
    {
        gameObject.transform.position = v;
    }

    public void MovePieceDelta(Vector3 v)
    {
        gameObject.transform.Translate(v);
    }

    public void RotatePiece(Vector3 r)
    {
        gameObject.transform.Rotate(r);
    }


}
