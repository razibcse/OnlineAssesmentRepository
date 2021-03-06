using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_Project.Data;
using API_Project.Models;
using Microsoft.AspNetCore.Cors;
using API_Project.Services;
using Microsoft.AspNetCore.Authorization;

namespace API_Project.Controllers
{
    
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService postService;

        public PostsController(IPostService postService)
        {
            this.postService = postService;
        }

        // GET: api/Posts
        [EnableCors]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            return await postService.GetPosts();
        }

        // GET: api/Posts/5
        [EnableCors]
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            return await postService.GetPost(this,id);
        }

        // PUT: api/Posts/5
        [HttpPut("{id}")]
        [EnableCors]
        public async Task<IActionResult> PutPost(int id, Post post)
        {
            return await postService.PutPost(this, id, post);
        }

        // POST: api/Posts
        [EnableCors]
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(Post post)
        {
            return await postService.PostPost(this, post);
        }


        // DELETE: api/Posts/5
        [EnableCors]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            return await postService.DeletePost(this, id);
        }
        [EnableCors]
        [HttpGet("recover/{id}")]
        public async Task<ActionResult<Post>> RecoverPost(int id)
        {
            return await postService.RecoverPost(this, id);
        }
        [EnableCors]
        [HttpGet("trashed-posts")]
        public async Task<ActionResult<IEnumerable<Post>>> AllTrashedPosts()
        {
            return await postService.TrashedPosts();
        }
    }
}
