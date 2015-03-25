//-------------------------------------------------------------------------------
// this class is for the block that acts as a reference to an empty tableau pile
//-------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class TableauAnc : MonoBehaviour {
	public bool pactive = false; // determines if this is active, which only occurs if the pile above is empty
	[SerializeField]
	public int 	pileID; // the tableau this placeholder goes with;
	public int 	pileSZ; // the size of the pile on the tableau this object represents

	// Use this for initialization
	void Start () {
		pactive = false;
	}
	
	// Update is called once per frame
	void Update () { 
		pileSZ = Ultimate_Solitaire.S.tableaus [pileID].Count;
		if (Ultimate_Solitaire.S.tableaus[pileID].Count == 0)
			pactive = true;
		if (pactive == true && Ultimate_Solitaire.S.tableaus [pileID].Count != 0)
						pactive = false;
	
	}
}


