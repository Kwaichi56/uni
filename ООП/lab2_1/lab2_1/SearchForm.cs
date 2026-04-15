using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace lab2_1
{
    public class SearchForm : Form
    {
        private readonly List<Student> _allStudents;
        public List<Student> Results { get; private set; } = new List<Student>();

        private TextBox fioBox, groupBox, cityBox, companyBox;
        private NumericUpDown courseFromBox, courseToBox, scoreFromBox, scoreToBox;
        private ComboBox genderCombo, specialityCombo;
        private CheckBox useRegexCheck;
        private Button searchBtn, saveBtn;
        private ListBox resultListBox;
        private Label resultCountLabel;

        public SearchForm(List<Student> students)
        {
            _allStudents = students;
            BuildUI();
        }

        private void BuildUI()
        {
            Text = "Расширенный поиск";
            Size = new Size(680, 580);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 4,
                RowCount = 7,
                AutoSize = true,
                Padding = new Padding(8),
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            for (int i = 0; i < 7; i++)
                panel.RowStyles.Add(new RowStyle(SizeType.Absolute, i < 6 ? 34 : 38));

            // Row 0: ФИО + Группа
            panel.Controls.Add(Lbl("ФИО:"), 0, 0);
            fioBox = new TextBox { Dock = DockStyle.Fill };
            panel.Controls.Add(fioBox, 1, 0);
            panel.Controls.Add(Lbl("Группа:"), 2, 0);
            groupBox = new TextBox { Dock = DockStyle.Fill };
            panel.Controls.Add(groupBox, 3, 0);

            // Row 1: Специальность + Пол
            panel.Controls.Add(Lbl("Специальность:"), 0, 1);
            specialityCombo = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            specialityCombo.Items.AddRange(new[] { "(любая)", "Программная инженерия",
                "Информационные системы", "Прикладная математика", "Экономика" });
            specialityCombo.SelectedIndex = 0;
            panel.Controls.Add(specialityCombo, 1, 1);
            panel.Controls.Add(Lbl("Пол:"), 2, 1);
            genderCombo = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            genderCombo.Items.AddRange(new[] { "(любой)", "Мужчина", "Женщина" });
            genderCombo.SelectedIndex = 0;
            panel.Controls.Add(genderCombo, 3, 1);

            // Row 2: Курс от/до
            panel.Controls.Add(Lbl("Курс (от — до):"), 0, 2);
            var cp = new FlowLayoutPanel { Dock = DockStyle.Fill };
            courseFromBox = new NumericUpDown { Minimum = 1, Maximum = 4, Value = 1, Width = 55 };
            courseToBox = new NumericUpDown { Minimum = 1, Maximum = 4, Value = 4, Width = 55 };
            cp.Controls.Add(courseFromBox);
            cp.Controls.Add(new Label { Text = "—", AutoSize = true, Padding = new Padding(3, 4, 3, 0) });
            cp.Controls.Add(courseToBox);
            panel.Controls.Add(cp, 1, 2);

            // Row 3: Балл от/до
            panel.Controls.Add(Lbl("Ср. балл (от — до):"), 0, 3);
            var sp = new FlowLayoutPanel { Dock = DockStyle.Fill };
            scoreFromBox = new NumericUpDown { Minimum = 0, Maximum = 10, DecimalPlaces = 1, Value = 0, Width = 65 };
            scoreToBox = new NumericUpDown { Minimum = 0, Maximum = 10, DecimalPlaces = 1, Value = 10, Width = 65 };
            sp.Controls.Add(scoreFromBox);
            sp.Controls.Add(new Label { Text = "—", AutoSize = true, Padding = new Padding(3, 4, 3, 0) });
            sp.Controls.Add(scoreToBox);
            panel.Controls.Add(sp, 1, 3);

            // Row 4: Город + Компания
            panel.Controls.Add(Lbl("Город:"), 0, 4);
            cityBox = new TextBox { Dock = DockStyle.Fill };
            panel.Controls.Add(cityBox, 1, 4);
            panel.Controls.Add(Lbl("Компания:"), 2, 4);
            companyBox = new TextBox { Dock = DockStyle.Fill };
            panel.Controls.Add(companyBox, 3, 4);

            // Row 5: RegEx checkbox
            useRegexCheck = new CheckBox
            {
                Text = "Использовать RegEx для текстовых полей",
                AutoSize = true
            };
            panel.SetColumnSpan(useRegexCheck, 4);
            panel.Controls.Add(useRegexCheck, 0, 5);

            // Row 6: Кнопки
            var bp = new FlowLayoutPanel { Dock = DockStyle.Fill };
            searchBtn = new Button { Text = "Найти", Width = 85 };
            searchBtn.Click += SearchBtn_Click;
            saveBtn = new Button { Text = "Сохранить в XML", Width = 130 };
            saveBtn.Click += SaveBtn_Click;
            bp.Controls.Add(searchBtn);
            bp.Controls.Add(saveBtn);
            panel.SetColumnSpan(bp, 4);
            panel.Controls.Add(bp, 0, 6);

            resultCountLabel = new Label
            {
                Text = "Результатов: 0",
                Dock = DockStyle.Top,
                Height = 22,
                Padding = new Padding(4, 0, 0, 0)
            };
            resultListBox = new ListBox { Dock = DockStyle.Fill };

            var closeBtn = new Button { Text = "Закрыть", Dock = DockStyle.Bottom, Height = 32 };
            closeBtn.Click += (s, e) => Close();

            Controls.Add(resultListBox);
            Controls.Add(resultCountLabel);
            Controls.Add(panel);
            Controls.Add(closeBtn);
        }

        private static Label Lbl(string t) =>
            new Label { Text = t, AutoSize = true };

        private void SearchBtn_Click(object sender, EventArgs e)
        {
            bool useRx = useRegexCheck.Checked;
            IEnumerable<Student> q = _allStudents;

            if (!string.IsNullOrWhiteSpace(fioBox.Text))
            { q = FilterText(q, fioBox.Text.Trim(), s => s.FullName, useRx, "ФИО"); if (q == null) return; }
            if (!string.IsNullOrWhiteSpace(groupBox.Text))
            { q = FilterText(q, groupBox.Text.Trim(), s => s.Group, useRx, "Группа"); if (q == null) return; }
            if (!string.IsNullOrWhiteSpace(cityBox.Text))
            { q = FilterText(q, cityBox.Text.Trim(), s => s.Address?.City ?? "", useRx, "Город"); if (q == null) return; }
            if (!string.IsNullOrWhiteSpace(companyBox.Text))
            { q = FilterText(q, companyBox.Text.Trim(), s => s.Job?.Company ?? "", useRx, "Компания"); if (q == null) return; }

            if (specialityCombo.SelectedIndex > 0)
                q = q.Where(s => s.Speciality == specialityCombo.SelectedItem.ToString());
            if (genderCombo.SelectedIndex == 1) q = q.Where(s => s.Gender == Gender.Male);
            else if (genderCombo.SelectedIndex == 2) q = q.Where(s => s.Gender == Gender.Female);

            int cf = (int)courseFromBox.Value, ct = (int)courseToBox.Value;
            q = q.Where(s => s.Course >= cf && s.Course <= ct);

            double sf = (double)scoreFromBox.Value, st = (double)scoreToBox.Value;
            q = q.Where(s => s.AverageScore >= sf && s.AverageScore <= st);

            Results = q.ToList();
            resultListBox.Items.Clear();
            foreach (var s in Results) resultListBox.Items.Add(s.ToString());
            resultCountLabel.Text = $"Результатов: {Results.Count}";
        }

        private IEnumerable<Student> FilterText(IEnumerable<Student> src, string pattern,
            Func<Student, string> sel, bool useRx, string field)
        {
            if (!useRx)
                return src.Where(s =>
                    sel(s).IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0);
            try
            {
                var rx = new Regex(pattern, RegexOptions.IgnoreCase);
                return src.Where(s => rx.IsMatch(sel(s)));
            }
            catch (RegexParseException ex)
            {
                MessageBox.Show($"Ошибка RegEx для «{field}»:\n{ex.Message}",
                    "Ошибка RegEx", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            if (Results.Count == 0)
            { MessageBox.Show("Нет результатов для сохранения."); return; }
            using (var sfd = new SaveFileDialog
            {
                Filter = "XML файлы (*.xml)|*.xml",
                FileName = $"search_{DateTime.Now:yyyyMMdd_HHmmss}.xml"
            })
            {
                if (sfd.ShowDialog() != DialogResult.OK) return;
                try
                {
                    XmlHelper.Serialize(Results, sfd.FileName);
                    MessageBox.Show($"Сохранено:\n{sfd.FileName}", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }
    }
}