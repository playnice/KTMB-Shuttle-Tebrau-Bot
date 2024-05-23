using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;


class KtmbTrain
{
    private static readonly string loginEmail = "wongzx96@gmail.com";
    private static readonly string loginPassword = "Omfgleh96@@";

    private IWebDriver driver;
    private void startBrowser()
    {
        driver = new ChromeDriver();
        driver.Url = "https://shuttleonline.ktmb.com.my/Home/Shuttle";
        string UrlTitle = driver.Title;
        Console.WriteLine("URL Title is: " + UrlTitle);
    }
    private void closeBrowser()
    {
        driver.Close();
    }
    private bool untilClickable(IWebElement element)
    {
        try
        {
            return (element.Displayed && element.Enabled);
        }
        catch (StaleElementReferenceException)
        {
            return false;
        }
        catch (NoSuchElementException)
        {
            return false;
        };
    }
    private void checkNoPaymentBtn()
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
        wait.PollingInterval = TimeSpan.FromMilliseconds(200);

        var noPaymentBtn = driver.FindElement(By.XPath("//*[@id=\"validationSummaryModal\"]/div/div/div[2]/div/div[2]/button"));
        if (noPaymentBtn.Displayed == false)
        {
            Console.WriteLine("noPaymentBtn not displayed");
            return;
        }

