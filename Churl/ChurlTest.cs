using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace churl
{
    [TestFixture]
    public class ChurlTest
    {
        /// <summary>
        /// A simple test case for a GET request
        /// </summary>
        [TestCase]
        public void GetTest()
        {
            // ask twitter about avocados...yummy
            HttpResponse response = Churl.Get("http://search.twitter.com/search.json", "q=avocado");
            Assert.AreEqual(200, response.ResponseCode);
            Assert.True(Regex.IsMatch(response.Data, "avocado", RegexOptions.IgnoreCase));

            // same thing, but use a dictionary<string,string> as input
            response = Churl.Get("http://search.twitter.com/search.json", new Dictionary<string, object>() {{ "q", "avocado" }});
			Assert.AreEqual(200, response.ResponseCode);
            Assert.True(Regex.IsMatch(response.Data, "avocado", RegexOptions.IgnoreCase));
        }

        /// <summary>
        /// A simple test case for a POST request
        /// </summary>
        [TestCase]
        public void PostTest()
        {
            // ask ddg about avocados
			// ddg doesn't actually process searches via POST, but does respond with a 200
            HttpResponse response = Churl.Post("https://www.duckduckgo.com", "q=avocado");
            Assert.AreEqual(200, response.ResponseCode);

            // same thing, but use a dictionary<string,string> as input
            response = Churl.Post("https://www.duckduckgo.com", new Dictionary<string, object>() { { "q", "avocado" } });
			Assert.AreEqual(200, response.ResponseCode);
        }

		/// <summary>
		///	Verify that UTF encoded text is properly formatted in URL format before sending to the server 
		/// </summary>
		[TestCase]
		public void UtfTest()
		{
			
            // ask cookpad about avocado pasta
            HttpResponse response = Churl.Get("http://www.cookpad.com/search/post", "keyword=アボカドパスタ&utf8=✓&order_by_date=検索");
            Assert.AreEqual(200, response.ResponseCode);
			Assert.True(Regex.IsMatch(response.Data, "アボカドパスタ", RegexOptions.IgnoreCase));

            // use the the verbose form, and include a custom header
            response = Churl.Request("GET", "http://www.cookpad.com/search/post", new Dictionary<string, string>() { {"Accept-Charset","ISO-8859-1,utf-8;q=0.7,*;q=0.3"} }, new Dictionary<string, object>() { {"keyword", "アボカドパスタ"}, {"utf8","✓"}, {"order_by_date","検索"} });
            Assert.AreEqual(200, response.ResponseCode);
			Console.WriteLine(response.Data);
			Assert.True(Regex.IsMatch(response.Data, "アボカドパスタ", RegexOptions.IgnoreCase));
		}
    }
}
