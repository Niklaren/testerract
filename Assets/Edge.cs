using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Edge {

    static int total_edges;
    int ID;
    public int GetID() { return ID; }

    Piece belongsTo;
    public void SetPiece(Piece p) { belongsTo = p; }
    public Piece GetPiece() { return belongsTo; }

    //Vector3 point_a;
    public Vector3 Get_point_a()
    {
        Vector3 piece_offset = Vector3.zero;
        if (belongsTo != null)
            piece_offset = belongsTo.GetPosition();

        if (cut_a == null)
            return orig_point_a + piece_offset;
        else
        {
            Vector3 point_a = orig_point_a + ((orig_point_b - orig_point_a) * cut_a.perecnt_across_orig);
            return point_a + piece_offset;

        }
    }
    public Vector3 Get_point_b()
    {
        Vector3 piece_offset = Vector3.zero;
        if (belongsTo != null)
            piece_offset = belongsTo.GetPosition();

        if (cut_b == null)
            return orig_point_b + piece_offset;
        else
        {
            Vector3 point_b = orig_point_a + ((orig_point_b - orig_point_a) * cut_b.perecnt_across_orig);
            return point_b + piece_offset;

        }
    }
    //Vector3 point_b;
    //public Vector3 Get_point_b() { return point_b; }
    public Vector3 Get_Position() { return (Get_point_a() + Get_point_b()) / 2; }

    Vector3 orig_point_a; // for after cuts
    Vector3 orig_point_b;

    Cut cut_a;
    public Cut Get_cut_a() { return cut_a; }
    Cut cut_b;
    public Cut Get_cut_b() { return cut_b; }

    public float percent_of_orig = 1.0f;

    List<Edge> connected_edges_a = new List<Edge>();
    List<Edge> connected_edges_b = new List<Edge>();
    public List<Edge> Get_connected_edges()
    {
        List<Edge> ret = new List<Edge>();
        if(connected_edges_a.Count>0)
            ret.AddRange(connected_edges_a);
        if (connected_edges_b.Count > 0)
            ret.AddRange(connected_edges_b);

        return ret;
    }

    public Edge(Vector3 p_point_a, Vector3 p_point_b)
    {
        //orig_point_a = point_a = p_point_a;
        // orig_point_b = point_b = p_point_b;
        orig_point_a = p_point_a;
        orig_point_b =  p_point_b;
        ID = total_edges++;
    }

/*    public Edge(Vector3 p_point_a, Vector3 p_point_b, Vector3 p_orig_a, Vector3 p_orig_b)
    {
        //point_a = p_point_a;
        //point_b = p_point_b;
        orig_point_a = p_orig_a;
        orig_point_b = p_orig_b;
        ID = total_edges++;
    }*/

    public bool check_connected_edge(Edge p_e)
    {
        if (!SharesCutWithEdge(p_e))
        {
            if (Get_point_a() == p_e.Get_point_a())
            {
                add_connected_edge_a(p_e);
                p_e.add_connected_edge_a(this);
                return true;
            }
            else if (Get_point_a() == p_e.Get_point_b())
            {
                add_connected_edge_a(p_e);
                p_e.add_connected_edge_b(this);
                return true;
            }
            else if (Get_point_b() == p_e.Get_point_a())
            {
                add_connected_edge_b(p_e);
                p_e.add_connected_edge_a(this);
                return true;
            }
            else if (Get_point_b() == p_e.Get_point_b())
            {
                add_connected_edge_b(p_e);
                p_e.add_connected_edge_b(this);
                return true;
            }
        }

        this.remove_connected_edge(p_e);
        p_e.remove_connected_edge(this);
        return false;
    }

    public void add_connected_edge_a(Edge e)
    {
        if(!connected_edges_a.Contains(e))
            connected_edges_a.Add(e);
    }
    public void add_connected_edge_b(Edge e)
    {
        if (!connected_edges_b.Contains(e))
            connected_edges_b.Add(e);
    }
    public void remove_connected_edge(Edge e)
    {
        if (connected_edges_a.Contains(e))
            connected_edges_a.Remove(e);
        if (connected_edges_b.Contains(e))
            connected_edges_b.Remove(e);
    }
    private void set_connected_edges_a(List<Edge> p_edges)
    {
        connected_edges_a.Clear();
        connected_edges_a.AddRange(p_edges);
    }
    private void set_connected_edges_b(List<Edge> p_edges)
    {
        connected_edges_b.Clear();
        connected_edges_b.AddRange(p_edges);
    }

    public Edge cut_edge(float d) // d is % along the edge
    {
        //if(d>=0 && d <= 1) {
        Vector3 cut_pos = Get_point_a() + ((Get_point_b() - Get_point_a()) * d);

        //Edge e = new Edge(cut_pos, point_b ,orig_point_a, orig_point_b);
        Edge e = new Edge(orig_point_a, orig_point_b);
        //this.point_b = cut_pos;

        e.percent_of_orig = percent_of_orig * (1 - d);
        percent_of_orig *= d;

        e.set_connected_edges_b(connected_edges_b);
        connected_edges_b.Clear();

        GameObject cut_A = new GameObject("Cut");
        GameObject cut_B = new GameObject("Cut");
        
        set_cut_b(cut_A.AddComponent<Cut>());
        e.set_cut_a(cut_B.AddComponent<Cut>());

        Get_cut_b().Set_cut_pos(cut_pos);
        e.Get_cut_a().Set_cut_pos(cut_pos);


        //Cut cut_a = new Cut(this);
        //Cut cut_b = new Cut(e);

        Get_cut_b().perecnt_across_orig = d; // todo if more than 1 cut across orig
        e.Get_cut_a().perecnt_across_orig = d; // todo if more than 1 cut across orig

        return e;

    }
	
    public bool set_cut_a(Cut c)
    {
        if (cut_a == null){
            cut_a = c;
            return true;
        }
        return false;
    }
    public bool set_cut_b(Cut c)
    {
        if (cut_b == null){
            cut_b = c;
            return true;
        }
        return false;
    }

    public bool join_cuts(Edge e)
    {
        if (cut_b == e.Get_cut_a() || cut_a == e.Get_cut_b())
        {
            // to do: logic


            return true;
        }

       return false;
    }
    private bool SharesCutWithEdge(Edge e)
    {
        //Debug.Log("check shares cut with edge: " + ID + " and " + e.GetID());

        if (cut_a == null && cut_b == null)
        {
            //Debug.Log("no cuts on this");
            return false;
        }

        if (e.Get_cut_a() == null && e.Get_cut_b() == null)
        {
            //Debug.Log("no cuts on other");
            return false;
        }

        if (cut_a != null)
        {
            if (e.Get_cut_a() != null)
            {
                //Debug.Log("check cut a");
                if (cut_a.GetID() == e.Get_cut_a().GetID())
                {
                    //Debug.Log("cuts match");
                    return true;
                }
            }
            if (e.Get_cut_b() != null)
            {
                if (cut_a.GetID() == e.Get_cut_b().GetID())
                {
                    //Debug.Log("cuts match");
                    return true;
                }
            }
        }
        if (cut_b != null)
        {
            if (e.Get_cut_a() != null)
            {
                //Debug.Log("check cut b");
                if (cut_b.GetID() == e.Get_cut_a().GetID())
                {
                    //Debug.Log("cuts match");
                    return true;
                }
            }
            if (e.Get_cut_b() != null)
            {
                if (cut_b.GetID() == e.Get_cut_b().GetID())
                {
                    //Debug.Log("cuts match");
                    return true;
                }
            }
        }

        //Debug.Log("cuts not shared edge: " + ID + " and " + e.GetID()); ;
        return false;
    }
}
