using System.Text.Json.Serialization;
using final_backend.Hubs;
using final_backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options => options.AddDefaultPolicy(builderr => builderr.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
  options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext using PostgreSQL provider
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();
app.MapHub<UpdatesHub>("/updatesHub");
app.UseAuthorization();
app.MapControllers();

app.Run();