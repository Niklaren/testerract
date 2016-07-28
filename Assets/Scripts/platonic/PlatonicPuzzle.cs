using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatonicPuzzle : MonoBehaviour {

    [SerializeField]
    List<ElementSolid> elements;

    [SerializeField]
    List<MeshRenderer> Clues;

    [SerializeField]
    List<Material> CluesStage0;

    [SerializeField]
    List<Material> CluesStage1;

    int stage = 0;
    public int GetStage() { return stage; }

	// Use this for initialization
	void Start () {
        SetStage0Clues();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void CheckPuzzleStage() // i did this a little awkwardly...
    {
        //if(!CheckStage0Complete())
        //{
        //    Debug.Log("stage 0");
        //}
        if (CheckStage0Complete())
        {
            Debug.Log("stage 1");
            stage = 1;
        }
        if(CheckStage1Complete())
        {
            Debug.Log("stage 2");
            stage = 2;
        }
        if (CheckStage2Complete())
        {
            Debug.Log("stage 3");
            stage = 3; // i guess this is just complete though
        }
    }

    private bool CheckStage0Complete()
    {
        if (stage == 0 && AllInCorrectSocket())
        {
            Debug.Log("stage 0 complete");
            stage = 1;

            // for every element check if it's complete (this should turn air on)
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].CheckComplete();
            }
            SetStage1Clues();
            return true;
        }
        return false;
    }

    private bool CheckStage1Complete()
    {
        if (stage == 1)
        {
            // for all but ether
            for (int i = 0; i < elements.Count-1; i++)
            {
                if (!elements[i].InCorrectSocket() || !elements[i].IsComplete())
                {
                    return false;
                }
            }
            Debug.Log("stage 1 complete");
            return true;
        }
        return false;
    }

    private bool CheckStage2Complete()
    {
        if (stage == 2)
        {
            // for all but ether
            for (int i = 0; i < elements.Count; i++)
            {
                if (!elements[i].InCorrectSocket() || !elements[i].IsComplete())
                {
                    return false;
                }
            }
            Debug.Log("stage 2 complete");
            return true;
        }
        return false;
    }

    public bool CheckComplete()
    {
        for(int i = 0; i < elements.Count; i++)
        {
            if(!elements[i].InCorrectSocket() || !elements[i].IsComplete())
            {
                return false;
            }
        }
        return true;
    }

    public bool AllInCorrectSocket()
    {
        for (int i = 0; i < elements.Count; i++)
        {
            if (!elements[i].InCorrectSocket())
            {
                return false;
            }
        }
        return true;
    }

    private void SetStage0Clues()
    {
        for (int i = 0; i < Clues.Count; i++)
        {
            Clues[i].material = CluesStage0[i];
        }
    }

    private void SetStage1Clues()
    {
        for (int i = 0; i < Clues.Count; i++)
        {
            Clues[i].material = CluesStage1[i];
        }
    }
}
