﻿using UnityEngine;

public class Circle : MonoBehaviour {
    [SerializeField] private int segments;
    [SerializeField] private float xradius;
    [SerializeField] private float yradius;

    [SerializeField] private int offsetSegments;
    [SerializeField] private float minOffset;
    [SerializeField] private float maxOffset;
    [SerializeField] private AnimationCurve circleSmoothing;

    [SerializeField, Range(0, 2f)] private float offsetMultiplier;
    private LineRenderer line;
    private float[] offsets;
    private float[] multipliedOffsets;

    void Start() {
        line = gameObject.GetComponent<LineRenderer>();
        line.useWorldSpace = true;

        offsets = new float[offsetSegments];
        multipliedOffsets = new float[offsetSegments];

        for (var i = 0; i < offsets.Length; i++) {
            offsets[i] = Random.Range(minOffset, maxOffset);
        }
    }

    private void Update() {
        UpdatePoints();

        for (var i = 0; i < multipliedOffsets.Length; i++) {
            multipliedOffsets[i] = offsets[i] * offsetMultiplier;
        }
    }


    void UpdatePoints() {
        line.positionCount = segments + 1;

        var noiseSegmentLength = segments / offsetSegments;
        float noiseSegmentLenghtFloat = segments / offsetSegments;

        float x;
        float y;
        var z = 0f;

        var angle = 20f;

        for (var i = 0; i < line.positionCount; i++) {
            var offsetVector = Vector2.zero;

            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

            if (i % noiseSegmentLength == 0) {
                if (i != line.positionCount - 1) {
                    offsetVector = new Vector2(x, y).normalized * multipliedOffsets[i / noiseSegmentLength];
                }
                else {
                    offsetVector = new Vector2(x, y).normalized * multipliedOffsets[0];
                }
            }
            else {
                if (i < line.positionCount - noiseSegmentLength) {
                    var lerpTime = circleSmoothing.Evaluate((i % noiseSegmentLength) / noiseSegmentLenghtFloat);

                    var currentOffset = multipliedOffsets[(i - (i % noiseSegmentLength)) / noiseSegmentLength];

                    var newOffset =
                        multipliedOffsets[(i + (noiseSegmentLength - (i % noiseSegmentLength))) / noiseSegmentLength];

                    offsetVector = new Vector2(x, y).normalized * Mathf.Lerp(currentOffset, newOffset, lerpTime);
                }
                else {
                    var lerpTime = circleSmoothing.Evaluate((i % noiseSegmentLength) / noiseSegmentLenghtFloat);

                    var currentOffset = multipliedOffsets[(i - (i % noiseSegmentLength)) / noiseSegmentLength];

                    var newOffset = multipliedOffsets[0];

                    offsetVector = new Vector2(x, y).normalized * Mathf.Lerp(currentOffset, newOffset, lerpTime);
                }
            }

            //Vector2 position = new Vector2(x, y);

            var position = new Vector3(x + offsetVector.x, y + offsetVector.y, z);

            line.SetPosition(i, position);

            angle += (360f / segments);
        }
    }
}
