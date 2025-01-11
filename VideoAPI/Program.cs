using Microsoft.OpenApi.Models;
using VideoAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var origin = builder.Configuration.GetSection("AllowOrigin").Get<string>();


builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowSpecificOrigins", policy =>
	{
		policy.WithOrigins(origin).AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
	});
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	//c.SwaggerDoc();
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "DataflowToUpper", Version = "v1" });

});
builder.Services.AddTransient<R2Service>();
//builder.Services.AddSingleton<IConfiguration>();

var app = builder.Build();

app.UseCors("AllowSpecificOrigins");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
