namespace lab1
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
            textInput = new TextBox();
            quaVowels = new TextBox();
            label1 = new Label();
            label2 = new Label();
            quaConsonant = new TextBox();
            quaWords = new TextBox();
            label3 = new Label();
            quaSent = new TextBox();
            label5 = new Label();
            stringLen = new TextBox();
            label4 = new Label();
            btnCalc = new Button();
            zadanye = new TextBox();
            SuspendLayout();
            // 
            // textInput
            // 
            textInput.BorderStyle = BorderStyle.FixedSingle;
            textInput.Location = new Point(101, 79);
            textInput.Name = "textInput";
            textInput.Size = new Size(397, 27);
            textInput.TabIndex = 0;
            textInput.Text = "Введите строку";
            textInput.TextChanged += textBox1_TextChanged;
            // 
            // quaVowels
            // 
            quaVowels.Location = new Point(298, 177);
            quaVowels.Name = "quaVowels";
            quaVowels.Size = new Size(200, 27);
            quaVowels.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(101, 177);
            label1.Name = "label1";
            label1.Size = new Size(150, 20);
            label1.TabIndex = 2;
            label1.Text = "Количество гласных";
            label1.Click += label1_Click_1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(101, 239);
            label2.Name = "label2";
            label2.Size = new Size(166, 20);
            label2.TabIndex = 3;
            label2.Text = "Количество согласных";
            // 
            // quaConsonant
            // 
            quaConsonant.Location = new Point(298, 232);
            quaConsonant.Name = "quaConsonant";
            quaConsonant.Size = new Size(200, 27);
            quaConsonant.TabIndex = 4;
            // 
            // quaWords
            // 
            quaWords.Location = new Point(298, 287);
            quaWords.Name = "quaWords";
            quaWords.Size = new Size(200, 27);
            quaWords.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(101, 294);
            label3.Name = "label3";
            label3.Size = new Size(126, 20);
            label3.TabIndex = 6;
            label3.Text = "Количество слов";
            // 
            // quaSent
            // 
            quaSent.Location = new Point(298, 339);
            quaSent.Name = "quaSent";
            quaSent.Size = new Size(200, 27);
            quaSent.TabIndex = 8;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(101, 346);
            label5.Name = "label5";
            label5.Size = new Size(191, 20);
            label5.TabIndex = 7;
            label5.Text = "Количество предложений";
            // 
            // stringLen
            // 
            stringLen.Location = new Point(298, 391);
            stringLen.Name = "stringLen";
            stringLen.Size = new Size(200, 27);
            stringLen.TabIndex = 10;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(101, 398);
            label4.Name = "label4";
            label4.Size = new Size(104, 20);
            label4.TabIndex = 9;
            label4.Text = "Длина строки";
            // 
            // btnCalc
            // 
            btnCalc.Location = new Point(620, 260);
            btnCalc.Name = "btnCalc";
            btnCalc.Size = new Size(94, 29);
            btnCalc.TabIndex = 11;
            btnCalc.Text = "рассчитать";
            btnCalc.UseVisualStyleBackColor = true;
            //
            // zadanye
            //
            zadanye.Location = new Point(620, 400);
            zadanye.Name = "textBOX";
            zadanye.Size = new Size(200, 27);
            zadanye.TabIndex = 12;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(900, 700);
            Controls.Add(btnCalc);
            Controls.Add(stringLen);
            Controls.Add(label4);
            Controls.Add(quaSent);
            Controls.Add(label5);
            Controls.Add(label3);
            Controls.Add(quaWords);
            Controls.Add(quaConsonant);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(quaVowels);
            Controls.Add(textInput);
            Controls.Add(zadanye);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textInput;
        private TextBox quaVowels;
        private Label label1;
        private Label label2;
        private TextBox quaConsonant;
        private TextBox quaWords;
        private Label label3;
        private TextBox quaSent;
        private Label label5;
        private TextBox stringLen;
        private Label label4;
        private Button btnCalc;
        private TextBox zadanye;
    }
}
