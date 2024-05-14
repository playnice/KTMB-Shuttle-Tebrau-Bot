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
        //driver = new FirefoxDriver();
        driver = new ChromeDriver();
    }

    public bool untilClickable(By by)
    {
        try
        {
            var elementToBeClicked = driver.FindElement(by);
            return(elementToBeClicked.Displayed && elementToBeClicked.Enabled);
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

    public void demo()
    {
        driver.Url = "https://shuttleonline.ktmb.com.my/Home/Shuttle";
        string UrlTitle = driver.Title;
        Console.WriteLine("URL Title is: " + UrlTitle);


        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        wait.PollingInterval = TimeSpan.FromMilliseconds(200);

        By firstBtn = By.XPath("//*[@id=\"validationSummaryModal\"]/div/div/div[2]/div/div[2]/button");
        var clickFirstbtn = wait.Until(condition => untilClickable(firstBtn));
  
        Thread.Sleep(500);

        Console.WriteLine("First btn clickable?" + clickFirstbtn);
        if (clickFirstbtn)
        {
            driver.FindElement(firstBtn).Click();
            Thread.Sleep(1000);
        };

        IWebElement popupScreen = driver.FindElement(By.Id("validationSummaryModal"));
        IWebElement randClick = popupScreen.FindElement(By.CssSelector("button.btn.payment-modal-btn"));

        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

        IWebElement travelDate = driver.FindElement(By.CssSelector("a.btn.picker-btn"));
        IWebElement testText = driver.FindElement(By.CssSelector("input#FromStationId.form-control"));
        string testBtnTxt = testText.GetAttribute("value");

        Console.WriteLine("testBtnTxt" + testBtnTxt);
        IWebElement OnwardDate = driver.FindElement(By.XPath("//*[@id=\"OnwardDate\"]"));
        try
        {
            Console.WriteLine("Element ID: " + $"{OnwardDate.GetAttribute("value")}");
            OnwardDate.Click();
            if (OnwardDate == null)
            {
                Console.WriteLine("Onward Date is null");
            }
            else
            {
                Console.WriteLine(OnwardDate);
            }
        }
        catch (NoSuchElementException)
        {
            Console.WriteLine("OnwardDate not found");
        }

        calendarMonth  = driver.FindElement(By.XPath())

        Thread.Sleep(2000);
        return;
    }

    public void closeBrowser()
    {
        driver.Close();
    }
    static void Main()
    {
        KtmbTrain ktmWeb = new KtmbTrain();
        ktmWeb.startBrowser();
        ktmWeb.demo();
        ktmWeb.closeBrowser();
        Environment.Exit(0);
    }


}


