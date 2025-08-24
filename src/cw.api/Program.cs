using CW.Api;
using CW.Api.Models;
using CW.Core.Events;
using CW.Core.interfaces;
using CW.Infra.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.AddServicebusClient().WithServicebusSender();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

var app = builder.Build();
app.MapScalarApiReference();
app.MapDefaultEndpoints();
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Order api");

app.MapPost("/orders", async ([FromBody] UpdateOrderRequest updateOrderRequest, IBusSender busSender, CancellationToken cancellationToken) =>
{
    if (ValidationHelper.Validate(updateOrderRequest, out var validationErrors) == false)
        ValidationHelper.ThrowValidationException(validationErrors);


    await busSender.SendMessage("orders", new OrderUpdatedEvent(updateOrderRequest.Id, updateOrderRequest.Text, updateOrderRequest.Count, updateOrderRequest.TotalAmount, TimeProvider.System.GetUtcNow()), cancellationToken);
    return Results.Accepted();
}).Produces(202);

app.Run();
