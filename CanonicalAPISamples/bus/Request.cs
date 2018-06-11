using System;
using System.Text;
using System.Net;
using System.IO;


namespace Cherwell.CanonicalAPISamples
{
	public class Values
	{

		//***********************************************
		//  SAMPLE CODE STEPS:
		//  1 - Update the Canonical API Values
		//  2 - Update the Token Request Values
		//  3 - Comment in the ReqeustName to use
		//  4 - Comment in and update the GetURLExtension to use for the RequestName
		//  5 - Comment in and update the PostData or PatchData for post or patch RequestName
		//
		//  NOTE: Use the describe method to retrieve field definitions for post data
		//    - Unique Key field should not be sent for create and will either be ignored or return an error if sent
		//    - All other fields are optional and if sent value will be used
		//      (Check business object definition for fields that have a default value if value is not sent)
		//      (DO NOT send field and value if not updating, sending blank value will update field to empty string or error for invalid data type)
		//
		//***********************************************



		//***********************************************
		// (1) - Canonical API Values
		//***********************************************
		public static string ApiURL { get; } = "http://localhost/Trebuchet.WebApi";  // = "{your api URL}";

		public static string Name { get; } = "ticket";  // name = "{name}";  // Canonical name (i.e. Ticket)
		
		public static string Version { get; } = "v3";  // = "{version}";  // Canonical version (i.e. V1, V2, etc.)



		//***********************************************
		// (2) - Token Request Values
		//***********************************************
		public static string Username { get; } = "csdadmin";  // = "{your username}";

		public static string Password { get; } = "CSDAdmin";  // = "{your password}";

		public static string ClientId { get; } = "00000000-0042-0000-0042-000000000000";   // = "{your clientId}";

		public static string GrantType { get; } = "password";  // grant type = password

		public static string TokenURL { get; } = "http://localhost/Trebuchet.WebApi/token";  // = "{your token URL}";



		//***********************************************
		// (3) - Request Type To Execute
		//***********************************************
		public static string RequestName { get; } =
			"get";  // describe, get by id, searchfield, searchterm, and searchdates
			// "post";  // search and create
			// "patch";  // update
			// "delete";  // delete



		//***********************************************
		//  (4a) - GET REQUEST URL Extensions (comment in url extension of operation to execute)
		//***********************************************
		public static string GetUrlExtension { get; } =
			"";  // Get item description
			// "{id}";  // Get by unique id
			// "search?term=[term]&fieldName=[fieldName]&fieldValue=[fieldValue]&dateFieldName=[dateFieldName]&dateFrom=[dateFrom]&dateTo=[dateTo]&page=[page]&pageSize=[pageSize]";  // Search by term, field value and/or search dates;
			// "{id}/comments?page=[page]&pageSize=[size]";  // Get comments for item by item id
			// "comments";  // Get comments description



		//***********************************************
		// (4b/5) - POST REQUEST URL Extensions and Data (comment in url extension and data for operation to execute)
		//***********************************************
		public static string PostUrlExtension { get; } =
			"search?page=1&pageSize=10"; // "search?page=[page]&pageSize=[size]";  // Search by filters (filters data also required)
			// "";  // Create item (item data also required)
			// "{id}/comment";  // Create comment for item (item data also required)


		// See data definitions section for specific data formats
		public static string PostData { get; } =
			// Search Data (search by field value, search term or range, multiple filters can be sent for multiple field value, search term or range values)  
			"{ \"filters\": [ { \"field\": { \"fieldName\": \"[search field]\", \"searchTerm\": \"[search term]\" }, \"searchTerm\": \"[search term]\", \"range\": { \"fieldName\": \"[search field]\", \"gte\": \"[greater than or equal value]\", \"lte\": \"[less than or equal value]\" } } ] }";

			// Create Data (valid fields can be retrieved using GET describe operation)
			// "{ \"[field name]\": \"[field value]\" }";

			// Create Comment Data (valid fields can be retrieved using GET describe operation)
			// "{ \"[field name]\": \"[field value]\" }";


