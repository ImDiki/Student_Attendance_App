using System.Windows;

namespace Student_Attendance_System
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // ---------------------------------------------------------
            // bo sann's code (App Initialization)
            // ---------------------------------------------------------
            // ဒီနေရာက App စဖွင့်ဖွင့်ချင်း အလုပ်လုပ်မယ့် နေရာ။
            // ဥပမာ - Database ရှိမရှိ စစ်တာ၊ မရှိရင် Create လုပ်တာမျိုး ရေးလို့ရတယ်။
            // DatabaseHelper.InitializeDatabase(); 
        }
    }
}