using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayIdleBehaviour : IdleBaseBehaviour
{
    public override void Action()
    {
        EndTurn();
    }
}