		//***********************************************
		// (5) - POST REQUEST Data Definitions
		//***********************************************
		// Get business objects by posting search data: field, search term, or search range (at least one required, sending multiple filters is valid)
		//
		/* Search filters format:
		  {
			"filters": [
			  {
				"field": {
				  "fieldName": "string",
					"searchTerm": "string"
				  },
				"searchTerm": "string",
				"range": {
				  "fieldName": "string",
				  "gte": "string",
				  "lte": "string"
				}
			  }
			]
		  }
		*/
		//
		// Create business object by posting item data (use GET Request describe for name to retrieve fields)
		//
		/* create data format:
		  { 
			"field1": "value",
			"fieldn": "value"
		  }
		*/



		//***********************************************
		// (4c) - DELETE REQUEST URL Extensions (comment in url extension for operation to execute)
		//***********************************************
		public static string DeleteUrlExtension { get; } =
			"{id}";  // Delete item by id
			// "comments/{id}";  // Delete comment by item and comment id ;



		//***********************************************
		// (4d/5) - PATCH REQUEST URL Extensions and Data (comment in url extension and data for operation to execute)
		//***********************************************
		public static string PatchUrlExtension { get; } =
			"{id}";  // Update item by id


		public static string PatchData { get; } =
			"{ \"[field name]\": \"[field value]\" }";  // Only send fields that values are being updated


		//***********************************************
		// (5) - PATCH REQUEST Data Definitions
		//***********************************************
		//
		// Update business object by posting item data (use GET Request describe for name to retrieve fields)
		//
		/* update data format:
		  { 
			"field1": "value",
			"fieldn": "value"
		  }
		*/
	}


	public class Request
	{

		public static bool RunRequest()
		{
			// Run request based on value of requestName
			switch (Values.RequestName.ToLower())
			{
				case "get":
					return GetRequest();

				case "post":
					return PostRequest();

				case "delete":
					return DeleteRequest();

				case "patch":
					return PatchRequest();

				default:
					return false;
			}
		}


		public static string TokenRequest()
		{
			try
			{
				// Create HTTP Web Request for the token request
				HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(Values.TokenURL);

				// Set data to use to post credentials for authorization token
				byte[] data = Encoding.ASCII.GetBytes($"username={Values.Username}&password={Values.Password}&client_id={Values.ClientId}&grant_type={Values.GrantType}");

				// Set request verb POST, content type and content length (length of data)
				tokenRequest.Method = "POST";
				tokenRequest.ContentType = "application/x-www-form-urlencoded";
				tokenRequest.ContentLength = data.Length;

				// Stream request data
				using (Stream stream = tokenRequest.GetRequestStream())
				{
					stream.Write(data, 0, data.Length);
				}

				// Get the response and read stream to string
				using (WebResponse response = tokenRequest.GetResponse())
				{
					using (Stream stream = response.GetResponseStream())
					{
						using (StreamReader sr = new StreamReader(stream))
						{
							// responseText = sr.ReadToEnd();
							return sr.ReadToEnd();
						}
					}
				}
			}
			catch (WebException ex)
			{
				// Catch error for bad URL (404) or bad request (400) resulting from bad input (username, password, client id, grant type in token request)
				if (ex.Message.Contains("400"))
				{
					// do something for bad request
				}
				else if (ex.Message.Contains("404"))
				{
					// do something for not found
				}
				else
				{
					// unknown error, do something
				}

				return null;
			}
			catch (Exception ex)
			{
				// General Exception
				return null;
			}

		}

