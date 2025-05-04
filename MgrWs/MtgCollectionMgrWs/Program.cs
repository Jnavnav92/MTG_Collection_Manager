using MtgCollectionMgrWs.Providers;

var builder = WebApplication.CreateBuilder(args);

//
var sqlConnString = builder.Configuration["MTGCollectionMgr:ConnString"];

//load the json file.
builder.Configuration.AddJsonFile("secrets.json",
        optional: true,
        reloadOnChange: true);

//register the service 
builder.Services.AddScoped<IDataRepository, DataRepository>();

//get the value from the secrets.json file
var name = builder.Configuration.GetSection("MTGCollectionMgr:ConnString").Value;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
