﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cathy1HeavyBlock : Cathy1Block {

	public override Cathy1Block.BlockType Type {
		get {
			return Cathy1Block.BlockType.Heavy;
		}
	}

	// Use this for initialization
	void Start () {
		WeightFactor = 2.5f;
	}
}
