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

namespace Gibbed.MassEffect3.FileFormats.Save
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Level : Unreal.ISerializable, INotifyPropertyChanged
    {
        private string _LevelName;
        private bool _ShouldBeLoaded;
        private bool _ShouldBeVisible;

        public void Serialize(Unreal.ISerializer stream)
        {
            stream.Serialize(ref this._LevelName);
            stream.Serialize(ref this._ShouldBeLoaded);
            stream.Serialize(ref this._ShouldBeVisible);
        }

        #region Properties
        public string LevelName
        {
            get { return this._LevelName; }
            set
            {
                if (value != this._LevelName)
                {
                    this._LevelName = value;
                    this.NotifyPropertyChanged("LevelName");
                }
            }
        }

        public bool ShouldBeLoaded
        {
            get { return this._ShouldBeLoaded; }
            set
            {
                if (value != this._ShouldBeLoaded)
                {
                    this._ShouldBeLoaded = value;
                    this.NotifyPropertyChanged("ShouldBeLoaded");
                }
            }
        }

        public bool ShouldBeVisible
        {
            get { return this._ShouldBeVisible; }
            set
            {
                if (value != this._ShouldBeVisible)
                {
                    this._ShouldBeVisible = value;
                    this.NotifyPropertyChanged("ShouldBeVisible");
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