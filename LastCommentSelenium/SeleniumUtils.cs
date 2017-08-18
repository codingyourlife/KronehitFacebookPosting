using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastCommentSelenium
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Remote;
    using System.Threading;

    public static class SeleniumUtil
    {
        public static void Highlight(this IWebElement context)
        {
            var rc = (RemoteWebElement)context;
            var driver = (IJavaScriptExecutor)rc.WrappedDriver;
            var script = @"arguments[0].style.cssText = ""border-width: 2px; border-style: solid; border-color: red""; ";
            driver.ExecuteScript(script, rc);
        }
    }

}
