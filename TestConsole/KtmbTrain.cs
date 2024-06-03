using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;


class KtmbTrain
{
    private static readonly string loginEmail = "wongzx96@gmail.com";
    private static readonly string loginPassword = "Omfgleh96@@";
    private IWebDriver? driver;
    private DateOnly searchDate;
    private KtmbTrain(DateTime inputDate)
    {
        searchDate = DateOnly.FromDateTime(inputDate);
    }
    private void startBrowser()
    {
        driver = new ChromeDriver();
        driver.Url = "https://shuttleonline.ktmb.com.my/Home/Shuttle";
        string UrlTitle = driver.Title;
        Console.WriteLine("URL Title is: " + UrlTitle);
        driver.Manage().Window.Maximize();
    }
    private void closeBrowser()
    {
        driver.Close();
        Console.WriteLine("browser closed");
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

    private bool elementAvailable(By findBy)
    {
        try
        {
            var elementToBeDisplayed = driver.FindElement(findBy);
            Console.WriteLine("Displayed?" + elementToBeDisplayed.Displayed);
            Console.WriteLine("Enabled?" + elementToBeDisplayed.Enabled);
            return elementToBeDisplayed.Displayed && elementToBeDisplayed.Enabled;
        }
        catch (StaleElementReferenceException)
        {
            Console.WriteLine("Stale element!!");
            return false;
        }
        catch (NoSuchElementException)
        {
            Console.WriteLine("No such element!!");
            return false;
        }
    }
    private void checkNoPaymentBtn()
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(1))
        {
            PollingInterval = TimeSpan.FromMilliseconds(200),
        };

        var noPaymentBtnBy = By.XPath("//*[@id=\"validationSummaryModal\"]/div/div/div[2]/div/div[2]/button");
        //var noPaymentBtn = driver.FindElement(By.XPath("//*[@id=\"validationSummaryModal\"]/div/div/div[2]/div/div[2]/button"));
        //if (noPaymentBtn.Displayed == false)
        //{
        //    Console.WriteLine("noPaymentBtn not displayed");
        //    return; //quit program
        //}

        try
        {
            var clicknoPaymentBtn = wait.Until(condition => elementAvailable(noPaymentBtnBy));
            //Thread.Sleep(500);

            if (clicknoPaymentBtn)
            {
                var noPaymentBtn = driver.FindElement(noPaymentBtnBy);
                noPaymentBtn.Click();
                Thread.Sleep(500);
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
                DateOnly calendarDateOnly = DateOnly.FromDateTime(sgtTime.Date);
                Console.WriteLine("test dateonly = " + calendarDateOnly);
                if(calendarDateOnly == searchDate)
                {
                    Console.WriteLine("input date found!");
                }
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

        var loginBtn = driver.FindElement(By.XPath("//*[@id=\"LoginButton\"]"));
        loginBtn.Click();

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
        Thread.Sleep(200);
    }

    //Function to get required departure time
    private void readTimetable()
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2))
        {
            PollingInterval = TimeSpan.FromMilliseconds(200),
        };

        var timeTableBy = By.CssSelector(".bg-white.depart-trips");
        wait.Until(condition => elementAvailable(timeTableBy));

        //IList<IWebElement> departureTimes = timeTable.FindElements(By.TagName("tr"));

        var timeTable = driver.FindElement(By.CssSelector(".bg-white.depart-trips"));
        IList<IWebElement> departureTimes = timeTable.FindElements(By.XPath("*"));

        var selectTimeBtnBy = By.CssSelector(".btn.select-btn.btn-seat-layout");
        

        int i = 0;
        foreach (IWebElement timeList in departureTimes)
        {
            i++;
            string timeDeparture = timeList.GetAttribute("data-hourminute");
            Console.WriteLine($"{i} {timeDeparture}");
            if(timeDeparture == "1900")
            {
                wait.Until(condition => elementAvailable(selectTimeBtnBy));
                var selectTimeBtn = timeList.FindElement(selectTimeBtnBy); 
                Actions actions = new Actions(driver);
                actions.MoveToElement(selectTimeBtn);
                actions.Perform();
                Thread.Sleep(500);
                Console.WriteLine("Click select now");
                selectTimeBtn.Click();
                Thread.Sleep(2000);

                //var captchaBox = driver.FindElement(By.Id("recaptcha-anchor"));
                //captchaBox.Click();
                //Thread.Sleep(500); 
            }
        }

    }
    
    //Main function to do all the stuffs
    private void checkLatestTicketForSale()
    {
        selectCalendar();
        Thread.Sleep(500);

        DateTime currDate = DateTime.Now;
        var lastDayOfMth = DateTime.DaysInMonth(currDate.Year, currDate.Month);

        
        int X;
        Console.WriteLine("currDate.day = " + currDate.Day + ",lastDayOfMth = " + lastDayOfMth);
        if (currDate.Day == lastDayOfMth)
        {
            X = 6; 
        }
        else
        {
            X = 5;
        }

        selectXMthFromNow(X);

        for (int i = 0;i < X; i++)
        {
            clickNextMonth();
            //Thread.Sleep(500);
        }

        var fridayCalendar = checkDays();
        fridayCalendar.Click();

        var travelOneWayBtn = driver.FindElement(By.XPath("/html/body/section/div/a[2]"));
        travelOneWayBtn.Click();

        var searchSubmitBtn = driver.FindElement(By.XPath("//*[@id=\"btnSubmit\"]"));
        searchSubmitBtn.Click();

        readTimetable();

        Thread.Sleep(2000);
        return;
    }


    static void Main()
    {
        //string strDate = "28/06/2024";
        string strDate = "1/11/2024";

        var ktmWeb = new KtmbTrain(DateTime.Parse(strDate));
        Console.WriteLine("search date = " + ktmWeb.searchDate);
        ktmWeb.startBrowser();
        ktmWeb.loginAccount();
        if (strDate == null)
        {
            ktmWeb.checkLatestTicketForSale();
        }
        else
        {
            ktmWeb.checkLatestTicketForSale();
            //ktmWeb.searchInputDate();
        }
        ktmWeb.closeBrowser();
        Environment.Exit(0);
    }
}


