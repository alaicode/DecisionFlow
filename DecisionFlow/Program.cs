using DecisionFlow.Application.Interfaces;
using DecisionFlow.Application.Services;
using DecisionFlow.Domain.Entities;
using DecisionFlow.Infrastructure.Persistence;
using DecisionFlow.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DecisionFlowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//TODO: put to a serviceCollection extension method
builder.Services.AddScoped(typeof(IDecisionRepository<Decision>), typeof(DecisionFlowRepository<Decision>));

//TODO: put to a serviceCollection extension method
builder.Services.AddScoped<CreateDecisionService>();
builder.Services.AddScoped<ApproveDecisionService>();
builder.Services.AddScoped<RejectDecisionService>();
builder.Services.AddScoped<GetDecisionsService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler("/error");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
