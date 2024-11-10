using OBS2Browser;

var _myAllowSpecificOrigins = "CorsPolicy";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: _myAllowSpecificOrigins,
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        }
    );
});

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(_myAllowSpecificOrigins);

//app.UseDefaultFiles();
app.UseStaticFiles();

app.UseWebSockets(); // Enable WebSocket support
app.UseRouting();

app.MapHub<WSHub>("/websocket");
app.MapControllers();

app.Run();