		public static Boolean GetRequest()
		{
			try
			{
				string getURL = $"{Values.ApiURL}/api/{Values.Version}/canonical/{Values.Name}/{Values.GetUrlExtension}";  // URL to use for the request populated with variables

				// Get authorization token required to use for each API request
				string tokenResponse = TokenRequest();
				dynamic token = !string.IsNullOrEmpty(tokenResponse) ? System.Web.Helpers.Json.Decode(tokenResponse) : null;

				// Check token returned with values needed
				if (string.IsNullOrEmpty(token?["access_token"]) || string.IsNullOrEmpty(token?["token_type"]))
				{
						// Invalid token so do something
						return false;
				}
				else
				{
					// Create HTTP Web Request for the API call
					HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(getURL);

					// Set the header authorization token type and value and set method to GET
					getRequest.Headers.Add("Authorization", token["token_type"] + " " + token["access_token"]);
					getRequest.Method = "GET";
					getRequest.ContentType = "application/json";

					// Get the response and read response stream
					string responseText = "";
					using (WebResponse response = getRequest.GetResponse())
					{
						using (Stream stream = response.GetResponseStream())
						{
							using (StreamReader sr = new StreamReader(stream))
							{
								responseText = sr.ReadToEnd();
							}
						}
					}

					// Deserialize response to dynamic object to read response (Here we could deserialize to a known object definition instead of dynamic)
					dynamic responseObject = System.Web.Helpers.Json.Decode(responseText);

					// Check for a valid expected response
					if (string.IsNullOrEmpty(responseText) || responseObject?["errors"] == null)
					{
						// No response or unexpected response format so do something.
						// 
						// Single response only returns errors and single item in place of items (i.e. GET by unique id and describe)
						//
						// Expected response:
						/*
							{
							  "pageReturned": 0,
							  "itemsPerPage": 0,
							  "errors": [
							    {
							      "key": "string",
							      "value": "string"
								}
							  ],
							  "items": [
							    {
							      "field1": "value",
							      "field2": "value",
							      "fieldn": "value"
								},
							    {
							      "field1": "value",
							      "field2": "value",
							      "fieldn": "value"
								}
							  ]
							}
						*/

						return false;
					}

					// Check for errors
					if (responseObject["errors"].Length > 0)
					{
						// Errors were received so do something
						//
						// responseObject["errors"][0]["key"] = type of error (i.e. FIELDVALIDATIONERROR)
						// responseObject["errors"][0]["value"] = description of error (i.e. Unable to find data for Incident with id X)

						return false;
					}

					// No errors so check for result(s)
					if (responseObject["item"] != null)
					{
						// Item received back so do something with it
						// responseObject["item"]

						return true;
					}
					else if (responseObject["items"].Length >= 0)
					{
						// 0 or more items return for search request so do something
						// responseObject["items"][0]

						return true;
					}

					// Return false.  A valid case wasn't handled?
					return false;
				}
			}
			catch (WebException ex)
			{
				// Catch error for bad URL (404) or bad request (400) resulting from bad input
				if (ex.Message.Contains("400"))
				{
					// do something for bad request
				}
				else if (ex.Message.Contains("404"))
				{
					// do something for not found
				}
				else
				{
					// other error, do something
				}

				return false;
			}
			catch (Exception ex)
			{
				// General Exception
				return false;
			}
		}

