namespace CellGame
{
    partial class CellGame
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
            FieldSize_num = new NumericUpDown();
            FieldSize_lb = new Label();
            TicInterval_lb = new Label();
            TicInterval_num = new NumericUpDown();
            Step_btn = new Button();
            PicBox = new PictureBox();
            Start_btn = new Button();
            ((System.ComponentModel.ISupportInitialize)FieldSize_num).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TicInterval_num).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PicBox).BeginInit();
            SuspendLayout();
            // 
            // FieldSize_num
            // 
            FieldSize_num.Location = new Point(75, 7);
            FieldSize_num.Name = "FieldSize_num";
            FieldSize_num.Size = new Size(120, 23);
            FieldSize_num.TabIndex = 0;
            FieldSize_num.ValueChanged += ChangeFieldSize;
            // 
            // FieldSize_lb
            // 
            FieldSize_lb.AutoSize = true;
            FieldSize_lb.Location = new Point(12, 9);
            FieldSize_lb.Name = "FieldSize_lb";
            FieldSize_lb.Size = new Size(57, 15);
            FieldSize_lb.TabIndex = 1;
            FieldSize_lb.Text = "Field size:";
            // 
            // TicInterval_lb
            // 
            TicInterval_lb.AutoSize = true;
            TicInterval_lb.Location = new Point(216, 9);
            TicInterval_lb.Name = "TicInterval_lb";
            TicInterval_lb.Size = new Size(79, 15);
            TicInterval_lb.TabIndex = 3;
            TicInterval_lb.Text = "Timer (msec):";
            // 
            // TicInterval_num
            // 
            TicInterval_num.Location = new Point(301, 7);
            TicInterval_num.Name = "TicInterval_num";
            TicInterval_num.Size = new Size(120, 23);
            TicInterval_num.TabIndex = 2;
            // 
            // Step_btn
            // 
            Step_btn.Location = new Point(427, 5);
            Step_btn.Name = "Step_btn";
            Step_btn.Size = new Size(75, 23);
            Step_btn.TabIndex = 4;
            Step_btn.Text = "Step";
            Step_btn.UseVisualStyleBackColor = true;
            Step_btn.Click += StepClick;
            // 
            // PicBox
            // 
            PicBox.Location = new Point(12, 36);
            PicBox.Name = "PicBox";
            PicBox.Size = new Size(601, 401);
            PicBox.TabIndex = 5;
            PicBox.TabStop = false;
            PicBox.Click += CreateCellOnClick;
            // 
            // Start_btn
            // 
            Start_btn.Location = new Point(508, 5);
            Start_btn.Name = "Start_btn";
            Start_btn.Size = new Size(75, 23);
            Start_btn.TabIndex = 6;
            Start_btn.Text = "Start";
            Start_btn.UseVisualStyleBackColor = true;
            // 
            // CellGame
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(624, 450);
            Controls.Add(Start_btn);
            Controls.Add(PicBox);
            Controls.Add(Step_btn);
            Controls.Add(TicInterval_lb);
            Controls.Add(TicInterval_num);
            Controls.Add(FieldSize_lb);
            Controls.Add(FieldSize_num);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "CellGame";
            Text = "Cell Game";
            ((System.ComponentModel.ISupportInitialize)FieldSize_num).EndInit();
            ((System.ComponentModel.ISupportInitialize)TicInterval_num).EndInit();
            ((System.ComponentModel.ISupportInitialize)PicBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NumericUpDown FieldSize_num;
        private Label FieldSize_lb;
        private Label TicInterval_lb;
        private NumericUpDown TicInterval_num;
        private Button Step_btn;
        private PictureBox PicBox;
        private Button Start_btn;
    }
}