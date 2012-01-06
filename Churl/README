Churl: A C# library for sending and receiving data over HTTP(S). Or a peon to do your HTTP bidding.

Churl grew out of a project I wrote that required pinging a webservice every minute, processing some json data, and sending some json data back. In C#. Anyone who has done this knows that sending & receiving data over http in C# is a pain; it goes something like this:

	// sending the request
	HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
	request.Method = "GET";
	using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
	{
		writer.Write(data);
	}

	// reading the response
	HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse();
	using (StreamReader reader = new StreamReader(httpResponse.GetResponseStream()))
	{
		responseData = reader.ReadToEnd();
	}

It gets a little repetitive after a while. What I wanted was something like cURL, but I didn't simply want to write a wrapper around that, especially considering that it's not installed by default on any Windows systems I know of. 

You can do the same thing as above, using Churl, this way:

	HttpResponse response = Churl.Get(url, data);

	// response contains everything you need to know
	// process the response data with foo(), if successful
	if (response.ResponseCode == 200) foo(response.Data);

Or a more concrete example (asking Twitter about avocados):

	HttpResponse response = Churl.Get("http://search.twitter.com/search.json", "q=avocado");

More examples can be found in the unit test file, ChurlTest.cs.

Churl ended up being a fun little project that I have used in a couple of different projects now; hopefully, someone else out there on the interwebs can make good use out of him.

-robu 

