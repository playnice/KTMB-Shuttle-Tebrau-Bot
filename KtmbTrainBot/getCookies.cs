using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KtmbTrainBot
{
    internal class Cookies : KtmbTrain
    {
        static void Main()
        {
            Cookies cookies = new Cookies();
            cookies.startBrowser();
            //cookies.loginAccount();
            Console.WriteLine("Testt");
            var cookiesList = driver.Manage().Cookies.AllCookies;
            Console.WriteLine("Cookies = " + cookiesList);

            cookies.closeBrowser();
        }
    }
}