        try
        {
            var clicknoPaymentBtn = wait.Until(condition => untilClickable(noPaymentBtn));
            Thread.Sleep(500);

            if (clicknoPaymentBtn)
            {
                noPaymentBtn.Click();
                Thread.Sleep(1000);
            }
        }
        catch (WebDriverTimeoutException)
        {
            Console.WriteLine("waiting for noPaymentBtn timeout");
        }
    }

    private void selectCalendar()
    {
        var OnwardDate = driver.FindElement(By.XPath("//*[@id=\"OnwardDate\"]"));
        try
        {
            OnwardDate.Click();
        }
        catch (NoSuchElementException)
        {
            Console.WriteLine("OnwardDate not found");
        }

        var calendar = driver.FindElement(By.XPath("/html/body/section/div/div[1]/section/header/div[1]/select[1]"));
        var currMonth = new SelectElement(calendar);


        Console.WriteLine("Current month is " + currMonth.SelectedOption.Text + ", value is: " + calendar.GetAttribute("value"));
    }

    private void selectXMthFromNow(int x)
    {
        // Get the current date and time
        DateTime currDate = DateTime.Now;

        // Add 6 months to the current date
        DateTime XMonthsFromNow = currDate.AddMonths(x);

        // Get the month and year as separate variables
        int monthXMonthsFromNow = XMonthsFromNow.Month;
        int yearXMonthsFromNow = XMonthsFromNow.Year; 

        // Print the result
        Console.WriteLine("Current Date: Month = " + currDate.Month + ", Year = " + currDate.Year);
        Console.WriteLine("Date x Months from Now: Month = " + monthXMonthsFromNow + ", Year = " + yearXMonthsFromNow);
    }

    private void clickNextMonth()
    {
        var nextMthBtn = driver.FindElement(By.XPath("/html/body/section/div/div[1]/section/header/div[2]/button[2]"));
        nextMthBtn.Click();
    }


    private IWebElement checkDays()
    {
        //IList<IWebElement> daysList = driver.FindElements(By.XPath("/html/body/section/div/div[1]/section/div[2]"));
        //IList<IWebElement> daysList = driver.FindElements(By.XPath("//*[@class=\"lightpick__days\"]"));
        var lightPick = driver.FindElement(By.ClassName("lightpick__days"));
        IList<IWebElement> daysList = lightPick.FindElements(By.TagName("div"));
        var i = 0;
        foreach (IWebElement day in daysList)
        {
            i++;
            //Console.WriteLine("count = " + i);
            //long dataTime = long.Parse(daysList[0].GetAttribute("data-time"));            
            long dataTime = long.Parse(day.GetAttribute("data-time"));
            DateTimeOffset date = DateTimeOffset.FromUnixTimeMilliseconds(dataTime);
            TimeZoneInfo sgtInfo = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
            DateTimeOffset sgtTime = TimeZoneInfo.ConvertTime(date, sgtInfo);
            //Console.WriteLine(sgtTime.Date);
            //allDays.Add(day.Text);
            if(sgtTime.Date.DayOfWeek == DayOfWeek.Friday)
            {
                Console.WriteLine("Friday found!" + sgtTime.Date);
                return day;
            }
        }

        return lightPick;
        //var j = 0;
        //foreach(string d in allDays)
        //{
        //    j++;
        //    Console.WriteLine("count days = " + j);
        //    Console.WriteLine("d = " + d);
        //}
    }

    private bool findLoginBtn(IWebElement loginBtn)
    {
        try
        {
            
            string hrefVal = loginBtn.GetAttribute("href");
            Console.WriteLine($"hrefVal={hrefVal}");
            if(hrefVal == "https://shuttleonline.ktmb.com.my/Account/Login")
            {
                return true;
            }
        }
        catch (NoSuchElementException)
        {
            return false;
        }

        return false;
    }

    private void loginAction()
    {
        var emailInput = driver.FindElement(By.XPath("//*[@id=\"Email\"]"));
        emailInput.SendKeys(loginEmail);

        var pwdInput = driver.FindElement(By.XPath("//*[@id=\"Password\"]"));
        pwdInput.SendKeys(loginPassword);

        Thread.Sleep(500);

        var loginBtn = driver.FindElement(By.XPath("//*[@id=\"LoginButton\"]"));
        loginBtn.Click();

        Thread.Sleep(500); 

        return;
    }

    private void loginAccount()
    {
        checkNoPaymentBtn();
        Thread.Sleep(500);
        var loginBtn = driver.FindElement(By.XPath("//*[@id=\"navbarSupportedContent\"]/ul[3]/li/a"));
        bool loginBtnFound = findLoginBtn(loginBtn);
        Console.WriteLine($"Logged in: {loginBtnFound}");
        Console.WriteLine($"{loginEmail}, {loginPassword}");
        if (loginBtnFound)
        {
            loginBtn.Click();
            loginAction();
            driver.Navigate().GoToUrl("https://shuttleonline.ktmb.com.my/Home/Shuttle");
            checkNoPaymentBtn();
        }
        Thread.Sleep(500);
    }

    private void checkIfTicketAvailable()
    {
        selectCalendar();
        Thread.Sleep(500);

        DateTime currDate = DateTime.Now;
        var lastDayOfMth = DateTime.DaysInMonth(currDate.Year, currDate.Month);

        int selectMthCount;
        Console.WriteLine("currDate.day = " + currDate.Day + ",lastDayOfMth = " + lastDayOfMth);
        if (currDate.Day == lastDayOfMth)
        {
            selectMthCount = 6; 
        }
        else
        {
            selectMthCount = 5;
        }

        selectXMthFromNow(selectMthCount);

        for (int i = 0;i < selectMthCount; i++)
        {
            clickNextMonth();
            Thread.Sleep(100);
        }

        var fridayCalendar = checkDays();
        fridayCalendar.Click();

        var travelOneWayBtn = driver.FindElement(By.XPath("/html/body/section/div/a[2]"));
        travelOneWayBtn.Click();

        var searchSubmitBtn = driver.FindElement(By.XPath("//*[@id=\"btnSubmit\"]"));
        searchSubmitBtn.Click();

        Thread.Sleep(2000);
        return;
    }


    static void Main()
    {
        var ktmWeb = new KtmbTrain();
        ktmWeb.startBrowser();
        ktmWeb.loginAccount();
        ktmWeb.checkIfTicketAvailable();
        ktmWeb.closeBrowser();
        Environment.Exit(0);
    }
}


