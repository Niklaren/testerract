using UnityEngine;
using System.Collections;

public class Cut : MonoBehaviour {

    static int total_cuts;
    int ID;
    public int GetID() { return ID;}

    public float perecnt_across_orig;

    Vector3 Get_cut_pos(){ return transform.position; } 

    Edge edge_a;
    Edge edge_b;

    public Cut(Edge p_edge_a, Edge p_edge_b)
    {
        ID = total_cuts++;
        edge_a = p_edge_a;
        edge_b = p_edge_b;
    }

	public bool IfCutsTouch()
    {
        if (edge_a.Get_point_b() == edge_b.Get_point_a())
            return true;
        return false;
    }
}
