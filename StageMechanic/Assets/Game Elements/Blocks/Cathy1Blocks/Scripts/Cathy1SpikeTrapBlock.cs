﻿/*  
 * Copyright (C) Catherine. All rights reserved.  
 * Licensed under the BSD 3-Clause License.
 * See LICENSE file in the project root for full license information.
 * See CONTRIBUTORS file in the project root for full list of contributors.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements a normal Cath1-style spike trap. Upon the player or enemy stepping on this
/// block it will trigger after the specified trigger time, allowing the player or enemy
/// a certain amount of time to move before the trap enters the IsTriggerd state before
/// moving into the disarned state.
/// </summary>
public sealed class Cathy1SpikeTrapBlock : Cathy1AbstractTrapBlock
{
    public Material ArmedStateMaterial;
    public Material TriggeredStateMaterial;
    public Material ActiveStateMaterial;
    public Material DisarmedStateMaterial;

    public ParticleSystem Animation;
    public float AnimationScale = DEFAULT_ANIMATION_SCALE;

    private const float DEFAULT_TRIGGER_TIME = 0.6f;
    private const float DEFAULT_ANIMATION_SCALE = 2;

    public sealed override BlockType Type { get; } = BlockType.SpikeTrap;


    /// <summary>
    /// Indicate to the Cathy1 game rules that this is a spike trap
    /// </summary>
    public sealed override TrapBlockType TrapType
    {
        get
        {
            return TrapBlockType.Spike;
        }
    }

    private enum State
    {
        NoPlayer = 0,
        PlayerEnter,
        PlayerStand,
        PlayerLeave,
        Disarmed
    }

    private State CurrentState = State.NoPlayer;

    /// <summary>
    /// Sets the trigger time of the spike trap
    /// </summary>
    public override void Awake()
    {
        base.Awake();
        TriggerTime = DEFAULT_TRIGGER_TIME;
    }

    bool hasPlayer()
    {
        Vector3 player = PlayerManager.Player1Location();
        return (player == transform.position + Vector3.up && (PlayerManager.PlayerStateName() == "Idle" || PlayerManager.PlayerStateName() == "Walk" || PlayerManager.PlayerStateName() == "Center"));
    }

    private IEnumerator HandleStep()
    {
        CurrentState = State.PlayerEnter;
        yield return new WaitForSeconds(TriggerTime);
        BlockManager.PlayEffect(this, Animation, 7f, TriggerTime, new Vector3(0f, 3f, 0f), Quaternion.Euler(0, 180, 90));
        GetComponent<AudioSource>().Play();
        if (hasPlayer())
        {
            CurrentState = State.PlayerStand;
            PlayerManager.Player(0).TakeDamage(float.PositiveInfinity);
        }
        Renderer rend = GetComponent<Renderer>();
        rend.material = DisarmedStateMaterial;
        CurrentState = State.Disarmed;
    }

    private void Update()
    {
        if (!BlockManager.PlayMode)
            return;
        if (CurrentState == State.Disarmed)
            return;
        if (CurrentState == State.PlayerEnter || CurrentState == State.PlayerStand)
            return;
        if (!hasPlayer())
            return;
        StartCoroutine(HandleStep());
    }

    public override Dictionary<string, KeyValuePair<string, string>> DefaultProperties
    {
        get
        {
            Dictionary<string, KeyValuePair<string, string>> ret = base.DefaultProperties;
            ret.Add("Trigger Time (seconds)", new KeyValuePair<string, string>("float", DEFAULT_TRIGGER_TIME.ToString()));
            ret.Add("Animation Scale", new KeyValuePair<string, string>("float", DEFAULT_ANIMATION_SCALE.ToString()));
            return ret;
        }
    }

    public override Dictionary<string, string> Properties
    {
        get
        {
            Dictionary<string, string> ret = base.Properties;
            if (TriggerTime != DEFAULT_TRIGGER_TIME)
                ret.Add("Trigger Time (seconds)", TriggerTime.ToString());
            if (AnimationScale != DEFAULT_ANIMATION_SCALE)
                ret.Add("Animation Scale", AnimationScale.ToString());
            return ret;
        }
        set
        {
            base.Properties = value;
            if (value.ContainsKey("Trigger Time (seconds)"))
                TriggerTime = float.Parse(value["Trigger Time (seconds)"]);
            if (value.ContainsKey("Animation Scale"))
                AnimationScale = float.Parse(value["Animation Scale"]);
        }
    }
}
