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

using System;
using System.ComponentModel;

namespace Gibbed.MassEffect3.SaveFormats
{
    [AttributeUsage(AttributeTargets.Property |
                    AttributeTargets.Event |
                    AttributeTargets.Class |
                    AttributeTargets.Method)]
    internal class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        private readonly LocalizedString _DisplayName = new LocalizedString();

        public LocalizedDisplayNameAttribute(string propertyName, Type resourceType)
            : this(propertyName != null ? "[FIX ME] " + propertyName : null, propertyName, resourceType)
        {
        }

        public LocalizedDisplayNameAttribute(string defaultValue, string propertyName, Type resourceType)
            : base(defaultValue)
        {
            if (resourceType == null)
            {
                throw new ArgumentNullException("resourceType");
            }

            if (string.IsNullOrEmpty(propertyName) == true)
            {
                throw new ArgumentNullException("propertyName");
            }

            this._DisplayName.ResourceType = resourceType;
            this._DisplayName.PropertyName = propertyName + "_DisplayName";
        }

        public override string DisplayName
        {
            get { return this._DisplayName.GetLocalizedValue() ?? base.DisplayName; }
        }
    }
}
