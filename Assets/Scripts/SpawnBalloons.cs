using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBalloons : MonoBehaviour {
    public GameObject balloon;
    public Wind windScript;

    private void Start() {
        // SpawnBalloon();
        StartCoroutine(Repeat());
    }

    IEnumerator Repeat() {
        while (true) {
            yield return new WaitForSeconds(1);
            SpawnBalloon();
        }
    }

    private void SpawnBalloon() {
        GameObject balloonObj = Instantiate(
            balloon,
            new Vector3(Random.Range(6, 12), 2, 0),
            Quaternion.identity
        );

        Balloon balloonScript = balloonObj.GetComponent<Balloon>();
        balloonScript.windScript = windScript;
    }
}
