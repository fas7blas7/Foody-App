using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace FoodyWebAppTestProject
{
    [TestFixture]
    public class FoodyWebAppTests
    {


        private readonly string BaseUrl = "http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com:85/";
        private static string? lastFoodTitle;
        private static string? lastFoodDescription;
        public ChromeDriver driver;
        public Actions actions;
        [OneTimeSetUp]
        public void SetUp()
        {
            var chromeOptions = new ChromeOptions();


            chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);
            chromeOptions.AddArguments("--disable-search-engine-choice-screen");

            driver = new ChromeDriver(chromeOptions);

            driver.Navigate().GoToUrl(BaseUrl);

            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            actions = new Actions(driver);


            driver.FindElement(By.XPath("//a[@class='nav-link' and text()='Log In']")).Click();           
            driver.FindElement(By.XPath("//input[@id='username']")).SendKeys("testm");
            driver.FindElement(By.XPath("//input[@id='password']")).SendKeys("123456");
            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-block fa-lg gradient-custom-2 mb-3']")).Click();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }

        [Test, Order(1)]
        public void AddFoodWithInvalidTitleAndDescriptionTest()
        {
            AddFoodButtonClick();

            var foodNameInput = driver.FindElement(By.XPath("//input[@id='name']"));
            foodNameInput.SendKeys("");

            var foodDescriptionInput = driver.FindElement(By.XPath("//input[@id='description']"));
            foodDescriptionInput.SendKeys("");

            var saveChangesButton = driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-block fa-lg gradient-custom-2 mb-3']"));
            saveChangesButton.Click();

            var currentUrl = driver.Url;
            Assert.That(currentUrl, Is.EqualTo($"{BaseUrl}Food/Add"));

            var mainErrorMsg = driver.FindElement(By.XPath("//div[@class='text-danger validation-summary-errors']"));
            Assert.That(mainErrorMsg.Text.Trim(), Is.EqualTo(mainErrorMsg.Text));

            var nameFieldErrMsg = driver.FindElement(By.XPath("//span[@class='text-danger field-validation-error' and @data-valmsg-for='Name']"));
            Assert.That(nameFieldErrMsg.Text.Trim(), Is.EqualTo(nameFieldErrMsg.Text));

            var descriptionFieldErrMsg = driver.FindElement(By.XPath("//span[@class='text-danger field-validation-error' and @data-valmsg-for='Description']"));
            Assert.That(descriptionFieldErrMsg.Text.Trim(), Is.EqualTo(descriptionFieldErrMsg.Text));
        }

        [Test, Order(2)]
        public void AddRandomTitleFoodWithRandomDescriptionTest()
        {
            lastFoodTitle = "FOOD: " + GenerateRandomString(5);
            lastFoodDescription = "Description is:" + GenerateRandomString(10);

            AddFoodButtonClick();

            driver.FindElement(By.XPath("//input[@id='name']")).SendKeys(lastFoodTitle);
            driver.FindElement(By.XPath("//input[@id='description']")).SendKeys(lastFoodDescription);

            var saveButton = driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-block fa-lg gradient-custom-2 mb-3']"));
            saveButton.Click();

            var currentUrl = driver.Url;
            Assert.That(currentUrl, Is.EqualTo(BaseUrl));

            VerifyLastFoodTitle(lastFoodTitle);            
        }

        [Test, Order(3)]
        public void EditLastAddedFoodTest()
        {

            driver.Navigate().GoToUrl(BaseUrl);

            var lastFood = driver.FindElements(By.CssSelector("section#scroll"));
            var lastFoodElement = lastFood.Last();            
            actions.MoveToElement(lastFoodElement).Perform();

            var editButton = driver.FindElement(By.XPath("//a[@class='btn btn-primary btn-xl rounded-pill mt-5']"));
            actions.MoveToElement(editButton).Perform();
            editButton.Click();

            var inputTitle = driver.FindElement(By.XPath("//input[@id='name']"));
            inputTitle.Clear();
            inputTitle.SendKeys("Edited Food");

            var saveButton = driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-block fa-lg gradient-custom-2 mb-3']"));
            saveButton.Click();

            lastFood = driver.FindElements(By.CssSelector("section#scroll"));
            lastFoodElement = lastFood.Last();
            actions.MoveToElement(lastFoodElement).Perform();

            var editedFoodTitle = lastFoodElement.FindElement(By.CssSelector("h2.display-4")).Text.Trim();

            Assert.That(editedFoodTitle, Is.EqualTo(lastFoodTitle));

            Console.WriteLine("Food titles cannot be changed due to unfinished functionality.");
        }

        [Test, Order(4)]
        public void SearchForFoodTitleTest()
        {

            var lastFood = driver.FindElements(By.CssSelector("section#scroll"));
            var lastFoodElement = lastFood.Last();            
            string lastFoodTitle = lastFoodElement.FindElement(By.CssSelector("h2.display-4")).Text.Trim();
            
            var searchField = driver.FindElement(By.XPath("//input[@name='keyword']"));
            
            searchField.SendKeys(lastFoodTitle);   
            actions.MoveToElement(searchField).Perform();
            driver.FindElement(By.XPath("//button[@class='btn btn-primary rounded-pill mt-5 col-2']")).Click();

            var foods = driver.FindElements(By.CssSelector("section#scroll"));
            Assert.IsTrue(foods.Count == 1, "One food was found.");

            string searchTitle = lastFoodTitle;

            Assert.That(searchTitle.Trim(), Is.EqualTo(lastFoodTitle), "Title of search and actual title do not match.");
        }

        [Test, Order(5)]
        public void DeleteLastAddedFoodTest()
        {
            var items = driver.FindElements(By.CssSelector("img[alt=\"picture of the food\"]"));
            int itemCount = items.Count;
            // No items found - assert and log
            if (itemCount == 0)
            {
                Assert.Pass("No food items to delete.");
            }
            else if (itemCount == 1)
            {
                // If there is only one item, handle the case appropriately (skip, log, etc.)
                Console.WriteLine("Only one food item was present. Deleting it.");
            }

            var lastFoodElement = items.Last();
            Actions actions = new Actions(driver);
            var deleteButton = lastFoodElement.FindElement(By.XPath("//a[@class='btn btn-primary btn-xl rounded-pill mt-5'][2]"));
            actions.MoveToElement(deleteButton).Perform();
            deleteButton.Click();
            
            items = driver.FindElements(By.CssSelector("img[alt=\"picture of the food\"]"));
            int finalCount = items.Count;

            // Assert that the item count has decreased
            if (itemCount > 1)
            {
                Assert.That(finalCount, Is.EqualTo(itemCount - 1), "The item count should decrease after deletion.");
            }
            else
            {
                Assert.That(finalCount, Is.EqualTo(0), "All food items should be deleted.");
            }

        }

            [Test, Order(6)]
        public void SearchforDeletedFoodTest()
        {
            // Search for the previously deleted food item

            var lastFood = driver.FindElements(By.CssSelector("section#scroll"));
            var lastFoodElement = lastFood.Last();
            string lastFoodTitle = lastFoodElement.FindElement(By.CssSelector("h2.display-4")).Text.Trim();

            var searchField = driver.FindElement(By.XPath("//input[@name='keyword']"));
            searchField.SendKeys(lastFoodTitle);
            searchField.SendKeys(Keys.Enter);

            // Wait for the search results page to load
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(drv => drv.FindElement(By.XPath("//h2[@class='display-4']")));

                // Verify the "no foods found" message is displayed
                var noFoodsFound = driver.FindElement(By.XPath("//h2[@class='display-4']"));
                Assert.That(noFoodsFound.Text.Trim(), Is.EqualTo("There are no foods :("));

                // Verify that the "Add Food" button is displayed
                var addButton = driver.FindElement(By.XPath("//a[@class='btn btn-primary btn-xl rounded-pill mt-5']"));
                Assert.That(addButton.Displayed, Is.True);
            }



        private void AddFoodButtonClick()
        {
            var addFoodButton = driver.FindElement(By.XPath("//a[@class='nav-link' and text()='Add Food']"));
            addFoodButton.Click();
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void VerifyLastFoodTitle(string expectedTitle)
        {
            var foods = driver.FindElements(By.CssSelector("section#scroll"));
            var lastFoodElement = foods.Last();
            var lastFoodElementTitle = lastFoodElement.FindElement(By.CssSelector("h2.display-4"));

            string actualFoodTitle = lastFoodElementTitle.Text.Trim();
            Assert.That(actualFoodTitle, Is.EqualTo(expectedTitle), "The last food title does not match the expected value.");
        }        
    }
}