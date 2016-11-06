namespace Deskband.Settings
{
    partial class SettingsForm
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
            this.pgSettings = new System.Windows.Forms.PropertyGrid();
            this.btnApply = new System.Windows.Forms.Button();
            this.tsCommands = new System.Windows.Forms.ToolStrip();
            this.tvItems = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // pgSettings
            // 
            this.pgSettings.Location = new System.Drawing.Point(265, 12);
            this.pgSettings.Name = "pgSettings";
            this.pgSettings.Size = new System.Drawing.Size(564, 382);
            this.pgSettings.TabIndex = 0;
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(754, 400);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            // 
            // tsCommands
            // 
            this.tsCommands.Dock = System.Windows.Forms.DockStyle.None;
            this.tsCommands.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsCommands.Location = new System.Drawing.Point(344, 12);
            this.tsCommands.Name = "tsCommands";
            this.tsCommands.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsCommands.Size = new System.Drawing.Size(102, 25);
            this.tsCommands.TabIndex = 4;
            this.tsCommands.Text = "toolStrip1";
            // 
            // tvItems
            // 
            this.tvItems.HideSelection = false;
            this.tvItems.Location = new System.Drawing.Point(13, 12);
            this.tvItems.Name = "tvItems";
            this.tvItems.Size = new System.Drawing.Size(246, 382);
            this.tvItems.TabIndex = 5;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 434);
            this.Controls.Add(this.tvItems);
            this.Controls.Add(this.tsCommands);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.pgSettings);
            this.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "SettingsForm";
            this.Text = "Deskband Controls Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid pgSettings;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.ToolStrip tsCommands;
        private System.Windows.Forms.TreeView tvItems;
    }
}