# TombalaAPI
This project is a implementation of the Turkish traditional game Tombala with .Net Core API and MongoDb.

# Installation

Open a terminal and run these prompts:

```sh
$ git clone https://github.com/HalilBerkayCoban/TombalaAPI.git
$ cd TombalaAPI/TombalaAPI
$ dotnet watch
```
This will open a new browser tab, you will be greeted with the SwaggerUI.

Before you start using the project, there are some configuration settings you need to modify in the appSettings.json file.

## Configuration

In the `appSettings.json` file, you will find the following sections that require your attention:

```
"Discord": {
"ClientId": "YOUR-CLIENT-ID",
"ClientSecret": "YOUR-CLIENT-SECRET"
}
```

To integrate Discord authentication into the project, you need to provide your Discord client ID and client secret.
Replace `"YOUR-CLIENT-ID"` with your actual Discord client ID, and `"YOUR-CLIENT-SECRET"` with your Discord client secret.

### Tombala Database Configuration

```
"TombalaDatabase": {
"ConnectionString": "mongodb://localhost:27017",
"DatabaseName": "tombala",
"GamesCollectionName": "game",
"UsersCollectionName": "user"
}
```

The Tombala game requires a MongoDB database for data storage. 
In this section, you need to update the connection string and other details related to the database. 
Replace `"mongodb://localhost:27017"` with the connection string for your MongoDB server. 
Also, you can modify the `"DatabaseName"`, `"GamesCollectionName"`, and `"UsersCollectionName"` values according to your preferences.

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
