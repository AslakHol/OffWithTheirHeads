using UnityEngine;
using System.Collections;

public class Petitioner : NodActor {

    override public void PerformNodAction(NodActor target){
        Debug.Log("petitioner targeted " + target);
    }

}
