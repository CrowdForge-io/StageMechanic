﻿/*  
 * Copyright (C) Catherine. All rights reserved.  
 * Licensed under the BSD 3-Clause License.
 * See LICENSE file in the project root for full license information.
 * See CONTRIBUTORS file in the project root for full list of contributors.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cathy1GoalBlock : Cathy1Block
{

    public AudioClip Applause;
    public string NextStageFilename;
    public string NextStageStartPos;

    private enum State
    {
        NoPlayer = 0,
        PlayerEnter,
        PlayerStand,
        PlayerLeave
    }

    private State CurrentState = State.NoPlayer;

    public sealed override BlockType Type { get; } = BlockType.Goal;

    private IEnumerator HandleStep()
    {
        CurrentState = State.PlayerEnter;
        yield return new WaitForEndOfFrame();
        CurrentState = State.PlayerStand;

        GetComponent<AudioSource>().PlayOneShot(Applause);

    }

    private void Start()
    {
        //Make block immobile
        WeightFactor = 0f;
    }

    private void Update()
    {
        List<Collider> crossColiders = new List<Collider>(Physics.OverlapBox(transform.position + new Vector3(0f, 0.75f, 0f), new Vector3(0.1f, 0.1f, 0.75f)));
        foreach (Collider col in crossColiders)
        {
            if (col.gameObject == gameObject)
                continue;
            Cathy1EdgeMechanic otherBlock = col.gameObject.GetComponent<Cathy1EdgeMechanic>();
            if (otherBlock != null)
                continue;
            Cathy1PlayerCharacter player = col.gameObject.GetComponent<Cathy1PlayerCharacter>();
            if (player != null)
            {
                if (CurrentState == State.PlayerStand)
                    continue;
                else if (CurrentState == State.NoPlayer)
                    StartCoroutine(HandleStep());

            }
            else
            {
                CurrentState = State.NoPlayer;
            }

        }
        if (crossColiders.Count == 0)
            CurrentState = State.NoPlayer;
    }

    public override Dictionary<string, KeyValuePair<string, string>> DefaultProperties
    {
        get
        {
            Dictionary<string, KeyValuePair<string, string>> ret = base.DefaultProperties;
            ret.Add("Next Stage Filename", new KeyValuePair<string, string>("string", ""));
            ret.Add("Next Stage Start Block Override", new KeyValuePair<string, string>("string", ""));
            return ret;
        }
    }

    public override Dictionary<string, string> Properties
    {
        get
        {
            Dictionary<string, string> ret = base.Properties;
            if (!string.IsNullOrEmpty(NextStageFilename) && !string.IsNullOrWhiteSpace(NextStageFilename))
                ret.Add("Next Stage Filename", NextStageFilename);
            if (!string.IsNullOrEmpty(NextStageStartPos) && !string.IsNullOrWhiteSpace(NextStageStartPos))
                ret.Add("Next Stage Start Block Override", NextStageStartPos);
            return ret;
        }
        set
        {
            if (value.ContainsKey("Next Stage Filename"))
                NextStageFilename = value["Next Stage Filename"];
            if (value.ContainsKey("Next Stage Start Block Override"))
                NextStageStartPos = value["Next Stage Start Block Override"];
        }
    }
}
