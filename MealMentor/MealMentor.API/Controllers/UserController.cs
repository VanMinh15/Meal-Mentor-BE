using MealMentor.API.Services.RecipeService;
using MealMentor.API.Services.UserService;
using MealMentor.Core.DTOs.UserDTO;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MealMentor.Core.DTOs.PaginationDTO.PaginationModel;

namespace MealMentor.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRecipeService _recipeService;

        public UserController(IUserService userService, IRecipeService recipeService)
        {
            _userService = userService;
            _recipeService = recipeService;
        }

        [HttpPatch("edit-profile")]
        public async Task<IActionResult> EditProfile([FromBody] EditProfileResponseDTO userRequest)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { Message = "User ID not found in token" });
            }
            var userId = userIdClaim.Value;


            var user = new EditProfileRequestDTO
            {
                Id = userId,
                Username = userRequest.Username,
                Height = userRequest.Height,
                Weight = userRequest.Weight,
                BirthDate = userRequest.BirthDate
            };

            var result = await _userService.EditProfile(user);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("search-user")]
        public async Task<IActionResult> SearchUserByEmailOrUserName(string? keyword, [FromQuery] PaginationParams pagingParams)
        {
            var result = await _userService.SearchUserByEmailOrUserName(keyword, pagingParams);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPost("send-request")]
        public async Task<IActionResult> SendFriendRequest([FromBody] FriendshipDTO request)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderId == null)
            {
                return Unauthorized();
            }

            await _userService.SendFriendRequest(senderId, request.ReceiverId);
            return Ok(new { Message = "Friend request sent." });
        }

        [HttpPost("accept-request")]
        public async Task<IActionResult> AcceptFriendRequest([FromBody] FriendshipDTO request)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderId == null)
            {
                return Unauthorized();
            }

            var result = await _userService.AcceptFriendRequest(senderId, request.ReceiverId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("reject-request")]
        public async Task<IActionResult> RejectFriendRequest([FromBody] FriendshipDTO request)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderId == null)
            {
                return Unauthorized();
            }

            var result = await _userService.RejectFriendRequest(senderId, request.ReceiverId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("remove-friendship")]
        public async Task<IActionResult> RemoveFriendship([FromBody] FriendshipDTO request)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderId == null)
            {
                return Unauthorized();
            }

            var result = await _userService.RemoveFriendShip(senderId, request.ReceiverId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("friend-requests")]
        public async Task<IActionResult> GetPendingFriendRequest([FromQuery] PaginationParams pagingParams)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _userService.GetPendingFriendRequest(userId, pagingParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("friends-list")]
        public async Task<IActionResult> GetFriendsList([FromQuery] PaginationParams pagingParams)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _userService.GetFriendsList(userId, pagingParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("like-recipe")]
        public async Task<IActionResult> LikeRecipe(int recipeId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _recipeService.LikeRecipe(userId, recipeId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("dislike-recipe")]
        public async Task<IActionResult> DisikeRecipe(int recipeId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _recipeService.UnlikeRecipe(userId, recipeId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("subcriber-list")]
        public async Task<IActionResult> GetSubcriberList()
        {
            var result = await _userService.GetSubcriberList();
            return StatusCode(result.StatusCode, result);
        }
    }
}
