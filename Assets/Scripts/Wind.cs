using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wind : MonoBehaviour {
    public float maxWind = 10;
    public Text windText;
    [HideInInspector]
    public float wind = 0;

    private void Start() {
        StartCoroutine(Repeat());
    }

    IEnumerator Repeat() {
        while (true) {
            yield return new WaitForSeconds(2);
            SpawnBalloon();
        }
    }

    private void SpawnBalloon() {
        wind = Random.Range(-maxWind, maxWind);

        windText.text = wind + "";
    }
}
