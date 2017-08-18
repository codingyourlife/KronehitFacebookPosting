namespace System
{
    public static class ConsoleEx
    {
        public static string HideCharacter()
        {
            ConsoleKeyInfo key;
            string code = "";
            do
            {
                key = Console.ReadKey(true);
                Console.Write("*");
                code += key.KeyChar;
            } while (key.Key != ConsoleKey.Enter);

            if (code.Length >= 2)
            {
                code = code.Substring(0, code.Length - 1);
            }
            else if (code == "\r")
            {
                code = string.Empty;
            }

            Console.WriteLine();

            return code;
        } 
    }
}
