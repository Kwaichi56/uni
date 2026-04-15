using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace UniversityApp
{
    public partial class MainForm : Form
    {
        
        private List<Student> students = new List<Student>();

       
        private const string XML_FILE_NAME = "students.xml";
        private string xmlFilePath;

        private TableLayoutPanel mainPanel;
        private Label lblFullName;
        private TextBox txtFullName;
        private Label lblAge;
        private NumericUpDown numAge;
        private Label lblSpeciality;
        private ComboBox cmbSpeciality;
        private Label lblBirthDate;
        private DateTimePicker dtpBirthDate;
        private Label lblCourse;
        private NumericUpDown numCourse;
        private Label lblGroup;
        private TextBox txtGroup;
        private Label lblAvgScore;
        private NumericUpDown numAvgScore;
        private Label lblGender;
        private FlowLayoutPanel genderPanel;
        private RadioButton rbtnMale;
        private RadioButton rbtnFemale;
        private Label lblJob;
        private TableLayoutPanel jobPanel;
        private CheckBox chkHasJob;
        private Label lblCompany;
        private TextBox txtCompany;
        private Label lblPosition;
        private TextBox txtPosition;
        private Label lblExperience;
        private NumericUpDown numExperience;
        private Label lblAddress;
        private TableLayoutPanel addressPanel;
        private Label lblCity;
        private TextBox txtCity;
        private Label lblZip;
        private TextBox txtZip;
        private Label lblStreet;
        private TextBox txtStreet;
        private Label lblHouse;
        private TextBox txtHouse;
        private Label lblApartment;
        private TextBox txtApartment;
        private Label lblStudentsList;
        private ListBox lstStudents;
        private FlowLayoutPanel buttonPanel;
        private Button btnAdd;
        private Button btnSave;
        private Button btnLoad;
        private Button btnCalculate;
        private ErrorProvider errorProvider;

        public MainForm()
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            xmlFilePath = Path.Combine(exeDirectory, XML_FILE_NAME);

            InitializeComponent();
            SetupAgeDateSync();
        }

        private void InitializeComponent()
        {
          
            this.Text = "Университет: данные студента";
            this.Size = new Size(700, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            // 
            // mainPanel 
            // 
            mainPanel = new TableLayoutPanel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.ColumnCount = 2;
            mainPanel.Padding = new Padding(10);
            mainPanel.AutoSize = true;
            // Настройка колонок
            ColumnStyle colStyle1 = new ColumnStyle(SizeType.Percent, 30F);
            ColumnStyle colStyle2 = new ColumnStyle(SizeType.Percent, 70F);
            mainPanel.ColumnStyles.Add(colStyle1);
            mainPanel.ColumnStyles.Add(colStyle2);
            // 
            // 
            // lblFullName 
            // 
            lblFullName = new Label();
            lblFullName.Text = "ФИО:";
            lblFullName.Anchor = AnchorStyles.Left;
            // 
            // txtFullName 
            // 
            txtFullName = new TextBox();
            txtFullName.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            mainPanel.Controls.Add(lblFullName, 0, 0);
            mainPanel.Controls.Add(txtFullName, 1, 0);
            // 
            // lblAge 
            // 
            lblAge = new Label();
            lblAge.Text = "Возраст:";
            lblAge.Anchor = AnchorStyles.Left;
            // 
            // numAge 
            // 
            numAge = new NumericUpDown();
            numAge.Minimum = 16;
            numAge.Maximum = 100;
            numAge.Value = 18;
            numAge.Anchor = AnchorStyles.Left;

            mainPanel.Controls.Add(lblAge, 0, 1);
            mainPanel.Controls.Add(numAge, 1, 1);
            // 
            // lblSpeciality 
            // 
            lblSpeciality = new Label();
            lblSpeciality.Text = "Специальность:";
            lblSpeciality.Anchor = AnchorStyles.Left;
            // 
            // cmbSpeciality 
            // 
            cmbSpeciality = new ComboBox();
            cmbSpeciality.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSpeciality.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cmbSpeciality.Items.AddRange(new[] { "Программная инженерия", "Информационные системы", "Прикладная математика", "Экономика" });
            cmbSpeciality.SelectedIndex = 0;

            mainPanel.Controls.Add(lblSpeciality, 0, 2);
            mainPanel.Controls.Add(cmbSpeciality, 1, 2);
            // 
            // lblBirthDate 
            // 
            lblBirthDate = new Label();
            lblBirthDate.Text = "Дата рождения:";
            lblBirthDate.Anchor = AnchorStyles.Left;
            // 
            // dtpBirthDate 
            // 
            dtpBirthDate = new DateTimePicker();
            dtpBirthDate.MaxDate = DateTime.Today;
            dtpBirthDate.Value = DateTime.Today.AddYears(-18);
            dtpBirthDate.Anchor = AnchorStyles.Left;

            mainPanel.Controls.Add(lblBirthDate, 0, 3);
            mainPanel.Controls.Add(dtpBirthDate, 1, 3);
            // 
            // lblCourse 
            // 
            lblCourse = new Label();
            lblCourse.Text = "Курс:";
            lblCourse.Anchor = AnchorStyles.Left;
            // 
            // numCourse 
            // 
            numCourse = new NumericUpDown();
            numCourse.Minimum = 1;
            numCourse.Maximum = 4;
            numCourse.Value = 1;
            numCourse.Anchor = AnchorStyles.Left;

            mainPanel.Controls.Add(lblCourse, 0, 4);
            mainPanel.Controls.Add(numCourse, 1, 4);
            // 
            // lblGroup
            // 
            lblGroup = new Label();
            lblGroup.Text = "Группа:";
            lblGroup.Anchor = AnchorStyles.Left;
            // 
            // txtGroup 
            // 
            txtGroup = new TextBox();
            txtGroup.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            mainPanel.Controls.Add(lblGroup, 0, 5);
            mainPanel.Controls.Add(txtGroup, 1, 5);
            // 
            // lblAvgScore 
            // 
            lblAvgScore = new Label();
            lblAvgScore.Text = "Средний балл:";
            lblAvgScore.Anchor = AnchorStyles.Left;
            // 
            // numAvgScore 
            // 
            numAvgScore = new NumericUpDown();
            numAvgScore.Minimum = 2;
            numAvgScore.Maximum = 10;
            numAvgScore.DecimalPlaces = 1;
            numAvgScore.Increment = 0.1m;
            numAvgScore.Value = 4.0m;
            numAvgScore.Anchor = AnchorStyles.Left;

            mainPanel.Controls.Add(lblAvgScore, 0, 6);
            mainPanel.Controls.Add(numAvgScore, 1, 6);
            // 
            // lblGender 
            // 
            lblGender = new Label();
            lblGender.Text = "Пол:";
            lblGender.Anchor = AnchorStyles.Left;
            // 
            // genderPanel 
            // 
            genderPanel = new FlowLayoutPanel();
            genderPanel.FlowDirection = FlowDirection.LeftToRight;
            genderPanel.AutoSize = true;
            // 
            // rbtnMale 
            // 
            rbtnMale = new RadioButton();
            rbtnMale.Text = "Мужской";
            rbtnMale.Checked = true;
            // 
            // rbtnFemale 
            // 
            rbtnFemale = new RadioButton();
            rbtnFemale.Text = "Женский";

            genderPanel.Controls.Add(rbtnMale);
            genderPanel.Controls.Add(rbtnFemale);

            mainPanel.Controls.Add(lblGender, 0, 7);
            mainPanel.Controls.Add(genderPanel, 1, 7);
            // 
            // lblJob 
            // 
            lblJob = new Label();
            lblJob.Text = "Место работы:";
            lblJob.Anchor = AnchorStyles.Left;
            // 
            // jobPanel
            // 
            jobPanel = new TableLayoutPanel();
            jobPanel.ColumnCount = 2;
            jobPanel.AutoSize = true;

            ColumnStyle jobColStyle1 = new ColumnStyle(SizeType.AutoSize);
            ColumnStyle jobColStyle2 = new ColumnStyle(SizeType.AutoSize);
            jobPanel.ColumnStyles.Add(jobColStyle1);
            jobPanel.ColumnStyles.Add(jobColStyle2);
            // 
            // chkHasJob 
            // 
            chkHasJob = new CheckBox();
            chkHasJob.Text = "Работает";
            chkHasJob.Checked = false;
            chkHasJob.AutoSize = true;
            chkHasJob.CheckedChanged += ChkHasJob_CheckedChanged;
            // 
            // lblCompany
            // 
            lblCompany = new Label();
            lblCompany.Text = "Компания:";
            lblCompany.AutoSize = true;
            // 
            // txtCompany 
            // 
            txtCompany = new TextBox();
            txtCompany.Width = 150;
            txtCompany.Enabled = false;
            // 
            // lblPosition 
            // 
            lblPosition = new Label();
            lblPosition.Text = "Должность:";
            lblPosition.AutoSize = true;
            // 
            // txtPosition 
            // 
            txtPosition = new TextBox();
            txtPosition.Width = 150;
            txtPosition.Enabled = false;
            // 
            // lblExperience 
            // 
            lblExperience = new Label();
            lblExperience.Text = "Стаж (лет):";
            lblExperience.AutoSize = true;
            // 
            // numExperience 
            // 
            numExperience = new NumericUpDown();
            numExperience.Minimum = 0;
            numExperience.Maximum = 50;
            numExperience.Value = 0;
            numExperience.Width = 80;
            numExperience.Enabled = false;

            jobPanel.Controls.Add(chkHasJob, 0, 0);
            jobPanel.SetColumnSpan(chkHasJob, 2);

            jobPanel.Controls.Add(lblCompany, 0, 1);
            jobPanel.Controls.Add(txtCompany, 1, 1);

            jobPanel.Controls.Add(lblPosition, 0, 2);
            jobPanel.Controls.Add(txtPosition, 1, 2);

            jobPanel.Controls.Add(lblExperience, 0, 3);
            jobPanel.Controls.Add(numExperience, 1, 3);

            mainPanel.Controls.Add(lblJob, 0, 8);
            mainPanel.Controls.Add(jobPanel, 1, 8);
            // 
            // lblAddress 
            // 
            lblAddress = new Label();
            lblAddress.Text = "Адрес:";
            lblAddress.Anchor = AnchorStyles.Left;
            // 
            // addressPanel 
            // 
            addressPanel = new TableLayoutPanel();
            addressPanel.ColumnCount = 2;
            addressPanel.AutoSize = true;

            ColumnStyle addrColStyle1 = new ColumnStyle(SizeType.AutoSize);
            ColumnStyle addrColStyle2 = new ColumnStyle(SizeType.AutoSize);
            addressPanel.ColumnStyles.Add(addrColStyle1);
            addressPanel.ColumnStyles.Add(addrColStyle2);
            // 
            // lblCity 
            // 
            lblCity = new Label();
            lblCity.Text = "Город:";
            lblCity.AutoSize = true;
            // 
            // txtCity 
            // 
            txtCity = new TextBox();
            txtCity.Width = 150;

            // 
            // lblZip 
            // 
            lblZip = new Label();
            lblZip.Text = "Индекс:";
            lblZip.AutoSize = true;
            // 
            // txtZip
            // 
            txtZip = new TextBox();
            txtZip.Width = 150;
            // 
            // lblStreet 
            // 
            lblStreet = new Label();
            lblStreet.Text = "Улица:";
            lblStreet.AutoSize = true;
            // 
            // txtStreet 
            // 
            txtStreet = new TextBox();
            txtStreet.Width = 150;
            // 
            // lblHouse 
            // 
            lblHouse = new Label();
            lblHouse.Text = "Дом:";
            lblHouse.AutoSize = true;
            // 
            // txtHouse 
            // 
            txtHouse = new TextBox();
            txtHouse.Width = 150;
            // 
            // lblApartment 
            // 
            lblApartment = new Label();
            lblApartment.Text = "Квартира:";
            lblApartment.AutoSize = true;
            // 
            // txtApartment 
            // 
            txtApartment = new TextBox();
            txtApartment.Width = 150;

            addressPanel.Controls.Add(lblCity, 0, 0);
            addressPanel.Controls.Add(txtCity, 1, 0);
            addressPanel.Controls.Add(lblZip, 0, 1);
            addressPanel.Controls.Add(txtZip, 1, 1);
            addressPanel.Controls.Add(lblStreet, 0, 2);
            addressPanel.Controls.Add(txtStreet, 1, 2);
            addressPanel.Controls.Add(lblHouse, 0, 3);
            addressPanel.Controls.Add(txtHouse, 1, 3);
            addressPanel.Controls.Add(lblApartment, 0, 4);
            addressPanel.Controls.Add(txtApartment, 1, 4);

            mainPanel.Controls.Add(lblAddress, 0, 9);
            mainPanel.Controls.Add(addressPanel, 1, 9);

            // 
            // lblStudentsList
            // 
            lblStudentsList = new Label();
            lblStudentsList.Text = "Список студентов:";
            lblStudentsList.Anchor = AnchorStyles.Left;
            // 
            // lstStudents 
            // 
            lstStudents = new ListBox();
            lstStudents.Dock = DockStyle.Fill;
            lstStudents.Height = 150;

            mainPanel.Controls.Add(lblStudentsList, 0, 10);
            mainPanel.Controls.Add(lstStudents, 1, 10);
            // 
            // buttonPanel 
            // 
            buttonPanel = new FlowLayoutPanel();
            buttonPanel.FlowDirection = FlowDirection.LeftToRight;
            buttonPanel.AutoSize = true;
            // 
            // btnAdd
            // 
            btnAdd = new Button();
            btnAdd.Text = "Добавить студента";
            btnAdd.AutoSize = true;
            btnAdd.Click += BtnAdd_Click;
            // 
            // btnSave
            // 
            btnSave = new Button();
            btnSave.Text = "Сохранить в XML";
            btnSave.AutoSize = true;
            btnSave.Click += BtnSave_Click;
            // 
            // btnLoad 
            // 
            btnLoad = new Button();
            btnLoad.Text = "Загрузить из XML";
            btnLoad.AutoSize = true;
            btnLoad.Click += BtnLoad_Click;
            // 
            // btnCalculate 
            // 
            btnCalculate = new Button();
            btnCalculate.Text = "Рассчитать бюджет";
            btnCalculate.AutoSize = true;
            btnCalculate.Click += BtnCalculate_Click;

            buttonPanel.Controls.Add(btnAdd);
            buttonPanel.Controls.Add(btnSave);
            buttonPanel.Controls.Add(btnLoad);
            buttonPanel.Controls.Add(btnCalculate);

            mainPanel.Controls.Add(buttonPanel, 1, 11);
            // 
            // errorProvider 
            // 
            errorProvider = new ErrorProvider();

           
            this.Controls.Add(mainPanel);
        }

        private void SetupAgeDateSync()
        {

            numAge.ValueChanged += NumAge_ValueChanged;
            dtpBirthDate.ValueChanged += DtpBirthDate_ValueChanged;
        }

        private void NumAge_ValueChanged(object sender, EventArgs e)
        {
            DateTime newBirthDate = DateTime.Today.AddYears(-(int)numAge.Value);
            if (newBirthDate > DateTime.Today)
                newBirthDate = DateTime.Today;

            // Временно отключаем обработчик, чтобы избежать зацикливания
            dtpBirthDate.ValueChanged -= DtpBirthDate_ValueChanged;
            dtpBirthDate.Value = newBirthDate;
            dtpBirthDate.ValueChanged += DtpBirthDate_ValueChanged;
        }

        private void DtpBirthDate_ValueChanged(object sender, EventArgs e)
        {
            int calculatedAge = DateTime.Today.Year - dtpBirthDate.Value.Year;
            if (DateTime.Today < dtpBirthDate.Value.AddYears(calculatedAge))
                calculatedAge--;

            if (calculatedAge >= 16 && calculatedAge <= 100)
            {
                // Временно отключаем обработчик, чтобы избежать зацикливания
                numAge.ValueChanged -= NumAge_ValueChanged;
                numAge.Value = calculatedAge;
                numAge.ValueChanged += NumAge_ValueChanged;
            }
        }

        private void ChkHasJob_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = chkHasJob.Checked;
            txtCompany.Enabled = enabled;
            txtPosition.Enabled = enabled;
            numExperience.Enabled = enabled;

            if (!enabled)
            {
                txtCompany.Clear();
                txtPosition.Clear();
                numExperience.Value = 0;
            }
        }

        // Валидация полей
        private bool ValidateStudent()
        {
            bool isValid = true;
            errorProvider.Clear();

            // ФИО
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                errorProvider.SetError(txtFullName, "Введите ФИО");
                isValid = false;
            }

            // Возраст в допустимых пределах
            if (numAge.Value < 16 || numAge.Value > 60)
            {
                errorProvider.SetError(numAge, "Возраст должен быть от 16 до 60");
                isValid = false;
            }

            // Проверка соответствия возраста и даты рождения
            int ageFromBirthDate = DateTime.Today.Year - dtpBirthDate.Value.Year;
            if (DateTime.Today < dtpBirthDate.Value.AddYears(ageFromBirthDate))
                ageFromBirthDate--;

            if ((int)numAge.Value != ageFromBirthDate)
            {
                errorProvider.SetError(dtpBirthDate, "Возраст не соответствует дате рождения");
                errorProvider.SetError(numAge, "Возраст не соответствует дате рождения");
                isValid = false;
            }

            // Специальность
            if (cmbSpeciality.SelectedItem == null)
            {
                errorProvider.SetError(cmbSpeciality, "Выберите специальность");
                isValid = false;
            }

            // Дата рождения не в будущем
            if (dtpBirthDate.Value > DateTime.Today)
            {
                errorProvider.SetError(dtpBirthDate, "Дата рождения не может быть в будущем");
                isValid = false;
            }

            // Курс
            if (numCourse.Value < 1 || numCourse.Value > 4)
            {
                errorProvider.SetError(numCourse, "Курс от 1 до 4");
                isValid = false;
            }

            // Группа
            if (string.IsNullOrWhiteSpace(txtGroup.Text))
            {
                errorProvider.SetError(txtGroup, "Введите группу");
                isValid = false;
            }

            // Средний балл
            if (numAvgScore.Value < 2 || numAvgScore.Value > 10)
            {
                errorProvider.SetError(numAvgScore, "Средний балл от 2 до 10");
                isValid = false;
            }

            // Если отмечена работа, проверяем поля работы
            if (chkHasJob.Checked)
            {
                if (string.IsNullOrWhiteSpace(txtCompany.Text))
                {
                    errorProvider.SetError(txtCompany, "Введите компанию");
                    isValid = false;
                }
                if (string.IsNullOrWhiteSpace(txtPosition.Text))
                {
                    errorProvider.SetError(txtPosition, "Введите должность");
                    isValid = false;
                }
                if (numExperience.Value < 0)
                {
                    errorProvider.SetError(numExperience, "Стаж не может быть отрицательным");
                    isValid = false;
                }
            }

            return isValid;
        }

        // Создание объекта Student из полей формы
        private Student GetStudentFromForm()
        {
            Student student = new Student();
            student.FullName = txtFullName.Text.Trim();
            student.Age = (int)numAge.Value;
            student.Speciality = cmbSpeciality.SelectedItem.ToString();
            student.BirthDate = dtpBirthDate.Value;
            student.Course = (int)numCourse.Value;
            student.Group = txtGroup.Text.Trim();
            student.AverageScore = (double)numAvgScore.Value;
            student.Gender = rbtnMale.Checked ? Gender.Male : Gender.Female;

            student.Address = new Address();
            student.Address.City = txtCity.Text.Trim();
            student.Address.Zip = txtZip.Text.Trim();
            student.Address.Street = txtStreet.Text.Trim();
            student.Address.House = txtHouse.Text.Trim();
            student.Address.Apartment = txtApartment.Text.Trim();

            if (chkHasJob.Checked)
            {
                student.Job = new Job();
                student.Job.Company = txtCompany.Text.Trim();
                student.Job.Position = txtPosition.Text.Trim();
                student.Job.Experience = (int)numExperience.Value;
            }

            return student;
        }

        // Очистка полей после добавления
        private void ClearForm()
        {
            txtFullName.Clear();
            numAge.Value = 18;
            cmbSpeciality.SelectedIndex = 0;
            dtpBirthDate.Value = DateTime.Today.AddYears(-18);
            numCourse.Value = 1;
            txtGroup.Clear();
            numAvgScore.Value = 4.0m;
            rbtnMale.Checked = true;
            chkHasJob.Checked = false;
            txtCity.Clear();
            txtZip.Clear();
            txtStreet.Clear();
            txtHouse.Clear();
            txtApartment.Clear();
        }

        // Добавление студента
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateStudent())
                return;

            Student student = GetStudentFromForm();
            students.Add(student);
            lstStudents.Items.Add(student.ToString());
            ClearForm();
        }

        // Сохранение в XML (автоматически в файл students.xml в папке с программой)
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (students.Count == 0)
            {
                MessageBox.Show("Нет студентов для сохранения.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                XmlHelper.Serialize(students, xmlFilePath);
                MessageBox.Show($"Список студентов сохранён в файл:\n{xmlFilePath}",
                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Загрузка из XML (автоматически из файла students.xml в папке с программой)
        private void BtnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(xmlFilePath))
                {
                    students = XmlHelper.Deserialize<List<Student>>(xmlFilePath);
                    lstStudents.Items.Clear();
                    foreach (var s in students)
                        lstStudents.Items.Add(s.ToString());
                    MessageBox.Show($"Список студентов загружен из файла:\n{xmlFilePath}",
                        "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Файл {XML_FILE_NAME} не найден.\nОжидаемый путь: {xmlFilePath}",
                        "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Расчёт бюджета университета
        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            if (students.Count == 0)
            {
                MessageBox.Show("Нет данных для расчёта бюджета.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            double totalBudget = 0;
            foreach (var s in students)
            {
                totalBudget += s.AverageScore * 1000 + s.Course * 1500;
            }

            MessageBox.Show($"Общий бюджет университета: {totalBudget:C}",
                "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}