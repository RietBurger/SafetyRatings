# SafetyRatings
Obtain safety ratings for places

This is an ASP.NET Web Application with MVC structure.

It consumes a public API from Amadeus for Developers.
The Web App obtains the safety rating for a place, based on Latitude and Longitude provided.

For Authorization information see:
     https://developers.amadeus.com/self-service/apis-docs/guides/authorization-262

API Reference:
     https://developers.amadeus.com/self-service/category/destination-content/api-doc/safe-place/api-reference

The API used is for development and has a limit of 400 calls.

To use the application:
     The application was developed and tested in Visual Studio 2019.
     Once project has been cloned, open file Web.Config


In Web.Config:
Insert API Key [addID], API Secret [addSecret] and API Token [addToken] into:

<configuration>
	<appSettings>
		...
		<add key="AmadeusClientID" value="addID" />
		<add key="AmadeusClientSecret" value="addSecret" />
		<add key="AmadeusToken" value="addToken" />
		...
	</appSettings>
	...
</configuration>

If you get an invalid client error, your AmadeusClientID or AmadeusClientSecret values are incorrect or does not exist.

The Tokens has a limited validity period. Once it expires, a new one will be requested by the Web App.

To obtain a valid token for testing, you can run test_check_token_validity().
Token will be written to Debug Output.

