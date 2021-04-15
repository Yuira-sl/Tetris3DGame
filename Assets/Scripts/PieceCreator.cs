using System.Collections.Generic;
using UnityEngine;

public static class PieceCreator
{
    private static readonly List<PiecesSet> Entries = new List<PiecesSet>();
    
    static PieceCreator()
    {
        Piece[] currentEntry;
        
        // //Single Block
        // currentEntry = new Piece[1];
        // currentEntry[0] = new Piece(Vector2.zero);
        // Entries.Add(new PiecesSet
        // {
        //     Pieces = currentEntry 
        // });
        //
        // //Cuad Block
        // currentEntry = new Piece[4];
        // currentEntry[0] = new Piece(new Vector2(0, 0));
        // currentEntry[1] = new Piece(new Vector2(1, 0));
        // currentEntry[2] = new Piece(new Vector2(1, -1));
        // currentEntry[3] = new Piece(new Vector2(0, -1));
        //
        // Entries.Add(new PiecesSet
        // {
        //     Pieces = currentEntry 
        // });
        //
        // //3 Long Straight Block
        // currentEntry = new Piece[3];
        // currentEntry[0] = new Piece(new Vector2(0, 0));
        // currentEntry[1] = new Piece(new Vector2(0, -1));
        // currentEntry[2] = new Piece(new Vector2(0, -2));
        //
        // Entries.Add(new PiecesSet
        // {
        //     Pieces = currentEntry
        // });
        //
        // //T Block
        // currentEntry = new Piece[4];
        // currentEntry[0] = new Piece(new Vector2(-1, 0));
        // currentEntry[1] = new Piece(new Vector2(1, 0));
        // currentEntry[2] = new Piece(new Vector2(0, 0));
        // currentEntry[3] = new Piece(new Vector2(0, -1));
        //
        // Entries.Add(new PiecesSet
        // {
        //     Pieces = currentEntry 
        // });
        //
        //  //Z Block
        //  currentEntry = new Piece[4];
        //  currentEntry[0] = new Piece(new Vector2(-1, 0));
        //  currentEntry[1] = new Piece(new Vector2(0, 0));
        //  currentEntry[2] = new Piece(new Vector2(0, -1));
        //  currentEntry[3] = new Piece(new Vector2(1, -1));
        //  Entries.Add(new PiecesSet
        //  {
        //      Pieces = currentEntry
        //  });
        //
        //  //Little L Block
        //  currentEntry = new Piece[3];
        //  currentEntry[0] = new Piece(new Vector2(-1, 0));
        //  currentEntry[1] = new Piece(new Vector2(-1, -1));
        //  currentEntry[2] = new Piece(new Vector2(0, -1));
        //  Entries.Add(new PiecesSet
        //  {
        //      Pieces = currentEntry 
        //  });
        
         //Big L Block
         currentEntry = new Piece[4];
         currentEntry[0] = new Piece(new Vector2(-1, 0));
         currentEntry[1] = new Piece(new Vector2(-1, -1));
         currentEntry[2] = new Piece(new Vector2(-1, -2));
         currentEntry[3] = new Piece(new Vector2(0, -2));
         Entries.Add(new PiecesSet
         {
             Pieces = currentEntry
         });
    }
    public static Piece[] PickRandomPiece()
    {
        int i = Random.Range(0, Entries.Count);
        return Entries[i].GetPieces();
    }
}

public struct PiecesSet
{
    public Piece[] Pieces { get; set; }
    public Piece[] GetPieces()
    {
        var output = new Piece[Pieces.Length];
        for(int i = 0; i < output.Length; i++)
        {
            output[i] = new Piece(new Vector3(Pieces[i].GetX(), Pieces[i].GetY()));
        }
        return output;
    }
}
