using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinate
{
    public int x;
    public int y;

    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    // Calculate the Manhattan distance between two coordinates
    public int ManhattanDistance(Coordinate other)
    {
        return Mathf.Abs(x - other.x) + Mathf.Abs(y - other.y);
    }

    // Override Equals method
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Coordinate other = (Coordinate)obj;
        return x == other.x && y == other.y;
    }

    // Override GetHashCode method
    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17;
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            return hash;
        }
    }
}