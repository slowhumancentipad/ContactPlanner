namespace ContactPlanner
{
    partial class SelectContactWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listViewAll = new System.Windows.Forms.ListView();
            this.columnContacts = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonToAll = new System.Windows.Forms.Button();
            this.buttonToSelected = new System.Windows.Forms.Button();
            this.buttonDone = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.columnContact = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewSelected = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // listViewAll
            // 
            this.listViewAll.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewAll.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnContacts});
            this.listViewAll.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewAll.Location = new System.Drawing.Point(285, 12);
            this.listViewAll.MultiSelect = false;
            this.listViewAll.Name = "listViewAll";
            this.listViewAll.Size = new System.Drawing.Size(165, 238);
            this.listViewAll.TabIndex = 3;
            this.listViewAll.UseCompatibleStateImageBehavior = false;
            this.listViewAll.View = System.Windows.Forms.View.Details;
            // 
            // columnContacts
            // 
            this.columnContacts.Text = "Все контакты";
            this.columnContacts.Width = 165;
            // 
            // buttonToAll
            // 
            this.buttonToAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonToAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonToAll.Location = new System.Drawing.Point(183, 91);
            this.buttonToAll.Name = "buttonToAll";
            this.buttonToAll.Size = new System.Drawing.Size(96, 23);
            this.buttonToAll.TabIndex = 1;
            this.buttonToAll.Text = "→";
            this.buttonToAll.UseVisualStyleBackColor = true;
            this.buttonToAll.Click += new System.EventHandler(this.buttonToAll_Click);
            // 
            // buttonToSelected
            // 
            this.buttonToSelected.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonToSelected.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonToSelected.Location = new System.Drawing.Point(183, 120);
            this.buttonToSelected.Name = "buttonToSelected";
            this.buttonToSelected.Size = new System.Drawing.Size(96, 23);
            this.buttonToSelected.TabIndex = 4;
            this.buttonToSelected.Text = "←";
            this.buttonToSelected.UseVisualStyleBackColor = true;
            this.buttonToSelected.Click += new System.EventHandler(this.buttonToSelected_Click);
            // 
            // buttonDone
            // 
            this.buttonDone.Location = new System.Drawing.Point(183, 198);
            this.buttonDone.Name = "buttonDone";
            this.buttonDone.Size = new System.Drawing.Size(96, 23);
            this.buttonDone.TabIndex = 5;
            this.buttonDone.Text = "Готово";
            this.buttonDone.UseVisualStyleBackColor = true;
            this.buttonDone.Click += new System.EventHandler(this.buttonDone_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(183, 227);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(96, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // columnContact
            // 
            this.columnContact.Text = "Добавленные контакты";
            this.columnContact.Width = 165;
            // 
            // listViewSelected
            // 
            this.listViewSelected.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewSelected.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnContact});
            this.listViewSelected.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewSelected.Location = new System.Drawing.Point(12, 12);
            this.listViewSelected.MultiSelect = false;
            this.listViewSelected.Name = "listViewSelected";
            this.listViewSelected.Size = new System.Drawing.Size(165, 238);
            this.listViewSelected.TabIndex = 1;
            this.listViewSelected.UseCompatibleStateImageBehavior = false;
            this.listViewSelected.View = System.Windows.Forms.View.Details;
            // 
            // SelectContactWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(462, 262);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonDone);
            this.Controls.Add(this.buttonToSelected);
            this.Controls.Add(this.buttonToAll);
            this.Controls.Add(this.listViewAll);
            this.Controls.Add(this.listViewSelected);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectContactWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SelectContactWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewAll;
        private System.Windows.Forms.ColumnHeader columnContacts;
        private System.Windows.Forms.Button buttonToAll;
        private System.Windows.Forms.Button buttonToSelected;
        private System.Windows.Forms.Button buttonDone;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ColumnHeader columnContact;
        private System.Windows.Forms.ListView listViewSelected;

    }
}