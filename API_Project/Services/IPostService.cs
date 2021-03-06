using API_Project.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_Project.Services
{
    public interface IPostService
    {
        public Task<ActionResult<IEnumerable<Post>>> GetPosts();
        public Task<ActionResult<Post>> GetPost(ControllerBase controller,int id);
        public Task<IActionResult> PutPost(ControllerBase controller,int id, Post post);
        public Task<ActionResult<Post>> PostPost(ControllerBase controller,Post post);
        public Task<IActionResult> DeletePost(ControllerBase controler,int id);
        public Task<ActionResult<Post>> RecoverPost(ControllerBase controller, int id);
        public Task<ActionResult<IEnumerable<Post>>> TrashedPosts();

    }
}