		public static bool PostRequest()
		{
			try
			{
				string postURL = $"{Values.ApiURL}/api/{Values.Version}/canonical/{Values.Name}/{Values.PostUrlExtension}";  // URL to use for the request populated with variables

				// Get authorization token required to use for each API request
				string tokenResponse = TokenRequest();
				dynamic token = !string.IsNullOrEmpty(tokenResponse) ? System.Web.Helpers.Json.Decode(tokenResponse) : null;

				// Check token returned with values needed
				if (string.IsNullOrEmpty(token?["access_token"]) || string.IsNullOrEmpty(token?["token_type"]))
				{
					// Invalid token so do something
					return false;
				}
				else
				{
					// Create HTTP Web Request for the API call
					HttpWebRequest postRequest = (HttpWebRequest)WebRequest.Create(postURL);

					// Get data to send
					byte[] data = Encoding.ASCII.GetBytes(Values.PostData);

					// Set request verb POST, content type and content length (length of data)
					postRequest.Headers.Add("Authorization", token["token_type"] + " " + token["access_token"]);
					postRequest.Method = "POST";
					postRequest.ContentType = "application/json";
					postRequest.ContentLength = data.Length;

					// Stream request data
					using (Stream stream = postRequest.GetRequestStream())
					{
						stream.Write(data, 0, data.Length);
					}

					// Get the response and read stream to return
					string responseText = "";
					using (WebResponse response = postRequest.GetResponse())
					{
						using (Stream stream = response.GetResponseStream())
						{
							using (StreamReader sr = new StreamReader(stream))
							{
								responseText = sr.ReadToEnd();
							}
						}
					}

					// Deserialize response to dynamic object to read response (Here we could deserialize to a known object definition instead of dynamic)
					dynamic responseObject = System.Web.Helpers.Json.Decode(responseText);

					// Check for a valid expected response
					if (string.IsNullOrEmpty(responseText) || responseObject?["errors"] == null)
					{
						// No response or unexpected response format so do something.
						// 
						// Single response only returns errors and single item in place of items (i.e. POST create)
						//
						// Expected response:
						/*
							{
							  "pageReturned": 0,
							  "itemsPerPage": 0,
							  "errors": [
							    {
							      "key": "string",
							      "value": "string"
								}
							  ],
							  "items": [
							    {
							      "field1": "value",
							      "field2": "value",
							      "fieldn": "value"
								},
							    {
							      "field1": "value",
							      "field2": "value",
							      "fieldn": "value"
								}
							  ]
							}
						*/

						return false;
					}

					// Check for errors
					if (responseObject["errors"].Length > 0)
					{
						// Errors were received so do something
						//
						// responseObject["errors"][0]["key"] = type of error (i.e. FIELDVALIDATIONERROR)
						// responseObject["errors"][0]["value"] = description of error (i.e. Unable to find data for Incident with id X)

						return false;
					}

					// No errors so check for result(s)
					if (responseObject["item"] != null)
					{
						// Item received back so do something with it
						// responseObject["item"]

						return true;
					}
					else if (responseObject["items"].Length >= 0)
					{
						// 0 or more items return for search request so do something
						// responseObject["items"][0]

						return true;
					}

					// Return false.  A valid case wasn't handled?
					return false;
				}
			}
			catch (WebException ex)
			{
				// Catch error for bad URL (404) or bad request (400) resulting from bad input
				if (ex.Message.Contains("400"))
				{
					// do something for bad request
				}
				else if (ex.Message.Contains("404"))
				{
					// do something for not found
				}
				else
				{
					// other error, do something
				}

				return false;
			}
			catch (Exception ex)
			{
				// General Exception
				return false;
			}
		}

		public static bool DeleteRequest()
		{
			try
			{
				string deleteURL = $"{Values.ApiURL}/api/{Values.Version}/canonical/{Values.Name}/{Values.DeleteUrlExtension}";  // URL to use for the request populated with variables

				// Get authorization token required to use for each API request
				string tokenResponse = TokenRequest();
				dynamic token = !string.IsNullOrEmpty(tokenResponse) ? System.Web.Helpers.Json.Decode(tokenResponse) : null;

				// Check token returned with values needed
				if (string.IsNullOrEmpty(token?["access_token"]) || string.IsNullOrEmpty(token?["token_type"]))
				{
					// Invalid token so do something
					return false;
				}
				else
				{
					// Create HTTP Web Request for the API call
					HttpWebRequest deleteRequest = (HttpWebRequest)WebRequest.Create(deleteURL);

					// Set the header authorization token type and value and set method to GET
					deleteRequest.Headers.Add("Authorization", token["token_type"] + " " + token["access_token"]);
					deleteRequest.Method = "DELETE";
					deleteRequest.ContentType = "application/json";

					// Get the response and read response stream
					string responseText = "";
					using (WebResponse response = deleteRequest.GetResponse())
					{
						using (Stream stream = response.GetResponseStream())
						{
							using (StreamReader sr = new StreamReader(stream))
							{
								responseText = sr.ReadToEnd();
							}
						}
					}

					// Deserialize response to dynamic object to read response (Here we could deserialize to a known object definition instead of dynamic)
					dynamic responseObject = System.Web.Helpers.Json.Decode(responseText);

					// Check for a valid expected response
					if (string.IsNullOrEmpty(responseText) || responseObject?["errors"] == null)
					{
						// No response or unexpected response format so do something.
						// 
						// Expected response:
						/*
							{
							  "errors": [
							    {
							      "key": "string",
							      "value": "string"
								}
							  ]
							}
						*/

						return false;
					}

					// Check for errors
					if (responseObject["errors"].Length > 0)
					{
						// Errors were received so do something
						//
						// responseObject["errors"][0]["key"] = type of error (i.e. FIELDVALIDATIONERROR)
						// responseObject["errors"][0]["value"] = description of error (i.e. Unable to find data for Incident with id X)

						return false;
					}

					// No errors so delete was successful
					return true;
				}
			}
			catch (WebException ex)
			{
				// Catch error for bad URL (404) or bad request (400) resulting from bad input
				if (ex.Message.Contains("400"))
				{
					// do something for bad request
				}
				else if (ex.Message.Contains("404"))
				{
					// do something for not found
				}
				else
				{
					// other error, do something
				}

				return false;
			}
			catch (Exception ex)
			{
				// General Exception
				return false;
			}
		}

