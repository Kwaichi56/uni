namespace lab1
{
    public partial class Form1 : Form
    {

        Calculator calc = new Calculator();
        public Form1()
        {
            InitializeComponent();

            btnCalc.Click += btnCalc_Click;
            zadanye.TextChanged += zadanye_TextChanged;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
         
        private void btnCalc_Click(object sender, EventArgs e)
        {
            try
            {
                string input = textInput.Text;

                var stats = calc.GetAllStats(input);

                quaVowels.Text = stats.vowl.ToString();
                quaConsonant.Text = stats.cons.ToString();
                quaWords.Text = stats.wrd.ToString();
                quaSent.Text = stats.sent.ToString();
                stringLen.Text = stats.len.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при анализе: " + ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void zadanye_TextChanged(object sender, EventArgs e)
        {
            MessageBox.Show("Text changed");
        }
    }
}
