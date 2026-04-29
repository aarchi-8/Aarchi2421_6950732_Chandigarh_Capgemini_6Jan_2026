using System.Security.Claims;
using Asp.Versioning;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace BookStore.API.Controllers;
[ApiVersion("1.0")] [Route("api/v{version:apiVersion}/[controller]")] [ApiController] [Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    public OrdersController(IOrderService orderService) => _orderService = orderService;

    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> PlaceOrder(OrderCreateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(ApiResponse<OrderResponseDto>.Ok(await _orderService.PlaceOrderAsync(userId, dto), "Order placed."));
    }

    [HttpGet("my")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponseDto>>>> GetMyOrders()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(ApiResponse<IEnumerable<OrderResponseDto>>.Ok(await _orderService.GetUserOrdersAsync(userId)));
    }

    [HttpGet("all")] [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponseDto>>>> GetAllOrders()
    { return Ok(ApiResponse<IEnumerable<OrderResponseDto>>.Ok(await _orderService.GetAllOrdersAsync())); }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> GetOrder(int id)
    { var o = await _orderService.GetOrderDetailsAsync(id); if (o == null) return NotFound(ApiResponse.Fail("Order not found.", 404)); return Ok(ApiResponse<OrderResponseDto>.Ok(o)); }

    [HttpPatch("{id}/status")] [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse>> UpdateStatus(int id, [FromBody] string status)
    { if (!await _orderService.UpdateOrderStatusAsync(id, status)) return NotFound(ApiResponse.Fail("Order not found.", 404)); return Ok(ApiResponse.Ok("Status updated.")); }
}