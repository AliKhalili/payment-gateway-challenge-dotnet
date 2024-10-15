using System;
using System.Reflection;
using System.Text.Json.Serialization;

using FluentValidation;

using MicroElements.Swashbuckle.FluentValidation.AspNetCore;

using PaymentGateway.Api.Applications.Payment;
using PaymentGateway.Api.Infrastructures;
using PaymentGateway.Api.Infrastructures.MockBank;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Validations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<PaymentsRepository>();
builder.Services.AddScoped<IValidator<PaymentRequest>, PaymentRequestValidator>();
builder.Services.AddFluentValidationRulesToSwagger();

builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<IMockBankClient, MockBankClient>();
builder.Services.AddHttpClient<IMockBankClient, MockBankClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MockBank:BaseUrl"]!);
});

var app = builder.Build();

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

public partial class Program { }