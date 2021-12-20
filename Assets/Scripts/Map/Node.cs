using System.Collections.Generic;

public class Node {
    public List<Node> neighbours;
    public int x;
    public int y;
    
    public Node() {
        neighbours = new List<Node>();
    }
}