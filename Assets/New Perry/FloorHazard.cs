using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorHazard : Hazard
{
    public override void WalkMovementAction()
    {
        base.WalkMovementAction();
        base.AlertPerry(medRadius);
    }
}
