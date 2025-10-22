using JobEngine.Core.Commons;
using JobEngine.SqlServer.Commons;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("ConnectionString")!;
builder.Services.AddJobEngine(opt => opt.UseSqlServerStorage(connectionString));
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
