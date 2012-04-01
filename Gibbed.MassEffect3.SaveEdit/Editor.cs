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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ColorPicker;
using Gibbed.IO;
using Gibbed.MassEffect3.SaveEdit.Resources;
using Newtonsoft.Json;

namespace Gibbed.MassEffect3.SaveEdit
{
    public partial class Editor : Form
    {
        public Editor()
        {
            this.InitializeComponent();

            this._SavePath = null;

            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            this.DoubleBuffered = true;
            if (Version.Revision > 0)
            {
                this.Text += String.Format(
                    " (Build revision {0} @ {1})",
                    Version.Revision,
                    Version.Date);
            }
            // ReSharper restore DoNotCallOverridableMethodsInConstructor

            this.wrexPictureBox.Image = Image.FromStream(new MemoryStream(Images.Wrex), true);

            this.LoadDefaultMaleSave();

            bool hasSaveFolder = false;
            var savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (string.IsNullOrEmpty(savePath) == false)
            {
                savePath = Path.Combine(savePath, "BioWare");
                savePath = Path.Combine(savePath, "Mass Effect 3");
                savePath = Path.Combine(savePath, "Save");

                if (Directory.Exists(savePath) == true)
                {
                    this._SavePath = savePath;
                    this.openFileDialog.InitialDirectory = savePath;
                    this.saveFileDialog.InitialDirectory = savePath;
                    hasSaveFolder = true;
                }
            }

            if (hasSaveFolder == false)
            {
                this.dontUseCareerPickerToolStripMenuItem.Checked = true;
                this.dontUseCareerPickerToolStripMenuItem.Enabled = false;
                this.openFromCareerMenuItem.Enabled = false;
                this.saveToCareerMenuItem.Enabled = false;
            }

            var presetPath = Path.Combine(GetExecutablePath(), "presets");
            if (Directory.Exists(presetPath) == true)
            {
                this.openAppearancePresetFileDialog.InitialDirectory = presetPath;
                this.saveAppearancePresetFileDialog.InitialDirectory = presetPath;
            }

            this.SuspendLayout();

            // ReSharper disable LocalizableElement
            this.iconImageList.Images.Add("Unknown", new Bitmap(16, 16));
            // ReSharper restore LocalizableElement

            //this.rootTabControl.SelectedTab = rawRootTabPage;
            this.rawSplitContainer.Panel2Collapsed = true;

            this.AddTable(Localization.Editor_BasicTable_Character_Label, BasicTable.Character.Build(this));
            this.AddTable(Localization.Editor_BasicTable_Reputation_Label, BasicTable.Reputation.Build(this));
            this.AddTable(Localization.Editor_BasicTable_Resources_Label, BasicTable.Resources.Build(this));

            this.AddPlotEditors();

            this.ResumeLayout();
        }

        private void AddTable(string name, List<BasicTable.TableItem> items)
        {
            int row = 0;

            var panel = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = items.Count
            };

            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
            // ReSharper disable ForCanBeConvertedToForeach
            for (int i = 0; i < items.Count; i++) // ReSharper restore ForCanBeConvertedToForeach
            {
                panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            foreach (var item in items)
            {
                panel.Controls.Add(item.Control);
                panel.SetRow(item.Control, row);
                panel.SetColumn(item.Control, 1);

                if (string.IsNullOrEmpty(item.Name) == false)
                {
                    var label = new Label()
                    {
                        Text = string.Format(Localization.Editor_BasicTable_ItemLabelFormat, item.Name),
                        Dock = DockStyle.Fill,
                        AutoSize = true,
                        TextAlign = ContentAlignment.MiddleRight,
                    };
                    panel.Controls.Add(label);
                    panel.SetRow(label, row);
                    panel.SetColumn(label, 0);
                }
                else
                {
                    panel.SetColumnSpan(item.Control, 2);
                }

                if (item.Binding != null)
                {
                    item.Control.DataBindings.Add(item.Binding);
                }

                row++;
            }

            panel.AutoSize = true;
            panel.Dock = DockStyle.Fill;

            var group = new GroupBox();
            group.Text = name;
            group.MinimumSize = new Size(320, 0);
            group.AutoSize = true;
            group.Controls.Add(panel);

            this.playerBasicPanel.Controls.Add(group);
        }

        private readonly List<CheckedListBox> _PlotBools = new List<CheckedListBox>();
        private readonly List<NumericUpDown> _PlotInts = new List<NumericUpDown>();

