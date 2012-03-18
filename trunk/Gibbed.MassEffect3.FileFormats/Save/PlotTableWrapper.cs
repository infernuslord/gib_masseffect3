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

namespace Gibbed.MassEffect3.FileFormats.Save
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PlotTableWrapper : INotifyPropertyChanged, Unreal.ISerializable
    {
        private readonly IPlotTable _Target;

        public PlotTableWrapper(IPlotTable target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            this._Target = target;
        }

        #region Properties
        private const int PersuadeMultiplierId = 10065;

        [DisplayName("Persuade Multiplier")]
        public float PersuadeMultiplier
        {
            get { return this._Target.GetFloatVariable(PersuadeMultiplierId); }
            set
            {
                if (Equals(this._Target.GetFloatVariable(PersuadeMultiplierId), value) == false)
                {
                    this._Target.SetFloatVariable(PersuadeMultiplierId, value);
                    this.NotifyPropertyChanged("PersuadeMultiplier");
                }
            }
        }

        private const int NewGamePlusCountId = 10475;

        [DisplayName("New Game+ Count")]
        public int NewGamePlusCount
        {
            get { return this._Target.GetIntVariable(NewGamePlusCountId); }
            set
            {
                if (this._Target.GetIntVariable(NewGamePlusCountId) != value)
                {
                    this._Target.SetIntVariable(NewGamePlusCountId, value);
                    this.NotifyPropertyChanged("NewGamePlusCount");
                }
            }
        }

        private const int ParagonPointsId = 10159;

        [DisplayName("Paragon Points")]
        public int ParagonPoints
        {
            get { return this._Target.GetIntVariable(ParagonPointsId); }
            set
            {
                if (this._Target.GetIntVariable(ParagonPointsId) != value)
                {
                    this._Target.SetIntVariable(ParagonPointsId, value);
                    this.NotifyPropertyChanged("ParagonPoints");
                }
            }
        }

        private const int RenegadePointsId = 10160;

        [DisplayName("Renegade Points")]
        public int RenegadePoints
        {
            get { return this._Target.GetIntVariable(RenegadePointsId); }
            set
            {
                if (this._Target.GetIntVariable(RenegadePointsId) != value)
                {
                    this._Target.SetIntVariable(RenegadePointsId, value);
                    this.NotifyPropertyChanged("RenegadePoints");
                }
            }
        }

        private const int ReputationId = 10297;

        [DisplayName("Reputation")]
        public int Reputation
        {
            get { return this._Target.GetIntVariable(ReputationId); }
            set
            {
                if (this._Target.GetIntVariable(ReputationId) != value)
                {
                    this._Target.SetIntVariable(ReputationId, value);
                    this.NotifyPropertyChanged("Reputation");
                }
            }
        }

        private const int ExtraMedigelId = 10300;

        [DisplayName("Extra Medigel")]
        public int ExtraMedigel
        {
            get { return this._Target.GetIntVariable(ExtraMedigelId); }
            set
            {
                if (this._Target.GetIntVariable(ExtraMedigelId) != value)
                {
                    this._Target.SetIntVariable(ExtraMedigelId, value);
                    this.NotifyPropertyChanged("ExtraMedigel");
                }
            }
        }

        private const int ReputationPointsId = 10380;

        [DisplayName("Reputation Points")]
        public int ReputationPoints
        {
            get { return this._Target.GetIntVariable(ReputationPointsId); }
            set
            {
                if (this._Target.GetIntVariable(ReputationPointsId) != value)
                {
                    this._Target.SetIntVariable(ReputationPointsId, value);
                    this.NotifyPropertyChanged("ReputationPoints");
                }
            }
        }

        private const int IsME2ImportId = 21554;

        [DisplayName("Is ME2 Import")]
        public bool IsME2Import
        {
            get { return this._Target.GetBoolVariable(IsME2ImportId); }
            set
            {
                if (this._Target.GetBoolVariable(IsME2ImportId) != value)
                {
                    this._Target.SetBoolVariable(IsME2ImportId, value);
                    this.NotifyPropertyChanged("IsME2Import");
                }
            }
        }

        private const int IsME1ImportId = 22226;

        [DisplayName("Is ME1 Import")]
        public bool IsME1Import
        {
            get { return this._Target.GetBoolVariable(IsME1ImportId); }
            set
            {
                if (this._Target.GetBoolVariable(IsME1ImportId) != value)
                {
                    this._Target.SetBoolVariable(IsME1ImportId, value);
                    this.NotifyPropertyChanged("IsME1Import");
                }
            }
        }

        private const int CosmeticSurgeryMe2Id = 5978;

        [DisplayName("Had Cosmetic Surgery in ME2")]
        public bool CosmeticSurgeryMe2
        {
            get { return this._Target.GetBoolVariable(CosmeticSurgeryMe2Id); }
            set
            {
                if (this._Target.GetBoolVariable(CosmeticSurgeryMe2Id) != value)
                {
                    this._Target.SetBoolVariable(CosmeticSurgeryMe2Id, value);
                    this.NotifyPropertyChanged("CosmeticSurgeryMe2");
                }
            }
        }

        private const int CosmeticSurgeryMe3Id = 22642;

        [DisplayName("Had Cosmetic Surgery in ME3")]
        public bool CosmeticSurgeryMe3
        {
            get { return this._Target.GetBoolVariable(CosmeticSurgeryMe3Id); }
            set
            {
                if (this._Target.GetBoolVariable(CosmeticSurgeryMe3Id) != value)
                {
                    this._Target.SetBoolVariable(CosmeticSurgeryMe3Id, value);
                    this.NotifyPropertyChanged("CosmeticSurgeryMe3");
                }
            }
        }
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        // for the propertygrid stuff
        public void Serialize(Unreal.ISerializer stream)
        {
            throw new NotSupportedException();
        }
    }
}
