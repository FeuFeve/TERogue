using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlyingNode : Node
{
    public FlyingNode(float x, float y, int matrixX, int matrixY) : base(x, y, matrixX, matrixY)
    {
    }

    public FlyingNode(float x, float y, int matrixX, int matrixY, float sizeX, float sizeY) : base(x, y, matrixX, matrixY, sizeX, sizeY)
    {
    }



    public override void setValid(float spaceX, float spaceY)
    {
        this.isValid = true;
        string[] masks = { "Obstacle" };
        int obstacleMask = LayerMask.GetMask(masks);
        //this.isValid = !Physics2D.OverlapPoint(new Vector2(x, y), obstacleMask);
        this.isValid = !Physics2D.OverlapCircle(new Vector2(this.x, this.y), Mathf.Max(spaceX / 2.0f, spaceY / 2.0f), obstacleMask);

    }




}
