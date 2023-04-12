using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMax
{
    public static int maxDepth = 3;
    public LinkedList<MinMaxNode> nodes = new LinkedList<MinMaxNode>();

    public MinMax(int white, int black, int queens)
    {
        nodes.AddFirst(new MinMaxNode(white, black, queens));
    }

}
