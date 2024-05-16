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


class KtmbTrain
{
    IWebDriver driver;
    public void startBrowser()
    {
        driver = new ChromeDriver();
        driver.Url = "https://shuttleonline.ktmb.com.my/Home/Shuttle";
        string UrlTitle = driver.Title;
        Console.WriteLine("URL Title is: " + UrlTitle);
    }
    public void closeBrowser()
    {
        driver.Close();
    }
    public bool untilClickable(IWebElement element)
    {
        try
        {
            return(element.Displayed && element.Enabled);
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
    public void checkNoPaymentBtn()
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(1));
        wait.PollingInterval = TimeSpan.FromMilliseconds(200);

        var noPaymentBtn = driver.FindElement(By.XPath("//*[@id=\"validationSummaryModal\"]/div/div/div[2]/div/div[2]/button"));
        if(noPaymentBtn.Displayed == false)
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

    public void checkDays()
    {
        //IList<IWebElement> daysList = driver.FindElements(By.XPath("/html/body/section/div/div[1]/section/div[2]"));
        //IList<IWebElement> daysList = driver.FindElements(By.XPath("//*[@class=\"lightpick__days\"]"));
        var lightPick = driver.FindElement(By.ClassName("lightpick__days"));
        IList<IWebElement> daysList = lightPick.FindElements(By.TagName("div"));
        var i = 0;
        foreach (IWebElement day in daysList)
        {
            i++;
            Console.WriteLine("count = " + i);
            //long dataTime = long.Parse(daysList[0].GetAttribute("data-time"));            
            long dataTime = long.Parse(day.GetAttribute("data-time"));
            DateTimeOffset date = DateTimeOffset.FromUnixTimeMilliseconds(dataTime);
            TimeZoneInfo sgtInfo = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
            DateTimeOffset sgtTime = TimeZoneInfo.ConvertTime(date, sgtInfo);
            Console.WriteLine(sgtTime.Date);
            //allDays.Add(day.Text);
        }

        //var j = 0;
        //foreach(string d in allDays)
        //{
        //    j++;
        //    Console.WriteLine("count days = " + j);
        //    Console.WriteLine("d = " + d);
        //}
    }

    public void checkIfTicketAvailable()
    {
        checkNoPaymentBtn();
        Thread.Sleep(1000);
        selectCalendar();
        Thread.Sleep(1000);
        checkDays();
        return;
    }


    static void Main()
    {
        var ktmWeb = new KtmbTrain();
        ktmWeb.startBrowser();
        ktmWeb.checkIfTicketAvailable();
        ktmWeb.closeBrowser();
        Environment.Exit(0);
    }


}


