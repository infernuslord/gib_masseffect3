﻿/* Copyright (c) 2012 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System.ComponentModel;
using System.Drawing.Design;
using DynamicTypeDescriptor;

namespace Gibbed.MassEffect3.FileFormats.Save
{
    [Editor(typeof(StandardValueEditor), typeof(UITypeEditor))]
    [OriginalName("EEndGameState")]
    public enum EndGameState
    {
        [StandardValue(null, DisplayName = "Not Finished")]
        [OriginalName("EGS_NotFinished")]
        NotFinished = 0,

        [StandardValue(null, DisplayName = "Out In A Blaze Of Glory")]
        [OriginalName("EGS_OutInABlazeOfGlory")]
        OutInABlazeOfGlory = 1,

        [StandardValue(null, DisplayName = "Lived To Fight Again")]
        [OriginalName("EGS_LivedToFightAgain")]
        LivedToFightAgain = 2,
    }
}
