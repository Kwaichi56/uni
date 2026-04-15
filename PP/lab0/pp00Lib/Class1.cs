namespace pp00Lib
{
    public class Class1
    {
        public string Up(string s)
        {
            return s.ToUpper();
        }

        public int Len(string s)
        {
            return s.Length;
        }

        public string Concat(string s, string b)
        {
            return s + b;
        }

        public static string GetInFo()
        {
            return "pp00Lib";
        }
    }
}
