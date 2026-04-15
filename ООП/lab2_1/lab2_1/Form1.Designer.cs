namespace lab2_1
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            fioLabel = new Label();
            fioTextBox = new TextBox();
            ageLabel = new Label();
            birthdateLabel = new Label();
            birthdatePicker = new DateTimePicker();
            agePicker = new NumericUpDown();
            courseLaber = new Label();
            coursePicker = new NumericUpDown();
            specLabel = new Label();
            specComboBox = new ComboBox();
            groupLaber = new Label();
            groupTextBox = new TextBox();
            genderLabel = new Label();
            maleRadio = new RadioButton();
            femaleRadio = new RadioButton();
            isWorkingCheckBox = new CheckBox();
            jobLabel = new Label();
            companyTextBox = new TextBox();
            companyLabel = new Label();
            positionLabel = new Label();
            positionTextBox = new TextBox();
            experienceLabel = new Label();
            experiencePicker = new NumericUpDown();
            addressLabel = new Label();
            cityLabel = new Label();
            cityTextBox = new TextBox();
            indexLabel = new Label();
            indexTextBox = new TextBox();
            streetLabel = new Label();
            streetTextBox = new TextBox();
            buildLabel = new Label();
            buildTextBox = new TextBox();
            flatLabel = new Label();
            flatTextBox = new TextBox();
            studentListLabel = new Label();
            studentListBox = new ListBox();
            saveButton = new Button();
            loadButton = new Button();
            addButton = new Button();
            solveButton = new Button();
            errorProvider1 = new ErrorProvider(components);
            avgLabel = new Label();
            avgScorePicker = new NumericUpDown();
            menuStrip1 = new MenuStrip();
            файлToolStripMenuItem = new ToolStripMenuItem();
            сохранитьToolStripMenuItem = new ToolStripMenuItem();
            загрузитьToolStripMenuItem = new ToolStripMenuItem();
            поискToolStripMenuItem = new ToolStripMenuItem();
            полноеToolStripMenuItem = new ToolStripMenuItem();
            регулярноеВыражениеToolStripMenuItem = new ToolStripMenuItem();
            расширенныйПоискToolStripMenuItem = new ToolStripMenuItem();
            сортировкаToolStripMenuItem = new ToolStripMenuItem();
            поФИОАЯToolStripMenuItem = new ToolStripMenuItem();
            поФИОЯАToolStripMenuItem = new ToolStripMenuItem();
            поКурсуToolStripMenuItem = new ToolStripMenuItem();
            поСреднемуБаллуToolStripMenuItem = new ToolStripMenuItem();
            оПрограммеToolStripMenuItem = new ToolStripMenuItem();
            toolStrip1 = new ToolStrip();
            toolStripButton1 = new ToolStripButton();
            toolStripButton2 = new ToolStripButton();
            toolStripButton3 = new ToolStripButton();
            toolStripButton4 = new ToolStripButton();
            toolStripButton5 = new ToolStripButton();
            toolStripButton6 = new ToolStripButton();
            statusStrip1 = new StatusStrip();
            ((System.ComponentModel.ISupportInitialize)agePicker).BeginInit();
            ((System.ComponentModel.ISupportInitialize)coursePicker).BeginInit();
            ((System.ComponentModel.ISupportInitialize)experiencePicker).BeginInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)avgScorePicker).BeginInit();
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // fioLabel
            // 
            fioLabel.AutoSize = true;
            fioLabel.Location = new Point(25, 86);
            fioLabel.Name = "fioLabel";
            fioLabel.Size = new Size(45, 20);
            fioLabel.TabIndex = 0;
            fioLabel.Text = "ФИО:";
            // 
            // fioTextBox
            // 
            fioTextBox.Location = new Point(155, 79);
            fioTextBox.Name = "fioTextBox";
            fioTextBox.Size = new Size(379, 27);
            fioTextBox.TabIndex = 1;
            // 
            // ageLabel
            // 
            ageLabel.AutoSize = true;
            ageLabel.Location = new Point(25, 119);
            ageLabel.Name = "ageLabel";
            ageLabel.Size = new Size(67, 20);
            ageLabel.TabIndex = 2;
            ageLabel.Text = "Возраст:";
            // 
            // birthdateLabel
            // 
            birthdateLabel.AutoSize = true;
            birthdateLabel.Location = new Point(25, 152);
            birthdateLabel.Name = "birthdateLabel";
            birthdateLabel.Size = new Size(119, 20);
            birthdateLabel.TabIndex = 4;
            birthdateLabel.Text = "Дата рождения:";
            // 
            // birthdatePicker
            // 
            birthdatePicker.Location = new Point(155, 145);
            birthdatePicker.Name = "birthdatePicker";
            birthdatePicker.Size = new Size(379, 27);
            birthdatePicker.TabIndex = 5;
            // 
            // agePicker
            // 
            agePicker.Location = new Point(155, 112);
            agePicker.Name = "agePicker";
            agePicker.Size = new Size(150, 27);
            agePicker.TabIndex = 6;
            // 
            // courseLaber
            // 
            courseLaber.AutoSize = true;
            courseLaber.Location = new Point(26, 240);
            courseLaber.Name = "courseLaber";
            courseLaber.Size = new Size(44, 20);
            courseLaber.TabIndex = 7;
            courseLaber.Text = "Курс:";
            // 
            // coursePicker
            // 
            coursePicker.Location = new Point(155, 233);
            coursePicker.Maximum = new decimal(new int[] { 4, 0, 0, 0 });
            coursePicker.Name = "coursePicker";
            coursePicker.Size = new Size(150, 27);
            coursePicker.TabIndex = 8;
            // 
            // specLabel
            // 
            specLabel.AutoSize = true;
            specLabel.Location = new Point(25, 269);
            specLabel.Name = "specLabel";
            specLabel.Size = new Size(119, 20);
            specLabel.TabIndex = 9;
            specLabel.Text = "Специальность:";
            // 
            // specComboBox
            // 
            specComboBox.FormattingEnabled = true;
            specComboBox.Location = new Point(155, 266);
            specComboBox.Name = "specComboBox";
            specComboBox.Size = new Size(379, 28);
            specComboBox.TabIndex = 10;
            specComboBox.SelectedIndexChanged += specComboBox_SelectedIndexChanged;
            // 
            // groupLaber
            // 
            groupLaber.AutoSize = true;
            groupLaber.Location = new Point(25, 303);
            groupLaber.Name = "groupLaber";
            groupLaber.Size = new Size(61, 20);
            groupLaber.TabIndex = 11;
            groupLaber.Text = "Группа:";
            // 
            // groupTextBox
            // 
            groupTextBox.Location = new Point(155, 300);
            groupTextBox.Name = "groupTextBox";
            groupTextBox.Size = new Size(150, 27);
            groupTextBox.TabIndex = 12;
            // 
            // genderLabel
            // 
            genderLabel.AutoSize = true;
            genderLabel.Location = new Point(25, 186);
            genderLabel.Name = "genderLabel";
            genderLabel.Size = new Size(40, 20);
            genderLabel.TabIndex = 13;
            genderLabel.Text = "Пол:";
            // 
            // maleRadio
            // 
            maleRadio.AutoSize = true;
            maleRadio.Location = new Point(155, 184);
            maleRadio.Name = "maleRadio";
            maleRadio.Size = new Size(95, 24);
            maleRadio.TabIndex = 14;
            maleRadio.TabStop = true;
            maleRadio.Text = "Мужчина";
            maleRadio.UseVisualStyleBackColor = true;
            maleRadio.CheckedChanged += maleRadio_CheckedChanged;
            // 
            // femaleRadio
            // 
            femaleRadio.AutoSize = true;
            femaleRadio.Location = new Point(269, 184);
            femaleRadio.Name = "femaleRadio";
            femaleRadio.Size = new Size(98, 24);
            femaleRadio.TabIndex = 15;
            femaleRadio.TabStop = true;
            femaleRadio.Text = "Женщина";
            femaleRadio.UseVisualStyleBackColor = true;
            femaleRadio.CheckedChanged += radioButton1_CheckedChanged;
            // 
            // isWorkingCheckBox
            // 
            isWorkingCheckBox.AutoSize = true;
            isWorkingCheckBox.Location = new Point(155, 363);
            isWorkingCheckBox.Name = "isWorkingCheckBox";
            isWorkingCheckBox.Size = new Size(93, 24);
            isWorkingCheckBox.TabIndex = 16;
            isWorkingCheckBox.Text = "Работает";
            isWorkingCheckBox.UseVisualStyleBackColor = true;
            // 
            // jobLabel
            // 
            jobLabel.AutoSize = true;
            jobLabel.Location = new Point(26, 396);
            jobLabel.Name = "jobLabel";
            jobLabel.Size = new Size(108, 20);
            jobLabel.TabIndex = 17;
            jobLabel.Text = "Место работы";
            // 
            // companyTextBox
            // 
            companyTextBox.Location = new Point(155, 428);
            companyTextBox.Name = "companyTextBox";
            companyTextBox.Size = new Size(379, 27);
            companyTextBox.TabIndex = 18;
            // 
            // companyLabel
            // 
            companyLabel.AutoSize = true;
            companyLabel.Location = new Point(26, 431);
            companyLabel.Name = "companyLabel";
            companyLabel.Size = new Size(84, 20);
            companyLabel.TabIndex = 19;
            companyLabel.Text = "Компания:";
            // 
            // positionLabel
            // 
            positionLabel.AutoSize = true;
            positionLabel.Location = new Point(27, 464);
            positionLabel.Name = "positionLabel";
            positionLabel.Size = new Size(89, 20);
            positionLabel.TabIndex = 20;
            positionLabel.Text = "Должность:";
            // 
            // positionTextBox
            // 
            positionTextBox.Location = new Point(155, 461);
            positionTextBox.Name = "positionTextBox";
            positionTextBox.Size = new Size(379, 27);
            positionTextBox.TabIndex = 21;
            // 
            // experienceLabel
            // 
            experienceLabel.AutoSize = true;
            experienceLabel.Location = new Point(27, 503);
            experienceLabel.Name = "experienceLabel";
            experienceLabel.Size = new Size(78, 20);
            experienceLabel.TabIndex = 22;
            experienceLabel.Text = "Стаж(лет):";
            // 
            // experiencePicker
            // 
            experiencePicker.Location = new Point(155, 496);
            experiencePicker.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            experiencePicker.Name = "experiencePicker";
            experiencePicker.Size = new Size(150, 27);
            experiencePicker.TabIndex = 23;
            // 
            // addressLabel
            // 
            addressLabel.AutoSize = true;
            addressLabel.Location = new Point(27, 548);
            addressLabel.Name = "addressLabel";
            addressLabel.Size = new Size(51, 20);
            addressLabel.TabIndex = 24;
            addressLabel.Text = "Адрес";
            // 
            // cityLabel
            // 
            cityLabel.AutoSize = true;
            cityLabel.Location = new Point(27, 582);
            cityLabel.Name = "cityLabel";
            cityLabel.Size = new Size(54, 20);
            cityLabel.TabIndex = 25;
            cityLabel.Text = "Город:";
            // 
            // cityTextBox
            // 
            cityTextBox.Location = new Point(155, 579);
            cityTextBox.Name = "cityTextBox";
            cityTextBox.Size = new Size(379, 27);
            cityTextBox.TabIndex = 26;
            // 
            // indexLabel
            // 
            indexLabel.AutoSize = true;
            indexLabel.Location = new Point(27, 615);
            indexLabel.Name = "indexLabel";
            indexLabel.Size = new Size(62, 20);
            indexLabel.TabIndex = 27;
            indexLabel.Text = "Индекс:";
            // 
            // indexTextBox
            // 
            indexTextBox.Location = new Point(155, 612);
            indexTextBox.Name = "indexTextBox";
            indexTextBox.Size = new Size(379, 27);
            indexTextBox.TabIndex = 28;
            // 
            // streetLabel
            // 
            streetLabel.AutoSize = true;
            streetLabel.Location = new Point(27, 648);
            streetLabel.Name = "streetLabel";
            streetLabel.Size = new Size(55, 20);
            streetLabel.TabIndex = 29;
            streetLabel.Text = "Улица:";
            // 
            // streetTextBox
            // 
            streetTextBox.Location = new Point(155, 645);
            streetTextBox.Name = "streetTextBox";
            streetTextBox.Size = new Size(379, 27);
            streetTextBox.TabIndex = 30;
            streetTextBox.TextChanged += streetTextBox_TextChanged;
            // 
            // buildLabel
            // 
            buildLabel.AutoSize = true;
            buildLabel.Location = new Point(27, 679);
            buildLabel.Name = "buildLabel";
            buildLabel.Size = new Size(42, 20);
            buildLabel.TabIndex = 32;
            buildLabel.Text = "Дом:";
            // 
            // buildTextBox
            // 
            buildTextBox.Location = new Point(155, 676);
            buildTextBox.Name = "buildTextBox";
            buildTextBox.Size = new Size(379, 27);
            buildTextBox.TabIndex = 33;
            // 
            // flatLabel
            // 
            flatLabel.AutoSize = true;
            flatLabel.Location = new Point(27, 712);
            flatLabel.Name = "flatLabel";
            flatLabel.Size = new Size(78, 20);
            flatLabel.TabIndex = 34;
            flatLabel.Text = "Квартира:";
            // 
            // flatTextBox
            // 
            flatTextBox.Location = new Point(155, 709);
            flatTextBox.Name = "flatTextBox";
            flatTextBox.Size = new Size(379, 27);
            flatTextBox.TabIndex = 35;
            // 
            // studentListLabel
            // 
            studentListLabel.AutoSize = true;
            studentListLabel.Location = new Point(18, 806);
            studentListLabel.Name = "studentListLabel";
            studentListLabel.Size = new Size(131, 20);
            studentListLabel.TabIndex = 36;
            studentListLabel.Text = "Список студентов";
            // 
            // studentListBox
            // 
            studentListBox.FormattingEnabled = true;
            studentListBox.Location = new Point(155, 806);
            studentListBox.Name = "studentListBox";
            studentListBox.Size = new Size(379, 144);
            studentListBox.TabIndex = 37;
            // 
            // saveButton
            // 
            saveButton.Location = new Point(239, 993);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(94, 29);
            saveButton.TabIndex = 38;
            saveButton.Text = "Сохранить";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // loadButton
            // 
            loadButton.Location = new Point(339, 993);
            loadButton.Name = "loadButton";
            loadButton.Size = new Size(94, 29);
            loadButton.TabIndex = 39;
            loadButton.Text = "Загрузить";
            loadButton.UseVisualStyleBackColor = true;
            loadButton.Click += loadButton_Click;
            // 
            // addButton
            // 
            addButton.Location = new Point(139, 993);
            addButton.Name = "addButton";
            addButton.Size = new Size(94, 29);
            addButton.TabIndex = 40;
            addButton.Text = "Добавить ";
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += addButton_Click;
            // 
            // solveButton
            // 
            solveButton.Location = new Point(440, 993);
            solveButton.Name = "solveButton";
            solveButton.Size = new Size(94, 29);
            solveButton.TabIndex = 41;
            solveButton.Text = "Расчитать";
            solveButton.UseVisualStyleBackColor = true;
            solveButton.Click += solveButton_Click;
            // 
            // errorProvider1
            // 
            errorProvider1.ContainerControl = this;
            // 
            // avgLabel
            // 
            avgLabel.AutoSize = true;
            avgLabel.Location = new Point(25, 340);
            avgLabel.Name = "avgLabel";
            avgLabel.Size = new Size(67, 20);
            avgLabel.TabIndex = 42;
            avgLabel.Text = "Ср балл:";
            // 
            // avgScorePicker
            // 
            avgScorePicker.DecimalPlaces = 1;
            avgScorePicker.Location = new Point(155, 333);
            avgScorePicker.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            avgScorePicker.Name = "avgScorePicker";
            avgScorePicker.Size = new Size(150, 27);
            avgScorePicker.TabIndex = 43;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { файлToolStripMenuItem, поискToolStripMenuItem, сортировкаToolStripMenuItem, оПрограммеToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 28);
            menuStrip1.TabIndex = 44;
            menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            файлToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { сохранитьToolStripMenuItem, загрузитьToolStripMenuItem });
            файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            файлToolStripMenuItem.Size = new Size(59, 24);
            файлToolStripMenuItem.Text = "Файл";
            // 
            // сохранитьToolStripMenuItem
            // 
            сохранитьToolStripMenuItem.Name = "сохранитьToolStripMenuItem";
            сохранитьToolStripMenuItem.Size = new Size(166, 26);
            сохранитьToolStripMenuItem.Text = "Сохранить";
            // 
            // загрузитьToolStripMenuItem
            // 
            загрузитьToolStripMenuItem.Name = "загрузитьToolStripMenuItem";
            загрузитьToolStripMenuItem.Size = new Size(166, 26);
            загрузитьToolStripMenuItem.Text = "Загрузить";
            // 
            // поискToolStripMenuItem
            // 
            поискToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { полноеToolStripMenuItem, регулярноеВыражениеToolStripMenuItem, расширенныйПоискToolStripMenuItem });
            поискToolStripMenuItem.Name = "поискToolStripMenuItem";
            поискToolStripMenuItem.Size = new Size(66, 24);
            поискToolStripMenuItem.Text = "Поиск";
            поискToolStripMenuItem.Click += поискToolStripMenuItem_Click;
            // 
            // полноеToolStripMenuItem
            // 
            полноеToolStripMenuItem.Name = "полноеToolStripMenuItem";
            полноеToolStripMenuItem.Size = new Size(257, 26);
            полноеToolStripMenuItem.Text = "Полное совпадение";
            // 
            // регулярноеВыражениеToolStripMenuItem
            // 
            регулярноеВыражениеToolStripMenuItem.Name = "регулярноеВыражениеToolStripMenuItem";
            регулярноеВыражениеToolStripMenuItem.Size = new Size(257, 26);
            регулярноеВыражениеToolStripMenuItem.Text = "Регулярное выражение";
            // 
            // расширенныйПоискToolStripMenuItem
            // 
            расширенныйПоискToolStripMenuItem.Name = "расширенныйПоискToolStripMenuItem";
            расширенныйПоискToolStripMenuItem.Size = new Size(257, 26);
            расширенныйПоискToolStripMenuItem.Text = "Расширенный поиск";
            // 
            // сортировкаToolStripMenuItem
            // 
            сортировкаToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { поФИОАЯToolStripMenuItem, поФИОЯАToolStripMenuItem, поКурсуToolStripMenuItem, поСреднемуБаллуToolStripMenuItem });
            сортировкаToolStripMenuItem.Name = "сортировкаToolStripMenuItem";
            сортировкаToolStripMenuItem.Size = new Size(106, 24);
            сортировкаToolStripMenuItem.Text = "Сортировка";
            // 
            // поФИОАЯToolStripMenuItem
            // 
            поФИОАЯToolStripMenuItem.Name = "поФИОАЯToolStripMenuItem";
            поФИОАЯToolStripMenuItem.Size = new Size(227, 26);
            поФИОАЯToolStripMenuItem.Text = "По ФИО (А-Я)";
            // 
            // поФИОЯАToolStripMenuItem
            // 
            поФИОЯАToolStripMenuItem.Name = "поФИОЯАToolStripMenuItem";
            поФИОЯАToolStripMenuItem.Size = new Size(227, 26);
            поФИОЯАToolStripMenuItem.Text = "По ФИО (Я-А)";
            // 
            // поКурсуToolStripMenuItem
            // 
            поКурсуToolStripMenuItem.Name = "поКурсуToolStripMenuItem";
            поКурсуToolStripMenuItem.Size = new Size(227, 26);
            поКурсуToolStripMenuItem.Text = "По курсу";
            // 
            // поСреднемуБаллуToolStripMenuItem
            // 
            поСреднемуБаллуToolStripMenuItem.Name = "поСреднемуБаллуToolStripMenuItem";
            поСреднемуБаллуToolStripMenuItem.Size = new Size(227, 26);
            поСреднемуБаллуToolStripMenuItem.Text = "По среднему баллу";
            // 
            // оПрограммеToolStripMenuItem
            // 
            оПрограммеToolStripMenuItem.Name = "оПрограммеToolStripMenuItem";
            оПрограммеToolStripMenuItem.Size = new Size(118, 24);
            оПрограммеToolStripMenuItem.Text = "О программе";
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new Size(20, 20);
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButton1, toolStripButton2, toolStripButton3, toolStripButton4, toolStripButton5, toolStripButton6 });
            toolStrip1.Location = new Point(0, 28);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(800, 27);
            toolStrip1.TabIndex = 45;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButton1.Image = (Image)resources.GetObject("toolStripButton1.Image");
            toolStripButton1.ImageTransparentColor = Color.Magenta;
            toolStripButton1.Name = "toolStripButton1";
            toolStripButton1.Size = new Size(56, 24);
            toolStripButton1.Text = "Поиск";
            // 
            // toolStripButton2
            // 
            toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButton2.Image = (Image)resources.GetObject("toolStripButton2.Image");
            toolStripButton2.ImageTransparentColor = Color.Magenta;
            toolStripButton2.Name = "toolStripButton2";
            toolStripButton2.Size = new Size(96, 24);
            toolStripButton2.Text = "Сортировка";
            // 
            // toolStripButton3
            // 
            toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButton3.Image = (Image)resources.GetObject("toolStripButton3.Image");
            toolStripButton3.ImageTransparentColor = Color.Magenta;
            toolStripButton3.Name = "toolStripButton3";
            toolStripButton3.Size = new Size(77, 24);
            toolStripButton3.Text = "Очистить";
            // 
            // toolStripButton4
            // 
            toolStripButton4.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButton4.Image = (Image)resources.GetObject("toolStripButton4.Image");
            toolStripButton4.ImageTransparentColor = Color.Magenta;
            toolStripButton4.Name = "toolStripButton4";
            toolStripButton4.Size = new Size(69, 24);
            toolStripButton4.Text = "Удалить";
            // 
            // toolStripButton5
            // 
            toolStripButton5.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButton5.Image = (Image)resources.GetObject("toolStripButton5.Image");
            toolStripButton5.ImageTransparentColor = Color.Magenta;
            toolStripButton5.Name = "toolStripButton5";
            toolStripButton5.Size = new Size(64, 24);
            toolStripButton5.Text = "Вперед";
            // 
            // toolStripButton6
            // 
            toolStripButton6.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButton6.Image = (Image)resources.GetObject("toolStripButton6.Image");
            toolStripButton6.ImageTransparentColor = Color.Magenta;
            toolStripButton6.Name = "toolStripButton6";
            toolStripButton6.Size = new Size(55, 24);
            toolStripButton6.Text = "Назад";
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Location = new Point(0, 1027);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(800, 22);
            statusStrip1.TabIndex = 46;
            statusStrip1.Text = "statusStrip1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 1049);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            Controls.Add(avgScorePicker);
            Controls.Add(avgLabel);
            Controls.Add(solveButton);
            Controls.Add(addButton);
            Controls.Add(loadButton);
            Controls.Add(saveButton);
            Controls.Add(studentListBox);
            Controls.Add(studentListLabel);
            Controls.Add(flatTextBox);
            Controls.Add(flatLabel);
            Controls.Add(buildTextBox);
            Controls.Add(buildLabel);
            Controls.Add(streetTextBox);
            Controls.Add(streetLabel);
            Controls.Add(indexTextBox);
            Controls.Add(indexLabel);
            Controls.Add(cityTextBox);
            Controls.Add(cityLabel);
            Controls.Add(addressLabel);
            Controls.Add(experiencePicker);
            Controls.Add(experienceLabel);
            Controls.Add(positionTextBox);
            Controls.Add(positionLabel);
            Controls.Add(companyLabel);
            Controls.Add(companyTextBox);
            Controls.Add(jobLabel);
            Controls.Add(isWorkingCheckBox);
            Controls.Add(femaleRadio);
            Controls.Add(maleRadio);
            Controls.Add(genderLabel);
            Controls.Add(groupTextBox);
            Controls.Add(groupLaber);
            Controls.Add(specComboBox);
            Controls.Add(specLabel);
            Controls.Add(coursePicker);
            Controls.Add(courseLaber);
            Controls.Add(agePicker);
            Controls.Add(birthdatePicker);
            Controls.Add(birthdateLabel);
            Controls.Add(ageLabel);
            Controls.Add(fioTextBox);
            Controls.Add(fioLabel);
            Controls.Add(menuStrip1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)agePicker).EndInit();
            ((System.ComponentModel.ISupportInitialize)coursePicker).EndInit();
            ((System.ComponentModel.ISupportInitialize)experiencePicker).EndInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            ((System.ComponentModel.ISupportInitialize)avgScorePicker).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label fioLabel;
        private TextBox fioTextBox;
        private Label ageLabel;
        private Label birthdateLabel;
        private DateTimePicker birthdatePicker;
        private NumericUpDown agePicker;
        private Label courseLaber;
        private NumericUpDown coursePicker;
        private Label specLabel;
        private ComboBox specComboBox;
        private Label groupLaber;
        private TextBox groupTextBox;
        private Label genderLabel;
        private RadioButton maleRadio;
        private RadioButton femaleRadio;
        private CheckBox isWorkingCheckBox;
        private Label jobLabel;
        private TextBox companyTextBox;
        private Label companyLabel;
        private Label positionLabel;
        private TextBox positionTextBox;
        private Label experienceLabel;
        private NumericUpDown experiencePicker;
        private Label addressLabel;
        private Label cityLabel;
        private TextBox cityTextBox;
        private Label indexLabel;
        private TextBox indexTextBox;
        private Label streetLabel;
        private TextBox streetTextBox;
        private Label buildLabel;
        private TextBox buildTextBox;
        private Label flatLabel;
        private TextBox flatTextBox;
        private Label studentListLabel;
        private ListBox studentListBox;
        private Button saveButton;
        private Button loadButton;
        private Button addButton;
        private Button solveButton;
        private ErrorProvider errorProvider1;
        private NumericUpDown avgScorePicker;
        private Label avgLabel;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem файлToolStripMenuItem;
        private ToolStripMenuItem сохранитьToolStripMenuItem;
        private ToolStripMenuItem загрузитьToolStripMenuItem;
        private ToolStripMenuItem поискToolStripMenuItem;
        private ToolStripMenuItem сортировкаToolStripMenuItem;
        private ToolStripMenuItem оПрограммеToolStripMenuItem;
        private ToolStripMenuItem полноеToolStripMenuItem;
        private ToolStripMenuItem регулярноеВыражениеToolStripMenuItem;
        private ToolStripMenuItem расширенныйПоискToolStripMenuItem;
        private ToolStripMenuItem поФИОАЯToolStripMenuItem;
        private ToolStripMenuItem поФИОЯАToolStripMenuItem;
        private ToolStripMenuItem поКурсуToolStripMenuItem;
        private ToolStripMenuItem поСреднемуБаллуToolStripMenuItem;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButton1;
        private ToolStripButton toolStripButton2;
        private ToolStripButton toolStripButton3;
        private ToolStripButton toolStripButton4;
        private ToolStripButton toolStripButton5;
        private ToolStripButton toolStripButton6;
        private StatusStrip statusStrip1;
    }
}
