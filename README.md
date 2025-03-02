# TombalaAPI
This project is an implementation of the Turkish traditional game Tombala with .Net Core API and PostgreSQL.

# Installation

Open a terminal and run these prompts:

```sh
$ git clone https://github.com/HalilBerkayCoban/TombalaAPI.git
$ cd TombalaAPI/TombalaAPI
$ dotnet watch
```
This will open a new browser tab, you will be greeted with the SwaggerUI.

Before you start using the project, there are some configuration settings you need to modify.

## Configuration

### Environment Variables

Create a `.env` file in the root directory of the project with the following content:

```
CONNECTION_STRING=Host=localhost;Database=tombala;Username=postgres;Password=postgres
```

Replace the connection string with your PostgreSQL server details.

### Discord Authentication

In the `appSettings.json` file, you will find the following section that requires your attention:

```
"Discord": {
"ClientId": "YOUR-CLIENT-ID",
"ClientSecret": "YOUR-CLIENT-SECRET"
}
```

To integrate Discord authentication into the project, you need to provide your Discord client ID and client secret.
Replace `"YOUR-CLIENT-ID"` with your actual Discord client ID, and `"YOUR-CLIENT-SECRET"` with your Discord client secret.

### Program.cs Configuration

There is also another section that requires your attention:
In Program class, you will find the following code snippet:

```csharp
app.MapGet("/login", () =>
{
    return Results.Challenge(new AuthenticationProperties
    {
        RedirectUri = "your-redirect-uri"
    }, authenticationSchemes: new List<string>() { "Discord" });
});
```

This is a login endpoint that handles authentication with Discord. 
You need to update the RedirectUri with the actual URI you want to use.

Once you have updated the necessary configuration settings, you can proceed with setting up and running the project. 

# License

MIT License

Copyright (c) 2023 Halil Berkay Çoban

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
