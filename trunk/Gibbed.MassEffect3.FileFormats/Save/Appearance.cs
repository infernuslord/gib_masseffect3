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
    [OriginalName("AppearanceSaveRecord")]
    public class Appearance : Unreal.ISerializable, INotifyPropertyChanged
    {
        #region Fields
        [OriginalName("CombatAppearance")]
        private PlayerAppearanceType _CombatAppearance;

        [OriginalName("CasualID")]
        private int _CasualId;

        [OriginalName("FullBodyID")]
        private int _FullBodyId;

        [OriginalName("TorsoID")]
        private int _TorsoId;

        [OriginalName("ShoulderID")]
        private int _ShoulderId;

        [OriginalName("ArmID")]
        private int _ArmId;

        [OriginalName("LegID")]
        private int _LegId;

        [OriginalName("SpecID")]
        private int _SpecId;

        [OriginalName("Tint1ID")]
        private int _Tint1Id;

        [OriginalName("Tint2ID")]
        private int _Tint2Id;

        [OriginalName("Tint3ID")]
        private int _Tint3Id;

        [OriginalName("PatternID")]
        private int _PatternId;

        [OriginalName("PatternColorID")]
        private int _PatternColorId;

        [OriginalName("HelmetID")]
        private int _HelmetId;

        [OriginalName("bHasMorphHead")]
        private bool _HasMorphHead;

        [OriginalName("MorphHead")]
        private MorphHead _MorphHead = new MorphHead();

        [OriginalName("EmissiveID")]
        private int _EmissiveId;
        #endregion

        public void Serialize(Unreal.ISerializer stream)
        {
            stream.SerializeEnum(ref this._CombatAppearance);
            stream.Serialize(ref this._CasualId);
            stream.Serialize(ref this._FullBodyId);
            stream.Serialize(ref this._TorsoId);
            stream.Serialize(ref this._ShoulderId);
            stream.Serialize(ref this._ArmId);
            stream.Serialize(ref this._LegId);
            stream.Serialize(ref this._SpecId);
            stream.Serialize(ref this._Tint1Id);
            stream.Serialize(ref this._Tint2Id);
            stream.Serialize(ref this._Tint3Id);
            stream.Serialize(ref this._PatternId);
            stream.Serialize(ref this._PatternColorId);
            stream.Serialize(ref this._HelmetId);
            stream.Serialize(ref this._HasMorphHead);

            if (this._HasMorphHead == true)
            {
                stream.Serialize(ref this._MorphHead);
            }

            stream.Serialize(ref this._EmissiveId, s => s.Version < 55, () => 0);
        }

        #region Properties
        [Category("Body")]
        [DisplayName("Combat Appearance")]
        public PlayerAppearanceType CombatAppearance
        {
            get { return this._CombatAppearance; }
            set
            {
                if (value != this._CombatAppearance)
                {
                    this._CombatAppearance = value;
                    this.NotifyPropertyChanged("CombatAppearance");
                }
            }
        }

        [Category("Body")]
        [DisplayName("Casual ID")]
        public int CasualId
        {
            get { return this._CasualId; }
            set
            {
                if (value != this._CasualId)
                {
                    this._CasualId = value;
                    this.NotifyPropertyChanged("CasualId");
                }
            }
        }

        [Category("Body")]
        [DisplayName("Full Body ID")]
        public int FullBodyId
        {
            get { return this._FullBodyId; }
            set
            {
                if (value != this._FullBodyId)
                {
                    this._FullBodyId = value;
                    this.NotifyPropertyChanged("FullBodyId");
                }
            }
        }

        [Category("Body")]
        [DisplayName("Torso ID")]
        public int TorsoId
        {
            get { return this._TorsoId; }
            set
            {
                if (value != this._TorsoId)
                {
                    this._TorsoId = value;
                    this.NotifyPropertyChanged("TorsoId");
                }
            }
        }

        [Category("Body")]
        [DisplayName("Shoulder ID")]
        public int ShoulderId
        {
            get { return this._ShoulderId; }
            set
            {
                if (value != this._ShoulderId)
                {
                    this._ShoulderId = value;
                    this.NotifyPropertyChanged("ShoulderId");
                }
            }
        }

        [Category("Body")]
        [DisplayName("Arm ID")]
        public int ArmId
        {
            get { return this._ArmId; }
            set
            {
                if (value != this._ArmId)
                {
                    this._ArmId = value;
                    this.NotifyPropertyChanged("ArmId");
                }
            }
        }

        [Category("Body")]
        [DisplayName("Leg ID")]
        public int LegId
        {
            get { return this._LegId; }
            set
            {
                if (value != this._LegId)
                {
                    this._LegId = value;
                    this.NotifyPropertyChanged("LegId");
                }
            }
        }

        [Category("Body")]
        [DisplayName("Specular ID")]
        public int SpecId
        {
            get { return this._SpecId; }
            set
            {
                if (value != this._SpecId)
                {
                    this._SpecId = value;
                    this.NotifyPropertyChanged("SpecId");
                }
            }
        }

        [Category("Body")]
        [DisplayName("Tint #1 ID")]
        public int Tint1Id
        {
            get { return this._Tint1Id; }
            set
            {
                if (value != this._Tint1Id)
                {
                    this._Tint1Id = value;
                    this.NotifyPropertyChanged("Tint1Id");
                }
            }
        }

        [Category("Body")]
        [DisplayName("Tint #2 ID")]
        public int Tint2Id
        {
            get { return this._Tint2Id; }
            set
            {
                if (value != this._Tint2Id)
                {
                    this._Tint2Id = value;
                    this.NotifyPropertyChanged("Tint2Id");
                }
            }
        }

        [Category("Body")]
        [DisplayName("Tint #3 ID")]
        public int Tint3Id
        {
            get { return this._Tint3Id; }
            set
            {
                if (value != this._Tint3Id)
                {
                    this._Tint3Id = value;
                    this.NotifyPropertyChanged("Tint3Id");
                }
            }
        }

        [Category("Body")]
        [DisplayName("Pattern ID")]
        public int PatternId
        {
            get { return this._PatternId; }
            set
            {
                if (value != this._PatternId)
                {
                    this._PatternId = value;
                    this.NotifyPropertyChanged("PatternId");
                }
            }
        }

        [Category("Body")]
        [DisplayName("Pattern Color ID")]
        public int PatternColorId
        {
            get { return this._PatternColorId; }
            set
            {
                if (value != this._PatternColorId)
                {
                    this._PatternColorId = value;
                    this.NotifyPropertyChanged("PatternColorId");
                }
            }
        }

        [Category("Body")]
        [DisplayName("Helmet ID")]
        public int HelmetId
        {
            get { return this._HelmetId; }
            set
            {
                if (value != this._HelmetId)
                {
                    this._HelmetId = value;
                    this.NotifyPropertyChanged("HelmetId");
                }
            }
        }

        [Category("Head")]
        [DisplayName("Has Morph Head")]
        public bool HasMorphHead
        {
            get { return this._HasMorphHead; }
            set
            {
                if (value != this._HasMorphHead)
                {
                    this._HasMorphHead = value;
                    this.NotifyPropertyChanged("HasMorphHead");
                }
            }
        }

        [Category("Head")]
        [DisplayName("Morph Head")]
        public MorphHead MorphHead
        {
            get { return this._MorphHead; }
            set
            {
                if (value != this._MorphHead)
                {
                    this._MorphHead = value;
                    this.NotifyPropertyChanged("MorphHead");
                }
            }
        }

        [Category("Body")]
        [DisplayName("Emissive ID")]
        public int EmissiveId
        {
            get { return this._EmissiveId; }
            set
            {
                if (value != this._EmissiveId)
                {
                    this._EmissiveId = value;
                    this.NotifyPropertyChanged("EmissiveId");
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
