using System;
using System.Collections;
using Mimont.Gameplay;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mimont.JobMode {
public class JobAI : MonoBehaviour, IPlayer {
    public JobSprite sprite;

    public event Action<Vector3> RingCreated;

    public event Action RingReleased;

    public bool kissing;

    public void StartOtherRing(Vector3 position) {
        /* Not implemented */
    }

    public void ReleaseOtherRing() {
        /* Not implemented */
    }

    public void AddTarget(Vector3 position, int tierIndex) {
        if (kissing) return;

        sprite.transform.position = position;
        sprite.SetKiss();

        var delay = Random.value;
        StartCoroutine(StartKissing(delay, position));

        StartCoroutine(StopKissing(delay));
    }

    private IEnumerator StartKissing(float delay, Vector3 position) {
        kissing = true;
        yield return new WaitForSeconds(delay);

        RingCreated?.Invoke(position);
    }

    private IEnumerator StopKissing(float delay) {
        yield return new WaitForSeconds(delay);

        var time = 5 + Random.Range(-1f, 1f) * Random.Range(.3f, 2.5f);

        yield return new WaitForSeconds(time);

        RingReleased?.Invoke();
        sprite.SetIdle();
        kissing = false;
    }
}
}
