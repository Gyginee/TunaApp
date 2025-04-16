using System.Reflection;

namespace Client
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            SetDefaultFormIcon();
            Application.Run(new Login());
        }
        private static void SetDefaultFormIcon()
        {
            var field = typeof(Form).GetField("defaultIcon", BindingFlags.Static | BindingFlags.NonPublic);

            using var stream = typeof(Program).Assembly.GetManifestResourceStream("Client.Images.Tuna.ico"); 
            if (stream != null)
            {
                var icon = new Icon(stream);
                field?.SetValue(null, icon);
            }
        }

    }
}