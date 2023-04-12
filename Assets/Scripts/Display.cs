using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BitBoard;

public class Display : MonoBehaviour
{
    public int colorIndex = 0;
    public TextMeshProUGUI title;
    public TextMeshProUGUI pieceCount;
    public TextMeshProUGUI captureCount;

    // Update is called once per frame
    void Update()
    {
        title.text = "Checkers";
        pieceCount.text = "Pieces: " + CountBits(Main.bitBoardManager.bitBoard.pieces[colorIndex]);
        captureCount.text = "Captures: " + (12 - CountBits(Main.bitBoardManager.bitBoard.pieces[colorIndex == 0 ? 1 : 0]));
    }

    private int CountBits(int value)
    {
        int count = 0;
        for (int i = 0; i < 32; i++)
        {
            if ((value & Main.bytePositions[i]) != 0)
            {
                count++;
            }
        }
        return count;
    }
}
