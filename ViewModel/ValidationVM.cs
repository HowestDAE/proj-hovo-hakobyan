using _2DAE15_HovhannesHakobyan_Exam.Repository;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using System.Threading;
using System.IO;

namespace _2DAE15_HovhannesHakobyan_Exam.ViewModel
{
    public class ValidationVM : ObservableObject
    {
        public string Message { get; set; }
        public RelayCommand GenerateAPIKeyCommand { get; private set; }
        public bool ShouldGenerateAPIKey { get; set; }

        public event EventHandler APIKeyApprovedEvent;
        public int WaitingTime { get; set; }

        public ValidationVM()
        {
            Message = "Validating API key, please wait ...";
            OnPropertyChanged(nameof(Message));
            GenerateAPIKeyCommand = new RelayCommand(RegenerateAPIKeyAsync);
            ShouldGenerateAPIKey = false;
            OnPropertyChanged(nameof(ShouldGenerateAPIKey));

            HandleAPIKey();
           
        }

        private async void HandleAPIKey()
        {
            try
            {
                await ValidateAPIKeyAsync();
            }
            catch (Exception)
            {
                Message = "API key is expired, please click on the button and follow the instructions";
                OnPropertyChanged(nameof(Message));
                ShouldGenerateAPIKey = true;
                OnPropertyChanged(nameof(ShouldGenerateAPIKey));
            }
        }

        private async Task ValidateAPIKeyAsync()
        {
            //Simple API call to check if the current API key is valid
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Riot-Token", SummonerAPIRepository.APIKey);
                string endpoint = "https://euw1.api.riotgames.com/lol/summoner/v4/summoners/by-name/TTT%20Alternative";
              
                try
                {
                    var response = await client.GetAsync(endpoint);
                    await Task.Delay(2000);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException(response.ReasonPhrase);
                    }

                    Message = "API key is active, please wait ...";
                    OnPropertyChanged(nameof(Message));
                    await Task.Delay(2000);

                    APIKeyApprovedEvent?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    //This means API key is not valid
                    throw ex;
                }
            }
        }

        private async void RegenerateAPIKeyAsync()
        {
            //Open the edge browser
            var options = new EdgeOptions();
            options.AddArgument("--disable-extensions");
            var driver = new EdgeDriver(options);

            // Navigate to the Riot Developer Portal and click the Login button
            driver.Navigate().GoToUrl("https://developer.riotgames.com/");
            driver.FindElement(By.CssSelector(".admin-title")).Click();

            // Wait for the login form to appear
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3600));
            wait.Until(ExpectedConditions.ElementIsVisible(By.Name("username")));

            // Enter the login credentials and submit the form
            driver.FindElement(By.Name("username")).SendKeys("oconnor23m");
            driver.FindElement(By.Name("password")).SendKeys("Tijdelijk12");
            driver.FindElement(By.Name("password")).SendKeys(Keys.Enter);

            //Wait until the page is loaded 
            wait.Until(ExpectedConditions.ElementIsVisible(By.Name("confirm_action")));

            //Get the Generate button
            IWebElement regenKey = driver.FindElement(By.Name("confirm_action"));
            System.Drawing.Point location = regenKey.Location;

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            //Locate the checkbox to highlight it
            int size = 100;
            int offset = 120;

            //scroll down the page
            js.ExecuteScript("window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' });");

            // Highlight the checkbox to click in
            js.ExecuteScript($"$(\"<div style='position:absolute;top:{location.Y - offset}px;left:{location.X}px;width:{size}px;height:{size}px;border:3px solid green;pointer-events:none;'></div>\").appendTo(\"body\");");

            //Highlight the button to click on
            js.ExecuteScript($"$(\"<div style='position:absolute;top:{location.Y}px;left:{location.X}px;width:{size * 2}px;height:{size / 2}px;border:3px solid green;pointer-events:none;'></div>\").appendTo(\"body\");");

            // Wait for the button to be clicked
            wait.Until(ExpectedConditions.StalenessOf(regenKey));

            //Wait untill the new key is generated succesfully
            wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("alert-success")));

            //Read the new api key value
            IWebElement apiKeyTextBox = driver.FindElement(By.Id("apikey"));
            string apiKey = apiKeyTextBox.GetAttribute("value");
            SummonerAPIRepository.APIKey = apiKey;

           
            //Close the browser
            driver.Close();

           

            ShouldGenerateAPIKey = false;
            OnPropertyChanged(nameof(ShouldGenerateAPIKey));

            //Save the key to a file
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Resources", "Data", "apikey.txt");

            StreamWriter writer = null;

            try
            {
                writer = new StreamWriter(filePath, false);
                writer.Write(apiKey);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }

            Message = "API key regenerated :) Please wait until the key is active.";
            OnPropertyChanged(nameof(Message));
            int secToWait = 120;

            for (int i = 0; i < secToWait; i++)
            {
                ++WaitingTime;
                OnPropertyChanged(nameof(WaitingTime));
                await Task.Delay(1000);
            }
            APIKeyApprovedEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
