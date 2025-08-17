using Application.Weather.Commands;
using Application.Weather.DTOs;
using Application.Weather.Queries;
using Asp.Versioning;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/weather")]
public class WeatherController : ControllerBase
{
	private readonly ISender _mediator;

	public WeatherController(ISender mediator)
	{
		_mediator = mediator;
	}

	[HttpPost]
	[ProducesResponseType(typeof(Result<WeatherDto>), 200)]
	public async Task<IActionResult> Create([FromBody] CreateWeatherRequest request)
	{
		var result = await _mediator.Send(new CreateWeatherCommand(request));
		if (!result.IsSuccess) return Problem(title: result.Error, statusCode: 400);
		return Ok(result);
	}

	[HttpGet("{id}")]
	[ProducesResponseType(typeof(Result<WeatherDto>), 200)]
	public async Task<IActionResult> GetById([FromRoute] Guid id)
	{
		var result = await _mediator.Send(new GetWeatherByIdQuery(id));
		if (!result.IsSuccess) return NotFound(result);
		return Ok(result);
	}

	[HttpGet]
	[ProducesResponseType(typeof(PagedResult<WeatherDto>), 200)]
	public async Task<IActionResult> List([FromQuery] Guid? locationId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
	{
		var result = await _mediator.Send(new ListWeatherQuery(locationId, from, to, search, page, pageSize));
		return Ok(result);
	}

	[HttpPut("{id}")]
	[ProducesResponseType(typeof(Result<WeatherDto>), 200)]
	public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateWeatherRequest request)
	{
		var result = await _mediator.Send(new UpdateWeatherCommand(id, request));
		if (!result.IsSuccess) return Conflict(result);
		return Ok(result);
	}

	[HttpDelete("{id}")]
	[ProducesResponseType(typeof(Result), 200)]
	public async Task<IActionResult> Delete([FromRoute] Guid id)
	{
		var result = await _mediator.Send(new DeleteWeatherCommand(id));
		if (!result.IsSuccess) return NotFound(result);
		return Ok(result);
	}
}