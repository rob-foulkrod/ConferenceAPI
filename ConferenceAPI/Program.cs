using System.Text.Json;
using System.IO;
using Microsoft.OpenApi.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// added options to the SwaggerGen which get reflected in the Swagger UI 
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Trainer Demo Conference API",
        Description = "An ASP.NET Core Web API for managing sample Conference data items such as speakers, sessions and topics. Idea is based on the original ConferenceAPI web app used in Microsoft Learn modules, which is no longer available.",
        TermsOfService = new Uri("https://www.microsoft.com/en-us/legal/intellectualproperty/open-source?msockid=327fe0b7a40f63fb2f5cf43da587623a&oneroute=true"),
        Contact = new OpenApiContact
        {
            Name = "Peter De Tender",
            Url = new Uri("https://aka.ms/pdtit")
        },
        License = new OpenApiLicense
        {
            Name = "The MIT Open Source License",
            Url = new Uri("https://opensource.org/license/MIT")
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment()) //PDT - commented this out, to always show Swagger UI when running in both Development or in Azure Production App Service 
//{
app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

// Read the JSON files for each data type (Sessions, Speakers, Topics)
// and return the data as a list of objects
List<Session> ReadSessionsFromFile(string FilePath)
{
    var json = File.ReadAllText(FilePath);
    return JsonSerializer.Deserialize<List<Session>>(json);
}

List <Speaker> ReadSpeakersFromFile(string FilePath)
{
    var json = File.ReadAllText(FilePath);
    return JsonSerializer.Deserialize<List<Speaker>>(json);
}

List <Topic> ReadTopicsFromFile(string FilePath)
{
    var json = File.ReadAllText(FilePath);
    return JsonSerializer.Deserialize<List<Topic>>(json);
}

// Using Minimal APIs to define the routes and return the data; each route is defined with a lambda expression
// to return the data from the JSON file; for the main (all data), we don't have a lambda expression, which means show all data from the JSON file
// same approach for speakers and topics

app.MapGet("/sessions", () =>
{
    var sessions = ReadSessionsFromFile("transformed_sessions.json");
    return sessions;
});
//.WithName("GetSessions")
//.WithOpenApi();

/// Using Minimal APIs to define the routes and return the data; each route is defined with a lambda expression
// to return the data from the JSON file; for the main (all data), we don't have a lambda expression, which means show all data from the JSON file
// The lambda expression used means: when a user connects to a specific session using the session ID, show only information where the session ID # corresponds to the session ID in the JSON file
// similar approach to filter on speaker id and topic id

app.MapGet("/session/{id}", (string id) =>
{
    var sessions = ReadSessionsFromFile("transformed_sessions.json");
    return sessions.Find(s => s.session_id == id);
});

app.MapGet("/speakers", () =>
{
    var speakers = ReadSpeakersFromFile("transformed_speakers.json");
    return speakers;
});
//.WithName("GetSpeakers")
//.WithOpenApi();
app.MapGet("/speaker/{id}", (string speakerID) =>
{
    var speakers = ReadSpeakersFromFile("transformed_speakers.json");
    return speakers.Find(sp => sp.speakerID == speakerID);
});

app.MapGet("/topics", () =>
{
    var sessions = ReadTopicsFromFile("transformed_topics.json");
    return sessions;
});
//.WithName("GetSessions")
//.WithOpenApi();

app.MapGet("/topics/{id}", (string id) =>
{
    var topics = ReadTopicsFromFile("transformed_topics.json");
    return topics.Find(t => t.topicid == id);
});

app.Run();

// the class definition for each data object; each item corresponds to the item in the JSON dataset
class Session
    
{
    public required string session_id { get; set; } //using string as data type, as the id is not used as a number value
    public required string title { get; set; }
    public required string timeslot { get; set; }
    public required string speaker { get; set; }
    public required string description { get; set; }
}

class Speaker
{
    public required string speakerID { get; set; }  //using string as data type, as the id is not used as a number value
    public required string name { get; set; }
}

class Topic
{
    public required string topicid { get; set; }    //using string as data type, as the id is not used as a number value
    public required string topicvalue { get; set; }
}




