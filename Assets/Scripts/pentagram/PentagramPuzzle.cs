using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PentagramPuzzle : Puzzle {

    public Collider wall;

    // Use this for initialization
    void Start () {
        CreateAllPipes();

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }


        }
    }

    void CreateAllPipes()
    {
        // A
        edges.Add(new Edge(new Vector3(3.9f, 1.9f, 0.0f), new Vector3(3.7f, 1.9f, 0.0f)));
        edges.Add(new Edge(new Vector3(3.7f, 1.9f, 0.0f), new Vector3(3.1f, 1.7f, 0.0f)));
        edges.Add(new Edge(new Vector3(3.1f, 1.7f, 0.0f), new Vector3(2.9f, 1.7f, 0.0f)));
        // B
        edges.Add(new Edge(new Vector3(2.9f, 1.7f, 1.0f), new Vector3(2.5f, 1.7f, 1.0f)));
        edges.Add(new Edge(new Vector3(2.5f, 1.7f, 1.0f), new Vector3(2.5f, 1.3f, 1.0f)));
        // C
        edges.Add(new Edge(new Vector3(2.5f, 1.3f, 2.0f), new Vector3(2.5f, 1.1f, 2.0f)));
        edges.Add(new Edge(new Vector3(2.5f, 1.1f, 2.0f), new Vector3(2.2f, 1.1f, 2.0f)));
        edges.Add(new Edge(new Vector3(2.5f, 1.1f, 2.0f), new Vector3(2.5f, 0.9f, 2.0f)));
        // D
        edges.Add(new Edge(new Vector3(2.5f, 0.9f, 3.0f), new Vector3(2.5f, 0.5f, 3.0f)));
        edges.Add(new Edge(new Vector3(2.5f, 0.5f, 3.0f), new Vector3(2.9f, 0.5f, 3.0f)));
        // E
        edges.Add(new Edge(new Vector3(2.9f, 0.5f, 4.0f), new Vector3(3.3f, 0.5f, 4.0f)));
        edges.Add(new Edge(new Vector3(3.3f, 0.5f, 4.0f), new Vector3(3.3f, 0.1f, 4.0f)));
        edges.Add(new Edge(new Vector3(3.3f, 0.1f, 4.0f), new Vector3(2.9f, 0.1f, 4.0f)));
        // F
        edges.Add(new Edge(new Vector3(2.9f, 0.1f, 5.0f), new Vector3(1.8f, 0.1f, 5.0f)));
        // G
        edges.Add(new Edge(new Vector3(1.8f, 0.1f, 6.0f), new Vector3(1.3f, 0.1f, 6.0f)));
        // H
        edges.Add(new Edge(new Vector3(2.2f, 1.1f, 7.0f), new Vector3(1.3f, 1.1f, 7.0f)));
        edges.Add(new Edge(new Vector3(1.3f, 1.1f, 7.0f), new Vector3(1.0f, 1.1f, 7.0f)));
        edges.Add(new Edge(new Vector3(1.3f, 1.1f, 7.0f), new Vector3(1.1f, 1.3f, 7.0f)));
        // I
        edges.Add(new Edge(new Vector3(1.1f, 1.3f, 8.0f), new Vector3(0.8f, 1.6f, 8.0f)));
        edges.Add(new Edge(new Vector3(0.8f, 1.6f, 8.0f), new Vector3(0.2f, 1.6f, 8.0f)));
        // J
        edges.Add(new Edge(new Vector3(1.0f, 1.1f, 9.0f), new Vector3(0.7f, 1.1f, 9.0f)));
        edges.Add(new Edge(new Vector3(0.7f, 1.1f, 9.0f), new Vector3(0.7f, 0.8f, 9.0f)));
        edges.Add(new Edge(new Vector3(0.7f, 0.8f, 9.0f), new Vector3(1.1f, 0.4f, 9.0f)));
        // K
        edges.Add(new Edge(new Vector3(0.5f, 0.5f, 10.0f), new Vector3(0.7f, 0.5f, 10.0f)));
        edges.Add(new Edge(new Vector3(0.7f, 0.5f, 10.0f), new Vector3(0.7f, 0.1f, 10.0f)));
        edges.Add(new Edge(new Vector3(0.7f, 0.1f, 10.0f), new Vector3(0.9f, 0.1f, 10.0f)));
        // L
        edges.Add(new Edge(new Vector3(1.3f, 0.1f, 11.0f), new Vector3(1.2f, 0.1f, 11.0f)));
        edges.Add(new Edge(new Vector3(1.2f, 0.1f, 11.0f), new Vector3(0.9f, 0.1f, 11.0f)));
        edges.Add(new Edge(new Vector3(1.2f, 0.3f, 11.0f), new Vector3(1.2f, 0.1f, 11.0f)));
        edges.Add(new Edge(new Vector3(1.1f, 0.4f, 11.0f), new Vector3(1.2f, 0.3f, 11.0f)));

        RecalculatePieces();

        for (int i = 0; i < pieces.Count; i++)
        {
            pieces[i].CreatePieceObject().gameObject.transform.SetParent(transform);
        }
    }
}