        private void AddPlotEditors()
        {
            var plotPath = Path.Combine(GetExecutablePath(), "plots");
            if (Directory.Exists(plotPath) == false)
            {
                return;
            }

            var containers = new List<PlotCategoryContainer>();
            foreach (var inputPath in Directory.GetFiles(plotPath, "*.me3plot", SearchOption.AllDirectories))
            {
                try
                {
                    string text;
                    using (var input = File.OpenRead(inputPath))
                    {
                        var reader = new StreamReader(input);
                        text = reader.ReadToEnd();
                    }

                    var settings = new JsonSerializerSettings()
                    {
                        MissingMemberHandling = MissingMemberHandling.Error,
                    };

                    var cat = JsonConvert.DeserializeObject<PlotCategoryContainer>(text, settings);
                    containers.Add(cat);
                }
                catch (Exception e)
                {
                    MessageBox.Show(
                        string.Format(Localization.Editor_PlotCategoryLoadError, inputPath, e),
                        Localization.Error,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            var consumedBools = new List<int>();
            var consumedInts = new List<int>();
            var consumedFloats = new List<int>();

            foreach (var container in containers.OrderBy(c => c.Order))
            {
                var tabs = new List<TabPage>();

                foreach (var category in container.Categories.OrderBy(c => c.Order))
                {
                    int count = 0;
                    count += string.IsNullOrEmpty(category.Note) == false ? 1 : 0;
                    count += category.Bools.Any() ? 1 : 0;
                    count += category.Ints.Any() ? 1 : 0;
                    count += category.Floats.Any() ? 1 : 0;

                    if (count == 0)
                    {
                        continue;
                    }

                    var categoryTabPage = new TabPage()
                    {
                        Text = category.Name,
                        UseVisualStyleBackColor = true,
                    };
                    tabs.Add(categoryTabPage);

                    Control boolControl = null;
                    Control intControl = null;
                    Control floatControl = null;

                    if (count > 1)
                    {
                        var categoryTabControl = new TabControl()
                        {
                            Dock = DockStyle.Fill,
                        };
                        categoryTabPage.Controls.Add(categoryTabControl);

                        if (string.IsNullOrEmpty(category.Note) == false)
                        {
                            var tabPage = new TabPage()
                            {
                                Text = Localization.Editor_PlotEditor_NoteLabel,
                                UseVisualStyleBackColor = true,
                            };
                            categoryTabControl.Controls.Add(tabPage);

                            var textBox = new TextBox()
                            {
                                Dock = DockStyle.Fill,
                                Multiline = true,
                                Text = category.Note.Trim(),
                                ReadOnly = true,
                                BackColor = SystemColors.Window,
                            };
                            tabPage.Controls.Add(textBox);
                        }

                        if (category.Bools.Count > 0)
                        {
                            var tabPage = new TabPage()
                            {
                                Text = Localization.Editor_PlotEditor_BoolsLabel,
                                UseVisualStyleBackColor = true,
                            };
                            categoryTabControl.Controls.Add(tabPage);

                            boolControl = tabPage;
                        }

                        if (category.Ints.Count > 0)
                        {
                            var tabPage = new TabPage()
                            {
                                Text = Localization.Editor_PlotEditor_IntsLabel,
                                UseVisualStyleBackColor = true,
                            };
                            categoryTabControl.Controls.Add(tabPage);

                            intControl = tabPage;
                        }

                        if (category.Floats.Count > 0)
                        {
                            var tabPage = new TabPage()
                            {
                                Text = Localization.Editor_PlotEditor_FloatsLabel,
                                UseVisualStyleBackColor = true,
                            };
                            categoryTabControl.Controls.Add(tabPage);

                            floatControl = tabPage;
                        }
                    }
                    else
                    {
                        boolControl = categoryTabPage;
                        intControl = categoryTabPage;
                        floatControl = categoryTabPage;
                    }

                    if (category.Bools.Count > 0)
                    {
                        var listBox = new CheckedListBox()
                        {
                            Dock = DockStyle.Fill,
                            MultiColumn = category.MultilineBools,
                            ColumnWidth = 225,
                            Sorted = true,
                            IntegralHeight = false,
                        };
                        listBox.ItemCheck += this.OnPlotBoolChecked;
                        // ReSharper disable PossibleNullReferenceException
                        boolControl.Controls.Add(listBox);
                        // ReSharper restore PossibleNullReferenceException

                        foreach (var plot in category.Bools)
                        {
                            if (consumedBools.Contains(plot.Id) == true)
                            {
                                throw new FormatException(string.Format("bool ID {0} already added", plot.Id));
                            }
                            consumedBools.Add(plot.Id);

                            listBox.Items.Add(plot);
                        }

                        this._PlotBools.Add(listBox);
                    }

                    if (category.Ints.Count > 0)
                    {
                        var panel = new TableLayoutPanel()
                        {
                            Dock = DockStyle.Fill,
                            ColumnCount = 2,
                            RowCount = category.Ints.Count + 1,
                        };

                        panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

                        foreach (var plot in category.Ints)
                        {
                            if (consumedInts.Contains(plot.Id) == true)
                            {
                                throw new FormatException(string.Format("int ID {0} already added", plot.Id));
                            }
                            consumedInts.Add(plot.Id);

                            var label = new Label
                            {
                                Text = string.Format(Localization.Editor_PlotEditor_ValueLabelFormat, plot.Name),
                                Dock = DockStyle.Fill,
                                AutoSize = true,
                                TextAlign = ContentAlignment.MiddleRight,
                            };
                            panel.Controls.Add(label);

                            var numericUpDown = new NumericUpDown()
                            {
                                Minimum = int.MinValue,
                                Maximum = int.MaxValue,
                                Increment = 1,
                                Tag = plot,
                            };
                            numericUpDown.ValueChanged += this.OnPlotIntValueChanged;
                            panel.Controls.Add(numericUpDown);

                            this._PlotInts.Add(numericUpDown);
                        }

                        // ReSharper disable PossibleNullReferenceException
                        intControl.Controls.Add(panel);
                        // ReSharper restore PossibleNullReferenceException
                    }
                }

                if (tabs.Any() == false)
                {
                    continue;
                }

                //if (tabs.Count > 1)
                {
                    var containerTabPage = new TabPage()
                    {
                        Text = container.Name,
                        UseVisualStyleBackColor = true,
                    };
                    this.plotTabControl.TabPages.Add(containerTabPage);

                    var containerTabControl = new TabControl()
                    {
                        Dock = DockStyle.Fill,
                    };
                    containerTabPage.Controls.Add(containerTabControl);

                    containerTabControl.TabPages.AddRange(tabs.ToArray());
                }
                /*else
                {
                    this.plotTabControl.TabPages.Add(tabs.First());
                }*/
            }
        }

        private void UpdatePlotEditors()
        {
            foreach (var list in this._PlotBools)
            {
                for (int i = 0; i < list.Items.Count; i++)
                {
                    var plot = list.Items[i] as PlotBool;
                    if (plot == null)
                    {
                        continue;
                    }

                    var value = this.SaveFile.Plot.GetBoolVariable(plot.Id);
                    list.SetItemChecked(i, value);
                }
            }

            foreach (var numericUpDown in this._PlotInts)
            {
                var plot = numericUpDown.Tag as PlotInt;
                if (plot == null)
                {
                    continue;
                }

                numericUpDown.Value = this.SaveFile.Plot.GetIntVariable(plot.Id);
            }
        }

        private void OnPlotBoolChecked(object sender, ItemCheckEventArgs e)
        {
            var list = sender as CheckedListBox;

            if (list == null)
            {
                e.NewValue = e.CurrentValue;
                return;
            }

            var plot = list.Items[e.Index] as PlotBool;

            if (plot == null)
            {
                e.NewValue = e.CurrentValue;
                return;
            }

            this.SaveFile.Plot.SetBoolVariable(plot.Id, e.NewValue == CheckState.Checked);
        }

        private void OnPlotIntValueChanged(object sender, EventArgs e)
        {
            var numericUpDown = sender as NumericUpDown;

            if (numericUpDown == null)
            {
                return;
            }

            var plot = numericUpDown.Tag as PlotInt;
            if (plot == null)
            {
                return;
            }

            this.SaveFile.Plot.SetIntVariable(plot.Id, (int)numericUpDown.Value);
        }

        private static string GetExecutablePath()
        {
            return Path.GetDirectoryName(Application.ExecutablePath);
        }

        private void LoadDefaultMaleSave()
        {
            using (var memory = new MemoryStream(Properties.Resources.DefaultMaleSave))
            {
                this.LoadSaveFromStream(memory);
                this.SaveFile.Player.Guid = Guid.NewGuid();
            }
        }

        private void LoadDefaultFemaleSave()
        {
            using (var memory = new MemoryStream(Properties.Resources.DefaultFemaleSave))
            {
                this.LoadSaveFromStream(memory);
                this.SaveFile.Player.Guid = Guid.NewGuid();
            }
        }

        private readonly string _SavePath;
        private FileFormats.SFXSaveGameFile _SaveFile;

        private FileFormats.SFXSaveGameFile SaveFile
        {
            get { return this._SaveFile; }
            set
            {
                if (this._SaveFile != value)
                {
                    if (this._SaveFile != null)
                    {
                        this._SaveFile.Player.PropertyChanged -= this.OnPlayerPropertyChanged;
                        this._SaveFile.Player.Appearance.PropertyChanged -= this.OnPlayerAppearancePropertyChanged;
                    }

                    var oldValue = this._SaveFile;
                    this._SaveFile = value;

                    if (this._SaveFile != null)
                    {
                        this._SaveFile.Player.PropertyChanged += this.OnPlayerPropertyChanged;
                        this._SaveFile.Player.Appearance.PropertyChanged += this.OnPlayerAppearancePropertyChanged;

                        this.rawParentPropertyGrid.SelectedObject = value;
                        this.saveFileBindingSource.DataSource = value;
                        this.vectorParametersBindingSource.DataSource =
                            value.Player.Appearance.MorphHead.VectorParameters;

                        this.playerRootTabPage.ImageKey =
                            this._SaveFile.Player.IsFemale == false
                                ? "Tab_Player_Root_Male"
                                : "Tab_Player_Root_Female";
                    }
                }
            }
        }

        private void OnPlayerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // goofy?

            if (e.PropertyName == "IsFemale")
            {
                this.playerRootTabPage.ImageKey =
                    (this._SaveFile == null || this._SaveFile.Player.IsFemale == false)
                        ? "Tab_Player_Root_Male"
                        : "Tab_Player_Root_Female";
            }
        }

        private void OnPlayerAppearancePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MorphHead")
            {
                this.vectorParametersBindingSource.DataSource =
                    this._SaveFile.Player.Appearance.MorphHead.VectorParameters;
            }
        }

        private void LoadSaveFromStream(Stream stream)
        {
            if (stream.ReadValueU32(Endian.Big) == 0x434F4E20) // 'CON '
            {
                MessageBox.Show(Localization.Editor_CannotLoadXbox360CONFile,
                                Localization.Error,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }
            stream.Seek(-4, SeekOrigin.Current);

            FileFormats.SFXSaveGameFile saveFile;
            try
            {
                saveFile = FileFormats.SFXSaveGameFile.Read(stream);
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format(CultureInfo.InvariantCulture, Localization.Editor_SaveReadException, e),
                                Localization.Error,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            if (saveFile.Version < 59)
            {
                MessageBox.Show(
                    Localization.Editor_SaveFileTooOld,
                    Localization.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            this.SaveFile = saveFile;
            this.UpdatePlotEditors();
        }

        private void OnSaveNewMale(object sender, EventArgs e)
        {
            this.LoadDefaultMaleSave();
        }

        private void OnSaveNewFemale(object sender, EventArgs e)
        {
            this.LoadDefaultFemaleSave();
        }

        private void OnSaveOpenFromGeneric(object sender, EventArgs e)
        {
            if (this.dontUseCareerPickerToolStripMenuItem.Checked == false)
            {
                this.OnSaveOpenFromCareer(sender, e);
            }
            else
            {
                this.OnSaveOpenFromFile(sender, e);
            }
        }

        private void OnSaveOpenFromCareer(object sender, EventArgs e)
        {
            using (var picker = new SavePicker())
            {
                picker.Owner = this;
                picker.FileMode = SavePicker.PickerMode.Load;
                picker.FilePath = this._SavePath;

                var result = picker.ShowDialog();
                if (result != DialogResult.OK)
                {
                    return;
                }

                if (string.IsNullOrEmpty(picker.SelectedPath) == true)
                {
                    MessageBox.Show(
                        Localization.Editor_ThisShouldNeverHappen,
                        Localization.Error,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                using (var input = File.OpenRead(picker.SelectedPath))
                {
                    this.LoadSaveFromStream(input);
                }
            }
        }

        private void OnSaveOpenFromFile(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var dir = Path.GetDirectoryName(this.openFileDialog.FileName);
            this.openFileDialog.InitialDirectory = dir;

            using (var input = this.openFileDialog.OpenFile())
            {
                this.LoadSaveFromStream(input);
            }
        }

        private void OnSaveSaveToGeneric(object sender, EventArgs e)
        {
            if (this.dontUseCareerPickerToolStripMenuItem.Checked == false)
            {
                this.OnSaveSaveToCareer(sender, e);
            }
            else
            {
                this.OnSaveSaveToFile(sender, e);
            }
        }

        private void OnSaveSaveToFile(object sender, EventArgs e)
        {
            if (this.SaveFile == null)
            {
                MessageBox.Show(
                    Localization.Editor_NoActiveSave,
                    Localization.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            this.saveFileDialog.FilterIndex =
                this.SaveFile.Endian == Endian.Little ? 1 : 2;
            if (this.saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var dir = Path.GetDirectoryName(this.saveFileDialog.FileName);
            this.saveFileDialog.InitialDirectory = dir;

            this.SaveFile.Endian = this.saveFileDialog.FilterIndex != 2
                                       ? Endian.Little
                                       : Endian.Big;
            using (var output = this.saveFileDialog.OpenFile())
            {
                FileFormats.SFXSaveGameFile.Write(this.SaveFile, output);
            }
        }

        private void OnSaveSaveToCareer(object sender, EventArgs e)
        {
            if (this.SaveFile == null)
            {
                MessageBox.Show(
                    Localization.Editor_NoActiveSave,
                    Localization.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            using (var picker = new SavePicker())
            {
                picker.Owner = this;
                picker.FileMode = SavePicker.PickerMode.Save;
                picker.FilePath = this._SavePath;
                picker.SaveFile = this.SaveFile;

                var result = picker.ShowDialog();
                if (result != DialogResult.OK)
                {
                    return;
                }

                if (string.IsNullOrEmpty(picker.SelectedPath) == false)
                {
                    var selectedDirectory = Path.GetDirectoryName(picker.SelectedPath);
                    if (selectedDirectory != null)
                    {
                        Directory.CreateDirectory(selectedDirectory);
                    }

                    using (var output = File.Create(picker.SelectedPath))
                    {
                        FileFormats.SFXSaveGameFile.Write(this.SaveFile, output);
                    }
                }
                else
                {
                    MessageBox.Show(
                        Localization.Editor_ThisShouldNeverHappen,
                        Localization.Error,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void OnSelectedGridItemChanged(
            object sender, SelectedGridItemChangedEventArgs e)
        {
            if (e.OldSelection != null)
            {
                var oldPc = e.OldSelection.Value as INotifyPropertyChanged;
                if (oldPc != null)
                {
                    oldPc.PropertyChanged -= this.OnPropertyChanged;
                }
            }

            if (e.NewSelection != null)
            {
                if ((e.NewSelection.Value is FileFormats.Unreal.ISerializable) == true)
                {
                    this.rawChildPropertyGrid.SelectedObject = e.NewSelection.Value;
                    this.rawSplitContainer.Panel2Collapsed = false;

                    var newPc = e.NewSelection.Value as INotifyPropertyChanged;
                    if (newPc != null)
                    {
                        newPc.PropertyChanged += this.OnPropertyChanged;
                    }

                    return;
                }
            }

            this.rawChildPropertyGrid.SelectedObject = null;
            this.rawSplitContainer.Panel2Collapsed = true;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.rawParentPropertyGrid.Refresh();
        }

        private const string HeadMorphMagic = "GIBBEDMASSEFFECT3HEADMORPH";

        private void OnImportHeadMorph(object sender, EventArgs e)
        {
            if (this.SaveFile == null)
            {
                MessageBox.Show(
                    Localization.Editor_NoActiveSave,
                    Localization.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (this.openHeadMorphDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var dir = Path.GetDirectoryName(this.openHeadMorphDialog.FileName);
            this.openHeadMorphDialog.InitialDirectory = dir;

            using (var input = this.openHeadMorphDialog.OpenFile())
            {
                if (input.ReadString(HeadMorphMagic.Length, Encoding.ASCII) != HeadMorphMagic)
                {
                    MessageBox.Show(
                        Localization.Editor_HeadMorphInvalid,
                        Localization.Error,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    input.Close();
                    return;
                }

                if (input.ReadValueU8() != 0)
                {
                    MessageBox.Show(
                        Localization.Editor_HeadMorphVersionUnsupported,
                        Localization.Error,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    input.Close();
                    return;
                }

                uint version = input.ReadValueU32();

                if (version != this.SaveFile.Version)
                {
                    if (MessageBox.Show(
                        string.Format(
                            Localization.Editor_HeadMorphVersionMaybeIncompatible,
                            version,
                            this.SaveFile.Version),
                        Localization.Question,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.No)
                    {
                        input.Close();
                        return;
                    }
                }

                var reader = new FileFormats.Unreal.FileReader(
                    input, version, Endian.Little);
                var morphHead = new FileFormats.Save.MorphHead();
                morphHead.Serialize(reader);
                this.SaveFile.Player.Appearance.MorphHead = morphHead;
                this.SaveFile.Player.Appearance.HasMorphHead = true;
            }
        }

        private void OnExportHeadMorph(object sender, EventArgs e)
        {
            if (this.SaveFile == null)
            {
                MessageBox.Show(
                    Localization.Editor_NoActiveSave,
                    Localization.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (this.SaveFile.Player.Appearance.HasMorphHead == false)
            {
                MessageBox.Show(
                    Localization.Editor_NoHeadMorph,
                    Localization.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (this.saveHeadMorphDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var dir = Path.GetDirectoryName(this.saveHeadMorphDialog.FileName);
            this.saveHeadMorphDialog.InitialDirectory = dir;

            using (var output = this.saveHeadMorphDialog.OpenFile())
            {
                output.WriteString(HeadMorphMagic, Encoding.ASCII);
                output.WriteValueU8(0); // "version" in case I break something in the future
                output.WriteValueU32(this.SaveFile.Version);
                var writer = new FileFormats.Unreal.FileWriter(
                    output, this.SaveFile.Version, Endian.Little);
                this.SaveFile.Player.Appearance.MorphHead.Serialize(writer);
            }
        }

        private const string HeadMorphMagicLegacy = "GIBBEDMASSEFFECT2HEADMORPH";

        private void OnImportHeadMorphLegacy(object sender, EventArgs e)
        {
            if (this.SaveFile == null)
            {
                MessageBox.Show(
                    Localization.Editor_NoActiveSave,
                    Localization.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show(
                Localization.Editor_HeadMorphLegacy,
                Localization.Warning,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            if (this.openHeadMorphLegacyDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var dir = Path.GetDirectoryName(this.openHeadMorphLegacyDialog.FileName);
            this.openHeadMorphLegacyDialog.InitialDirectory = dir;

            using (var input = this.openHeadMorphLegacyDialog.OpenFile())
            {
                if (input.ReadString(HeadMorphMagicLegacy.Length, Encoding.ASCII) != HeadMorphMagicLegacy)
                {
                    MessageBox.Show(
                        Localization.Editor_HeadMorphInvalid,
                        Localization.Error,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    input.Close();
                    return;
                }

                if (input.ReadValueU8() != 0)
                {
                    MessageBox.Show(
                        Localization.Editor_HeadMorphVersionUnsupported,
                        Localization.Error,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    input.Close();
                    return;
                }

                uint version = input.ReadValueU32();

                if (version != 29)
                {
                    if (MessageBox.Show(
                        string.Format(
                            Localization.Editor_HeadMorphVersionMaybeIncompatible,
                            version,
                            this.SaveFile.Version),
                        Localization.Question,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.No)
                    {
                        input.Close();
                        return;
                    }
                }

                var reader = new FileFormats.Unreal.FileReader(
                    input, version, Endian.Little);
                var morphHead = new FileFormats.Save.MorphHead();
                morphHead.Serialize(reader);
                this.SaveFile.Player.Appearance.MorphHead = morphHead;
                this.SaveFile.Player.Appearance.HasMorphHead = true;
            }
        }

        private void AppendToPlotManualLog(string format, params object[] args)
        {
            if (string.IsNullOrEmpty(this.plotManualLogTextBox.Text) == false)
            {
                this.plotManualLogTextBox.AppendText(Environment.NewLine);
            }
            this.plotManualLogTextBox.AppendText(
                string.Format(
                    Thread.CurrentThread.CurrentCulture,
                    format,
                    args));
        }

        private void OnPlotManualGetBool(object sender, EventArgs e)
        {
            if (this.SaveFile == null)
            {
                MessageBox.Show(
                    Localization.Editor_NoActiveSave,
                    Localization.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            int id;
            if (int.TryParse(
                this.plotManualBoolIdTextBox.Text,
                NumberStyles.None,
                Thread.CurrentThread.CurrentCulture,
                out id) == false)
            {
                MessageBox.Show(Localization.Editor_FailedToParseId,
                                Localization.Error,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            var value = this.SaveFile.Plot.GetBoolVariable(id);
            this.AppendToPlotManualLog(Localization.Editor_PlotManualLogBoolGet,
                                       id,
                                       value);
            this.plotManualBoolValueCheckBox.Checked = value;
        }

        private void OnPlotManualSetBool(object sender, EventArgs e)
        {
            if (this.SaveFile == null)
            {
                MessageBox.Show(
                    Localization.Editor_NoActiveSave,
                    Localization.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            int id;
            if (int.TryParse(
                this.plotManualBoolIdTextBox.Text,
                NumberStyles.None,
                Thread.CurrentThread.CurrentCulture,
                out id) == false)
            {
                MessageBox.Show(Localization.Editor_FailedToParseId,
                                Localization.Error,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            var newValue = this.plotManualBoolValueCheckBox.Checked;
            var oldValue = this.SaveFile.Plot.GetBoolVariable(id);
            this.SaveFile.Plot.SetBoolVariable(id, newValue);
            this.AppendToPlotManualLog(Localization.Editor_PlotManualLogBoolSet,
                                       id,
                                       newValue,
                                       oldValue);
        }

        private void OnPlotManualGetInt(object sender, EventArgs e)
        {
            if (this.SaveFile == null)
            {
                MessageBox.Show(
                    Localization.Editor_NoActiveSave,
                    Localization.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            int id;
            if (int.TryParse(
                this.plotManualIntIdTextBox.Text,
                NumberStyles.None,
                Thread.CurrentThread.CurrentCulture,
                out id) == false)
            {
                MessageBox.Show(Localization.Editor_FailedToParseId,
                                Localization.Error,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            var value = this.SaveFile.Plot.GetIntVariable(id);
            this.AppendToPlotManualLog(Localization.Editor_PlotManualLogIntGet,
                                       id,
                                       value);
            this.plotManualIntValueTextBox.Text =
                value.ToString(Thread.CurrentThread.CurrentCulture);
        }

        private void OnPlotManualSetInt(object sender, EventArgs e)
        {
            if (this.SaveFile == null)
            {
                MessageBox.Show(
                    Localization.Editor_NoActiveSave,
                    Localization.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            int id;
            if (int.TryParse(
                this.plotManualIntIdTextBox.Text,
                NumberStyles.None,
                Thread.CurrentThread.CurrentCulture,
                out id) == false)
            {
                MessageBox.Show(Localization.Editor_FailedToParseId,
                                Localization.Error,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            int newValue;
            if (int.TryParse(
                this.plotManualIntValueTextBox.Text,
                NumberStyles.None,
                Thread.CurrentThread.CurrentCulture,
                out newValue) == false)
            {
                MessageBox.Show(Localization.Editor_FailedToParseValue,
                                Localization.Error,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            var oldValue = this.SaveFile.Plot.GetIntVariable(id);
            this.SaveFile.Plot.SetIntVariable(id, newValue);
            this.AppendToPlotManualLog(Localization.Editor_PlotManualLogIntSet,
                                       id,
                                       newValue,
                                       oldValue);
        }

        private void OnPlotManualGetFloat(object sender, EventArgs e)
        {
            if (this.SaveFile == null)
            {
                MessageBox.Show(
                    Localization.Editor_NoActiveSave,
                    Localization.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            int id;
            if (int.TryParse(
                this.plotManualFloatIdTextBox.Text,
                NumberStyles.None,
                Thread.CurrentThread.CurrentCulture,
                out id) == false)
            {
                MessageBox.Show(Localization.Editor_FailedToParseId,
                                Localization.Error,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            var value = this.SaveFile.Plot.GetFloatVariable(id);
            this.AppendToPlotManualLog(Localization.Editor_PlotManualLogFloatGet,
                                       id,
                                       value);
            this.plotManualFloatValueTextBox.Text =
                value.ToString(Thread.CurrentThread.CurrentCulture);
        }

        private void OnPlotManualSetFloat(object sender, EventArgs e)
        {
            if (this.SaveFile == null)
            {
                MessageBox.Show(
                    Localization.Editor_NoActiveSave,
                    Localization.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            int id;
            if (int.TryParse(
                this.plotManualFloatIdTextBox.Text,
                NumberStyles.None,
                Thread.CurrentThread.CurrentCulture,
                out id) == false)
            {
                MessageBox.Show(Localization.Editor_FailedToParseId,
                                Localization.Error,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            float newValue;
            if (float.TryParse(
                this.plotManualIntValueTextBox.Text,
                NumberStyles.None,
                Thread.CurrentThread.CurrentCulture,
                out newValue) == false)
            {
                MessageBox.Show(Localization.Editor_FailedToParseValue,
                                Localization.Error,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            var oldValue = this.SaveFile.Plot.GetFloatVariable(id);
            this.SaveFile.Plot.SetFloatVariable(id, newValue);
            this.AppendToPlotManualLog(Localization.Editor_PlotManualLogFloatSet,
                                       id,
                                       newValue,
                                       oldValue);
        }

        private void OnPlotManualClearLog(object sender, EventArgs e)
        {
            this.plotManualLogTextBox.Clear();
        }

        private void OnLinkFaq(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://code.google.com/p/me3tools/wiki/FAQ");
        }

        private void OnLinkIssues(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://code.google.com/p/me3tools/wiki/IssuesNotice?tm=3");
        }

        private static void ApplyAppearancePreset(FileFormats.Save.MorphHead morphHead,
                                                  AppearancePreset preset)
        {
            if (morphHead == null)
            {
                throw new ArgumentNullException("morphHead");
            }

            if (preset == null)
            {
                throw new ArgumentNullException("preset");
            }

            if (string.IsNullOrEmpty(preset.HairMesh) == false)
            {
                morphHead.HairMesh = preset.HairMesh;
            }

            if (preset.Scalars != null)
            {
                if (preset.Scalars.Clear == true)
                {
                    morphHead.ScalarParameters.Clear();
                }

                if (preset.Scalars.Remove != null)
                {
                    foreach (var scalar in preset.Scalars.Remove)
                    {
                        morphHead.ScalarParameters.RemoveAll(
                            p => string.Compare(p.Name, scalar, StringComparison.InvariantCultureIgnoreCase) == 0);
                    }
                }

                if (preset.Scalars.Add != null)
                {
                    foreach (var scalar in preset.Scalars.Add)
                    {
                        morphHead.ScalarParameters.Add(
                            new FileFormats.Save.MorphHead.ScalarParameter()
                            {
                                Name = scalar.Key,
                                Value = scalar.Value,
                            });
                    }
                }

                if (preset.Scalars.Set != null)
                {
                    foreach (var scalar in preset.Scalars.Set)
                    {
                        morphHead.ScalarParameters.RemoveAll(
                            p => string.Compare(p.Name, scalar.Key, StringComparison.InvariantCultureIgnoreCase) == 0);
                        morphHead.ScalarParameters.Add(
                            new FileFormats.Save.MorphHead.ScalarParameter()
                            {
                                Name = scalar.Key,
                                Value = scalar.Value,
                            });
                    }
                }
            }

            if (preset.Textures != null)
            {
                if (preset.Textures.Clear == true)
                {
                    morphHead.TextureParameters.Clear();
                }

                if (preset.Textures.Remove != null)
                {
                    foreach (var texture in preset.Textures.Remove)
                    {
                        morphHead.TextureParameters.RemoveAll(
                            p => string.Compare(p.Name, texture, StringComparison.InvariantCultureIgnoreCase) == 0);
                    }
                }

                if (preset.Textures.Add != null)
                {
                    foreach (var texture in preset.Textures.Add)
                    {
                        morphHead.TextureParameters.Add(
                            new FileFormats.Save.MorphHead.TextureParameter()
                            {
                                Name = texture.Key,
                                Value = texture.Value,
                            });
                    }
                }

                if (preset.Textures.Set != null)
                {
                    foreach (var texture in preset.Textures.Set)
                    {
                        morphHead.TextureParameters.RemoveAll(
                            p => string.Compare(p.Name, texture.Key, StringComparison.InvariantCultureIgnoreCase) == 0);
                        morphHead.TextureParameters.Add(
                            new FileFormats.Save.MorphHead.TextureParameter()
                            {
                                Name = texture.Key,
                                Value = texture.Value,
                            });
                    }
                }
            }

            if (preset.Vectors != null)
            {
                if (preset.Vectors.Clear == true)
                {
                    morphHead.VectorParameters.Clear();
                }

                if (preset.Vectors.Remove != null)
                {
                    foreach (var vector in preset.Vectors.Remove)
                    {
                        string temp = vector;
                        morphHead.VectorParameters.RemoveAll(
                            p => string.Compare(p.Name, temp, StringComparison.InvariantCultureIgnoreCase) == 0);
                    }
                }

                if (preset.Vectors.Add != null)
                {
                    foreach (var vector in preset.Vectors.Add)
                    {
                        morphHead.VectorParameters.Add(
                            new FileFormats.Save.MorphHead.VectorParameter()
                            {
                                Name = vector.Key,
                                Value = new FileFormats.Save.LinearColor()
                                {
                                    R = vector.Value.R,
                                    G = vector.Value.G,
                                    B = vector.Value.B,
                                    A = vector.Value.A,
                                },
                            });
                    }
                }

                if (preset.Vectors.Set != null)
                {
                    foreach (var vector in preset.Vectors.Set)
                    {
                        var temp = vector;
                        morphHead.VectorParameters.RemoveAll(
                            p => string.Compare(p.Name, temp.Key, StringComparison.InvariantCultureIgnoreCase) == 0);
                        morphHead.VectorParameters.Add(
                            new FileFormats.Save.MorphHead.VectorParameter()
                            {
                                Name = vector.Key,
                                Value = new FileFormats.Save.LinearColor()
                                {
                                    R = vector.Value.R,
                                    G = vector.Value.G,
                                    B = vector.Value.B,
                                    A = vector.Value.A,
                                },
                            });
                    }
                }
            }
        }

        private void OnLoadAppearancePresetFromFile(object sender, EventArgs e)
        {
            if (this._SaveFile == null)
            {
                MessageBox.Show(
                    Localization.Editor_NoActiveSave,
                    Localization.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (this._SaveFile.Player.Appearance.HasMorphHead == false)
            {
                MessageBox.Show(
                    Localization.Editor_NoHeadMorph,
                    Localization.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (this.openAppearancePresetFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var dir = Path.GetDirectoryName(this.openAppearancePresetFileDialog.FileName);
            this.openAppearancePresetFileDialog.InitialDirectory = dir;

            string text;
            using (var input = this.openAppearancePresetFileDialog.OpenFile())
            {
                var reader = new StreamReader(input);
                text = reader.ReadToEnd();
            }

            var preset = JsonConvert.DeserializeObject<AppearancePreset>(text);
            ApplyAppearancePreset(this._SaveFile.Player.Appearance.MorphHead, preset);
        }

        private void OnSaveAppearancePresetToFile(object sender, EventArgs e)
        {
            if (this._SaveFile == null)
            {
                MessageBox.Show(
                    Localization.Editor_NoActiveSave,
                    Localization.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (this._SaveFile.Player.Appearance.HasMorphHead == false)
            {
                MessageBox.Show(
                    Localization.Editor_NoHeadMorph,
                    Localization.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (this.saveAppearancePresetFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var dir = Path.GetDirectoryName(this.saveAppearancePresetFileDialog.FileName);
            this.saveAppearancePresetFileDialog.InitialDirectory = dir;

            var headMorph = this.SaveFile.Player.Appearance.MorphHead;

            // ReSharper disable UseObjectOrCollectionInitializer
            var preset = new AppearancePreset();
            // ReSharper restore UseObjectOrCollectionInitializer

            preset.HairMesh = headMorph.HairMesh;

            foreach (var scalar in headMorph.ScalarParameters)
            {
                preset.Scalars.Set.Add(new KeyValuePair<string, float>(scalar.Name, scalar.Value));
            }

            foreach (var texture in headMorph.TextureParameters)
            {
                preset.Textures.Set.Add(new KeyValuePair<string, string>(texture.Name, texture.Value));
            }

            foreach (var vector in headMorph.VectorParameters)
            {
                preset.Vectors.Set.Add(new KeyValuePair<string, AppearancePreset.
                                           LinearColor>(vector.Name,
                                                        new AppearancePreset.
                                                            LinearColor()
                                                        {
                                                            R = vector.Value.R,
                                                            G = vector.Value.G,
                                                            B = vector.Value.B,
                                                            A = vector.Value.A,
                                                        }));
            }

            using (var output = File.Create(this.saveAppearancePresetFileDialog.FileName))
            {
                var writer = new StreamWriter(output);
                writer.Write(JsonConvert.SerializeObject(
                    preset, Formatting.Indented));
                writer.Flush();
            }
        }

        private static ColorBgra LinearColorToBgra(FileFormats.Save.LinearColor linearColor)
        {
            return LinearColorToBgra(
                linearColor.R,
                linearColor.G,
                linearColor.B,
                linearColor.A);
        }

        private static ColorBgra LinearColorToBgra(float r, float g, float b, float a)
        {
            var rb = (byte)Math.Round(r * 255);
            var gb = (byte)Math.Round(g * 255);
            var bb = (byte)Math.Round(b * 255);
            var ab = (byte)Math.Round(a * 255);
            return ColorBgra.FromBgra(bb, gb, rb, ab);
        }

        private static FileFormats.Save.LinearColor BgraToLinearColor(ColorBgra bgra)
        {
            return new FileFormats.Save.LinearColor(
                (float)bgra.R / 255,
                (float)bgra.G / 255,
                (float)bgra.B / 255,
                (float)bgra.A / 255);
        }

        private void OnDrawColorListBoxItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }

            var g = e.Graphics;
            var listbox = (ListBox)sender;

            var backColor = (e.State & DrawItemState.Selected) != 0
                                ? SystemColors.Highlight
                                : listbox.BackColor;
            var foreColor = (e.State & DrawItemState.Selected) != 0
                                ? SystemColors.HighlightText
                                : listbox.ForeColor;

            g.FillRectangle(new SolidBrush(backColor), e.Bounds);

            var colorBounds = e.Bounds;

            colorBounds.Width = 30;
            colorBounds.Height -= 4;
            colorBounds.X += 2;
            colorBounds.Y += 2;

            var textBounds = e.Bounds;
            textBounds.Offset(30, 0);
            textBounds.Inflate(-2, -2);
            var textBoundsF = new RectangleF(textBounds.X, textBounds.Y, textBounds.Width, textBounds.Height);

            g.FillRectangle(
                new HatchBrush(
                    HatchStyle.LargeCheckerBoard,
                    Color.White,
                    Color.Gray),
                colorBounds);

            var item = listbox.Items[e.Index];

            var vector = item as FileFormats.Save.MorphHead.VectorParameter;
            if (vector != null)
            {
                var valueColor = LinearColorToBgra(vector.Value).ToColor();

                g.FillRectangle(new SolidBrush(valueColor), colorBounds);
                g.DrawRectangle(Pens.Black, colorBounds);

                var format = StringFormat.GenericDefault;
                format.LineAlignment = StringAlignment.Center;

                e.Graphics.DrawString(vector.Name,
                                      listbox.Font,
                                      new SolidBrush(foreColor),
                                      textBoundsF,
                                      format);
            }
        }

        private void OnPlayerAppearanceColorRemove(object sender, EventArgs e)
        {
            var item = this.playerAppearanceColorsListBox.SelectedItem as FileFormats.Save.MorphHead.VectorParameter;
            if (item != null)
            {
                this._SaveFile.Player.Appearance.MorphHead.VectorParameters.Remove(item);
            }
        }

        private void OnPlayerAppearanceColorAdd(object sender, EventArgs e)
        {
            var input = new InputBox
            {
                Owner = this,
                Text = Localization.Editor_ColorName,
                InputText = "",
            };

            if (input.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this._SaveFile.Player.Appearance.MorphHead.VectorParameters.Add(
                new FileFormats.Save.MorphHead.VectorParameter()
                {
                    Name = input.InputText,
                    Value = new FileFormats.Save.LinearColor(1, 1, 1, 1),
                });
        }

        private void OnPlayerAppearanceColorChange(object sender, EventArgs e)
        {
            var item = this.playerAppearanceColorsListBox.SelectedItem as FileFormats.Save.MorphHead.VectorParameter;
            if (item != null)
            {
                var bgra = LinearColorToBgra(item.Value);

                // ReSharper disable UseObjectOrCollectionInitializer
                var picker = new ColorPicker.ColorDialog();
                // ReSharper restore UseObjectOrCollectionInitializer
                picker.WheelColor = bgra;

                if (picker.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                item.Value = BgraToLinearColor(picker.WheelColor);
            }
        }

        private void OnRootTabIndexChanged(object sender, EventArgs e)
        {
            if (this.rootTabControl.SelectedTab == this.rawTabPage)
            {
                // HACK: refresh property grids, just in case
                this.rawParentPropertyGrid.Refresh();
                this.rawChildPropertyGrid.Refresh();
            }
        }
    }
}
