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
using Unreal = Gibbed.MassEffect3.FileFormats.Unreal;

namespace Gibbed.MassEffect3.SaveFormats
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [OriginalName("KismetBoolSaveRecord")]
    public class KismetBool : Unreal.ISerializable, INotifyPropertyChanged
    {
        #region Fields
        [OriginalName("BoolGUID")]
        private Guid _Guid;

        [OriginalName("Value")]
        private bool _Value;
        #endregion

        public void Serialize(Unreal.ISerializer stream)
        {
            stream.Serialize(ref this._Guid);
            stream.Serialize(ref this._Value);
        }

        #region Properties
        [LocalizedDisplayName("Guid", typeof(Localization.KismetBool))]
        public Guid Guid
        {
            get { return this._Guid; }
            set
            {
                if (value != this._Guid)
                {
                    this._Guid = value;
                    this.NotifyPropertyChanged("Guid");
                }
            }
        }

        [LocalizedDisplayName("Value", typeof(Localization.KismetBool))]
        public bool Value
        {
            get { return this._Value; }
            set
            {
                if (value != this._Value)
                {
                    this._Value = value;
                    this.NotifyPropertyChanged("Value");
                }
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
