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
using Unreal = Gibbed.MassEffect3.FileFormats.Unreal;

namespace Gibbed.MassEffect3.SaveFormats
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Vector : Unreal.ISerializable, INotifyPropertyChanged
    {
        private float _X;
        private float _Y;
        private float _Z;

        public void Serialize(Unreal.ISerializer stream)
        {
            stream.Serialize(ref this._X);
            stream.Serialize(ref this._Y);
            stream.Serialize(ref this._Z);
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}",
                                 this._X,
                                 this._Y,
                                 this._Z);
        }

        #region Properties
        [LocalizedDisplayName("X", typeof(Localization.Vector))]
        public float X
        {
            get { return this._X; }
            set
            {
                if (Equals(value, this._X) == false)
                {
                    this._X = value;
                    this.NotifyPropertyChanged("X");
                }
            }
        }

        [LocalizedDisplayName("Y", typeof(Localization.Vector))]
        public float Y
        {
            get { return this._Y; }
            set
            {
                if (Equals(value, this._Y) == false)
                {
                    this._Y = value;
                    this.NotifyPropertyChanged("Y");
                }
            }
        }

        [LocalizedDisplayName("Z", typeof(Localization.Vector))]
        public float Z
        {
            get { return this._Z; }
            set
            {
                if (Equals(value, this._Z) == false)
                {
                    this._Z = value;
                    this.NotifyPropertyChanged("Z");
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
