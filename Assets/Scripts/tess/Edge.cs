using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Edge : ScriptableObject {

    static int total_edges;
    int ID;
    public int GetID() { return ID; }

    Piece belongsTo;
    public void SetPiece(Piece p) { belongsTo = p; }
    public Piece GetPiece() { return belongsTo; }

    public Vector3 __point_a;
    public Vector3 Get_point_a()
    {
        Vector3 piece_offset = Vector3.zero;
        //if (belongsTo != null)
        //{
        //    piece_offset = belongsTo.transform.position;
        //    Debug.Log("piece offset is : " + piece_offset + " for piece " + belongsTo.name);
        //}

        if (cut_a == null)
        {
            return orig_point_a + piece_offset;
        }
        else
        {
            Vector3 point_a = orig_point_a + ((orig_point_b - orig_point_a) * cut_a.percent_across_orig);
            return point_a + piece_offset;
        }
    }
    public Vector3 Get_point_b()
    {
        Vector3 piece_offset = Vector3.zero;
        //if (belongsTo != null)
        //{
        //    piece_offset = belongsTo.transform.position;
        //    Debug.Log("piece offset is : " + piece_offset + " for piece " + belongsTo.name);
        //}

        if (cut_b == null)
        {
            return orig_point_b + piece_offset;
        }
        else
        {
            Vector3 point_b = orig_point_a + ((orig_point_b - orig_point_a) * cut_b.percent_across_orig);
            return point_b + piece_offset;

        }
    }
    public Vector3 __point_b;
    //public Vector3 Get_point_b() { return point_b; }
    public Vector3 Get_Position() { return (Get_point_a() + Get_point_b()) / 2; }

    Vector3 orig_point_a; // for after cuts
    Vector3 orig_point_b;

    Cut cut_a;
    public Cut Get_cut_a() { return cut_a; }
    public void Delete_cut_a() { cut_a.Destroy();  cut_a = null; }
    Cut cut_b;
    public Cut Get_cut_b() { return cut_b; }
    public void Delete_cut_b () { cut_b.Destroy(); cut_b = null; }

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

    public void Delete()
    {
        Destroy(this);
    }

    public Edge(Vector3 p_point_a, Vector3 p_point_b)
    {
        orig_point_a = __point_a = p_point_a;
        orig_point_b = __point_b = p_point_b;
        //orig_point_a = p_point_a;
        //orig_point_b =  p_point_b;
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

        e.set_connected_edges_b(connected_edges_b);
        connected_edges_b.Clear();

        e.percent_of_orig = percent_of_orig * (1 - d);
        percent_of_orig *= d;

        GameObject cut_A = new GameObject();
        GameObject cut_B = new GameObject();

        //todo if cut_b already exists -  move this cut b down and make new edge with old cut_b ??

        set_cut_b(cut_B.AddComponent<Cut>());
        e.set_cut_a(cut_A.AddComponent<Cut>());

        string cut_name = "Cut " + cut_b.GetID();
        cut_A.name = cut_name;
        cut_B.name = cut_name;

        cut_b.SetEdge(this);
        e.Get_cut_a().SetEdge(e);

        Get_cut_b().Set_cut_pos(cut_pos);
        e.Get_cut_a().Set_cut_pos(cut_pos);

        e.Get_cut_a().percent_across_orig = Get_cut_b().percent_across_orig = 1.0f - e.percent_of_orig;

        return e;

    }

    public void AddCutOnUnconnected()
    {
        Debug.Log("check for unconnected on edge " + ID);
        if (connected_edges_a.Count <= 0 && cut_a == null)
        {
            Debug.Log("add cut to unconnected a. on edge " +ID);

            GameObject cut_A = new GameObject();

            set_cut_a(cut_A.AddComponent<Cut>());

            string cut_name = "Cut " + cut_a.GetID();
            cut_A.name = cut_name;

            cut_a.SetEdge(this);

            cut_a.percent_across_orig = 0;

            Get_cut_a().Set_cut_pos(orig_point_a);
        }

        if (connected_edges_b.Count <= 0 && cut_b == null)
        {
            Debug.Log("add cut to unconnected b. on edge " + ID);

            GameObject cut_B = new GameObject();

            set_cut_b(cut_B.AddComponent<Cut>());

            string cut_name = "Cut " + cut_b.GetID();
            cut_B.name = cut_name;

            cut_b.SetEdge(this);

            cut_b.percent_across_orig = 1;

            Get_cut_b().Set_cut_pos(orig_point_b);
        }
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
        // if both edges have cuts
        if (cut_b != null && e.Get_cut_a() != null)
        {
            //Debug.Log("join: point b at: " + Get_point_b() + "cut b at " + Get_cut_b().Get_cut_pos());
            //Debug.Log("join: e.point a at: " + e.Get_point_a() + "e.cut a at " + e.Get_cut_a().Get_cut_pos());

            //if (cut_b.GetID() == e.Get_cut_a().GetID())
            //{

            if (Get_point_b() == e.Get_point_a())
            {
                percent_of_orig += e.percent_of_orig;

                //connected_edges_b = e. // this should be done by check_connected_edge

                Debug.Log("delete edge " + e.ID);
                e.Delete_cut_a();
                e.Delete();      // delete unused edge // does this do anything even??
                Delete_cut_b();

                return true;
            }
        }
        if (cut_a != null && e.Get_cut_b() != null)
        {
            //Debug.Log("join: e.point b at: " + e.Get_point_b() + "e.cut b at " + e.Get_cut_b().Get_cut_pos());
            //Debug.Log("join: point a at: " + Get_point_a() + "cut a at " + Get_cut_a().Get_cut_pos());

            //if (cut_a.GetID() == e.Get_cut_b().GetID())
            //{
            if (Get_point_a() == e.Get_point_b())
            {
                percent_of_orig += e.percent_of_orig;

                //connected_edges_b = e. // this should be done by check_connected_edge

                Debug.Log("delete edge " + e.ID);
                e.Delete_cut_b();
                e.Delete();      // delete unused edge
                Delete_cut_a();

                return true;
            }
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
