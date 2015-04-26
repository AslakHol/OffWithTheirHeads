using UnityEngine;
using System.Collections;

public class Musician : NodActor {

    public AudioSource musicSource;

    override public void PerformNodAction(NodActor target){
        Debug.Log("Musician playing louder!");
        musicSource.volume += 0.1f;
    }
}
