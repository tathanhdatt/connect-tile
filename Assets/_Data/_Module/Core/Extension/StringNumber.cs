namespace Core.Extension
{
    public static class StringNumber
    {
        private static readonly string[] NumberString;

        static StringNumber()
        {
            NumberString = new string[100000];
            for (var i = 0; i < NumberString.Length; i++)
            {
                NumberString[i] = $"{i}";
            }
        }

        public static string IntToText(this int number)
        {
            if (number < 0)
            {
                return NumberString[0];
            }

            if (number >= NumberString.Length)
            {
                return NumberString[NumberString.Length - 1];
            }

            return NumberString[number];
        }
    }
}