		public static bool PatchRequest()
		{
			try
			{
				string patchURL = $"{Values.ApiURL}/api/{Values.Version}/canonical/{Values.Name}/{Values.PatchUrlExtension}";  // URL to use for the request populated with variables

				// Get authorization token required to use for each API request
				string tokenResponse = TokenRequest();
				dynamic token = !string.IsNullOrEmpty(tokenResponse) ? System.Web.Helpers.Json.Decode(tokenResponse) : null;

				// Check token returned with values needed
				if (string.IsNullOrEmpty(token?["access_token"]) || string.IsNullOrEmpty(token?["token_type"]))
				{
					// Invalid token so do something
					return false;
				}
				else
				{
					// Create HTTP Web Request for the API call
					HttpWebRequest patchRequest = (HttpWebRequest)WebRequest.Create(patchURL);

					// Get data to send
					byte[] data = Encoding.ASCII.GetBytes(Values.PatchData);

					// Set request verb PATCH, content type and content length (length of data)
					patchRequest.Headers.Add("Authorization", token["token_type"] + " " + token["access_token"]);
					patchRequest.Method = "PATCH";
					patchRequest.ContentType = "application/json";
					patchRequest.ContentLength = data.Length;

					// Stream request data
					using (Stream stream = patchRequest.GetRequestStream())
					{
						stream.Write(data, 0, data.Length);
					}

					// Get the response and read stream to return
					string responseText = "";
					using (WebResponse response = patchRequest.GetResponse())
					{
						using (Stream stream = response.GetResponseStream())
						{
							using (StreamReader sr = new StreamReader(stream))
							{
								responseText = sr.ReadToEnd();
							}
						}
					}

					// Deserialize response to dynamic object to read response (Here we could deserialize to a known object definition instead of dynamic)
					dynamic responseObject = System.Web.Helpers.Json.Decode(responseText);

					// Check for a valid expected response
					if (string.IsNullOrEmpty(responseText) || responseObject?["errors"] == null)
					{
						// No response or unexpected response format so do something.
						// 
						// Expected response:
						/*
							{
							  "errors": [
							    {
							      "key": "string",
							      "value": "string"
								}
							  ],
							  "item":
							  {
								"field1": "value",
								"field2": "value",
								"fieldn": "value"
							  }
							}
						*/

						return false;
					}

					// Check for errors
					if (responseObject["errors"].Length > 0)
					{
						// Errors were received so do something
						//
						// responseObject["errors"][0]["key"] = type of error (i.e. FIELDVALIDATIONERROR)
						// responseObject["errors"][0]["value"] = description of error (i.e. Unable to find data for Incident with id X)

						return false;
					}

					// No errors so check for result
					if (responseObject["item"] != null)
					{
						// Item received back so do something with it
						// responseObject["item"]

						return true;
					}

					// Return false.  A valid case wasn't handled?
					return false;
				}
			}
			catch (WebException ex)
			{
				// Catch error for bad URL (404) or bad request (400) resulting from bad input
				if (ex.Message.Contains("400"))
				{
					// do something for bad request
				}
				else if (ex.Message.Contains("404"))
				{
					// do something for not found
				}
				else
				{
					// other error, do something
				}

				return false;
			}
			catch (Exception ex)
			{
				// General Exception
				return false;
			}
		}
	}
}
