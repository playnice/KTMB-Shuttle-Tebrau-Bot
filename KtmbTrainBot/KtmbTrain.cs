using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using TwoCaptcha.Captcha;

namespace TwoCaptcha
{
    public class KtmbTrain
    {
        public static readonly string loginEmail = "wongzx96@gmail.com";
        public static readonly string loginPassword = "Omfgleh96@@";
        public static IWebDriver driver;
        public DateOnly searchDate;
        public static string strSearchDate;
        public SmtpClient smtpClient;
        public static TwoCaptcha solver;
        public static ReCaptcha captcha;
        public static List<string> preferredDepartTime = new List<string> { "2000", "2115", "2130" };
        public struct Passenger
        {
            public static readonly string name = "Kilina Lawrence";
            public static readonly string passportNo = "H57698763";
            public static readonly string passportExp = "30 Jun 2028";
            public static readonly string contactNo = "+601133981245";
        }

        public KtmbTrain()
        {
            setupTwoCaptcha();
        }
        public KtmbTrain(DateTime inputDate)
        {
            searchDate = DateOnly.FromDateTime(inputDate);
            setupTwoCaptcha();
        }
        public void startBrowser()
        {
            ChromeOptions options = new();
            options.AddArgument("no-sandbox");
            driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));

            driver.Url = "https://shuttleonline.ktmb.com.my/Home/Shuttle";
            string UrlTitle = driver.Title;
            Console.WriteLine("URL Title is: " + UrlTitle);
            //driver.Manage().Window.Maximize();
        }
        public void closeBrowser()
        {
            Console.WriteLine("Closing browser now");
            driver.Close();
            Console.WriteLine("browser closed");
        }

        //public void setupSmtp()
        //{
        //    smtpClient = new SmtpClient("smtp.gmail.com")
        //    {
        //        Port = 587,
        //        Credentials = new NetworkCredential("wongzx96@gmail.com", "fnekhicwwzfxuevt"),
        //        EnableSsl = true,
        //    };
        //}

        public void setupTwoCaptcha()
        {
            solver = new TwoCaptcha("601bac9183ec262456c416baef5a2de9");
            captcha = new ReCaptcha();
            captcha.SetSiteKey("6LcoccMUAAAAAJj5QkZEIcFBbs9v6tTtQ1SuVW23");
            captcha.SetUrl("https://shuttleonline.ktmb.com.my/ShuttleTrip");
        }

        public bool untilClickable(IWebElement element)
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

        public bool elementAvailable(By findBy)
        {
            try
            {
                var elementToBeDisplayed = driver.FindElement(findBy);
                //Console.WriteLine("Displayed?" + elementToBeDisplayed.Displayed);
                //Console.WriteLine("Enabled?" + elementToBeDisplayed.Enabled);
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
        public void checkNoPaymentBtn()
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

        public void selectCalendar()
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

        public void selectXMthFromNow(int x)
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

        public void clickNextMonth()
        {
            var nextMthBtn = driver.FindElement(By.XPath("/html/body/section/div/div[1]/section/header/div[2]/button[2]"));
            nextMthBtn.Click();
        }


        public IWebElement checkDays()
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
                DateOnly calendarDateOnly = DateOnly.FromDateTime(sgtTime.Date);
                //if (sgtTime.Date.DayOfWeek == DayOfWeek.Friday)
                //{
                //    Console.WriteLine("Friday found!" + sgtTime.Date);

                //    Console.WriteLine("test dateonly = " + calendarDateOnly);
                if (calendarDateOnly == searchDate)
                {
                    Console.WriteLine("input date found!" + calendarDateOnly);
                    return day;
                }

                //if(i > 31)
                //{
                //    break;
                //}
                //}
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

        public bool findLoginBtn(IWebElement loginBtn)
        {
            try
            {

                string hrefVal = loginBtn.GetAttribute("href");
                Console.WriteLine($"hrefVal={hrefVal}");
                if (hrefVal == "https://shuttleonline.ktmb.com.my/Account/Login")
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

        public void loginAction()
        {
            var emailInput = driver.FindElement(By.XPath("//*[@id=\"Email\"]"));
            emailInput.SendKeys(loginEmail);

            var pwdInput = driver.FindElement(By.XPath("//*[@id=\"Password\"]"));
            pwdInput.SendKeys(loginPassword);

            var loginBtn = driver.FindElement(By.XPath("//*[@id=\"LoginButton\"]"));
            loginBtn.Click();

            return;
        }

        public void loginAccount()
        {
            Thread.Sleep(1000);
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

        public void switchDestination()
        {
            var switchBtn = driver.FindElement(By.XPath("//*[@id=\"theForm\"]/div/div[1]/i"));
            switchBtn.Click();
            Thread.Sleep(500);
        }

        public void sendEmail(string timeDeparture, string timeTableDate)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10))
            {
                PollingInterval = TimeSpan.FromMilliseconds(200),
            };

            var bookingConfirmedElem = By.XPath("/html/body/div[2]/div/div[1]/div[1]/div");
            try
            {
                wait.Until(condition => elementAvailable(bookingConfirmedElem));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error waiting for bookingConfirmedElem: " + ex.Message);
            }
            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                //Port = 587,
                smtpClient.Credentials = new NetworkCredential("wongzx96@gmail.com", "fnekhicwwzfxuevt");
                smtpClient.EnableSsl = true;

                var bookingConfirmed = driver.FindElement(bookingConfirmedElem);
                var fromTo = driver.FindElement(By.XPath("/html/body/div[2]/div/div[2]/div[2]/div[2]/div/div/div[1]/p"));
                if (bookingConfirmed.Text == "Your booking has been confirmed.")
                {
                    smtpClient.Send("wongzx96@gmail.com",
                        "wongzx96@gmail.com",
                        $"KTMB Ticket for {fromTo.Text}, {timeTableDate} @{timeDeparture} purchased",
                        $"Ticket is purchased for {fromTo.Text}, departure time {timeDeparture} at {timeTableDate}!");
                    smtpClient.Send("wongzx96@gmail.com",
                        "holycowhell3@gmail.com",
                        $"KTMB Ticket for {fromTo.Text}, {timeTableDate} @{timeDeparture} purchased",
                        $"Ticket is purchased for {fromTo.Text}, departure time {timeDeparture} at {timeTableDate}!");
                }
                else
                {
                    smtpClient.Send("wongzx96@gmail.com",
                        "wongzx96@gmail.com",
                        $"KTMB Available Ticket {timeTableDate} @{timeDeparture} Alert",
                        $"Ticket is available now at departure time {timeDeparture} at {timeTableDate}!");
                    smtpClient.Send("wongzx96@gmail.com",
                        "holycowhell3@gmail.com",
                        $"KTMB Available Ticket {timeTableDate} @{timeDeparture} Alert",
                        $"Ticket is available now at departure time {timeDeparture} at {timeTableDate}!");
                    //smtpClient.Send("wongzx96@gmail.com",
                    //    "kilinalaw@gmail.com",
                    //    $"KTMB Available Ticket {timeTableDate} @{timeDeparture} Alert",
                    //    $"Ticket is available now at departure time {timeDeparture} at {timeTableDate}!");
                    //smtpClient.Send("wongzx96@gmail.com",
                    //    "kilinalawrencemarcus@gmail.com",
                    //    $"KTMB Available Ticket {timeTableDate} @{timeDeparture} Alert",
                    //    $"Ticket is available now at departure time {timeDeparture} at {timeTableDate}!");
                }
                Console.WriteLine("Email sent...");
            }
        }

        public string solveCaptcha()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine("Solving captcha...");
            try
            {
                solver.Solve(captcha).Wait();
                Console.WriteLine("Captcha solved: " + captcha.Code);
            }
            catch (AggregateException e)
            {
                Console.WriteLine("Error occurred: " + e.InnerExceptions.First().Message);
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"Captcha solved in {elapsedMs}ms");
            return captcha.Code;
        }

        public void fillInPersonalDetails()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10))
            {
                PollingInterval = TimeSpan.FromMilliseconds(200),
            };

            var paymentBtnElem = By.XPath("//*[@id=\"btnConfirmPayment\"]");
            try
            {
                wait.Until(condition => elementAvailable(paymentBtnElem));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error waiting for paymentBtnElem: " + ex.Message);
            }

            var passengerNameBox = driver.FindElement(By.XPath("//*[@id=\"Passengers_0__FullName\"]"));
            passengerNameBox.SendKeys(Passenger.name);

            //var passportNoBox = driver.FindElement(By.XPath("//*[@id=\"Passengers_0__PassportNo\"]"));
            //passportNoBox.SendKeys(Passenger.passportNo);
            ////((IJavaScriptExecutor)driver).ExecuteScript("document.getElementById('Passengers_0__PassportExpiryDate').removeAttribute('readonly',0);");

            //var passportExpBox = driver.FindElement(By.XPath("//*[@id=\"Passengers_0__PassportExpiryDate\"]"));
            ////passportExpBox.SendKeys(Passenger.passportExp);
            //passportExpBox.Click();
            //var expYearPick = driver.FindElement(By.XPath("/html/body/section/div/div[1]/section/header/div[1]/select[2]"));

            var ticketType = driver.FindElement(By.XPath("//*[@id=\"Passengers_0__TicketTypeId\"]"));
            SelectElement selectTicketType = new SelectElement(ticketType);
            selectTicketType.SelectByText("DEWASA/ADULT");

        }

        public void proceedPayment()
        {
            var proceedPaymentBtn = driver.FindElement(By.XPath("//*[@id=\"btnConfirmPayment\"]"));
            proceedPaymentBtn.Click();


            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10))
            {
                PollingInterval = TimeSpan.FromMilliseconds(200),
            };

            var ktmbWalletElem = By.XPath("//*[@id=\"btnKtmbEWallet\"]");
            try
            {
                wait.Until(condition => elementAvailable(ktmbWalletElem));
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error waiting for ktmbWalletBtn: " + ex.Message);
            }

            Thread.Sleep(1000);
            var ktmbWalletBtn = driver.FindElement(ktmbWalletElem);
            ktmbWalletBtn.Click();

        }
        //Function to get required departure time
        public void readTimetable()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2))
            {
                PollingInterval = TimeSpan.FromMilliseconds(200),
            };

            var timeTableBy = By.CssSelector(".bg-white.depart-trips");
            try
            {
                wait.Until(condition => elementAvailable(timeTableBy));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in readTimetable() =>" + ex);
                if (ex is WebDriverTimeoutException)
                {
                    var tooManyRequest = driver.FindElement(By.XPath("//*[@id=\"popupModalBody\"]"));
                    Console.WriteLine("tooManyRequest.Text = " + tooManyRequest.Text);
                    if (tooManyRequest.Text == "Too Many Requests")
                    {
                        int timeSleep = 300000;
                        Console.WriteLine($"Too many requests! Wait for {timeSleep / 60000} minutes");
                        Thread.Sleep(timeSleep);
                    }
                    Thread.Sleep(10000);
                }
                else
                {
                    Console.WriteLine("Not WebDriverTimeoutException");
                    closeBrowser();
                    mainProcess(strSearchDate); //rerun if hit error
                }
            }

            try
            {
                //IList<IWebElement> departureTimes = timeTable.FindElements(By.TagName("tr"));
                Thread.Sleep(1000);
                var timeTable = driver.FindElement(By.CssSelector(".bg-white.depart-trips"));
                IList<IWebElement> departureTimes = timeTable.FindElements(By.XPath("*"));

                //var selectTimeBtnBy = By.CssSelector(".btn.select-btn.btn-seat-layout");
                var selectTimeBtnBy = By.CssSelector(".btn.select-btn");

                var timeTableDateElm = driver.FindElement(By.XPath("/html/body/div[2]/div[1]/div/div[2]/div[1]/div[2]/div/table/thead/tr/th[4]/div"));
                string timeTableDate = timeTableDateElm.Text;
                //Console.WriteLine($"timeTableDate={timeTableDate}");

                Thread.Sleep(1000);

                int i = 0;
                foreach (IWebElement timeList in departureTimes)
                {
                    i++;
                    string timeDeparture = timeList.GetAttribute("data-hourminute");
                    //Console.WriteLine($"{i} {timeDeparture}");
                    //if ((timeDeparture == "2000") || (timeDeparture == "2115") || (timeDeparture == "2130"))
                    if(preferredDepartTime.Contains(timeDeparture))
                    {
                        string selectBtnText;
                        //wait.Until(condition => elementAvailable(selectTimeBtnBy));
                        var selectTimeBtn = timeList.FindElement(selectTimeBtnBy);
                        //string selectBtnText = timeList.GetAttribute("text");                
                        //selectBtnText = timeList.GetAttribute("value");
                        selectBtnText = selectTimeBtn.Text;
                        Console.WriteLine("Btn Value: " + selectBtnText);
                        switch (selectBtnText)
                        {
                            case "Sold Out":
                                Console.WriteLine($"Ticket sold out for departure time {timeDeparture} at {timeTableDate}!");
                                break;
                            case "Select":
                                Console.WriteLine($"Ticket available for departure time {timeDeparture} at {timeTableDate}!");
                                selectTimeBtn.Click();
                                string captchaCode = solveCaptcha();
                                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                                try
                                {
                                    js.ExecuteScript($"RecaptchaCallback(\"{captchaCode}\")");
                                    Thread.Sleep(500);
                                    fillInPersonalDetails();
                                    proceedPayment();
                                    sendEmail(timeDeparture, timeTableDate);
                                    terminatePgm();
                                }
                                catch (Exception jsException)
                                {
                                    Console.WriteLine("Submit captcha function failed: " + jsException);
                                }
                                break;
                            case "Login to view":
                                Console.WriteLine("Not yet logged in!");
                                loginAccount();
                                break;
                        }
                        //Actions actions = new Actions(driver);
                        //actions.MoveToElement(selectTimeBtn);
                        //actions.Perform();
                        //Thread.Sleep(500);
                        //Console.WriteLine("Click select now");
                        //selectTimeBtn.Click();
                        //Thread.Sleep(2000);

                        //var captchaBox = driver.FindElement(By.Id("recaptcha-anchor"));
                        //captchaBox.Click();
                        //Thread.Sleep(500); 
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception when finding timetable => " + ex.Message);
                closeBrowser();
                mainProcess(strSearchDate); //rerun if hit error
            }

        }

        public int getOpenSalesMonth()
        {
            DateTime currDate = DateTime.Now;
            var lastDayOfMth = DateTime.DaysInMonth(currDate.Year, currDate.Month);


            int mthNumber;
            Console.WriteLine("currDate.day = " + currDate.Day + ",lastDayOfMth = " + lastDayOfMth);
            if (currDate.Day == lastDayOfMth)
            {
                mthNumber = 6;
            }
            else
            {
                mthNumber = 5;
            }

            return mthNumber;
        }
        public int inputDateMth()
        {
            DateTime currDate = DateTime.Now;
            var lastDayOfMth = DateTime.DaysInMonth(currDate.Year, currDate.Month);


            int mthNumber = (searchDate.Year - currDate.Year) * 12 + (searchDate.Month - currDate.Month);
            Console.WriteLine("mthNumber = " + mthNumber);

            return mthNumber;
        }
        //Main function to do all the stuffs
        public void checkLatestTicketForSale(int monthToSkip)
        {
            selectCalendar();
            Thread.Sleep(500);


            //selectXMthFromNow(X);

            for (int i = 0; i < monthToSkip; i++)
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

            Thread.Sleep(1000);

            int foreverLoop = 0;
            do
            {
                readTimetable();
                driver.Navigate().Refresh();
                Thread.Sleep(10000); //Refresh every x seconds
                foreverLoop--;
                Console.WriteLine("Loop no." + foreverLoop);
                Console.WriteLine("Time: " + DateTime.Now.TimeOfDay);
            }
            while (foreverLoop < 1);


            Thread.Sleep(2000);
            return;
        }

        public void mainProcess(string searchDate)
        {

            //ktmWeb.setupSmtp();
            startBrowser();
            loginAccount();

            Thread.Sleep(500);
            checkNoPaymentBtn();
            switchDestination();

            int mthToSkip;
            if (searchDate.Length == 0)
            {
                mthToSkip = getOpenSalesMonth();
                checkLatestTicketForSale(mthToSkip);
            }
            else
            {
                mthToSkip = inputDateMth();
                checkLatestTicketForSale(mthToSkip);
                //ktmWeb.searchInputDate();
            }
            closeBrowser();
        }

        public static void Main(string[] args)
        {
            string strDate;
            if (args.Length == 0)
            {
                strDate = DateTime.Now.ToString();
            }
            else
            {
                strDate = args[0];
                Console.WriteLine($"Input date = {args[0]}");
            }

            strSearchDate = strDate;
            //string strDate = "7/6/2024";
            var ktmWeb = new KtmbTrain(DateTime.ParseExact(strDate, "d/M/yyyy", CultureInfo.InvariantCulture));
            //var ktmWeb = new KtmbTrain(DateTime.Parse(strDate));
            ktmWeb.setupTwoCaptcha();

            Console.WriteLine("search date = " + ktmWeb.searchDate);

            try
            {
                ktmWeb.mainProcess(strSearchDate);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception hit in main! " + e);
                ktmWeb.closeBrowser();
                ktmWeb.mainProcess(strSearchDate); //rerun if hit error
            }

            Environment.Exit(0);
        }

        public void terminatePgm()
        {
            Console.WriteLine("Terminating program...");
            closeBrowser();
            Environment.Exit(0);
        }
    }
}