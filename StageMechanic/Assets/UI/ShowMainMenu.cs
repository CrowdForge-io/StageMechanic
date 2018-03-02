﻿/*  
 * Copyright (C) Catherine. All rights reserved.  
 * Licensed under the BSD 3-Clause License.
 * See LICENSE file in the project root for full license information.
 * See CONTRIBUTORS file in the project root for full list of contributors.
 */
using UnityEngine;

public class ShowMainMenu : MonoBehaviour {
   
	public void onButtonClicked()
    {
        UIManager.ToggleMainMenu();
    }
}
