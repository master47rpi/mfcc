namespace mirada_finanza_control_central
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            panel1 = new Panel();
            labelControlCentral = new Label();
            labelMiradaFinanza = new Label();
            buttonAbout = new Button();
            buttonSettings = new Button();
            buttonExport = new Button();
            buttonAssets = new Button();
            buttonOverview = new Button();
            buttonJournal = new Button();
            buttonEntry = new Button();
            tabControl = new TabControl();
            tabPageOverview = new TabPage();
            labelOverview = new Label();
            tabPageEntry = new TabPage();
            textBoxEntryPostingType = new TextBox();
            textBoxEntryNote = new TextBox();
            labelInputNotes = new Label();
            labelEntryText = new Label();
            labelPostingType = new Label();
            labelSelectedFilename = new Label();
            labelReversalReferenceVoucher = new Label();
            labelReversal = new Label();
            labelCategory = new Label();
            labelAmount = new Label();
            labelVoucherDate = new Label();
            label2 = new Label();
            buttonVoucherPost = new Button();
            labelFilename = new Label();
            pictureBoxEntryPreview = new PictureBox();
            buttonLoadFile = new Button();
            comboBoxReferenceVoucher = new ComboBox();
            checkBoxReversal = new CheckBox();
            textBoxText = new TextBox();
            comboBoxEntryCategory = new ComboBox();
            numericUpDownAmount = new NumericUpDown();
            dateTimePickerVoucherDate = new DateTimePicker();
            tabPageJournal = new TabPage();
            comboBoxJournalFilterMonth = new ComboBox();
            labelJournalFilterMonth = new Label();
            comboBoxJournalFilter = new ComboBox();
            labelJournalFilterYear = new Label();
            textBoxJournalVoucher = new TextBox();
            labelJournalVoucher = new Label();
            textBoxJournalReversalVoucher = new TextBox();
            textBoxJournalReversal = new TextBox();
            textBoxJournalPostingType = new TextBox();
            textBoxJournalCategory = new TextBox();
            textBoxJournalPostingText = new TextBox();
            textBoxJournalAmount = new TextBox();
            textBoxJournalVoucherDate = new TextBox();
            labelJournalReversalVoucher = new Label();
            labelJournalReversal = new Label();
            labelJournalPostingType = new Label();
            labelJournalCategory = new Label();
            labelJournalPostingText = new Label();
            labelJournalAmount = new Label();
            labelJournalVoucherDate = new Label();
            labelJournalNote = new Label();
            textBoxJournalNote = new TextBox();
            textBoxJournalCreationDate = new TextBox();
            label1 = new Label();
            buttonJournalPicturePDF = new Button();
            labelTransactionDetail = new Label();
            labelJournal = new Label();
            dataGridViewJournal = new DataGridView();
            tabPageAssets = new TabPage();
            tabPageDataexport = new TabPage();
            tabPageSettings = new TabPage();
            tabPageAbout = new TabPage();
            buttonItemEntry = new Button();
            buttonItems = new Button();
            buttonInvoiceEntry = new Button();
            buttonInvoices = new Button();
            buttonCustomerEntry = new Button();
            buttonCustomers = new Button();
            panel1.SuspendLayout();
            tabControl.SuspendLayout();
            tabPageOverview.SuspendLayout();
            tabPageEntry.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxEntryPreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownAmount).BeginInit();
            tabPageJournal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewJournal).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(buttonCustomers);
            panel1.Controls.Add(buttonInvoices);
            panel1.Controls.Add(buttonInvoiceEntry);
            panel1.Controls.Add(buttonCustomerEntry);
            panel1.Controls.Add(buttonItems);
            panel1.Controls.Add(buttonItemEntry);
            panel1.Controls.Add(labelControlCentral);
            panel1.Controls.Add(labelMiradaFinanza);
            panel1.Controls.Add(buttonAbout);
            panel1.Controls.Add(buttonSettings);
            panel1.Controls.Add(buttonExport);
            panel1.Controls.Add(buttonAssets);
            panel1.Controls.Add(buttonOverview);
            panel1.Controls.Add(buttonJournal);
            panel1.Controls.Add(buttonEntry);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(200, 644);
            panel1.TabIndex = 0;
            // 
            // labelControlCentral
            // 
            labelControlCentral.AutoSize = true;
            labelControlCentral.Font = new Font("Segoe Print", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelControlCentral.ForeColor = Color.MediumPurple;
            labelControlCentral.Location = new Point(40, 24);
            labelControlCentral.Name = "labelControlCentral";
            labelControlCentral.Size = new Size(137, 28);
            labelControlCentral.TabIndex = 8;
            labelControlCentral.Text = "Control Central";
            // 
            // labelMiradaFinanza
            // 
            labelMiradaFinanza.AutoSize = true;
            labelMiradaFinanza.Font = new Font("Segoe Script", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelMiradaFinanza.ForeColor = Color.BlueViolet;
            labelMiradaFinanza.Location = new Point(0, 0);
            labelMiradaFinanza.Name = "labelMiradaFinanza";
            labelMiradaFinanza.Size = new Size(180, 31);
            labelMiradaFinanza.TabIndex = 7;
            labelMiradaFinanza.Text = "Mirada Finanza";
            // 
            // buttonAbout
            // 
            buttonAbout.BackColor = Color.DarkSlateBlue;
            buttonAbout.FlatAppearance.BorderSize = 0;
            buttonAbout.FlatStyle = FlatStyle.Flat;
            buttonAbout.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonAbout.ForeColor = Color.White;
            buttonAbout.Location = new Point(0, 608);
            buttonAbout.Name = "buttonAbout";
            buttonAbout.Size = new Size(200, 32);
            buttonAbout.TabIndex = 6;
            buttonAbout.Text = "Über...";
            buttonAbout.UseVisualStyleBackColor = false;
            buttonAbout.Click += buttonAbout_Click;
            // 
            // buttonSettings
            // 
            buttonSettings.BackColor = Color.DarkSlateBlue;
            buttonSettings.FlatAppearance.BorderSize = 0;
            buttonSettings.FlatStyle = FlatStyle.Flat;
            buttonSettings.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonSettings.ForeColor = Color.White;
            buttonSettings.Location = new Point(0, 568);
            buttonSettings.Name = "buttonSettings";
            buttonSettings.Size = new Size(200, 32);
            buttonSettings.TabIndex = 5;
            buttonSettings.Text = "Einstellungen";
            buttonSettings.UseVisualStyleBackColor = false;
            buttonSettings.Click += buttonSettings_Click;
            // 
            // buttonExport
            // 
            buttonExport.BackColor = Color.DarkSlateBlue;
            buttonExport.FlatAppearance.BorderSize = 0;
            buttonExport.FlatStyle = FlatStyle.Flat;
            buttonExport.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonExport.ForeColor = Color.White;
            buttonExport.Location = new Point(0, 528);
            buttonExport.Name = "buttonExport";
            buttonExport.Size = new Size(200, 32);
            buttonExport.TabIndex = 4;
            buttonExport.Text = "Datenexport";
            buttonExport.UseVisualStyleBackColor = false;
            buttonExport.Click += buttonExport_Click;
            // 
            // buttonAssets
            // 
            buttonAssets.BackColor = Color.DarkSlateBlue;
            buttonAssets.FlatAppearance.BorderSize = 0;
            buttonAssets.FlatStyle = FlatStyle.Flat;
            buttonAssets.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonAssets.ForeColor = Color.White;
            buttonAssets.Location = new Point(0, 176);
            buttonAssets.Name = "buttonAssets";
            buttonAssets.Size = new Size(200, 32);
            buttonAssets.TabIndex = 3;
            buttonAssets.Text = "Anlagen";
            buttonAssets.UseVisualStyleBackColor = false;
            buttonAssets.Click += buttonAssets_Click;
            // 
            // buttonOverview
            // 
            buttonOverview.BackColor = Color.DarkSlateBlue;
            buttonOverview.FlatAppearance.BorderSize = 0;
            buttonOverview.FlatStyle = FlatStyle.Flat;
            buttonOverview.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonOverview.ForeColor = Color.White;
            buttonOverview.Location = new Point(0, 56);
            buttonOverview.Name = "buttonOverview";
            buttonOverview.Size = new Size(200, 32);
            buttonOverview.TabIndex = 2;
            buttonOverview.Text = "Überblick";
            buttonOverview.UseVisualStyleBackColor = false;
            buttonOverview.Click += buttonOverview_Click;
            // 
            // buttonJournal
            // 
            buttonJournal.BackColor = Color.DarkSlateBlue;
            buttonJournal.FlatAppearance.BorderSize = 0;
            buttonJournal.FlatStyle = FlatStyle.Flat;
            buttonJournal.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonJournal.ForeColor = Color.White;
            buttonJournal.Location = new Point(0, 136);
            buttonJournal.Name = "buttonJournal";
            buttonJournal.Size = new Size(200, 32);
            buttonJournal.TabIndex = 1;
            buttonJournal.Text = "Journal";
            buttonJournal.UseVisualStyleBackColor = false;
            buttonJournal.Click += buttonJournal_Click;
            // 
            // buttonEntry
            // 
            buttonEntry.BackColor = Color.DarkSlateBlue;
            buttonEntry.FlatAppearance.BorderSize = 0;
            buttonEntry.FlatStyle = FlatStyle.Flat;
            buttonEntry.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonEntry.ForeColor = SystemColors.Control;
            buttonEntry.Location = new Point(0, 96);
            buttonEntry.Name = "buttonEntry";
            buttonEntry.Size = new Size(200, 32);
            buttonEntry.TabIndex = 0;
            buttonEntry.Text = "Belegerfassung";
            buttonEntry.UseVisualStyleBackColor = false;
            buttonEntry.Click += buttonEntry_Click;
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabPageOverview);
            tabControl.Controls.Add(tabPageEntry);
            tabControl.Controls.Add(tabPageJournal);
            tabControl.Controls.Add(tabPageAssets);
            tabControl.Controls.Add(tabPageDataexport);
            tabControl.Controls.Add(tabPageSettings);
            tabControl.Controls.Add(tabPageAbout);
            tabControl.Location = new Point(218, -24);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(770, 696);
            tabControl.TabIndex = 1;
            // 
            // tabPageOverview
            // 
            tabPageOverview.Controls.Add(labelOverview);
            tabPageOverview.Location = new Point(4, 24);
            tabPageOverview.Name = "tabPageOverview";
            tabPageOverview.Size = new Size(762, 668);
            tabPageOverview.TabIndex = 2;
            tabPageOverview.Text = "Überblick";
            tabPageOverview.UseVisualStyleBackColor = true;
            // 
            // labelOverview
            // 
            labelOverview.AutoSize = true;
            labelOverview.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelOverview.Location = new Point(0, 16);
            labelOverview.Name = "labelOverview";
            labelOverview.Size = new Size(141, 37);
            labelOverview.TabIndex = 12;
            labelOverview.Text = "Überblick";
            // 
            // tabPageEntry
            // 
            tabPageEntry.BackColor = Color.Transparent;
            tabPageEntry.Controls.Add(textBoxEntryPostingType);
            tabPageEntry.Controls.Add(textBoxEntryNote);
            tabPageEntry.Controls.Add(labelInputNotes);
            tabPageEntry.Controls.Add(labelEntryText);
            tabPageEntry.Controls.Add(labelPostingType);
            tabPageEntry.Controls.Add(labelSelectedFilename);
            tabPageEntry.Controls.Add(labelReversalReferenceVoucher);
            tabPageEntry.Controls.Add(labelReversal);
            tabPageEntry.Controls.Add(labelCategory);
            tabPageEntry.Controls.Add(labelAmount);
            tabPageEntry.Controls.Add(labelVoucherDate);
            tabPageEntry.Controls.Add(label2);
            tabPageEntry.Controls.Add(buttonVoucherPost);
            tabPageEntry.Controls.Add(labelFilename);
            tabPageEntry.Controls.Add(pictureBoxEntryPreview);
            tabPageEntry.Controls.Add(buttonLoadFile);
            tabPageEntry.Controls.Add(comboBoxReferenceVoucher);
            tabPageEntry.Controls.Add(checkBoxReversal);
            tabPageEntry.Controls.Add(textBoxText);
            tabPageEntry.Controls.Add(comboBoxEntryCategory);
            tabPageEntry.Controls.Add(numericUpDownAmount);
            tabPageEntry.Controls.Add(dateTimePickerVoucherDate);
            tabPageEntry.Location = new Point(4, 24);
            tabPageEntry.Margin = new Padding(0);
            tabPageEntry.Name = "tabPageEntry";
            tabPageEntry.Size = new Size(762, 668);
            tabPageEntry.TabIndex = 0;
            tabPageEntry.Text = "Erfassung";
            // 
            // textBoxEntryPostingType
            // 
            textBoxEntryPostingType.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxEntryPostingType.Location = new Point(8, 384);
            textBoxEntryPostingType.Name = "textBoxEntryPostingType";
            textBoxEntryPostingType.ReadOnly = true;
            textBoxEntryPostingType.Size = new Size(270, 29);
            textBoxEntryPostingType.TabIndex = 24;
            textBoxEntryPostingType.TabStop = false;
            textBoxEntryPostingType.TextChanged += textBoxEntryPostingType_TextChanged;
            // 
            // textBoxEntryNote
            // 
            textBoxEntryNote.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxEntryNote.Location = new Point(288, 88);
            textBoxEntryNote.Multiline = true;
            textBoxEntryNote.Name = "textBoxEntryNote";
            textBoxEntryNote.Size = new Size(464, 112);
            textBoxEntryNote.TabIndex = 21;
            // 
            // labelInputNotes
            // 
            labelInputNotes.AutoSize = true;
            labelInputNotes.Font = new Font("Segoe UI", 12F);
            labelInputNotes.ForeColor = Color.Black;
            labelInputNotes.Location = new Point(288, 64);
            labelInputNotes.Name = "labelInputNotes";
            labelInputNotes.Size = new Size(64, 21);
            labelInputNotes.TabIndex = 20;
            labelInputNotes.Text = "Notizen";
            // 
            // labelEntryText
            // 
            labelEntryText.AutoSize = true;
            labelEntryText.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelEntryText.ForeColor = Color.Black;
            labelEntryText.Location = new Point(8, 192);
            labelEntryText.Name = "labelEntryText";
            labelEntryText.Size = new Size(103, 21);
            labelEntryText.TabIndex = 19;
            labelEntryText.Text = "Buchungstext";
            // 
            // labelPostingType
            // 
            labelPostingType.AutoSize = true;
            labelPostingType.Font = new Font("Segoe UI", 12F);
            labelPostingType.ForeColor = Color.Black;
            labelPostingType.Location = new Point(8, 360);
            labelPostingType.Name = "labelPostingType";
            labelPostingType.Size = new Size(97, 21);
            labelPostingType.TabIndex = 18;
            labelPostingType.Text = "Buchungsart";
            // 
            // labelSelectedFilename
            // 
            labelSelectedFilename.AutoSize = true;
            labelSelectedFilename.Font = new Font("Segoe UI", 12F);
            labelSelectedFilename.ForeColor = Color.Black;
            labelSelectedFilename.Location = new Point(376, 608);
            labelSelectedFilename.Name = "labelSelectedFilename";
            labelSelectedFilename.Size = new Size(36, 21);
            labelSelectedFilename.TabIndex = 16;
            labelSelectedFilename.Text = "leer";
            // 
            // labelReversalReferenceVoucher
            // 
            labelReversalReferenceVoucher.AutoSize = true;
            labelReversalReferenceVoucher.Font = new Font("Segoe UI", 12F);
            labelReversalReferenceVoucher.ForeColor = Color.Black;
            labelReversalReferenceVoucher.Location = new Point(8, 480);
            labelReversalReferenceVoucher.Name = "labelReversalReferenceVoucher";
            labelReversalReferenceVoucher.Size = new Size(109, 21);
            labelReversalReferenceVoucher.TabIndex = 15;
            labelReversalReferenceVoucher.Text = "Referenzbeleg";
            // 
            // labelReversal
            // 
            labelReversal.AutoSize = true;
            labelReversal.Font = new Font("Segoe UI", 12F);
            labelReversal.ForeColor = Color.Black;
            labelReversal.Location = new Point(8, 424);
            labelReversal.Name = "labelReversal";
            labelReversal.Size = new Size(56, 21);
            labelReversal.TabIndex = 14;
            labelReversal.Text = "Storno";
            // 
            // labelCategory
            // 
            labelCategory.AutoSize = true;
            labelCategory.Font = new Font("Segoe UI", 12F);
            labelCategory.ForeColor = Color.Black;
            labelCategory.Location = new Point(8, 296);
            labelCategory.Name = "labelCategory";
            labelCategory.Size = new Size(76, 21);
            labelCategory.TabIndex = 13;
            labelCategory.Text = "Kategorie";
            // 
            // labelAmount
            // 
            labelAmount.AutoSize = true;
            labelAmount.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelAmount.ForeColor = Color.Black;
            labelAmount.Location = new Point(8, 128);
            labelAmount.Name = "labelAmount";
            labelAmount.Size = new Size(55, 21);
            labelAmount.TabIndex = 12;
            labelAmount.Text = "Betrag";
            // 
            // labelVoucherDate
            // 
            labelVoucherDate.AutoSize = true;
            labelVoucherDate.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelVoucherDate.ForeColor = Color.Black;
            labelVoucherDate.Location = new Point(8, 64);
            labelVoucherDate.Name = "labelVoucherDate";
            labelVoucherDate.Size = new Size(93, 21);
            labelVoucherDate.TabIndex = 11;
            labelVoucherDate.Text = "Belegdatum";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.Black;
            label2.Location = new Point(0, 16);
            label2.Name = "label2";
            label2.Size = new Size(141, 37);
            label2.TabIndex = 10;
            label2.Text = "Erfassung";
            // 
            // buttonVoucherPost
            // 
            buttonVoucherPost.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonVoucherPost.Location = new Point(8, 592);
            buttonVoucherPost.Name = "buttonVoucherPost";
            buttonVoucherPost.Size = new Size(272, 35);
            buttonVoucherPost.TabIndex = 9;
            buttonVoucherPost.Text = "Erfassung buchen";
            buttonVoucherPost.UseVisualStyleBackColor = true;
            buttonVoucherPost.Click += buttonVoucherPost_Click;
            // 
            // labelFilename
            // 
            labelFilename.AutoSize = true;
            labelFilename.Font = new Font("Segoe UI", 12F);
            labelFilename.ForeColor = Color.Black;
            labelFilename.Location = new Point(288, 608);
            labelFilename.Name = "labelFilename";
            labelFilename.Size = new Size(88, 21);
            labelFilename.TabIndex = 8;
            labelFilename.Text = "Dateiname:";
            // 
            // pictureBoxEntryPreview
            // 
            pictureBoxEntryPreview.Location = new Point(288, 248);
            pictureBoxEntryPreview.Name = "pictureBoxEntryPreview";
            pictureBoxEntryPreview.Size = new Size(464, 360);
            pictureBoxEntryPreview.TabIndex = 7;
            pictureBoxEntryPreview.TabStop = false;
            // 
            // buttonLoadFile
            // 
            buttonLoadFile.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonLoadFile.Location = new Point(288, 208);
            buttonLoadFile.Name = "buttonLoadFile";
            buttonLoadFile.Size = new Size(464, 32);
            buttonLoadFile.TabIndex = 6;
            buttonLoadFile.Text = "Bild / PDF hinzufügen...";
            buttonLoadFile.UseVisualStyleBackColor = true;
            buttonLoadFile.Click += buttonLoadFile_Click;
            // 
            // comboBoxReferenceVoucher
            // 
            comboBoxReferenceVoucher.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxReferenceVoucher.Enabled = false;
            comboBoxReferenceVoucher.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            comboBoxReferenceVoucher.FormattingEnabled = true;
            comboBoxReferenceVoucher.Location = new Point(8, 504);
            comboBoxReferenceVoucher.Name = "comboBoxReferenceVoucher";
            comboBoxReferenceVoucher.Size = new Size(270, 29);
            comboBoxReferenceVoucher.TabIndex = 5;
            comboBoxReferenceVoucher.SelectedIndexChanged += comboBoxReferenceVoucher_SelectedIndexChanged;
            // 
            // checkBoxReversal
            // 
            checkBoxReversal.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBoxReversal.Location = new Point(8, 448);
            checkBoxReversal.Name = "checkBoxReversal";
            checkBoxReversal.Size = new Size(20, 20);
            checkBoxReversal.TabIndex = 4;
            checkBoxReversal.UseVisualStyleBackColor = true;
            checkBoxReversal.CheckedChanged += checkBoxReversal_CheckedChanged;
            // 
            // textBoxText
            // 
            textBoxText.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxText.Location = new Point(8, 216);
            textBoxText.Multiline = true;
            textBoxText.Name = "textBoxText";
            textBoxText.Size = new Size(270, 72);
            textBoxText.TabIndex = 3;
            // 
            // comboBoxEntryCategory
            // 
            comboBoxEntryCategory.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBoxEntryCategory.AutoCompleteSource = AutoCompleteSource.ListItems;
            comboBoxEntryCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxEntryCategory.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            comboBoxEntryCategory.FormattingEnabled = true;
            comboBoxEntryCategory.Items.AddRange(new object[] { "Material / Bedarf", "Facebook-Rechnung", "Sonstiges" });
            comboBoxEntryCategory.Location = new Point(8, 320);
            comboBoxEntryCategory.Name = "comboBoxEntryCategory";
            comboBoxEntryCategory.Size = new Size(270, 29);
            comboBoxEntryCategory.TabIndex = 2;
            comboBoxEntryCategory.SelectedIndexChanged += comboBoxEntryCategory_SelectedIndexChanged;
            // 
            // numericUpDownAmount
            // 
            numericUpDownAmount.DecimalPlaces = 2;
            numericUpDownAmount.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            numericUpDownAmount.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            numericUpDownAmount.Location = new Point(8, 152);
            numericUpDownAmount.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            numericUpDownAmount.Name = "numericUpDownAmount";
            numericUpDownAmount.Size = new Size(270, 29);
            numericUpDownAmount.TabIndex = 1;
            numericUpDownAmount.ThousandsSeparator = true;
            // 
            // dateTimePickerVoucherDate
            // 
            dateTimePickerVoucherDate.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dateTimePickerVoucherDate.Format = DateTimePickerFormat.Short;
            dateTimePickerVoucherDate.Location = new Point(8, 88);
            dateTimePickerVoucherDate.Name = "dateTimePickerVoucherDate";
            dateTimePickerVoucherDate.Size = new Size(270, 29);
            dateTimePickerVoucherDate.TabIndex = 0;
            // 
            // tabPageJournal
            // 
            tabPageJournal.Controls.Add(comboBoxJournalFilterMonth);
            tabPageJournal.Controls.Add(labelJournalFilterMonth);
            tabPageJournal.Controls.Add(comboBoxJournalFilter);
            tabPageJournal.Controls.Add(labelJournalFilterYear);
            tabPageJournal.Controls.Add(textBoxJournalVoucher);
            tabPageJournal.Controls.Add(labelJournalVoucher);
            tabPageJournal.Controls.Add(textBoxJournalReversalVoucher);
            tabPageJournal.Controls.Add(textBoxJournalReversal);
            tabPageJournal.Controls.Add(textBoxJournalPostingType);
            tabPageJournal.Controls.Add(textBoxJournalCategory);
            tabPageJournal.Controls.Add(textBoxJournalPostingText);
            tabPageJournal.Controls.Add(textBoxJournalAmount);
            tabPageJournal.Controls.Add(textBoxJournalVoucherDate);
            tabPageJournal.Controls.Add(labelJournalReversalVoucher);
            tabPageJournal.Controls.Add(labelJournalReversal);
            tabPageJournal.Controls.Add(labelJournalPostingType);
            tabPageJournal.Controls.Add(labelJournalCategory);
            tabPageJournal.Controls.Add(labelJournalPostingText);
            tabPageJournal.Controls.Add(labelJournalAmount);
            tabPageJournal.Controls.Add(labelJournalVoucherDate);
            tabPageJournal.Controls.Add(labelJournalNote);
            tabPageJournal.Controls.Add(textBoxJournalNote);
            tabPageJournal.Controls.Add(textBoxJournalCreationDate);
            tabPageJournal.Controls.Add(label1);
            tabPageJournal.Controls.Add(buttonJournalPicturePDF);
            tabPageJournal.Controls.Add(labelTransactionDetail);
            tabPageJournal.Controls.Add(labelJournal);
            tabPageJournal.Controls.Add(dataGridViewJournal);
            tabPageJournal.Location = new Point(4, 24);
            tabPageJournal.Name = "tabPageJournal";
            tabPageJournal.Padding = new Padding(3);
            tabPageJournal.Size = new Size(762, 668);
            tabPageJournal.TabIndex = 1;
            tabPageJournal.Text = "Journal";
            tabPageJournal.UseVisualStyleBackColor = true;
            // 
            // comboBoxJournalFilterMonth
            // 
            comboBoxJournalFilterMonth.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            comboBoxJournalFilterMonth.FormattingEnabled = true;
            comboBoxJournalFilterMonth.Location = new Point(376, 56);
            comboBoxJournalFilterMonth.Name = "comboBoxJournalFilterMonth";
            comboBoxJournalFilterMonth.Size = new Size(176, 29);
            comboBoxJournalFilterMonth.TabIndex = 36;
            // 
            // labelJournalFilterMonth
            // 
            labelJournalFilterMonth.AutoSize = true;
            labelJournalFilterMonth.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelJournalFilterMonth.Location = new Point(280, 56);
            labelJournalFilterMonth.Name = "labelJournalFilterMonth";
            labelJournalFilterMonth.Size = new Size(97, 21);
            labelJournalFilterMonth.TabIndex = 35;
            labelJournalFilterMonth.Text = "Filter Monat:";
            // 
            // comboBoxJournalFilter
            // 
            comboBoxJournalFilter.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            comboBoxJournalFilter.FormattingEnabled = true;
            comboBoxJournalFilter.Location = new Point(88, 56);
            comboBoxJournalFilter.Name = "comboBoxJournalFilter";
            comboBoxJournalFilter.Size = new Size(176, 29);
            comboBoxJournalFilter.TabIndex = 34;
            // 
            // labelJournalFilterYear
            // 
            labelJournalFilterYear.AutoSize = true;
            labelJournalFilterYear.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelJournalFilterYear.Location = new Point(8, 56);
            labelJournalFilterYear.Name = "labelJournalFilterYear";
            labelJournalFilterYear.Size = new Size(81, 21);
            labelJournalFilterYear.TabIndex = 33;
            labelJournalFilterYear.Text = "Filter Jahr:";
            // 
            // textBoxJournalVoucher
            // 
            textBoxJournalVoucher.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxJournalVoucher.Location = new Point(568, 112);
            textBoxJournalVoucher.Name = "textBoxJournalVoucher";
            textBoxJournalVoucher.Size = new Size(184, 29);
            textBoxJournalVoucher.TabIndex = 32;
            // 
            // labelJournalVoucher
            // 
            labelJournalVoucher.AutoSize = true;
            labelJournalVoucher.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelJournalVoucher.Location = new Point(560, 88);
            labelJournalVoucher.Name = "labelJournalVoucher";
            labelJournalVoucher.Size = new Size(48, 21);
            labelJournalVoucher.TabIndex = 31;
            labelJournalVoucher.Text = "Beleg";
            // 
            // textBoxJournalReversalVoucher
            // 
            textBoxJournalReversalVoucher.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxJournalReversalVoucher.Location = new Point(632, 448);
            textBoxJournalReversalVoucher.Name = "textBoxJournalReversalVoucher";
            textBoxJournalReversalVoucher.Size = new Size(120, 29);
            textBoxJournalReversalVoucher.TabIndex = 30;
            // 
            // textBoxJournalReversal
            // 
            textBoxJournalReversal.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxJournalReversal.Location = new Point(568, 448);
            textBoxJournalReversal.Name = "textBoxJournalReversal";
            textBoxJournalReversal.Size = new Size(56, 29);
            textBoxJournalReversal.TabIndex = 29;
            // 
            // textBoxJournalPostingType
            // 
            textBoxJournalPostingType.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxJournalPostingType.Location = new Point(568, 392);
            textBoxJournalPostingType.Name = "textBoxJournalPostingType";
            textBoxJournalPostingType.Size = new Size(184, 29);
            textBoxJournalPostingType.TabIndex = 28;
            // 
            // textBoxJournalCategory
            // 
            textBoxJournalCategory.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxJournalCategory.Location = new Point(568, 336);
            textBoxJournalCategory.Name = "textBoxJournalCategory";
            textBoxJournalCategory.Size = new Size(184, 29);
            textBoxJournalCategory.TabIndex = 27;
            // 
            // textBoxJournalPostingText
            // 
            textBoxJournalPostingText.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxJournalPostingText.Location = new Point(568, 280);
            textBoxJournalPostingText.Name = "textBoxJournalPostingText";
            textBoxJournalPostingText.Size = new Size(184, 29);
            textBoxJournalPostingText.TabIndex = 26;
            // 
            // textBoxJournalAmount
            // 
            textBoxJournalAmount.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxJournalAmount.Location = new Point(568, 224);
            textBoxJournalAmount.Name = "textBoxJournalAmount";
            textBoxJournalAmount.Size = new Size(184, 29);
            textBoxJournalAmount.TabIndex = 25;
            // 
            // textBoxJournalVoucherDate
            // 
            textBoxJournalVoucherDate.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxJournalVoucherDate.Location = new Point(568, 168);
            textBoxJournalVoucherDate.Name = "textBoxJournalVoucherDate";
            textBoxJournalVoucherDate.Size = new Size(184, 29);
            textBoxJournalVoucherDate.TabIndex = 24;
            // 
            // labelJournalReversalVoucher
            // 
            labelJournalReversalVoucher.AutoSize = true;
            labelJournalReversalVoucher.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelJournalReversalVoucher.Location = new Point(624, 424);
            labelJournalReversalVoucher.Name = "labelJournalReversalVoucher";
            labelJournalReversalVoucher.Size = new Size(94, 21);
            labelJournalReversalVoucher.TabIndex = 23;
            labelJournalReversalVoucher.Text = "Stornobeleg";
            // 
            // labelJournalReversal
            // 
            labelJournalReversal.AutoSize = true;
            labelJournalReversal.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelJournalReversal.Location = new Point(560, 424);
            labelJournalReversal.Name = "labelJournalReversal";
            labelJournalReversal.Size = new Size(56, 21);
            labelJournalReversal.TabIndex = 22;
            labelJournalReversal.Text = "Storno";
            // 
            // labelJournalPostingType
            // 
            labelJournalPostingType.AutoSize = true;
            labelJournalPostingType.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelJournalPostingType.Location = new Point(560, 368);
            labelJournalPostingType.Name = "labelJournalPostingType";
            labelJournalPostingType.Size = new Size(97, 21);
            labelJournalPostingType.TabIndex = 21;
            labelJournalPostingType.Text = "Buchungsart";
            // 
            // labelJournalCategory
            // 
            labelJournalCategory.AutoSize = true;
            labelJournalCategory.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelJournalCategory.Location = new Point(560, 312);
            labelJournalCategory.Name = "labelJournalCategory";
            labelJournalCategory.Size = new Size(76, 21);
            labelJournalCategory.TabIndex = 20;
            labelJournalCategory.Text = "Kategorie";
            // 
            // labelJournalPostingText
            // 
            labelJournalPostingText.AutoSize = true;
            labelJournalPostingText.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelJournalPostingText.Location = new Point(560, 256);
            labelJournalPostingText.Name = "labelJournalPostingText";
            labelJournalPostingText.Size = new Size(103, 21);
            labelJournalPostingText.TabIndex = 19;
            labelJournalPostingText.Text = "Buchungstext";
            // 
            // labelJournalAmount
            // 
            labelJournalAmount.AutoSize = true;
            labelJournalAmount.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelJournalAmount.Location = new Point(560, 200);
            labelJournalAmount.Name = "labelJournalAmount";
            labelJournalAmount.Size = new Size(55, 21);
            labelJournalAmount.TabIndex = 18;
            labelJournalAmount.Text = "Betrag";
            // 
            // labelJournalVoucherDate
            // 
            labelJournalVoucherDate.AutoSize = true;
            labelJournalVoucherDate.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelJournalVoucherDate.Location = new Point(560, 144);
            labelJournalVoucherDate.Name = "labelJournalVoucherDate";
            labelJournalVoucherDate.Size = new Size(93, 21);
            labelJournalVoucherDate.TabIndex = 17;
            labelJournalVoucherDate.Text = "Belegdatum";
            // 
            // labelJournalNote
            // 
            labelJournalNote.AutoSize = true;
            labelJournalNote.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelJournalNote.Location = new Point(560, 576);
            labelJournalNote.Name = "labelJournalNote";
            labelJournalNote.Size = new Size(47, 21);
            labelJournalNote.TabIndex = 16;
            labelJournalNote.Text = "Notiz";
            // 
            // textBoxJournalNote
            // 
            textBoxJournalNote.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxJournalNote.Location = new Point(568, 600);
            textBoxJournalNote.Multiline = true;
            textBoxJournalNote.Name = "textBoxJournalNote";
            textBoxJournalNote.Size = new Size(184, 48);
            textBoxJournalNote.TabIndex = 15;
            // 
            // textBoxJournalCreationDate
            // 
            textBoxJournalCreationDate.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxJournalCreationDate.Location = new Point(568, 544);
            textBoxJournalCreationDate.Multiline = true;
            textBoxJournalCreationDate.Name = "textBoxJournalCreationDate";
            textBoxJournalCreationDate.Size = new Size(184, 24);
            textBoxJournalCreationDate.TabIndex = 14;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(560, 520);
            label1.Name = "label1";
            label1.Size = new Size(131, 21);
            label1.TabIndex = 13;
            label1.Text = "Erstellungsdatum";
            // 
            // buttonJournalPicturePDF
            // 
            buttonJournalPicturePDF.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonJournalPicturePDF.Location = new Point(568, 480);
            buttonJournalPicturePDF.Name = "buttonJournalPicturePDF";
            buttonJournalPicturePDF.Size = new Size(184, 32);
            buttonJournalPicturePDF.TabIndex = 12;
            buttonJournalPicturePDF.Text = "Bild / PDF";
            buttonJournalPicturePDF.UseVisualStyleBackColor = true;
            buttonJournalPicturePDF.Click += buttonJournalPicturePDF_Click;
            // 
            // labelTransactionDetail
            // 
            labelTransactionDetail.AutoSize = true;
            labelTransactionDetail.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelTransactionDetail.Location = new Point(560, 56);
            labelTransactionDetail.Name = "labelTransactionDetail";
            labelTransactionDetail.Size = new Size(150, 25);
            labelTransactionDetail.TabIndex = 2;
            labelTransactionDetail.Text = "Buchungsdetails";
            // 
            // labelJournal
            // 
            labelJournal.AutoSize = true;
            labelJournal.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelJournal.Location = new Point(0, 16);
            labelJournal.Name = "labelJournal";
            labelJournal.Size = new Size(112, 37);
            labelJournal.TabIndex = 11;
            labelJournal.Text = "Journal";
            // 
            // dataGridViewJournal
            // 
            dataGridViewJournal.BackgroundColor = SystemColors.Control;
            dataGridViewJournal.BorderStyle = BorderStyle.None;
            dataGridViewJournal.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridViewJournal.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = SystemColors.Control;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            dataGridViewJournal.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dataGridViewJournal.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = SystemColors.Window;
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle4.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Control;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.Control;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            dataGridViewJournal.DefaultCellStyle = dataGridViewCellStyle4;
            dataGridViewJournal.GridColor = SystemColors.Window;
            dataGridViewJournal.Location = new Point(-40, 88);
            dataGridViewJournal.MultiSelect = false;
            dataGridViewJournal.Name = "dataGridViewJournal";
            dataGridViewJournal.ReadOnly = true;
            dataGridViewJournal.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewJournal.RowTemplate.Height = 50;
            dataGridViewJournal.RowTemplate.ReadOnly = true;
            dataGridViewJournal.RowTemplate.Resizable = DataGridViewTriState.False;
            dataGridViewJournal.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewJournal.Size = new Size(600, 544);
            dataGridViewJournal.TabIndex = 0;
            dataGridViewJournal.CellFormatting += dataGridViewJournal_CellFormatting;
            dataGridViewJournal.SelectionChanged += dataGridViewJournal_SelectionChanged;
            // 
            // tabPageAssets
            // 
            tabPageAssets.Location = new Point(4, 24);
            tabPageAssets.Name = "tabPageAssets";
            tabPageAssets.Size = new Size(762, 668);
            tabPageAssets.TabIndex = 3;
            tabPageAssets.Text = "Anlagen";
            tabPageAssets.UseVisualStyleBackColor = true;
            // 
            // tabPageDataexport
            // 
            tabPageDataexport.Location = new Point(4, 24);
            tabPageDataexport.Name = "tabPageDataexport";
            tabPageDataexport.Size = new Size(762, 668);
            tabPageDataexport.TabIndex = 4;
            tabPageDataexport.Text = "Datenexport";
            tabPageDataexport.UseVisualStyleBackColor = true;
            // 
            // tabPageSettings
            // 
            tabPageSettings.Location = new Point(4, 24);
            tabPageSettings.Name = "tabPageSettings";
            tabPageSettings.Size = new Size(762, 668);
            tabPageSettings.TabIndex = 5;
            tabPageSettings.Text = "Einstellungen";
            tabPageSettings.UseVisualStyleBackColor = true;
            // 
            // tabPageAbout
            // 
            tabPageAbout.Location = new Point(4, 24);
            tabPageAbout.Name = "tabPageAbout";
            tabPageAbout.Size = new Size(762, 668);
            tabPageAbout.TabIndex = 6;
            tabPageAbout.Text = "Über...";
            tabPageAbout.UseVisualStyleBackColor = true;
            // 
            // buttonItemEntry
            // 
            buttonItemEntry.BackColor = Color.DarkSlateBlue;
            buttonItemEntry.FlatAppearance.BorderSize = 0;
            buttonItemEntry.FlatStyle = FlatStyle.Flat;
            buttonItemEntry.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonItemEntry.ForeColor = Color.White;
            buttonItemEntry.Location = new Point(0, 296);
            buttonItemEntry.Name = "buttonItemEntry";
            buttonItemEntry.Size = new Size(200, 32);
            buttonItemEntry.TabIndex = 9;
            buttonItemEntry.Text = "Artikelerfassung";
            buttonItemEntry.UseVisualStyleBackColor = false;
            // 
            // buttonItems
            // 
            buttonItems.BackColor = Color.DarkSlateBlue;
            buttonItems.FlatAppearance.BorderSize = 0;
            buttonItems.FlatStyle = FlatStyle.Flat;
            buttonItems.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonItems.ForeColor = Color.White;
            buttonItems.Location = new Point(0, 336);
            buttonItems.Name = "buttonItems";
            buttonItems.Size = new Size(200, 32);
            buttonItems.TabIndex = 10;
            buttonItems.Text = "Artikel";
            buttonItems.UseVisualStyleBackColor = false;
            // 
            // buttonInvoiceEntry
            // 
            buttonInvoiceEntry.BackColor = Color.DarkSlateBlue;
            buttonInvoiceEntry.FlatAppearance.BorderSize = 0;
            buttonInvoiceEntry.FlatStyle = FlatStyle.Flat;
            buttonInvoiceEntry.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonInvoiceEntry.ForeColor = Color.White;
            buttonInvoiceEntry.Location = new Point(0, 376);
            buttonInvoiceEntry.Name = "buttonInvoiceEntry";
            buttonInvoiceEntry.Size = new Size(200, 32);
            buttonInvoiceEntry.TabIndex = 11;
            buttonInvoiceEntry.Text = "Rechnungserfassung";
            buttonInvoiceEntry.UseVisualStyleBackColor = false;
            // 
            // buttonInvoices
            // 
            buttonInvoices.BackColor = Color.DarkSlateBlue;
            buttonInvoices.FlatAppearance.BorderSize = 0;
            buttonInvoices.FlatStyle = FlatStyle.Flat;
            buttonInvoices.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonInvoices.ForeColor = Color.White;
            buttonInvoices.Location = new Point(0, 416);
            buttonInvoices.Name = "buttonInvoices";
            buttonInvoices.Size = new Size(200, 32);
            buttonInvoices.TabIndex = 12;
            buttonInvoices.Text = "Rechnungen";
            buttonInvoices.UseVisualStyleBackColor = false;
            // 
            // buttonCustomerEntry
            // 
            buttonCustomerEntry.BackColor = Color.DarkSlateBlue;
            buttonCustomerEntry.FlatAppearance.BorderSize = 0;
            buttonCustomerEntry.FlatStyle = FlatStyle.Flat;
            buttonCustomerEntry.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonCustomerEntry.ForeColor = Color.White;
            buttonCustomerEntry.Location = new Point(0, 216);
            buttonCustomerEntry.Name = "buttonCustomerEntry";
            buttonCustomerEntry.Size = new Size(200, 32);
            buttonCustomerEntry.TabIndex = 13;
            buttonCustomerEntry.Text = "Kundenerfassung";
            buttonCustomerEntry.UseVisualStyleBackColor = false;
            // 
            // buttonCustomers
            // 
            buttonCustomers.BackColor = Color.DarkSlateBlue;
            buttonCustomers.FlatAppearance.BorderSize = 0;
            buttonCustomers.FlatStyle = FlatStyle.Flat;
            buttonCustomers.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonCustomers.ForeColor = Color.White;
            buttonCustomers.Location = new Point(0, 256);
            buttonCustomers.Name = "buttonCustomers";
            buttonCustomers.Size = new Size(200, 32);
            buttonCustomers.TabIndex = 14;
            buttonCustomers.Text = "Kunden";
            buttonCustomers.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.MidnightBlue;
            ClientSize = new Size(984, 661);
            Controls.Add(tabControl);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            Text = "Mirada Finanza Control Central";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            tabControl.ResumeLayout(false);
            tabPageOverview.ResumeLayout(false);
            tabPageOverview.PerformLayout();
            tabPageEntry.ResumeLayout(false);
            tabPageEntry.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxEntryPreview).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownAmount).EndInit();
            tabPageJournal.ResumeLayout(false);
            tabPageJournal.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewJournal).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button buttonJournal;
        private Button buttonEntry;
        private TabControl tabControl;
        private TabPage tabPageEntry;
        private TabPage tabPageJournal;
        private ComboBox comboBoxReferenceVoucher;
        private CheckBox checkBoxReversal;
        private TextBox textBoxText;
        private ComboBox comboBoxEntryCategory;
        private NumericUpDown numericUpDownAmount;
        private DateTimePicker dateTimePickerVoucherDate;
        private Button buttonVoucherPost;
        private Label labelFilename;
        private PictureBox pictureBoxEntryPreview;
        private Button buttonLoadFile;
        private DataGridView dataGridViewJournal;
        private Label labelVoucherDate;
        private Label label2;
        private Label labelAmount;
        private Label labelCategory;
        private Label labelReversalReferenceVoucher;
        private Label labelReversal;
        private Label labelSelectedFilename;
        private Label labelJournal;
        private Label labelTransactionDetail;
        private Label labelEntryText;
        private Label labelPostingType;
        private TextBox textBoxEntryNote;
        private Label labelInputNotes;
        private TextBox textBoxEntryPostingType;
        private Button buttonOverview;
        private Button buttonAssets;
        private Button buttonSettings;
        private Button buttonExport;
        private Button buttonAbout;
        private TabPage tabPageOverview;
        private TabPage tabPageAssets;
        private TabPage tabPageDataexport;
        private TabPage tabPageSettings;
        private TabPage tabPageAbout;
        private Label labelJournalPicturePDF;
        private Button buttonJournalPicturePDF;
        private TextBox textBoxJournalNote;
        private TextBox textBoxJournalCreationDate;
        private Label label1;
        private Label labelJournalNote;
        private Label labelMiradaFinanza;
        private Label labelControlCentral;
        private Label labelJournalVoucherDate;
        private Label labelJournalCategory;
        private Label labelJournalPostingText;
        private Label labelJournalAmount;
        private Label labelJournalReversalVoucher;
        private Label labelJournalReversal;
        private Label labelJournalPostingType;
        private TextBox textBoxJournalReversal;
        private TextBox textBoxJournalPostingType;
        private TextBox textBoxJournalCategory;
        private TextBox textBoxJournalPostingText;
        private TextBox textBoxJournalAmount;
        private TextBox textBoxJournalVoucherDate;
        private TextBox textBoxJournalReversalVoucher;
        private TextBox textBoxJournalVoucher;
        private Label labelJournalVoucher;
        private Label labelJournalFilterYear;
        private ComboBox comboBoxJournalFilter;
        private ComboBox comboBoxJournalFilterMonth;
        private Label labelJournalFilterMonth;
        private Label labelOverview;
        private Button buttonInvoices;
        private Button buttonInvoiceEntry;
        private Button buttonItems;
        private Button buttonItemEntry;
        private Button buttonCustomers;
        private Button buttonCustomerEntry;
    }
}
