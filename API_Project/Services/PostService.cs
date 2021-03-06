using API_Project.Data;
using API_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_Project.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext context;

        public PostService(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<IActionResult> DeletePost(ControllerBase controller,int id)
        {
            var post = await context.Posts.FindAsync(id);
            if (post == null)
            {
                return controller.NotFound();
            }
            
            // context.Posts.Remove(post);
            post.IsDeleted = true;
            context.Update(post);
            await context.SaveChangesAsync();

            return controller.NoContent();
        }

        public async Task<ActionResult<Post>> RecoverPost(ControllerBase controller,int id)
        {
            var post = await context.Posts.FindAsync(id);
            if (post == null)
            {
                return controller.NotFound();
            }
            post.IsDeleted = false;
            context.Update(post);
            await context.SaveChangesAsync();

            return await GetPost(controller, id);
        }

        public async Task<ActionResult<Post>> GetPost(ControllerBase controller,int id)
        {
            var post = await context.Posts.FindAsync(id);

            if (post == null)
            {
                 return controller.NotFound();
            }

            if (post.IsDeleted == true)
            {
                return controller.NotFound();
            }

            return post;
        }

        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            return await context.Posts.Where(post=>post.IsDeleted !=true).ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<Post>>> TrashedPosts()
        {
            return await context.Posts.Where(post => post.IsDeleted == true).ToListAsync();
        }

        public async Task<ActionResult<Post>> PostPost(ControllerBase controller, Post post)
        {
            context.Posts.Add(post);
            await context.SaveChangesAsync();
            return await GetPost(controller, post.Id);
        }

        public async Task<IActionResult> PutPost(ControllerBase controller,int id, Post post)
        {
            if (id != post.Id)
            {
                return controller.BadRequest();
            }

            context.Entry(post).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    return controller.NotFound();
                }
                else
                {
                    throw;
                }
            }

            return controller.NoContent();
        }
        private bool PostExists(int id)
        {
            return context.Posts.Any(e => e.Id == id);
        }
    }
}
