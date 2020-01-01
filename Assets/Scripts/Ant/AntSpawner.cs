using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntSpawner : MonoBehaviour
{
    public GameObject spawnedParticleSystem;

    public GameObject antPrefab;
    public GameObject queenAnt;
    

    public void SpawnAnt(string currTargetWord)
    {
        GameObject ant = Instantiate(antPrefab, this.transform.position, Quaternion.identity);
        Ant antScript = ant.GetComponent<Ant>();
        Animator queenAnimator = queenAnt.GetComponent<Animator>();

        antScript.SetCargo(currTargetWord);
        antScript.myQueen = this.queenAnt;
        antScript.mySpawner = this.gameObject;
        antScript.myParticleSystem = spawnedParticleSystem.GetComponent<ParticleSystem>();

    }

}
