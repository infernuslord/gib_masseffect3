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
    [OriginalName("EDifficultyOptions")]
    public enum DifficultyOptions : byte
    {
        [StandardValue(null, DisplayName = "Narrative")]
        [OriginalName("DO_Level1")]
        Level1 = 0,

        [StandardValue(null, DisplayName = "Casual")]
        [OriginalName("DO_Level2")]
        Level2 = 1,

        [StandardValue(null, DisplayName = "Normal")]
        [OriginalName("DO_Level3")]
        Level3 = 2,

        [StandardValue(null, DisplayName = "Hardcore")]
        [OriginalName("DO_Level4")]
        Level4 = 3,

        [StandardValue(null, DisplayName = "Insanity")]
        [OriginalName("DO_Level5")]
        Level5 = 4,

        [StandardValue(null, DisplayName = "What is beyond Insanity?", Description = "(it is a mystery)")]
        [OriginalName("DO_Level6")]
        Level6 = 5,
    }
}