using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntSpawner : MonoBehaviour
{

    public Utilities.OutputRestriction outputRestriction;
    public GameObject spawnedParticleSystem;

    public GameObject antPrefab;
    public GameObject QueenAnt;
    

    public void SpawnAnt(string currTargetWord)
    {
        GameObject ant = Instantiate(antPrefab, this.transform.position, Quaternion.identity);
        Ant antScript = ant.GetComponent<Ant>();
        Animator queenAnimator = QueenAnt.GetComponent<Animator>();

        antScript.SetCargo(currTargetWord);
        antScript.myQueen = this.QueenAnt;
        antScript.mySpawner = this.gameObject;
        antScript.outputRestriction = outputRestriction;
        antScript.myParticleSystem = spawnedParticleSystem.GetComponent<ParticleSystem>();

    }

}
