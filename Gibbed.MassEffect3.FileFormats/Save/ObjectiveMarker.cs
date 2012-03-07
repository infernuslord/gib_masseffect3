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
    public class ObjectiveMarker : Unreal.ISerializable, INotifyPropertyChanged
    {
        private string _MarkerOwnerPath;
        private Vector _MarkerOffset;
        private int _MarkerLabel;
        private string _BoneToAttachTo;
        private ObjectiveMarkerIconType _MarkerIconType;

        public void Serialize(Unreal.ISerializer stream)
        {
            stream.Serialize(ref this._MarkerOwnerPath);
            stream.Serialize(ref this._MarkerOffset);
            stream.Serialize(ref this._MarkerLabel);
            stream.Serialize(ref this._BoneToAttachTo);
            stream.SerializeEnum(ref this._MarkerIconType);
        }

        #region Properties
        public string MarkerOwnerPath
        {
            get { return this._MarkerOwnerPath; }
            set
            {
                if (value != this._MarkerOwnerPath)
                {
                    this._MarkerOwnerPath = value;
                    this.NotifyPropertyChanged("MarkerOwnerPath");
                }
            }
        }

        public Vector MarkerOffset
        {
            get { return this._MarkerOffset; }
            set
            {
                if (value != this._MarkerOffset)
                {
                    this._MarkerOffset = value;
                    this.NotifyPropertyChanged("MarkerOffset");
                }
            }
        }

        public int MarkerLabel
        {
            get { return this._MarkerLabel; }
            set
            {
                if (value != this._MarkerLabel)
                {
                    this._MarkerLabel = value;
                    this.NotifyPropertyChanged("MarkerLabel");
                }
            }
        }

        public string BoneToAttachTo
        {
            get { return this._BoneToAttachTo; }
            set
            {
                if (value != this._BoneToAttachTo)
                {
                    this._BoneToAttachTo = value;
                    this.NotifyPropertyChanged("BoneToAttachTo");
                }
            }
        }

        public ObjectiveMarkerIconType MarkerIconType
        {
            get { return this._MarkerIconType; }
            set
            {
                if (value != this._MarkerIconType)
                {
                    this._MarkerIconType = value;
                    this.NotifyPropertyChanged("MarkerIconType");
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
