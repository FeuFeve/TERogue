using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node
{
    public float x, y;
    public float f, g, h;
    public Node parent;
    public bool isValid;
    public int matrixX, matrixY;
    public bool isReached;

    public Node(float x, float y, int matrixX, int matrixY)
    {
        this.x = x;
        this.y = y;
        this.matrixX = matrixX;
        this.matrixY = matrixY;
        this.parent = null;
        this.isValid = true;
        this.isReached = false;
        //string[] masks = { "Obstacle", "VoidObstacle" };
        //int obstacleMask = LayerMask.GetMask(masks);
        //this.isValid = !Physics2D.OverlapPoint(new Vector2(x, y), obstacleMask);
        //this.isValid = !Physics2D.OverlapCapsule(new Vector2(this.x, this.y), new Vector2(0.2f, 0.2f), CapsuleDirection2D.Horizontal, 0f, obstacleMask);
    }

    public Node(float x, float y, int matrixX, int matrixY, float sizeX, float sizeY)
    {
        this.x = x;
        this.y = y;
        this.matrixX = matrixX;
        this.matrixY = matrixY;
        this.parent = null;
        string[] masks = { "Obstacle", "VoidObstacle" };
        int obstacleMask = LayerMask.GetMask(masks);
        setValid(sizeX, sizeY);
    }

    public bool IsValid()
    {
        return this.isValid;
    }

    public void setG(float g)
    {
        this.g = g;
    }

    public void setF(float f)
    {
        this.f = f;
    }

    public void setH(float h)
    {
        this.h = h;
    }

    void reset()
    {
        this.g = 0;
        this.h = 0;
        this.f = 0;
    }

    public void setParent(Node parent)
    {
        this.parent = parent;
    }

    public virtual void setValid(float spaceX, float spaceY)
    {
        this.isValid = true;
        string[] masks = { "Obstacle", "VoidObstacle" };
        int obstacleMask = LayerMask.GetMask(masks);
        //this.isValid = !Physics2D.OverlapPoint(new Vector2(x, y), obstacleMask);
        //this.isValid = !Physics2D.OverlapCapsule(new Vector2(this.x, this.y), new Vector2(spaceX * 1.02f, spaceY * 1.02f), CapsuleDirection2D.Horizontal, 0f, obstacleMask);
        this.isValid = !Physics2D.OverlapCircle(new Vector2(this.x, this.y), Mathf.Max(spaceX / 2.0f, spaceY / 2.0f), obstacleMask);
    }

    public override bool Equals(object obj)
    {
        return obj is Node node &&
               matrixX == node.matrixX &&
               matrixY == node.matrixY;
    }

    public override int GetHashCode()
    {
        int hashCode = 539040427;
        hashCode = hashCode * -1521134295 + matrixX.GetHashCode();
        hashCode = hashCode * -1521134295 + matrixY.GetHashCode();
        return hashCode;
    }
}

public class NodeComparator : IComparer<Node>
{
    public int Compare(Node x, Node y)
    {
        if (x.f.Equals(y.f)) return 0;
        return x.f < y.f ? -1 : 1;
    }

}