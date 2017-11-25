using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceLogicModule : MonoBehaviour {
    public Transform racer;

    public RaceSet raceSet;

    private Queue<Vector3> nodes = new Queue<Vector3>();

    private Transform CheckPoint;

    private TextMesh raceTimeMesh;

    private float t;



    public void StartRace(System.Action onRaceOver){
        raceSet = Instantiate(Resources.Load<RaceSet>("Modules/DesertRace"));

        CheckPoint = Instantiate(Resources.Load<Transform>("Modules/CheckPoint"));

        raceTimeMesh = CheckPoint.Find("Time").GetComponent<TextMesh>();

        for (int i = 0; i < raceSet.raceNodes.Length; i++) {
            nodes.Enqueue(raceSet.raceNodes[i]);
        }

        t = 0;

        StartCoroutine(RacelogicLoop(onRaceOver));
    }

    private void Update() {
        t += Time.deltaTime;
        System.TimeSpan time = new System.DateTime().AddSeconds((double)t).TimeOfDay;
        raceTimeMesh.text = string.Format("{0}:{1}:{2}", time.Minutes, time.Seconds,time.Milliseconds);

        CheckPoint.transform.rotation = Quaternion.LookRotation(new Vector3(racer.position.x, 0, racer.position.z)-new Vector3(CheckPoint.position.x, 0, CheckPoint.position.z),Vector3.up);
    }

    private IEnumerator RacelogicLoop(System.Action onRaceOver) {
        while(nodes.Count!=0){
            Vector3 currentPoint = nodes.Dequeue();

            RaceNodeUpdate(currentPoint);

            while(Vector3.Distance(racer.position,currentPoint)>30){
                yield return new WaitForEndOfFrame();
            }

        }

        onRaceOver();

    }

    private void RaceNodeUpdate(Vector3 nextPoint){
        CheckPoint.transform.position = nextPoint;
    }
}
