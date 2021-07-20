//-----------------------------------------------------------------------
//// <copyright file="Slithering.cs" company="Google">
/////
///// Copyright 2017 Google Inc. All Rights Reserved.
/////
///// Licensed under the Apache License, Version 2.0 (the "License");
///// you may not use this file except in compliance with the License.
///// You may obtain a copy of the License at
/////
///// http://www.apache.org/licenses/LICENSE-2.0
/////
///// Unless required by applicable law or agreed to in writing, software
///// distributed under the License is distributed on an "AS IS" BASIS,
///// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
///// See the License for the specific language governing permissions and
///// limitations under the License.
/////
///// </copyright>
/////-----------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Class for creating a snake-like segmented motion.
/// </summary>
/// <remarks>
///     The speed and distances are scale sensitive, so when changing
///     the size of the object, keep that in mind.
/// </remarks>
public class Slithering : MonoBehaviour
{
    // The body prefab, each segment is made from this prefab.
    public GameObject bodyPrefab;

    // The head of the body.
    public Transform fixedTransform;
    public float speed = 15;
    public float rotationSpeed = 50;
    public float minDistance = .05f;

    // List of body segments, the head is expected to be [0].
    private readonly List<Transform> bodyParts = new List<Transform>();
    private Transform _head;

    public Color color;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    public void SetHead(bool ai, Transform head)
    {
        _head = head;
        ResetSize();
        AddBodyPart(ai, _head);
        var rigidBody = _head.gameObject.GetComponent<Rigidbody>();
        rigidBody.velocity = Vector3.zero;
    }

    private void Update()
    {
        // if (_AISnake)
        // {
        //     var rigidbody = head.gameObject.GetComponent<Rigidbody>();
        //     rigidbody.transform.LookAt(_playerSnake.transform.position);
        //     rigidbody.velocity = head.transform.forward * (head.transform.localScale.x * .15f) / .01f;
        // }

        Move();
    }

    public List<Vector3> GetWholeBodyCoords()
    {
        var bodyCoords = new List<Vector3>();
        bodyCoords.Add(_head.position);
        for (var i = 0; i < bodyParts.Count; i++) bodyCoords.Add(bodyParts[i].transform.position);

        return bodyCoords;
    }


    public void ResetSize()
    {
        while (bodyParts.Count > 0)
        {
            var t = bodyParts[bodyParts.Count - 1];
            if (t != null) Destroy(t.gameObject);

            bodyParts.RemoveAt(bodyParts.Count - 1);
        }
    }

    private void Move()
    {
        Transform current;
        Transform prev;

        // For each part of the body move it towards the previous part,
        // with a springy effect so faster if it is further away.
        for (var i = 1; i < bodyParts.Count; i++)
        {
            current = bodyParts[i];
            prev = bodyParts[i - 1];

            var dist = Vector3.Distance(prev.position, current.position);

            // The new position is the previous position.  Keep the y value
            // the same as the head.
            var newPos = prev.position;
            newPos.y = bodyParts[0].position.y;

            // Move faster based on the distance.
            var amt = Mathf.Clamp(Time.deltaTime * dist / minDistance * speed, 0, .5f);

            // Don't move if we're really close, but always rotate to give that
            // slithery look.
            if (dist >= minDistance / 2f) current.position = Vector3.Lerp(current.position, newPos, amt);

            current.rotation = Quaternion.Slerp(current.rotation, prev.rotation, amt);
        }
    }

    public void AddBodyPart(bool ai, Transform newPart = null)
    {
        var pos = fixedTransform.position;
        var rot = fixedTransform.rotation;
        if (bodyParts.Count != 0)
        {
            var lastPart = bodyParts[bodyParts.Count - 1];
            pos = lastPart.position - lastPart.forward * lastPart.localScale.x;
            rot = lastPart.rotation;
        }

        if (newPart == null) newPart = Instantiate(bodyPrefab, pos, rot).transform;
        if (ai)
        {
            newPart.gameObject.tag = "AI";
            var material = newPart.GetComponent<Renderer>().material;
            material.SetVector(EmissionColor, color);
        }

        newPart.SetParent(fixedTransform);
        bodyParts.Add(newPart);
    }

    public int GetLength()
    {
        return bodyParts.Count;
    }
}