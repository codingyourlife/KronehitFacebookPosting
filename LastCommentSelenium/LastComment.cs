namespace LastCommentSelenium
{
    using OpenQA.Selenium;
    using System;
    using System.Linq;
    using OpenQA.Selenium.Chrome;
    using System.Threading;

    public class LastComment
    {
        public ChromeDriver Driver { get; private set; }
        public string Url { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        public DateTime SignOfLiving { get; private set; }
        public DateTime Deadline { get; private set; }

        public LastComment(string url, string username, string password, DateTime deadline)
        {
            this.Url = url;
            this.Username = username;
            this.Password = password;
            this.SignOfLiving = DateTime.Now;
            this.Deadline = deadline;
        }

        public void Run()
        {
            try
            {
                if (this.Driver != null)
                {
                    this.Driver.Dispose();
                }
            }
            catch (Exception ex)
            {
            }

            try
            {
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--disable-notifications");
                this.Driver = new ChromeDriver(options);

                this.GoToPostingOrReload();

                this.Login();

                while (true)
                {
                    if(this.Deadline.Subtract(DateTime.Now).TotalMinutes < 0)
                    {
                        Console.WriteLine("Deadline reached!");
                        //Deadline reached...
                        System.Diagnostics.Debugger.Break();
                    }

                    this.SignOfLiving = DateTime.Now;
                    bool renewed = this.RenewComment();

                    if(!renewed)
                    {
                        Console.WriteLine(string.Format("Didn't comment because high comment frequency (avoid spam). Sleep 1min ({0})", DateTime.Now));
                        Thread.Sleep(60000);
                        Thread.Sleep(random.Next(100, 5000)); //stay bellow the radar ^^
                        continue;
                    }

                    Console.WriteLine("Sleeping for 2min");
                    Thread.Sleep(120000);
                    Thread.Sleep(random.Next(100, 5000)); //stay bellow the radar ^^
                }
            }
            catch (Exception ex)
            {
                this.SignOfLiving = default(DateTime);
            }
        }

        private Random random = new Random();

        private void Login()
        {
            var emailInput = this.Driver.FindElement(By.XPath("//*[@id=\"email\"]"));
            emailInput.Click();
            emailInput.SendKeys(this.Username);

            var passwordInput = this.Driver.FindElement(By.XPath("//*[@id=\"pass\"]"));
            passwordInput.Click();
            passwordInput.SendKeys(this.Password);
            passwordInput.SendKeys(Keys.Enter);
        }

        private void GoToPostingOrReload()
        {
            this.Driver.Navigate().GoToUrl(this.Url);
        }

        private bool RenewComment()
        {
            bool renewed = false;

            if (!this.CheckAmILastComment())
            {
                if(!this.LastCommentWasVeryRecent_ToAvoidSpam())
                {
                    var newComment = _msg;
                    var exclaimationMarks = Enumerable.Range(0, random.Next(1, 5));
                    for (int i = 0; i < exclaimationMarks.Count(); i++)
                    {
                        newComment += "!";
                    }

                    IWebElement commentBox = this.GetCommentInputBox();
                    commentBox.Click();
                    Thread.Sleep(1500);
                    this.Driver.Keyboard.SendKeys(newComment);
                    Thread.Sleep(500);
                    this.Driver.Keyboard.SendKeys(Keys.Enter);
                    Console.WriteLine(string.Format("Submitted new comment \"{0}\"", newComment));

                    this.CheckAmILastComment(false); //just for highlighting and cw

                    renewed = true;
                }
            }

            return renewed;
        }

        private bool LastCommentWasVeryRecent_ToAvoidSpam(bool reloadBeforeCheck = false)
        {
            if (reloadBeforeCheck)
            {
                this.GoToPostingOrReload();
            }

            var lastLiveTimestamp = this.GetLastLiveTimeStamp();

            if(lastLiveTimestamp.Text == "Gerade eben")
            {
                return true;
            }

            return false;
        }

        private string _msg = "TESLA rocks";

        private bool CheckAmILastComment(bool reloadBeforeCheck = true)
        {
            if (reloadBeforeCheck)
            {
                this.GoToPostingOrReload();
            }

            var lastAnswerName = this.GetLastAnswerName();
            var lastAnswerText = this.GetLastAnswerText();

            if (lastAnswerName.Text.Equals("Johannes Mayer") && lastAnswerText.Text.StartsWith(_msg))
            {
                Console.WriteLine(string.Format("Newest comment is my own!!! - {0}", DateTime.Now));
                return true;
            }
            else
            {
                try
                {
                    Console.WriteLine(string.Format("Newest comment is NOT mine but from \"{0}\": {1}", lastAnswerName.Text, lastAnswerText.Text.Truncate(20)));
                }
                catch (Exception)
                {
                    //maybe encoding errors or smth
                }

                return false;
            }
        }

        private IWebElement GetLastLiveTimeStamp()
        {
            var emojiButton = this.Driver.FindElements(By.CssSelector(".UFICommentEmojiIcon")).First();

            var found = false;
            var parentIterator = emojiButton;
            IWebElement lastCommentsLiveTimeStamp = null;
            while (!found)
            {
                parentIterator = parentIterator.FindElement(By.XPath(".."));
                var commentsLiveTimeStamps = parentIterator.FindElements(By.CssSelector(".livetimestamp"));

                if (commentsLiveTimeStamps.Count == 0)
                {
                    continue;
                }

                found = true;

                lastCommentsLiveTimeStamp = commentsLiveTimeStamps.Last();
            }

            lastCommentsLiveTimeStamp.Highlight();

            return lastCommentsLiveTimeStamp;
        }

        private IWebElement GetLastAnswerText()
        {
            var emojiButton = this.Driver.FindElements(By.CssSelector(".UFICommentEmojiIcon")).First();

            var found = false;
            var parentIterator = emojiButton;
            IWebElement lastCommentText = null;
            while (!found)
            {
                parentIterator = parentIterator.FindElement(By.XPath(".."));
                var commentsBody = parentIterator.FindElements(By.CssSelector("span > .UFICommentBody"));

                if (commentsBody.Count == 0)
                {
                    continue;
                }

                found = true;

                lastCommentText = commentsBody.Last();
            }

            lastCommentText.Highlight();

            return lastCommentText;
        }

        private IWebElement GetLastAnswerName()
        {
            var emojiButton = this.Driver.FindElements(By.CssSelector(".UFICommentEmojiIcon")).First();

            var found = false;
            var parentIterator = emojiButton;
            IWebElement lastCommentLink = null;
            while (!found)
            {
                parentIterator = parentIterator.FindElement(By.XPath(".."));
                var lastCommentLinks = parentIterator.FindElements(By.CssSelector("a.UFICommentActorName"));

                if(lastCommentLinks.Count == 0)
                {
                    continue;
                }

                found = true;

                lastCommentLink = lastCommentLinks.Last();
            }

            lastCommentLink.Highlight();

            return lastCommentLink;
        }

        private IWebElement GetCommentInputBox()
        {
            var emojiButton = this.Driver.FindElements(By.CssSelector(".UFICommentEmojiIcon")).First();

            var found = false;
            var parentIterator = emojiButton;
            while (!found)
            {
                parentIterator = parentIterator.FindElement(By.XPath(".."));
                if (parentIterator.Text == "Kommentieren ...")
                {
                    found = true;
                }
            }
            var commentBox = parentIterator;

            commentBox.Highlight();

            return commentBox;
        }
    }
}
