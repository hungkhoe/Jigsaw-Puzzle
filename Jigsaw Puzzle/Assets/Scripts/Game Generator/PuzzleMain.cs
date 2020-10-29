using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleMain : MonoBehaviour
{
    public GameObject jigsaw = null;  		// 	will contain the FBX imported blender file with all (25*9) puzzle peaces
    public Material linesHorizontal = null;	//	material for horizontal lines
    public Material linesVertical = null;	//	material for vertical ines
    public Material scatteredPieces = null;	//	material for scattered pieces
    public Material pieceBase = null;		//	material for piece bases
    public Material outline = null;
    public float spacing = 0.01f;

    private Hashtable basePieceTransforms = new Hashtable();
    // Use this for initialization
    void Start()
    {
        // load references to all puzzle piece Transform objects into HashTable       
    }

    // get base piece prototype
    public Transform GetBase(string ident)
    {
       return jigsaw.transform.Find(ident) as Transform;     
    }

    // get specific piece prototype
    public Transform GetPiece(string ident, string piece)
    {
        Transform basePiece = basePieceTransforms[ident] as Transform;
        if (basePiece != null)
            return basePiece.Find(ident + piece.ToUpper());
        else
            return null;
    }

    // Load all 25 base puzzle pieces into HashTable
    public void GetBasePieces()
    {
        bool aPieceNotFound = false;
        for (int px = 1; px <= 5; px++)
        {
            for (int py = 1; py <= 5; py++)
            {
                string ident = "" + px + "" + py;
                Transform t = GetBase(ident);
                if (t == null)
                    aPieceNotFound = true;
                basePieceTransforms.Add(ident, t);
            }
        }
    }
}
