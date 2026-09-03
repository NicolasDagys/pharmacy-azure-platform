using Azure;
using Azure.AI.OpenAI;
using Microsoft.EntityFrameworkCore;
using OpenAI.Chat;
using PharmacyApiEF.DTOs;
using PharmacyApiEF.DTOs;
using PharmacyApiEF.Models;
using PharmacyApiEF.Services.Interfaces;
using System.ClientModel;

namespace PharmacyApiEF.Services;

public class AiInventoryService : IAiInventoryService
{
    private readonly PharmacyContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AiInventoryService> _logger;

    public AiInventoryService(
        PharmacyContext context,
        IConfiguration configuration,
        ILogger<AiInventoryService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AiInventoryChatResponseDto> AskAsync(string question)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            throw new ArgumentException(
                "The question cannot be empty.",
                nameof(question));
        }

        var endpoint = _configuration["OpenAI:Endpoint"];
        var apiKey = _configuration["OpenAI:ApiKey"];
        var deployment = _configuration["OpenAI:Deployment"];

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new InvalidOperationException(
                "OpenAI endpoint is not configured.");
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "OpenAI API key is not configured.");
        }

        if (string.IsNullOrWhiteSpace(deployment))
        {
            throw new InvalidOperationException(
                "OpenAI deployment is not configured.");
        }

        try
        {
            // ==========================================
            // 1. Obtener información de inventario
            // ==========================================

            var lowStock = await _context.Products
                .Where(p => p.StockQty < 10 && p.Active)
                .Select(p => new
                {
                    p.NameProd,
                    p.StockQty
                })
                .ToListAsync();

            var today = DateOnly.FromDateTime(DateTime.Now);
            var expirationLimit = today.AddMonths(3);

            var expiringSoon = await _context.Products
        .Where(p =>
            p.ExpDateProd >= today &&
            p.ExpDateProd <= expirationLimit &&
            p.Active)
        .Select(p => new
        {
            p.NameProd,
            p.ExpDateProd
        })
        .ToListAsync();

            var totalProducts = await _context.Products
                .CountAsync(p => p.Active);

            var totalCategories = await _context.Categories
                .CountAsync(c => c.Active);

            // ==========================================
            // 2. Construir contexto para el modelo
            // ==========================================

            var lowStockText = lowStock.Any()
                ? string.Join(
                    Environment.NewLine,
                    lowStock.Select(p =>
                        $"- {p.NameProd}: {p.StockQty} units"))
                : "No products currently have low stock.";

            var expiringSoonText = expiringSoon.Any()
                ? string.Join(
                    Environment.NewLine,
                    expiringSoon.Select(p =>
                        $"- {p.NameProd}: expires on {p.ExpDateProd:yyyy-MM-dd}"))
                : "No products are expiring within the next three months.";

            var inventoryContext = $"""
                Current Pharmacy Inventory Data:

                Total active products: {totalProducts}

                Total active categories: {totalCategories}

                Low Stock Products (less than 10 units):
                {lowStockText}

                Products Expiring Within The Next Three Months:
                {expiringSoonText}
                """;

            // ==========================================
            // 3. Crear cliente Azure OpenAI
            // ==========================================

            var client = new AzureOpenAIClient(
                new Uri(endpoint),
                new AzureKeyCredential(apiKey));

            var chatClient = client.GetChatClient(deployment);

            // ==========================================
            // 4. Definir comportamiento del asistente
            // ==========================================

            var systemMessage = """
                You are an inventory assistant for a pharmacy management system.

                Your job is to answer questions about the pharmacy inventory
                using ONLY the inventory data provided in the conversation.

                Rules:
                - Do not invent products, quantities, categories or dates.
                - If the requested information is not available, say so clearly.
                - Be concise and practical.
                - When appropriate, recommend which products require attention.
                - Do not provide medical advice.
                - The inventory data comes from the pharmacy database.
                """;

            var userMessage = $"""
                Pharmacy inventory information:

                {inventoryContext}

                User question:

                {question}
                """;

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemMessage),
                new UserChatMessage(userMessage)
            };

            // ==========================================
            // 5. Ejecutar consulta al modelo
            // ==========================================

            var response = await chatClient.CompleteChatAsync(messages);

            var answer = response.Value.Content.FirstOrDefault()?.Text
                         ?? "The AI assistant did not return an answer.";

            return new AiInventoryChatResponseDto
            {
                Answer = answer
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error while processing AI inventory question.");

            throw;
        }
    }
}