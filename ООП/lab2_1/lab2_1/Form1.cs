using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace lab2_1
{
    public partial class Form1 : Form
    {
        private List<Student> students = new List<Student>();
        private const string XML_FILE_NAME = "students.xml";
        private string xmlFilePath;

        private ToolStripStatusLabel statusCountLabel;
        private ToolStripStatusLabel statusLastActionLabel;
        private ToolStripStatusLabel statusDateTimeLabel;
        private System.Windows.Forms.Timer clockTimer;

        private int currentIndex = -1;
        public Form1()
        {
            InitializeComponent();
            xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, XML_FILE_NAME);
            SetupControls();
            SubscribeEvents();
            SetupStatusStrip();
            SetupToolbarContextMenu();
        }

        private void SetupStatusStrip()
        {
            statusCountLabel = new ToolStripStatusLabel("Студентов: 0");
            statusLastActionLabel = new ToolStripStatusLabel("Ожидание...");
            var spring = new ToolStripStatusLabel { Spring = true };
            statusDateTimeLabel = new ToolStripStatusLabel(
                DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));

            statusStrip1.Items.AddRange(new ToolStripItem[]
            {
                statusCountLabel,
                new ToolStripSeparator(),
                statusLastActionLabel,
                spring,
                statusDateTimeLabel
            });

            clockTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            clockTimer.Tick += (s, e) =>
                statusDateTimeLabel.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            clockTimer.Start();
        }

        private void SetupToolbarContextMenu()
        {
            var stripMenu = new ContextMenuStrip();
            var hideItem = new ToolStripMenuItem("Скрыть панель инструментов");
            hideItem.Click += (s, e) => { toolStrip1.Visible = false; UpdateStatus("Панель скрыта"); };
            stripMenu.Items.Add(hideItem);
            toolStrip1.ContextMenuStrip = stripMenu;

            var formMenu = new ContextMenuStrip();
            var showItem = new ToolStripMenuItem("Показать панель инструментов");
            showItem.Click += (s, e) => { toolStrip1.Visible = true; UpdateStatus("Панель показана"); };
            formMenu.Items.Add(showItem);
            this.ContextMenuStrip = formMenu;
        }

        private void SetupControls()
        {
            specComboBox.Items.Clear();
            specComboBox.Items.AddRange(new[]
            {
                "Программная инженерия", "Информационные системы",
                "Прикладная математика", "Экономика"
            });
            specComboBox.SelectedIndex = 0;

            agePicker.Minimum = 16; agePicker.Maximum = 60; agePicker.Value = 18;
            coursePicker.Minimum = 1; coursePicker.Maximum = 4; coursePicker.Value = 1;

            if (avgScorePicker != null)
            {
                avgScorePicker.Minimum = 0; avgScorePicker.Maximum = 10;
                avgScorePicker.DecimalPlaces = 1; avgScorePicker.Increment = 0.1m;
                avgScorePicker.Value = 4.0m;
            }

            experiencePicker.Minimum = 0; experiencePicker.Maximum = 50; experiencePicker.Value = 0;
            birthdatePicker.MaxDate = DateTime.Today;
            birthdatePicker.Value = DateTime.Today.AddYears(-18);
            SetJobFieldsEnabled(false);
        }

        private void SubscribeEvents()
        {
            isWorkingCheckBox.CheckedChanged += IsWorkingCheckBox_CheckedChanged;
            agePicker.ValueChanged += AgePicker_ValueChanged;
            birthdatePicker.ValueChanged += BirthdatePicker_ValueChanged;
            this.Load += Form1_Load;

            сохранитьToolStripMenuItem.Click += saveButton_Click;
            загрузитьToolStripMenuItem.Click += loadButton_Click;

            полноеToolStripMenuItem.Click += FullSearchMenuItem_Click;
            регулярноеВыражениеToolStripMenuItem.Click += RegexSearchMenuItem_Click;
            расширенныйПоискToolStripMenuItem.Click += AdvancedSearchMenuItem_Click;

            поФИОАЯToolStripMenuItem.Click += SortByNameAscMenuItem_Click;
            поФИОЯАToolStripMenuItem.Click += SortByNameDescMenuItem_Click;
            поКурсуToolStripMenuItem.Click += SortByCourseMenuItem_Click;
            поСреднемуБаллуToolStripMenuItem.Click += SortByScoreMenuItem_Click;

            оПрограммеToolStripMenuItem.Click += AboutMenuItem_Click;

            toolStripButton1.Click += (s, e) => FullSearchMenuItem_Click(s, e);
            toolStripButton2.Click += (s, e) => SortByNameAscMenuItem_Click(s, e);
            toolStripButton3.Click += (s, e) => ClearToolStripButton_Click(s, e);
            toolStripButton4.Click += (s, e) => DeleteToolStripButton_Click(s, e);
            toolStripButton5.Click += (s, e) => NextToolStripButton_Click(s, e);
            toolStripButton6.Click += (s, e) => PrevToolStripButton_Click(s, e);
        }


        private void SetJobFieldsEnabled(bool enabled)
        {
            companyTextBox.Enabled = positionTextBox.Enabled = experiencePicker.Enabled = enabled;
            if (!enabled)
            { companyTextBox.Clear(); positionTextBox.Clear(); experiencePicker.Value = 0; }
        }

        private void UpdateStatus(string action)
        {
            statusCountLabel.Text = $"Студентов: {students.Count}";
            statusLastActionLabel.Text = $"Последнее действие: {action}";
        }

        private void RefreshListBox(IEnumerable<Student> source = null)
        {
            studentListBox.Items.Clear();
            foreach (var s in source ?? students)
                studentListBox.Items.Add(s.ToString());
        }

        private string ShowInputBox(string prompt, string title)
        {
            using (var f = new Form
            {
                Text = title,
                Width = 420,
                Height = 160,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false
            })
            {
                var lbl = new Label { Left = 12, Top = 12, Width = 390, Text = prompt };
                var tb = new TextBox { Left = 12, Top = 40, Width = 390 };
                var ok = new Button { Text = "OK", Left = 225, Top = 75, Width = 80, DialogResult = DialogResult.OK };
                var cl = new Button { Text = "Отмена", Left = 315, Top = 75, Width = 85, DialogResult = DialogResult.Cancel };
                f.Controls.AddRange(new Control[] { lbl, tb, ok, cl });
                f.AcceptButton = ok; f.CancelButton = cl;
                return f.ShowDialog(this) == DialogResult.OK ? tb.Text : null;
            }
        }


        private void IsWorkingCheckBox_CheckedChanged(object sender, EventArgs e)
            => SetJobFieldsEnabled(isWorkingCheckBox.Checked);

        private void AgePicker_ValueChanged(object sender, EventArgs e)
        {
            var nb = DateTime.Today.AddYears(-(int)agePicker.Value);
            birthdatePicker.ValueChanged -= BirthdatePicker_ValueChanged;
            birthdatePicker.Value = nb > DateTime.Today ? DateTime.Today : nb;
            birthdatePicker.ValueChanged += BirthdatePicker_ValueChanged;
        }

        private void BirthdatePicker_ValueChanged(object sender, EventArgs e)
        {
            int age = DateTime.Today.Year - birthdatePicker.Value.Year;
            if (DateTime.Today < birthdatePicker.Value.AddYears(age)) age--;
            if (age >= 16 && age <= 60)
            {
                agePicker.ValueChanged -= AgePicker_ValueChanged;
                agePicker.Value = age;
                agePicker.ValueChanged += AgePicker_ValueChanged;
            }
        }

        private void Form1_Load(object sender, EventArgs e) { }
        private void specComboBox_SelectedIndexChanged(object sender, EventArgs e) { }
        private void maleRadio_CheckedChanged(object sender, EventArgs e) { }
        private void radioButton1_CheckedChanged(object sender, EventArgs e) { }
        private void streetTextBox_TextChanged(object sender, EventArgs e) { }
        private void поискToolStripMenuItem_Click(object sender, EventArgs e) { }

        
        private bool ValidateStudent()
        {
            errorProvider1.Clear();
            bool ok = true;

            if (string.IsNullOrWhiteSpace(fioTextBox.Text))
            { errorProvider1.SetError(fioTextBox, "Введите ФИО"); ok = false; }
            else if (!Regex.IsMatch(fioTextBox.Text.Trim(), @"^[А-ЯЁа-яёA-Za-z\s\-]{2,100}$"))
            { errorProvider1.SetError(fioTextBox, "ФИО: только буквы (2-100 символов)"); ok = false; }

            int ageFromDate = DateTime.Today.Year - birthdatePicker.Value.Year;
            if (DateTime.Today < birthdatePicker.Value.AddYears(ageFromDate)) ageFromDate--;
            if ((int)agePicker.Value != ageFromDate)
            {
                errorProvider1.SetError(agePicker, "Возраст не соответствует дате рождения");
                errorProvider1.SetError(birthdatePicker, "Дата не соответствует возрасту");
                ok = false;
            }

            if (specComboBox.SelectedItem == null)
            { errorProvider1.SetError(specComboBox, "Выберите специальность"); ok = false; }

            if (string.IsNullOrWhiteSpace(groupTextBox.Text))
            { errorProvider1.SetError(groupTextBox, "Введите группу"); ok = false; }

            if (isWorkingCheckBox.Checked)
            {
                if (string.IsNullOrWhiteSpace(companyTextBox.Text))
                { errorProvider1.SetError(companyTextBox, "Введите компанию"); ok = false; }
                if (string.IsNullOrWhiteSpace(positionTextBox.Text))
                { errorProvider1.SetError(positionTextBox, "Введите должность"); ok = false; }
            }

            // DataAnnotations валидация
            if (ok)
            {
                var student = GetStudentFromForm();
                var ctx = new ValidationContext(student);
                var results = new List<ValidationResult>();
                if (!Validator.TryValidateObject(student, ctx, results, true))
                {
                    var sb = new StringBuilder();
                    foreach (var r in results) sb.AppendLine("• " + r.ErrorMessage);
                    MessageBox.Show(sb.ToString(), "Ошибки валидации",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ok = false;
                }
            }
            return ok;
        }


        private Student GetStudentFromForm() => new Student
        {
            FullName = fioTextBox.Text.Trim(),
            Age = (int)agePicker.Value,
            Speciality = specComboBox.SelectedItem?.ToString(),
            BirthDate = birthdatePicker.Value,
            Course = (int)coursePicker.Value,
            Group = groupTextBox.Text.Trim(),
            AverageScore = avgScorePicker != null ? (double)avgScorePicker.Value : 4.0,
            Gender = maleRadio.Checked ? Gender.Male : Gender.Female,
            Address = new Address
            {
                City = cityTextBox.Text.Trim(),
                Zip = indexTextBox.Text.Trim(),
                Street = streetTextBox.Text.Trim(),
                House = buildTextBox.Text.Trim(),
                Apartment = flatTextBox.Text.Trim()
            },
            Job = isWorkingCheckBox.Checked ? new Job
            {
                Company = companyTextBox.Text.Trim(),
                Position = positionTextBox.Text.Trim(),
                Experience = (int)experiencePicker.Value
            } : null
        };

        private void ClearForm()
        {
            errorProvider1.Clear();
            fioTextBox.Clear(); groupTextBox.Clear();
            agePicker.Value = 18; coursePicker.Value = 1;
            if (avgScorePicker != null) avgScorePicker.Value = 4.0m;
            birthdatePicker.Value = DateTime.Today.AddYears(-18);
            specComboBox.SelectedIndex = 0;
            maleRadio.Checked = true;
            isWorkingCheckBox.Checked = false;
            cityTextBox.Clear(); indexTextBox.Clear();
            streetTextBox.Clear(); buildTextBox.Clear(); flatTextBox.Clear();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (!ValidateStudent()) return;
            var s = GetStudentFromForm();
            students.Add(s);
            currentIndex = students.Count - 1;
            RefreshListBox();
            studentListBox.SelectedIndex = currentIndex;
            ClearForm();
            UpdateStatus("Добавлен: " + s.FullName);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (students.Count == 0)
            { MessageBox.Show("Нет студентов.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            try
            {
                XmlHelper.Serialize(students, xmlFilePath); UpdateStatus("Список сохранён");
                MessageBox.Show($"Сохранено:\n{xmlFilePath}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            if (!File.Exists(xmlFilePath))
            { MessageBox.Show("Файл не найден.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            try
            {
                students = XmlHelper.Deserialize<List<Student>>(xmlFilePath);
                RefreshListBox();
                currentIndex = students.Count > 0 ? 0 : -1;
                if (currentIndex >= 0) studentListBox.SelectedIndex = 0;
                UpdateStatus("Список загружен");
                MessageBox.Show($"Загружено:\n{xmlFilePath}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void solveButton_Click(object sender, EventArgs e)
        {
            if (students.Count == 0)
            { MessageBox.Show("Нет данных.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            double total = students.Sum(s => s.AverageScore * 1000 + s.Course * 1500);
            UpdateStatus("Расчёт бюджета");
            MessageBox.Show($"Общий бюджет: {total:C}", "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

       
        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Версия: 1.0.0\nРазработчик: Чередниченко Фёдор Денисович\n© 2026",
                "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
            UpdateStatus("Открыто «О программе»");
        }

    
        private void FullSearchMenuItem_Click(object sender, EventArgs e)
        {
            if (students.Count == 0) { MessageBox.Show("Список пуст."); return; }
            string q = ShowInputBox("Введите ФИО (точное совпадение):", "Полный поиск");
            if (q == null) return;
            var res = students
                .Where(s => s.FullName.Equals(q.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();
            ShowSearchResults(res, $"Полный поиск: «{q}»");
        }

        private void RegexSearchMenuItem_Click(object sender, EventArgs e)
        {
            if (students.Count == 0) { MessageBox.Show("Список пуст."); return; }
            string pattern = ShowInputBox(
                "Введите RegEx для ФИО:\nПример:  ^Ива.*  — начинается с «Ива»\n[А-Я][а-я]{4,}  — слово 5+ букв",
                "Поиск по RegEx");
            if (pattern == null) return;
            try
            {
                var rx = new Regex(pattern, RegexOptions.IgnoreCase);
                var res = students.Where(s => rx.IsMatch(s.FullName)).ToList();
                ShowSearchResults(res, $"Regex: «{pattern}»");
            }
            catch (RegexParseException ex)
            { MessageBox.Show($"Ошибка в RegEx:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void AdvancedSearchMenuItem_Click(object sender, EventArgs e)
        {
            if (students.Count == 0) { MessageBox.Show("Список пуст."); return; }
            using (var sf = new SearchForm(students))
            {
                sf.ShowDialog(this);
                if (sf.Results.Count > 0)
                { RefreshListBox(sf.Results); UpdateStatus($"Расширенный поиск: {sf.Results.Count}"); }
            }
        }

        private void ShowSearchResults(List<Student> results, string title)
        {
            if (results.Count == 0)
            {
                MessageBox.Show("Ничего не найдено.", "Поиск", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateStatus("Поиск: ничего не найдено"); return;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Найдено: {results.Count}");
            sb.AppendLine(new string('─', 40));
            foreach (var s in results) sb.AppendLine(s.ToString());

            var dlg = new Form
            {
                Text = title,
                Size = new System.Drawing.Size(520, 400),
                StartPosition = FormStartPosition.CenterParent
            };
            var rtb = new RichTextBox { Dock = DockStyle.Fill, ReadOnly = true, Text = sb.ToString() };
            var btn = new Button { Text = "Сохранить результаты в XML", Dock = DockStyle.Bottom, Height = 35 };
            btn.Click += (s, e2) => SaveResultsToXml(results);
            dlg.Controls.Add(rtb);
            dlg.Controls.Add(btn);
            dlg.ShowDialog(this);
            UpdateStatus($"Поиск: найдено {results.Count}");
        }

        private void SaveResultsToXml(List<Student> results)
        {
            using (var sfd = new SaveFileDialog
            { Filter = "XML файлы (*.xml)|*.xml", FileName = $"search_{DateTime.Now:yyyyMMdd_HHmmss}.xml" })
            {
                if (sfd.ShowDialog() != DialogResult.OK) return;
                try
                {
                    XmlHelper.Serialize(results, sfd.FileName);
                    MessageBox.Show($"Сохранено:\n{sfd.FileName}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateStatus("Результаты сохранены");
                }
                catch (Exception ex)
                { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }


        private void SortByNameAscMenuItem_Click(object sender, EventArgs e)
        { students = students.OrderBy(s => s.FullName).ToList(); RefreshListBox(); UpdateStatus("Сортировка ФИО А→Я"); }

        private void SortByNameDescMenuItem_Click(object sender, EventArgs e)
        { students = students.OrderByDescending(s => s.FullName).ToList(); RefreshListBox(); UpdateStatus("Сортировка ФИО Я→А"); }

        private void SortByCourseMenuItem_Click(object sender, EventArgs e)
        { students = students.OrderBy(s => s.Course).ToList(); RefreshListBox(); UpdateStatus("Сортировка по курсу"); }

        private void SortByScoreMenuItem_Click(object sender, EventArgs e)
        { students = students.OrderByDescending(s => s.AverageScore).ToList(); RefreshListBox(); UpdateStatus("Сортировка по баллу"); }

        

        private void ClearToolStripButton_Click(object sender, EventArgs e)
        { ClearForm(); UpdateStatus("Форма очищена"); }

        private void DeleteToolStripButton_Click(object sender, EventArgs e)
        {
            int idx = studentListBox.SelectedIndex;
            if (idx < 0 || idx >= students.Count)
            { MessageBox.Show("Выберите студента для удаления."); return; }
            string name = students[idx].FullName;
            if (MessageBox.Show($"Удалить «{name}»?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            students.RemoveAt(idx);
            RefreshListBox();
            currentIndex = Math.Min(idx, students.Count - 1);
            if (currentIndex >= 0) studentListBox.SelectedIndex = currentIndex;
            UpdateStatus($"Удалён: {name}");
        }

        private void NextToolStripButton_Click(object sender, EventArgs e)
        {
            if (students.Count == 0) return;
            currentIndex = (currentIndex + 1) % students.Count;
            studentListBox.SelectedIndex = currentIndex;
            UpdateStatus($"Вперёд [{currentIndex + 1}/{students.Count}]");
        }

        private void PrevToolStripButton_Click(object sender, EventArgs e)
        {
            if (students.Count == 0) return;
            currentIndex = (currentIndex - 1 + students.Count) % students.Count;
            studentListBox.SelectedIndex = currentIndex;
            UpdateStatus($"Назад [{currentIndex + 1}/{students.Count}]");
        }
    }
}